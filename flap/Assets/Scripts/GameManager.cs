using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool levelStarted, levelFailed, cheatOn, spawnSomePipes;

    [SerializeField]
    Transform levelObjectsFolder;

    float previousSpawnedLocation;

    [SerializeField]
    Camera myCam;

    [SerializeField] GameObject[] pipes = new GameObject[4];
    int prevPrevObj = 0, prevObj = 0;
    int currentSpawn = 0;

    [SerializeField] Transform spawnFolder;
    float spawnHeight = 8f;

    int score;

    public static bool shouldSpawn, resetScore, playerLost, shouldRestart, isRestarting, canRestart, bobScore;
    [SerializeField] TextMeshPro scoreText;
    [SerializeField] SpriteRenderer highScoreImg, highscoreBGImg, tilteImg, titleBGImg, tapImg, tapBGImg, retryImg, retryBGImg, fullSquare;

    [SerializeField] SpriteRenderer screenSquare;
    [SerializeField] Transform lightTransform;

    public static int objectsSpawned;

    [SerializeField] Transform scoreTransform;
    [SerializeField] Color[] bgColors = new Color[2];
    Vector3[] lightAngle = new Vector3[2];

    [SerializeField] SpriteRenderer[] soundIconds;

    [SerializeField] AudioSource mainMenuMusic;

    [SerializeField] GameObject poopObj, chickenObbj;

    [SerializeField] Material planeGreen, planePurple;
    int pipeColor = 0;

    [SerializeField] SoundManagerLogic mySoundManager;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        lightAngle[0] = new Vector3(26.313f, 0.683f, 14.691f);
        lightAngle[1] = new Vector3(-75.977f, -116.771f, 110.256f);

        if (Screen.height / Screen.width > 1.75f)
            myCam.orthographicSize = 14.67f;
        else
            myCam.orthographicSize = 14.37f;

        objectsSpawned = 0;
        pipeColor = 0;

        levelStarted = false;
        levelFailed = false;
        shouldSpawn = false;
        resetScore = false;
        playerLost = false;
        shouldRestart = false;
        isRestarting = false;
        canRestart = false;
        bobScore = false;

        spawnSomePipes = false;

        cheatOn = false;

        planePurple.color = new Color(0.5499385f, 0.3713843f, 0.6392157f);
        planeGreen.color = new Color(0.1364f, 0.44f, 0.1580f);
    }

    private void Start()
    {
        spawnSomePipes = false;
        SpawnPipe();
        SpawnPipe();

        if (PlayerPrefs.GetInt("EggEnabled", 0) == 0) // is off
        {
            poopObj.SetActive(false);
            chickenObbj.SetActive(true);
            cheatOn = false;
        }
        else if (PlayerPrefs.GetInt("EggEnabled", 0) == 1) // is on
        {
            poopObj.SetActive(true);
            chickenObbj.SetActive(false);
            cheatOn = true;
        }

        score = 0;
        scoreText.text = PlayerPrefs.GetInt("highScore", score) + "";

        StartCoroutine(FadeImageOut(fullSquare));
        StartCoroutine(FadeImageIn(tilteImg, 24));
        StartCoroutine(FadeImageIn(titleBGImg, 24));
        StartCoroutine(FadeImageIn(highScoreImg, 24));
        StartCoroutine(FadeImageIn(highscoreBGImg, 24));
        StartCoroutine(FadeImageIn(tapImg, 24));
        StartCoroutine(FadeImageIn(tapBGImg, 24));
    }

    IEnumerator FadeOutAudio(AudioSource myAudio)
    {
        float timer = 0, totalTime = 24;
        float startingLevel = myAudio.volume;
        while (timer <= totalTime)
        {
            myAudio.volume = Mathf.Lerp(startingLevel, 0, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    private void Update()
    {
        if (spawnSomePipes)
        {
            spawnSomePipes = false;
            SpawnPipe();
            SpawnPipe();
        }

        if (cheatOn && PlayerPrefs.GetInt("EggEnabled", 0) == 0) // turn on
        {
            poopObj.SetActive(true);
            chickenObbj.SetActive(false);
            PlayerPrefs.SetInt("EggEnabled", 1);
        }
        else if (!cheatOn && PlayerPrefs.GetInt("EggEnabled", 0) == 1) // turn off
        {
            poopObj.SetActive(false);
            chickenObbj.SetActive(true);
            PlayerPrefs.SetInt("EggEnabled", 0);
        }

        if (resetScore)
        {
            foreach (SpriteRenderer sprite in soundIconds)
            {
                StartCoroutine(FadeImageOut(sprite));
            }

            StartCoroutine(FadeOutAudio(mainMenuMusic));

            StartCoroutine(FadeImageOut(tilteImg));
            StartCoroutine(FadeImageOut(titleBGImg));
            StartCoroutine(FadeImageOut(highScoreImg));
            StartCoroutine(FadeImageOut(highscoreBGImg));
            StartCoroutine(FadeImageOut(tapImg));
            StartCoroutine(FadeImageOut(tapBGImg));

            if (levelObjectsFolder.childCount < 2)
            {
                SpawnPipe();
                SpawnPipe();
            }

            scoreText.text = score + "";

            resetScore = false;
        }
        if (shouldSpawn)
        {
            SpawnPipe();
            shouldSpawn = false;
        }
        if (playerLost)
        {
            PlayerPrefs.SetInt("PointsSinceLastAdPop", PlayerPrefs.GetInt("PointsSinceLastAdPop", 0)+score);
            StartCoroutine(RestartWait());
            StartCoroutine(FlashScreen());
            playerLost = false;
        }

        if (shouldRestart)
        {
            StartCoroutine(FlashScreen());
            StartCoroutine(RestartLevel(fullSquare));
            shouldRestart = false;
        }

        if (bobScore)
        {
            StartCoroutine(BobScore());
            bobScore = false;
        }

        // set light angle
        lightTransform.eulerAngles = Vector3.Lerp(lightAngle[0], lightAngle[1], (float)score / 1000f);
        // set camera bg
        myCam.backgroundColor = Color.Lerp(bgColors[0], bgColors[1], (float)score / 1000f);
    }

    IEnumerator BobScore()
    {
        Vector3 startingPos = scoreTransform.position, endPos = scoreTransform.position;
        endPos += new Vector3(0, 0.15f, 0);

        float timer = 0, totalTime = 5;
        while (timer <= totalTime)
        {
            scoreTransform.position = Vector3.Lerp(startingPos, endPos, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }

        timer = 0;
        while (timer <= totalTime)
        {
            scoreTransform.position = Vector3.Lerp(endPos, startingPos, timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }

        score++;
        scoreText.text = score + "";

        if (score % 10 == 0 && score > 0)
        {
            mySoundManager.Play("ding"); // loss jingle

            Color newPipeColor, newMiddleColor = new Color(0.5499385f, 0.3713843f, 0.6392157f);
            pipeColor++;
            // change mat color
            switch (pipeColor)
            {
                case 0:
                    newPipeColor = new Color(0.1364f, 0.44f, 0.1580f);
                    newMiddleColor = new Color(0.5499385f, 0.3713843f, 0.6392157f);
                    break;
                case 1:
                    newPipeColor = new Color(0.1372f, 0.419f, 0.4392f);
                    newMiddleColor = new Color(0.6392157f, 0.372549f, 0.4614379f);
                    break;
                case 2:
                    newPipeColor = new Color(0.1573f, 0.1372f, 0.4392f);
                    newMiddleColor = new Color(0.6392157f, 0.5503268f, 0.372549f);
                    break;
                case 3:
                    newPipeColor = new Color(0.4392157f, 0.1372549f, 0.419085f);
                    newMiddleColor = new Color(0.4614379f, 0.6392157f, 0.372549f);
                    break;
                case 4:
                    newPipeColor = new Color(0.4392157f, 0.1573856f, 0.1372549f);
                    newMiddleColor = new Color(0.372549f, 0.6392157f, 0.5503268f);
                    break;
                case 5:
                    newPipeColor = new Color(0.419085f, 0.4392157f, 0.1372549f);
                    newMiddleColor = new Color(0.372549f, 0.4614379f, 0.6392157f);
                    pipeColor = -1;
                    break;
                default:
                    newPipeColor = new Color(0.1364f, 0.44f, 0.1580f);
                    newMiddleColor = new Color(0.5499385f, 0.3713843f, 0.6392157f);
                    break;
            }
            StartCoroutine(ChangeMatColors(newMiddleColor, newPipeColor));
        }
    }

    IEnumerator ChangeMatColors(Color endMiddleColor, Color endPipeColor)
    {
        float timer = 0, totalTimer = Random.Range(3, 25);

        Color startMiddleColor = planePurple.color;
        Color startPipeColor = planeGreen.color;

        while (timer <= totalTimer)
        {
            planePurple.color = Color.Lerp(startMiddleColor, endMiddleColor, timer / totalTimer);
            planeGreen.color = Color.Lerp(startPipeColor, endPipeColor, timer / totalTimer);
            timer++;
            yield return new WaitForFixedUpdate();
        }


    }

    IEnumerator RestartWait()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        StartCoroutine(FadeImageIn(retryImg, 54));
        StartCoroutine(FadeImageIn(retryBGImg, 54));

        yield return new WaitForSecondsRealtime(0.5f);
        canRestart = true;
    }

    IEnumerator RestartLevel(SpriteRenderer myImage)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myImage.color;
        myImage.enabled = true;
        if (score > PlayerPrefs.GetInt("highScore", 0))
            PlayerPrefs.SetInt("highScore", score);
        while (timer <= totalTime)
        {
            myImage.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 0), new Color(startingColor.r, startingColor.g, startingColor.b, 1), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    IEnumerator FadeImageOut(SpriteRenderer myImage)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myImage.color;
        myImage.enabled = true;
        while (timer <= totalTime)
        {
            myImage.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 1), new Color(startingColor.r, startingColor.g, startingColor.b, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
        myImage.enabled = false;
    }

    IEnumerator FadeImageIn(SpriteRenderer myImage, float totalTime)
    {
        float timer = 0;
        Color startingColor = myImage.color;
        myImage.enabled = true;
        while (timer <= totalTime)
        {
            myImage.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 0), new Color(startingColor.r, startingColor.g, startingColor.b, 1), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator FadeTextOut(TextMeshPro myTtext)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myTtext.color;
        while (timer <= totalTime)
        {
            myTtext.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 1), new Color(startingColor.r, startingColor.g, startingColor.b, 0), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    IEnumerator FadeTextIn(TextMeshPro myTtext)
    {
        float timer = 0, totalTime = 24;
        Color startingColor = myTtext.color;
        while (timer <= totalTime)
        {
            myTtext.color = Color.Lerp(new Color(startingColor.r, startingColor.g, startingColor.b, 0), new Color(startingColor.r, startingColor.g, startingColor.b, 1), timer / totalTime);
            yield return new WaitForFixedUpdate();
            timer++;
        }
    }

    void SpawnPipe()
    {
        currentSpawn = Random.Range(1, pipes.Length);
        if (score % 10 == 2 || score % 10 == 7)
            currentSpawn = 0;
        //PipeChecker(currentSpawn);
        Instantiate(pipes[currentSpawn], new Vector3(0, spawnHeight, 4.5f), Quaternion.identity, spawnFolder);
        spawnHeight += 10.88f;

        //prevPrevObj = prevObj;
        // = currentSpawn;
    }

    // if last two were 3, no 3
    // if either of last two were 0, no 0

    /*
    void PipeChecker(int mySpawnerNum)
    {        
        switch (mySpawnerNum)
        {
            case 0: // full
                if (prevObj == 0 || prevPrevObj == 0)
                {
                    if (prevObj == 1)
                        mySpawnerNum = Random.Range(2, pipes.Length);
                    else
                        mySpawnerNum = Random.Range(1, pipes.Length);
                }
                break;
            case 1: // half
                if (prevObj == 1)
                {
                    if (prevPrevObj == 0)
                        mySpawnerNum = Random.Range(2, pipes.Length);
                    else
                    {
                        while (mySpawnerNum == 1)
                            mySpawnerNum = Random.Range(0, pipes.Length);
                    }                    
                }
                else if (prevPrevObj == 1)
                {
                    if (prevObj == 0)
                        mySpawnerNum = Random.Range(2, pipes.Length);
                    else
                    {
                        if (Random.Range(0, 2) == 0)
                        {
                            while (mySpawnerNum == 1)
                                mySpawnerNum = Random.Range(0, pipes.Length);
                        }
                    }
                }
                break;
            case 2: // thirds
                if (prevObj == 2 && prevPrevObj == 2)
                {
                    while (mySpawnerNum == 2)
                        mySpawnerNum = Random.Range(0, pipes.Length);
                }
                else if (prevObj == 2)
                {
                    if (prevPrevObj == 0)
                    {
                        if (Random.Range(0, 10) > 6)
                        {
                            while (mySpawnerNum == 2)
                                mySpawnerNum = Random.Range(1, pipes.Length);

                        }
                    }
                    else
                    {
                        if (Random.Range(0, 10) > 6)
                        {
                            while (mySpawnerNum == 2)
                                mySpawnerNum = Random.Range(0, pipes.Length);
                        }
                    }
                }
                break;
            case 3: // fourth
                if (prevObj == 3 && prevPrevObj == 3)
                    mySpawnerNum = Random.Range(0, pipes.Length - 1);
                else if (prevObj == 3)
                {
                    if (prevPrevObj == 0)
                    {
                        if (Random.Range(0, 20) > 16)
                        {
                            mySpawnerNum = Random.Range(1, pipes.Length-1);

                        }
                    }
                    else
                    {
                        if (Random.Range(0, 20) > 16)
                        {
                            mySpawnerNum = Random.Range(0, pipes.Length-1);
                        }
                    }
                }
                break;
            default:
                mySpawnerNum = Random.Range(0, pipes.Length);
                break;
        }
    }
    */
    IEnumerator FlashScreen()
    {
        screenSquare.enabled = true;
        yield return new WaitForFixedUpdate();
        screenSquare.enabled = false;
    }
}
