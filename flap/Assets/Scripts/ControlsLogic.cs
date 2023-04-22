using Unity.Services.Mediation.Samples;
using UnityEngine;

public class ControlsLogic : MonoBehaviour
{
    bool touchedDown;

    [SerializeField] Rigidbody rb;

    float force = 28.4f; // 27.4

    [SerializeField] Animator myAnim;

    [SerializeField] CapsuleCollider myCol;
    bool levelFailedCheck;

    [SerializeField] SoundManagerLogic mySoundManager;

    [SerializeField] GameObject noIcon;

    [SerializeField] Animator soundAnim;

    int cheatCounter;
    void Awake()
    {
        touchedDown = false;

        cheatCounter = 0;
    }

    private void Start()
    {
        levelFailedCheck = false;
        myCol.enabled = true;

        if (PlayerPrefs.GetInt("SoundStatus", 1) == 1)
        {
            noIcon.SetActive(false);
            AudioListener.volume = 1;
        }
        else
        {
            noIcon.SetActive(true);
            AudioListener.volume = 0;
        }
    }

    private void Update()
    {
        rb.transform.localEulerAngles = new Vector3(rb.velocity.y*1.9f, 0, 0);

        if (!levelFailedCheck && GameManager.levelFailed)
        {
            myCol.enabled = false; // turn of colider

            rb.velocity = force * 2 * Vector3.up; // bounce up
            levelFailedCheck = true;
        }
    }

    void OnTouchDown(Vector3 point)
    {
        if (!touchedDown)
        {
            touchedDown = true;

           if (ShowAds.poppedUp)
            {
                if (point.x <= 0)
                {
                    ShowAds.shouldShowRewardedAd = true;
                }
                else
                {
                    ShowAds.dontShow = true;
                }
            }
            else
            {
                // cheat: top-right, top-right, top-left, bottom-right
                // top right tap
                if (!GameManager.levelStarted && (cheatCounter == 0 || cheatCounter == 1) && point.x >= 0.03f && point.y >= 8f)
                {
                    cheatCounter++;
                }
                // top left tap
                else if (!GameManager.levelStarted && (cheatCounter == 2) && point.x <= -0.03f && point.y >= 8f)
                {
                    cheatCounter++;
                }
                // bottom right tap
                else if (!GameManager.levelStarted && (cheatCounter == 3) && point.x >= 0.03f && point.y <= 7.92f)
                {
                    cheatCounter = 0;
                    if (!GameManager.cheatOn)
                        GameManager.cheatOn = true;
                    else
                        GameManager.cheatOn = false;
                }

                else if (!GameManager.levelStarted && point.x <= -0.01f && point.y <= 7.92f) // bottom left button clicked
                {
                    if (PlayerPrefs.GetInt("SoundStatus", 1) == 1)
                    {
                        PlayerPrefs.SetInt("SoundStatus", 0);
                        noIcon.SetActive(true);
                        AudioListener.volume = 0;
                    }
                    else
                    {
                        PlayerPrefs.SetInt("SoundStatus", 1);
                        noIcon.SetActive(false);
                        AudioListener.volume = 1;
                    }
                    soundAnim.SetTrigger("Blob");
                }
                else
                {
                    if (!GameManager.levelFailed)
                    {
                        if (!GameManager.levelStarted)
                        {
                            GameManager.resetScore = true;
                            GameManager.levelStarted = true;
                            rb.useGravity = true;
                        }

                        // add force
                        if (!GameManager.levelFailed)
                            rb.velocity = force * Vector3.up;


                        mySoundManager.Play("Flap"); // flap sound

                        myAnim.SetTrigger("flapWings");
                    }
                    else if (!GameManager.isRestarting && GameManager.canRestart)
                    {
                        GameManager.isRestarting = true;
                        GameManager.shouldRestart = true;
                    }
                }
            }
        }
    }

    void OnTouchUp()
    {
        if (touchedDown)
        {
            touchedDown = false;        
        }
    }

    void OnTouchExit()
    {
        if (touchedDown)
        {
            touchedDown = false;          
        }
    }
}
