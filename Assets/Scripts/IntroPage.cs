using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPage : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.Mouse0)) this.gameObject.SetActive(false);
    }
}
