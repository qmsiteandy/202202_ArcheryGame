using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootRecorder : MonoBehaviour
{
    public int _score = 0;   //分數
    public int _arrow_number = 10;   //箭的數量

    public Text text_score;
    public Transform arrowsFolder;

    void Start()
    {
        Reset();
    }

    public void Record(int score)
    {
        _score += score;
        text_score.text = _score.ToString();
    }

    public void Shoot()
    {
        _arrow_number -= 1;
        arrowsFolder.GetChild(_arrow_number).gameObject.SetActive(false);
    }

    public void Reset()
    {
        _score = 0;
        _arrow_number = 10;

        text_score.text = "0";
        for (int i=0;i < arrowsFolder.childCount; i++)
        {
            arrowsFolder.GetChild(i).gameObject.SetActive(true);
        }
    }
}
