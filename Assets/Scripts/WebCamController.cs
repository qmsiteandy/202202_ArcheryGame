using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class WebCamController : MonoBehaviour
{
    public RawImage UIRawImage;
    private WebCamTexture webCamTexture;
    private byte[] photoBytes;

    void Start()
    {
        webCamTexture = new WebCamTexture();
        UIRawImage.texture = webCamTexture;  //set webcam image on panel
        webCamTexture.Play();
    }

    void OnDestroy()
    {
        webCamTexture.Stop();
    }

    //呼叫取得新的畫面Bytes
    public byte[] GetImageBytes()
    {
        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        //Encode to a PNG
        photoBytes = photo.EncodeToJPG();

        return photoBytes;
    }
}