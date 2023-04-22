using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(SpawnCheck());
    }

    IEnumerator SpawnCheck()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        if (transform.childCount < 2)
            GameManager.spawnSomePipes = true;
    }
}
