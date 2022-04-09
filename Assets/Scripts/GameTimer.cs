using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public DataRecorder dataRecorder;
    public Text timeText;
    public float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        if(dataRecorder.mwConnectedTime != 0f)
        {
            timer = Time.time - dataRecorder.mwConnectedTime;
            timeText.text = "" + Mathf.FloorToInt(timer);
        }
    }
}
