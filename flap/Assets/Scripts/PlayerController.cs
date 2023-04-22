using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] SoundManagerLogic mySoundManager;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Pipe")
        {
            mySoundManager.Play("Smack"); // smack sound
            mySoundManager.Play("loseJingle"); // loss jingle

            GameManager.levelFailed = true;
            GameManager.playerLost = true;            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SpawnTrigger")
        {
            GameManager.bobScore = true;
            other.enabled = false;
            GameManager.shouldSpawn = true;
        }
    }
}
