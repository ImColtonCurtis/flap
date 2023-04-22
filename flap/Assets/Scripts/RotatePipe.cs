using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePipe : MonoBehaviour
{
    float rotateSpeed;
    int tensPlace;

    // Start is called before the first frame update\
    private void OnEnable()
    {
        // start at random rotation
        transform.eulerAngles = new Vector3(0, Random.Range(0f, 180f), 0);       

        tensPlace = 0;
        if (GameManager.objectsSpawned >= 10)
            tensPlace = GameManager.objectsSpawned / 10;
        if (GameManager.objectsSpawned < 3)
            rotateSpeed = Random.Range(0.35f, 0.75f);
        else
            rotateSpeed = Random.Range(0.5f, 1.2f);

        if (tensPlace % 10 == 1 || tensPlace % 10 == 5 || tensPlace % 10 == 8)
            rotateSpeed = Random.Range(0.7f, 1.4f);

        if (GameManager.objectsSpawned >= 5)
        {
            if (Random.Range(0, 2) == 0)
                rotateSpeed *= -1;
        }
        else if(GameManager.objectsSpawned == 3)
            rotateSpeed *= -1;

        if (gameObject.tag == "Fly")
        {
            if (GameManager.objectsSpawned >= 4)
            {
                if (Random.Range(0, 7) != 0)
                    Destroy(gameObject);
                // determine speed
                rotateSpeed = Random.Range(1.4f, 1.9f);

                // determine direction
                if (Random.Range(0, 2) == 0)
                    rotateSpeed *= -1;
            }
            else
                Destroy(gameObject);
        }

        GameManager.objectsSpawned++;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, rotateSpeed, 0));

        if (FollowCamera.targetHeight >= transform.position.y + 20 && gameObject.tag != "Fly" && !GameManager.levelFailed)
            Destroy(gameObject);
    }
}
