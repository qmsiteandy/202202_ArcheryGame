using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class WebCamController : MonoBehaviour
{
    public float updateInteval = 0.1f;
    public RawImage UIRawImage;
    private WebCamTexture webCamTexture;
    private byte[] photoBytes;
    private Coroutine WebCamUpdateRouting;

    void Start()
    {
        webCamTexture = new WebCamTexture();
        UIRawImage.texture = webCamTexture;  //set webcam image on panel
        webCamTexture.Play();

        WebCamUpdateRouting = StartCoroutine(WebCamUpdate());
    }

    void OnDestroy()
    {
        webCamTexture.Stop();

        StopCoroutine(WebCamUpdateRouting);
        WebCamUpdateRouting = null;
    }

    void Update()
    {
        //Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        //photo.SetPixels(webCamTexture.GetPixels());
        //photo.Apply();

        //Encode to a PNG
        //photoBytes = photo.EncodeToJPG();
    }

    IEnumerator WebCamUpdate()
    {
        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        //Encode to a PNG
        photoBytes = photo.EncodeToJPG();

        yield return new WaitForSeconds(updateInteval);
        
        WebCamUpdateRouting = StartCoroutine(WebCamUpdate());
    }

    public byte[] GetImageBytes()
    {
        return photoBytes;
    }
}