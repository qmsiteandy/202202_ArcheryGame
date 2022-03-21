using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPage : MonoBehaviour
{
    public MindwaveController mindwaveController;
    public GameObject button_start, text_waitMindwave;
    
    void Update()
    {
        if (mindwaveController.gameObject.activeInHierarchy) {
            button_start.SetActive(mindwaveController.IsConnected);
            text_waitMindwave.SetActive(!mindwaveController.IsConnected);
        }
    }
}
