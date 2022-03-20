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

        //StartCoroutine(UpdateWebcamPhoto());
    }

    void OnDestroy()
    {
        webCamTexture.Stop();
    }

    //IEnumerator UpdateWebcamPhoto()  // Start this Coroutine on some button click
    //{
    //    while (true)
    //    {
    //        yield return new WaitForEndOfFrame();

    //        // it's a rare case where the Unity doco is pretty clear,
    //        // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
    //        // be sure to scroll down to the SECOND long example on that doco page 

    //        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
    //        photo.SetPixels(webCamTexture.GetPixels());
    //        photo.Apply();

    //        //Encode to a PNG
    //        photoBytes = photo.EncodeToJPG();

    //        ////Write out the jpg. Of course you have to substitute your_path for something sensible
    //        //File.WriteAllBytes("./" + "photo.jpg", photoBytes);
    //    }
    //}

    void Update()
    {
        Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
        photo.SetPixels(webCamTexture.GetPixels());
        photo.Apply();

        //Encode to a PNG
        photoBytes = photo.EncodeToJPG();
    }

    public byte[] GetImageBytes()
    {
        return photoBytes;
    }
}