using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [Header("ConcentretionSystem")]
    public AudioSource bgm_audio;
    public AudioSource heartBeat_audio;
    public ConcentrationSystem concentrationSystem;
    private bool isConcentretionSystemActive = false;

    void Start()
    {
        //判斷是否有專注系統
        isConcentretionSystemActive = concentrationSystem.gameObject.activeInHierarchy;
        //關閉心跳音效
        heartBeat_audio.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isConcentretionSystemActive)
        {
            //越專注，BGM越小聲
            bgm_audio.volume = Mathf.Lerp(1f, 0f, concentrationSystem.concentration);
            //越專注，心跳越大聲
            heartBeat_audio.volume = Mathf.Lerp(0f, 1f, concentrationSystem.concentration);
        }
    }
}
