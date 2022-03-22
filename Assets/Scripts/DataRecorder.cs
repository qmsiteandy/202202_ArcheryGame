using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataRecorder : MonoBehaviour
{
	private float mwConnectedTime = 0f;

	private string testMode = "";
	private string testSys = "";

	public ConcentrationSystem concentrationSystem;
	public ShootController shootController;
	private bool[] archeryProcess = new bool[50000];
	private bool[] isFocus = new bool[50000];
	private float[] concentration = new float[50000];

	private MindwaveDataESenseModel[] eSenseData = new MindwaveDataESenseModel[50000];

	private float[] delta = new float[50000];
	private float[] theta = new float[50000];
	private float[] lowAlpha = new float[50000];
	private float[] highAlpha = new float[50000];
	private float[] lowBeta = new float[50000];
	private float[] highBeta = new float[50000];
	private float[] lowGamma = new float[50000];
	private float[] highGamma = new float[50000];
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
		gameTime[dataCount] = Time.time - mwConnectedTime;
		datetime[dataCount] = DateTime.Now.ToString("yyyyMMdd-HHmmss");

		//紀錄原始數值
		delta[dataCount] = _Data.eegPower.delta;
		theta[dataCount] = _Data.eegPower.theta;
		lowAlpha[dataCount] = _Data.eegPower.lowAlpha;
		highAlpha[dataCount] = _Data.eegPower.highAlpha;
		lowBeta[dataCount] = _Data.eegPower.lowBeta;
		highBeta[dataCount] = _Data.eegPower.highBeta;
		lowGamma[dataCount] = _Data.eegPower.lowGamma;
		highGamma[dataCount] = _Data.eegPower.highGamma;

		//紀錄數值比例
		//delta[dataCount] = m_Calibrator.EvaluateRatio(Brainwave.Delta, _Data.eegPower.delta);
		//theta[dataCount] = m_Calibrator.EvaluateRatio(Brainwave.Theta, _Data.eegPower.theta);
		//lowAlpha[dataCount] = m_Calibrator.EvaluateRatio(Brainwave.LowAlpha, _Data.eegPower.lowAlpha);
		//highAlpha[dataCount] = m_Calibrator.EvaluateRatio(Brainwave.HighAlpha, _Data.eegPower.highAlpha);
		//lowBeta[dataCount] = m_Calibrator.EvaluateRatio(Brainwave.LowBeta, _Data.eegPower.lowBeta);
		//highBeta[dataCount] = m_Calibrator.EvaluateRatio(Brainwave.HighBeta, _Data.eegPower.highBeta);
		//lowGamma[dataCount] = m_Calibrator.EvaluateRatio(Brainwave.LowGamma, _Data.eegPower.lowGamma);
		//highGamma[dataCount] = m_Calibrator.EvaluateRatio(Brainwave.HighGamma, _Data.eegPower.highGamma);

		archeryProcess[dataCount] = shootController.isArcheryProcess;
        if (concentrationSystem != null)
        {
			isFocus[dataCount] = concentrationSystem.isFocus;
			concentration[dataCount] = concentrationSystem.concentration;
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
			stream.WriteLine("gameTime,datetime,mw_attention,mw_meditation,Delta,Theta,Low Alpha,High Alpha,Low Beta,High Beta,Low Gamma,High Gamma,sys_archeryProcess,sys_isFocus,sys_concentration");

			for (int i = 0; i < dataCount; ++i)
			{
				stream.WriteLine(gameTime[i]
					+ "," + datetime[i]
					+ "," + eSenseData[i].attention
					+ "," + eSenseData[i].meditation
					+ "," + delta[i]
					+ "," + theta[i]
					+ "," + lowAlpha[i]
					+ "," + highAlpha[i]
					 + "," + lowBeta[i]
					 + "," + highBeta[i]
					 + "," + lowGamma[i]
					 + "," + highGamma[i]
					 + "," + archeryProcess[i]
					 + "," + isFocus[i]
					 + "," + concentration[i]);
			}
		}
	}
}
