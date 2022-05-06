using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Page_InputTester : MonoBehaviour
{
    public InputField inputField;
    public Button startBtn;
    public GameObject nextPage;
    public Toggle[] toggles;

    void Start()
    {
        startBtn.onClick.AddListener(StartBtn);
    }

    // Update is called once per frame
    void StartBtn()
    {
        foreach(Toggle toggle in toggles) { if (toggle.isOn == false) return; }
        if (inputField.text == "") return;


        FindObjectOfType<GameManager>().SetTester(inputField.text);

        this.gameObject.SetActive(false);
        nextPage.SetActive(true);
    }
}
