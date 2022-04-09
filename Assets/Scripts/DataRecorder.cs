using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataRecorder : MonoBehaviour
{
	public float mwConnectedTime = 0f;

	private string testMode = "";
	private string testSys = "";

	[Header("專注度")]
	public ConcentrationSystem concentrationSystem;
	public ShootController shootController;
	private bool[] archeryProcess = new bool[50000];
	private bool[] isFocus = new bool[50000];
	private float[] concentration = new float[50000];
	private string[] statusSring = new string[50000];

	[Header("臉部資訊")]
	private float[] emotionValue = new float[50000];
	private float[] neutral = new float[50000];
	private float[] happiness = new float[50000];
	private float[] surprise = new float[50000];
	private float[] sadness = new float[50000];
	private float[] disgust = new float[50000];
	private float[] anger = new float[50000];
	private float[] fear = new float[50000];
	private float[] pupilMidPosX = new float[50000];
	private float[] pupilMidPosY = new float[50000];
	private float[] gazePosX = new float[50000];
	private float[] gazePosY = new float[50000];

	[Header("腦波")]
	private MindwaveDataESenseModel[] eSenseData = new MindwaveDataESenseModel[50000];

	[Header("時間紀錄")]
	public GameTimer gameTimer;
	private float[] gameTime = new float[50000];
	private string[] datetime = new string[50000];



	[SerializeField]
	public MindwaveCalibrator m_Calibrator = null;

	public int dataCount = 0;

	private void Start()
	{
		MindwaveManager.Instance.Controller.OnUpdateMindwaveData += OnUpdateMindwaveData;

		testMode = FindObjectOfType<GameManager>().mode;
		testMode = testMode == "" ? "noMode" : testMode;

		testSys = concentrationSystem == null ? "noSys" : "withSys";
	}

	public void OnUpdateMindwaveData(MindwaveDataModel _Data)
	{
		if (mwConnectedTime == 0f) mwConnectedTime = Time.time;



		eSenseData[dataCount] = _Data.eSense;
		gameTime[dataCount] = gameTimer.timer;
		datetime[dataCount] = DateTime.Now.ToString("yyyyMMdd-HHmmss");

		

		archeryProcess[dataCount] = shootController.isArcheryProcess;
        if (concentrationSystem != null)
        {
			isFocus[dataCount] = concentrationSystem.isFocus;
			concentration[dataCount] = concentrationSystem.concentration;
			emotionValue[dataCount] = concentrationSystem.emotionValue;
			if(concentrationSystem.azureFaceResponse.faceList.Length > 0)
            {
				neutral[dataCount] = concentrationSystem.azureFaceResponse.faceList[0].faceAttributes.emotion.neutral;
				happiness[dataCount] = concentrationSystem.azureFaceResponse.faceList[0].faceAttributes.emotion.happiness;
				surprise[dataCount] = concentrationSystem.azureFaceResponse.faceList[0].faceAttributes.emotion.surprise;
				sadness[dataCount] = concentrationSystem.azureFaceResponse.faceList[0].faceAttributes.emotion.sadness;
				disgust[dataCount] = concentrationSystem.azureFaceResponse.faceList[0].faceAttributes.emotion.disgust;
				anger[dataCount] = concentrationSystem.azureFaceResponse.faceList[0].faceAttributes.emotion.anger;
				fear[dataCount] = concentrationSystem.azureFaceResponse.faceList[0].faceAttributes.emotion.fear;
				pupilMidPosX[dataCount] = concentrationSystem.pupilMidPos.x;
				pupilMidPosY[dataCount] = concentrationSystem.pupilMidPos.y;
				gazePosX[dataCount] = concentrationSystem.gazePos.x;
				gazePosY[dataCount] = concentrationSystem.gazePos.y;
			}
			statusSring[dataCount] = concentrationSystem.statusSring;
		}


		dataCount += 1;
	}

	void OnDisable()
    {
		WriteCsv();
	}

	public void WriteCsv()
	{
		string path = $"./data_{testMode}_{DateTime.Now.ToString("yyyyMMdd-HHmm")}_{testSys}.csv";

		if (!File.Exists(path)) File.Create(path).Dispose();

		using (StreamWriter stream = new StreamWriter(path))
		{
			stream.WriteLine("gameTime," +
				"datetime," +
				"mw_attention," +
				"mw_meditation," +
				"sys_archeryProcess," +
				"face_emotionValue," +
				"face_neutral," +
				"face_happiness," +
				"face_surprise," +
				"face_sadness," +
				"face_disgust," +
				"face_anger," +
				"face_fear," +
				"face_pupilMidPosX," +
				"face_pupilMidPosY," +
				"gazePosX," +
				"gazePosY," +
				"sys_state," +
				"sys_isFocus," +
				"sys_concentration");

			for (int i = 0; i < dataCount; ++i)
			{
				stream.WriteLine(gameTime[i]
					+ "," + datetime[i]
					+ "," + eSenseData[i].attention
					+ "," + eSenseData[i].meditation
					+ "," + archeryProcess[i]
					+ "," + emotionValue[i]
					+ "," + neutral[i]
					+ "," + happiness[i]
					+ "," + surprise[i]
					+ "," + sadness[i]
					+ "," + disgust[i]
					+ "," + anger[i]
					+ "," + fear[i]
					+ "," + pupilMidPosX[i]
					+ "," + pupilMidPosY[i]
					+ "," + gazePosX[i]
					+ "," + gazePosY[i]
					+ "," + statusSring[i]
					+ "," + isFocus[i]
					+ "," + concentration[i]);
			}
		}
	}
}
