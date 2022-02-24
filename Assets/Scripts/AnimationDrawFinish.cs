using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationDrawFinish : MonoBehaviour
{
    public UnityEvent unityEvent;
    void DrawFinish()
    {
        unityEvent.Invoke();
    }
}
