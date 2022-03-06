using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootRecorder : MonoBehaviour
{
    public int score = 0;   //分數
    public int arrow_count = 10;   //箭的數量

    public Text text_score;
    public Transform arrowsFolder;

    public GameObject finishPage;
    public Text text_Score;

    void Start()
    {
        Reset();
    }

    public void Record(int _score)
    {
        score += _score;
        text_score.text = score.ToString();

        if(arrow_count <= 0)
        {
            finishPage.SetActive(true);
            text_Score.text = "" + score;
            Cursor.visible = true;
        }
    }

    public void Shoot()
    {
        arrow_count -= 1;
        arrowsFolder.GetChild(arrow_count).gameObject.SetActive(false);
    }

    public void Reset()
    {
        score = 0;
        arrow_count = 10;

        text_score.text = "0";

        for (int i=0;i < arrowsFolder.childCount; i++)
        {
            arrowsFolder.GetChild(i).gameObject.SetActive(true);
        }
    }
}
