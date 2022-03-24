using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootManager : MonoBehaviour
{
    [Header("箭數&分數&狀態")]
    public int score = 0;   //分數
    public int arrow_init_count = 10;   //箭的數量
    public int arrow_count = 0;

    public Text text_score;
    public Transform arrowsFolder;

    public GameObject finishPage;
    public Text text_Score;

    public bool canShoot { get { return (arrow_count > 0 && !isRelaxing); } }

    [Header("組間休息")]
    public RelaxPage relaxPage;
    public float relaxTime = 0f;
    public bool isRelaxing = false;
 

    void Start()
    {
        Reset();
    }

    public void Record(int _score)
    {
        score += _score;
        text_score.text = score.ToString();

        if (arrow_count <= 0)
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
        arrow_count = arrow_init_count;

        text_score.text = "0";

        for (int i = 0; i < arrowsFolder.childCount; i++)
        {
            if (i < arrow_count) arrowsFolder.GetChild(i).gameObject.SetActive(true);
            else arrowsFolder.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OnArrowInactive()
    {
        if (relaxTime > 0f && arrow_count > 0)
        {
            //組間休息
            StartCoroutine(Relax());
        }
    }

    IEnumerator Relax() 
    {
        yield return new WaitForSeconds(1f);

        relaxPage.gameObject.SetActive(true);
        relaxPage.StartRelax(relaxTime);

        isRelaxing = true;
        yield return new WaitForSeconds(relaxTime);
        isRelaxing = false;
    }
}
