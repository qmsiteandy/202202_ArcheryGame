using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataRecorder : MonoBehaviour
{
	[Header("通用資訊")]
	public float mwConnectedTime = 0f;
	private string testMode = "";
	private string testSys = "";
	public GameTimer gameTimer;

	[Header("腦波紀錄")]
	private string[] mw_datetime = new string[50000];
	private float[] mw_gameTime = new float[50000];
	private bool[] mw_archeryProcess = new bool[50000];
	private float[] mw_attention = new float[50000];
	private float[] mw_updateCostTime = new float[50000];
	public int mw_dataCount = 0;

	[Header("專注度")]
	public ConcentrationSystem concentrationSystem;
	public ShootController shootController;
	private string[] sys_datetime = new string[50000];
	private float[] sys_gameTime = new float[50000];
	private bool[] sys_archeryProcess = new bool[50000];
	private bool[] sys_isFocus = new bool[50000];
	private float[] sys_concentration = new float[50000];
	private string[] sys_statusSring = new string[50000];
	private float[] sys_updateCostTime = new float[50000];
	private float[] sys_emotionValue = new float[50000];
	private float[] sys_neutral = new float[50000];
	private float[] sys_happiness = new float[50000];
	private float[] sys_surprise = new float[50000];
	private float[] sys_sadness = new float[50000];
	private float[] sys_disgust = new float[50000];
	private float[] sys_anger = new float[50000];
	private float[] sys_fear = new float[50000];
	private float[] sys_gazeMoveDist = new float[50000];
	public int sys_dataCount = 0;

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

		mw_gameTime[mw_dataCount] = gameTimer.timer;
		mw_datetime[mw_dataCount] = DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
		if (mw_dataCount > 0) mw_updateCostTime[mw_dataCount] = gameTimer.timer - mw_gameTime[mw_dataCount - 1];

		mw_archeryProcess[mw_dataCount] = shootController.isArcheryProcess;
		mw_attention[mw_dataCount] = _Data.eSense.attention;

		mw_dataCount += 1;
	}

	public void OnUpdateConcentrationData(AzureFaceResponse _azureFaceResponse, float _emotionValue, float _gazeMoveDist, bool _isFocus, float _concentration, string _statusSring, float _updateCostTime)
    {
		sys_gameTime[sys_dataCount] = gameTimer.timer;
		sys_datetime[sys_dataCount] = DateTime.Now.ToString("yyyyMMdd-HHmmssfff");
		sys_archeryProcess[sys_dataCount] = shootController.isArcheryProcess;

		if (_azureFaceResponse.faceList.Length > 0)
        {
			sys_neutral[sys_dataCount] = _azureFaceResponse.faceList[0].faceAttributes.emotion.neutral;
			sys_happiness[sys_dataCount] = _azureFaceResponse.faceList[0].faceAttributes.emotion.happiness;
			sys_surprise[sys_dataCount] = _azureFaceResponse.faceList[0].faceAttributes.emotion.surprise;
			sys_sadness[sys_dataCount] = _azureFaceResponse.faceList[0].faceAttributes.emotion.sadness;
			sys_disgust[sys_dataCount] = _azureFaceResponse.faceList[0].faceAttributes.emotion.disgust;
			sys_anger[sys_dataCount] = _azureFaceResponse.faceList[0].faceAttributes.emotion.anger;
			sys_fear[sys_dataCount] = _azureFaceResponse.faceList[0].faceAttributes.emotion.fear;

			sys_gazeMoveDist[sys_dataCount] = _gazeMoveDist;
        }
		sys_emotionValue[sys_dataCount] = _emotionValue;
		sys_isFocus[sys_dataCount] = _isFocus;
		sys_concentration[sys_dataCount] = _concentration;
		sys_statusSring[sys_dataCount] = _statusSring;
		sys_updateCostTime[sys_dataCount] = _updateCostTime;

		sys_dataCount += 1;
	}

	void OnDisable()
    {
		WriteCsv_Mindwave();
		if (concentrationSystem) WriteCsv_FaceSystem();
	}

	public void WriteCsv_Mindwave()
	{
		string path = $"./data_{testMode}_{DateTime.Now.ToString("yyyyMMdd-HHmm")}_{testSys}_Mindwave.csv";

		if (!File.Exists(path)) File.Create(path).Dispose();

		using (StreamWriter stream = new StreamWriter(path))
		{
			stream.WriteLine("gameTime," +
				"datetime," +
				"archeryProcess," +
				"mw_attention," +
				"mw_updateCostTime");

			for (int i = 0; i < mw_dataCount; ++i)
			{
				stream.WriteLine(mw_gameTime[i]
					+ "," + mw_datetime[i]
					+ "," + mw_archeryProcess[i]
					+ "," + mw_attention[i]
					+ "," + mw_updateCostTime[i]);
			}
		}
	}

	public void WriteCsv_FaceSystem()
	{
		string path = $"./data_{testMode}_{DateTime.Now.ToString("yyyyMMdd-HHmm")}_{testSys}_FaceSystem.csv";

		if (!File.Exists(path)) File.Create(path).Dispose();

		using (StreamWriter stream = new StreamWriter(path))
		{
			stream.WriteLine("gameTime," +
				"datetime," +
				"archeryProcess," +
				"face_emotionValue," +
				"face_neutral," +
				"face_happiness," +
				"face_surprise," +
				"face_sadness," +
				"face_disgust," +
				"face_anger," +
				"face_fear," +
				"sys_gazeMoveDist," +
				"sys_state," +
				"sys_isFocus," +
				"sys_concentration," +
				"sys_updateCostTime");

			for (int i = 0; i < sys_dataCount; ++i)
			{
				stream.WriteLine(sys_gameTime[i]
					+ "," + sys_datetime[i]
					+ "," + sys_archeryProcess[i]
					+ "," + sys_emotionValue[i]
					+ "," + sys_neutral[i]
					+ "," + sys_happiness[i]
					+ "," + sys_surprise[i]
					+ "," + sys_sadness[i]
					+ "," + sys_disgust[i]
					+ "," + sys_anger[i]
					+ "," + sys_fear[i]
					+ "," + sys_gazeMoveDist[i]
					+ "," + sys_statusSring[i]
					+ "," + sys_isFocus[i]
					+ "," + sys_concentration[i]
					+ "," + sys_updateCostTime[i]);
			}
		}
	}
}
