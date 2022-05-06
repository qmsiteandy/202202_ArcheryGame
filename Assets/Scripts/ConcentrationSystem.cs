using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class ConcentrationSystem : MonoBehaviour
{
    [Header("Azure Endpoints and Secrets")]
    private string[] baseEndpoint =
    {
        "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True",  //此服務在本帳中
        "https://japaneast.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True", //此服務在本帳中
        "https://japanwest.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True", //此服務在本帳中
        "https://koreacentral.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True",    //此服務在本帳中
        "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True",  //此服務在碩士帳中
        "https://japaneast.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True", //此服務在碩士帳中
        "https://japanwest.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True", //此服務在碩士帳中
        "https://koreacentral.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True",    //此服務在碩士帳中
        "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True",  //此服務在潘帳中
        "https://japaneast.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True", //此服務在潘帳中
        "https://japanwest.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True", //此服務在潘帳中
        "https://koreacentral.api.cognitive.microsoft.com/face/v1.0/detect?overload=stream&returnFaceAttributes=emotion&returnFaceLandmarks=True",    //此服務在潘帳中
    };
    private string[] clientSecret =
    {
        "4b66c477f57a4f208990124e15adb976", //本帳中eastasia
        "8a547d53bc2249b68f1c5206b8c82d85", //本帳中japaneast
        "e1fe91cd774c4138a1b5661d72b1cfa1", //本帳中japanwest
        "1f45fb1a1eae40128e807a6d152fdfa3", //本帳中koreacentral
        "0f78ef77914d4299943e3391ecedad9e", //碩士帳中eastasia
        "1ee55e4982964241a5a95597d451bfa4", //碩士帳中japaneast
        "8d1660db1bb64216a00a70e60704962c", //碩士帳中japanwest
        "4f9b8562b3a345b38a0727337452c938", //碩士帳中koreacentral
        "2eb8f2660c6b4751a9ca3b4c834f6054", //潘帳中eastasia
        "6587b3ef32164f8c9fe4503b69b6da37", //潘帳中japaneast
        "1caca48a14b342538dba5554d2c3a11f", //潘帳中japanwest
        "9c15571866434b3db58a5357d14b53ed", //潘帳中koreacentral
    };
    private int faceApiIndex = 0;

    [Header("Azure Detection")]
    [SerializeField] public AzureFaceResponse azureFaceResponse = new AzureFaceResponse();
    private float detectInterval = 0.2f;    //偵測週期，考量Azure免費方案有呼叫API頻率限制
    private float detectIntervalTimer = 0f;

    [Header("Emotion")]
    public float emotionValue = 0f;
         
    [Header("Gaze Detection")]
    private Vector2 gazePos;    //目前凝視位置
    private Vector2 pupilMidPos;  //最新的位置
    private float gazeMoveDist = 0f;
    private float thresholdDistance = 7f;   //誤差距離閾值，若瞬間位移超過此值，判定可能是數值誤差
    private float errorThresholdTime = 0.1f;   //誤差判定時間，若誤差持續時間小於此值，判定是誤差
    private float errorThresholdTimer = 0f;
    private float gazeHoldingTime = 1f;   //凝視時間，大於此閾值才算有專注
    private float gazeHoldingTimer = 0f;
    private bool isGazeHolding = false; //偵測是否判斷為凝視狀態

    [Header("Concentration Detect")]
    public bool isFocus = false;
    public string statusSring = "";
    [Range(0, 1)] public float concentration = 0f;    //專注度
    public float concentrationRecoveryTime = 5f;    //從0->1慢慢增加的時間
    private float lastUpdateTime = 0f;
    public float updateCostTime = 0f;
    private List<Coroutine> detect_Routine = new List<Coroutine>();

    [Header("UI")]
    public GameObject faceDetectUI;
    public Text t_header, t_console1, t_console2;
    public Outline outline;

    [Header("Player")]
    public ShootController shootController;

    [Header("Record")]
    public DataRecorder dataRecorder;

    private void Start()
    {
        FocusDetectionReset();
    }

    private void Update()
    {
        //每N秒呼叫一次API，等API回傳後立即運算專注度
        if(shootController.isArcheryProcess && detectIntervalTimer < Time.time)
        {
            detectIntervalTimer = Time.time + detectInterval;
            detect_Routine.Add(StartCoroutine(FocusDetect()));
        }


        if (shootController.isArcheryProcess && isFocus)
        {
            //專注狀態，慢慢增加專注度
            if (concentration < 1f) concentration += 1f / concentrationRecoveryTime * Time.deltaTime;
            if (concentration > 1f) concentration = 1f;
        }
        else
        {
            concentration = 0f;
        }
            
        //設定外框顏色
        outline.effectColor = Color.Lerp(Color.red, Color.green, concentration);
        //設定Header文字
        //t_header.text = concentration < 1 ? $"專心度提升中：{concentration}" : $"專心度MAX：1";
    }

    IEnumerator FocusDetect()
    {
        //用來計算專注度更新週期
        float FocusDetectStartTime = Time.time;

        //取得鏡頭畫面並呼叫AZURE偵測
        byte[] imageBytes = this.GetComponent<WebCamController>().GetImageBytes();
        if (imageBytes == null)
        {
            Debug.LogError("imageBytes is null");
        }
        else
        {
            #region -----呼叫Face API-----

            //取得輪流使用的endpoint&secret
            string[] faceApiAndSecret = GetFaceApiAndSecret();

            WWWForm webForm = new WWWForm();
            using (UnityWebRequest www = UnityWebRequest.Post(faceApiAndSecret[0], webForm))
            {
                www.SetRequestHeader("Ocp-Apim-Subscription-Key", faceApiAndSecret[1]);
                www.SetRequestHeader("Content-Type", "application/octet-stream");
                www.uploadHandler.contentType = "application/json";
                www.uploadHandler = new UploadHandlerRaw(imageBytes);
                www.downloadHandler = new DownloadHandlerBuffer();

                www.SendWebRequest();

                while (!www.isDone)
                    yield return null;

                if (www.isNetworkError)
                {
                    Debug.LogError($"statue:{www.responseCode} error:{www.error}");
                    ShowAzureError(www.responseCode, www.error);
                }

                if (www.isDone)
                {
                    if (www.responseCode / 100 == 2)
                    {
                        string data = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                        //Debug.Log($"statue:{www.responseCode} data:{data}");

                        azureFaceResponse = JsonUtility.FromJson<AzureFaceResponse>("{\"faceList\":" + data + "}");

                        ShowAzureResponse(azureFaceResponse);
                    }
                    else
                    {
                        Debug.LogError($"statue:{www.responseCode} error:{www.error}");
                        ShowAzureError(www.responseCode, www.error);

                        //azureFaceResponse = null;

                        //重置最後更新時間
                        lastUpdateTime = 0f;
                    }
                }
            }
            #endregion -----呼叫Face API-----

            #region -----專注度判斷-----

            if (azureFaceResponse != null)
            { 
                //沒有偵測到人臉
                if (azureFaceResponse.faceList.Length == 0)
                {
                    statusSring = "No Face";
                    //設定Header文字
                    t_header.text = "偵測不到臉部";

                    //設定專注狀態
                    isFocus = false;
                }
                else
                {
                    //依權重計算表情專注分數
                    emotionValue = azureFaceResponse.faceList[0].faceAttributes.emotion.neutral * 0.9f +
                        azureFaceResponse.faceList[0].faceAttributes.emotion.happiness * 0.6f +
                        azureFaceResponse.faceList[0].faceAttributes.emotion.surprise * 0.6f +
                        azureFaceResponse.faceList[0].faceAttributes.emotion.sadness * 0.3f +
                        azureFaceResponse.faceList[0].faceAttributes.emotion.disgust * 0.2f +
                        azureFaceResponse.faceList[0].faceAttributes.emotion.anger * 0.25f +
                        azureFaceResponse.faceList[0].faceAttributes.emotion.fear * 0.3f;

                    //表情偵測為不專心(0.8是自己測試的數值)
                    if (emotionValue < 0.8f)
                    {
                        statusSring = "Face Emotion Unfocus";
                        //設定Header文字
                        t_header.text = "情緒偵測為不專心";

                        //設定專注狀態
                        isFocus = false;
                    }
                    else
                    {
                        //凝視位置取左右瞳孔位置中點
                        pupilMidPos = new Vector2(
                            (azureFaceResponse.faceList[0].faceLandmarks.pupilLeft.x + azureFaceResponse.faceList[0].faceLandmarks.pupilRight.x) * 0.5f,
                            (azureFaceResponse.faceList[0].faceLandmarks.pupilLeft.y + azureFaceResponse.faceList[0].faceLandmarks.pupilRight.y) * 0.5f);

                        if (gazePos == Vector2.zero) { gazePos = pupilMidPos; }
                        else
                        {
                            //Debug.Log((pupilMidPos - gazePos).magnitude + ", " + (pupilMidPos - gazePos).sqrMagnitude);
                            gazeMoveDist = (pupilMidPos - gazePos).magnitude;

                            //瞳孔位移過大，但可能是誤差，需再判斷
                            if (gazeMoveDist > thresholdDistance)
                            {
                                //計時是否持續，若持續時間超過閾值，代表此位移為真
                                if (errorThresholdTimer == 0f) errorThresholdTimer = Time.time + errorThresholdTime;
                                else
                                {
                                    //確定為位移
                                    if (Time.time >= errorThresholdTimer)
                                    {
                                        //重置errorThresholdTimer
                                        errorThresholdTimer = 0f;
                                        //重置凝視狀態
                                        isGazeHolding = false;
                                        statusSring = "Pupils move too fast";
                                        //設定Header文字
                                        t_header.text = "瞳孔位移過快不專心";
                                        //紀錄新的瞳孔位置
                                        gazePos = pupilMidPos;
                                        //設定專注狀態
                                        isFocus = false;
                                    }
                                }
                            }
                            //瞳孔位移在允許範圍
                            else
                            {
                                //重置errorThresholdTimer
                                errorThresholdTimer = 0f;
                                //紀錄新的瞳孔位置
                                gazePos = pupilMidPos;

                                //設定凝視判斷的計時器，需要持續凝視超過秒數才算專注
                                if (!isGazeHolding)
                                {
                                    //開始倒數
                                    gazeHoldingTimer = Time.time + gazeHoldingTime;
                                    isGazeHolding = true;
                                }
                                else
                                {
                                    //如果Gaze還沒持續一段時間，還不會被判斷為專注
                                    if (Time.time < gazeHoldingTimer)
                                    {
                                        //設定Header文字
                                        t_header.text = $"瞳孔位移過快不專心{gazeHoldingTimer}";
                                    }
                                    else
                                    {
                                        //設定專注狀態
                                        isFocus = true;
                                        statusSring = "Focus";
                                        t_header.text = "";
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion  -----專注度判斷-----
        }


        if(lastUpdateTime != 0f)
        {
            //Debug.Log($"專注度更新時間 { Time.time - lastUpdateTime} 秒" );
            updateCostTime = Time.time - lastUpdateTime;
        }
        lastUpdateTime = Time.time;

        //紀錄
        if(dataRecorder != null)
        {
            dataRecorder.OnUpdateConcentrationData(azureFaceResponse, emotionValue, gazeMoveDist, isFocus, concentration, statusSring, updateCostTime);
        }
    }

    public void FocusDetectionStart()
    {
        //開啟介面
        faceDetectUI.SetActive(true);
        outline.enabled = true;

        //紀錄
        if (dataRecorder != null)
        {
            dataRecorder.OnUpdateConcentrationData(azureFaceResponse, emotionValue, gazeMoveDist, isFocus, concentration, statusSring, updateCostTime);
        }
    }

    public void FocusDetectionReset()
    {
        //關閉介面
        faceDetectUI.SetActive(false);
        outline.enabled = false;

        //設定專注狀態
        isFocus = false;
        statusSring = "";
        concentration = 0f;

        //重置最後更新時間
        lastUpdateTime = 0f;

        //停止routine
        foreach (Coroutine routine in detect_Routine)
        {
            if (routine != null) StopCoroutine(routine);
        }
        detect_Routine = new List<Coroutine>();

        //重置眼神紀錄
        gazeMoveDist = 0f;
        gazePos = Vector2.zero;

        //紀錄
        if (dataRecorder != null)
        {
            dataRecorder.OnUpdateConcentrationData(azureFaceResponse, emotionValue, gazeMoveDist, isFocus, concentration, statusSring, updateCostTime);
        }
    }

    private void ShowAzureResponse(AzureFaceResponse azureFaceResponse)
    {
        #region //設定UI顯示內容

        int count = azureFaceResponse.faceList.Length; 

        if (count > 0)
        {
            string emotions = string.Empty;
            emotions += $"Anger: {azureFaceResponse.faceList[0].faceAttributes.emotion.anger} \n";
            emotions += $"Contempt: {azureFaceResponse.faceList[0].faceAttributes.emotion.contempt} \n";
            emotions += $"Disgust: {azureFaceResponse.faceList[0].faceAttributes.emotion.disgust} \n";
            emotions += $"Fear: {azureFaceResponse.faceList[0].faceAttributes.emotion.fear} \n";
            emotions += $"Happiness: {azureFaceResponse.faceList[0].faceAttributes.emotion.happiness} \n";
            emotions += $"Neutral: {azureFaceResponse.faceList[0].faceAttributes.emotion.neutral} \n";
            emotions += $"Sadness: {azureFaceResponse.faceList[0].faceAttributes.emotion.sadness} \n";
            emotions += $"Surprise: {azureFaceResponse.faceList[0].faceAttributes.emotion.surprise} \n";
            t_console1.text = emotions;

            string s_landmark = string.Empty;
            s_landmark += $"pupilLeft:[{azureFaceResponse.faceList[0].faceLandmarks.pupilLeft.x},{azureFaceResponse.faceList[0].faceLandmarks.pupilLeft.y}] \n";
            s_landmark += $"pupilRight:[{azureFaceResponse.faceList[0].faceLandmarks.pupilRight.x},{azureFaceResponse.faceList[0].faceLandmarks.pupilRight.y}] \n";
            t_console2.text = s_landmark;
        }

        #endregion
    }

    private void ShowAzureError(long responseCode, string error)
    {
        t_header.text = $"偵測失敗錯誤代碼{responseCode}";
        t_console1.text = error;
        t_console2.text = string.Empty;
    }


    //取得輪流使用的endpoint&secret
    private string[] GetFaceApiAndSecret()
    {
        string _baseEndpoint = baseEndpoint[faceApiIndex];
        string _clientSecret = clientSecret[faceApiIndex];
        faceApiIndex = faceApiIndex + 1 >= baseEndpoint.Length ? faceApiIndex = 0 : faceApiIndex + 1;
        return new string[] { baseEndpoint[faceApiIndex], clientSecret[faceApiIndex] };
    }


}
