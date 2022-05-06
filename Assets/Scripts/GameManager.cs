using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static private bool isDontDestroy = false;

    //場景順序
    private int sceneindex = 0;
    private string[] sceneSeq = {};

    public string mode = "";
    public string tester = "";

    void Awake()
    {
        if (!isDontDestroy)
        {
            DontDestroyOnLoad(this.gameObject);
            isDontDestroy = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ChooseSequenceMode(string _mode)
    {
        //先普通模式再專注模式
        if(_mode == "A")
        {
            mode = _mode;
            sceneSeq = new string[] { "_1_IntroScene", "_2_SceneNormal", "_S_Survey", "_3_IntroConcentration", "_4_SceneConcentration", "_5_EndSenen" };
        }
        //先專注模式再普通模式
        else if(_mode == "B")
        {
            mode = _mode;
            sceneSeq = new string[] { "_1_IntroScene", "_3_IntroConcentration", "_4_SceneConcentration", "_S_Survey", "_2_SceneNormal", "_5_EndSenen" };
        }
    }

    public void SetTester(string _tester)
    {
        tester = _tester;
    }

    public void NextScene()
    {
        if (sceneSeq.Length == 0) return;
        if(sceneindex < sceneSeq.Length)
        {
            SceneManager.LoadScene(sceneSeq[sceneindex]);
            sceneindex += 1;
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
