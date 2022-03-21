using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToNextScene : MonoBehaviour
{
    public void NextScene()
    {
        GameObject.FindObjectOfType<GameManager>().NextScene();
    }

    public void ReloadScene()
    {
        GameObject.FindObjectOfType<GameManager>().ReloadScene();
    }
}
