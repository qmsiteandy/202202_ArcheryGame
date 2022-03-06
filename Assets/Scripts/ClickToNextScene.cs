using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToNextScene : MonoBehaviour
{
    public void Click()
    {
        GameObject.FindObjectOfType<GameManager>().NextScene();
    }
}
