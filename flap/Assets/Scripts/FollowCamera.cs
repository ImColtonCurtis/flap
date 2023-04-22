using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector2 offset;

    [SerializeField] Transform bottom_Border;
    float tracker, clamper, starter;

    public static float targetHeight;
    [SerializeField] GameObject bottomBorderObj;

    private void Awake()
    {
        tracker = 0;
        clamper = 0;
        starter = target.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tracker = target.position.y - starter;
        if (tracker > clamper)
            clamper = tracker;
        if (bottom_Border != null)
            bottom_Border.transform.position = new Vector3(0, Mathf.Max(-14.96f+ clamper, -7.35f), 4.5f);
        else
        {
            GameObject tempObj = Instantiate(bottomBorderObj, new Vector3(0, Mathf.Max(-14.96f + clamper, -7.35f), 4.5f), Quaternion.identity);
            bottom_Border = tempObj.transform;
        }

        if (!GameManager.levelFailed)
            transform.position = new Vector3(target.position.x + offset.x, Mathf.Max((target.position.y + offset.y), clamper-0.7f), -10);
        targetHeight = target.position.y;
    }
}