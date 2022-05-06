using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelaxPage : MonoBehaviour
{
    public Text text_second;
    public Slider TimeSlider;
    public GameObject text_startNote;

    private bool isRelaxing = false;


    // Update is called once per frame
    void Update()
    {
        if (!isRelaxing && Input.GetKeyDown(KeyCode.Space)) this.gameObject.SetActive(false);
    }

    public void StartRelax(float relaxTime)
    {
        text_second.text = "" + relaxTime;

        isRelaxing = true;
        text_startNote.SetActive(false);

        StartCoroutine(Relaxing(relaxTime));
    }

    IEnumerator Relaxing(float relaxTime)
    {
        float timer = relaxTime;

        do
        {
            timer -= Time.deltaTime;
            TimeSlider.value = timer / relaxTime;

            yield return null;

        } while (timer >= 0f);

        isRelaxing = false;
        text_startNote.SetActive(true);
    }
}
