using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataRecorder : MonoBehaviour
{
	private MindwaveDataESenseModel[] eSenseData = new MindwaveDataESenseModel[50000];
	private MindwaveDataEegPowerModel[] eegPowerData = new MindwaveDataEegPowerModel[50000];
	private float[] gameTime = new float[50000];
	private string[] datetime = new string[50000];
	private bool[] thinkFocus = new bool[50000];
	public int dataCount = 0;

	private void Start()
	{
		MindwaveManager.Instance.Controller.OnUpdateMindwaveData += OnUpdateMindwaveData;
	}

	public void OnUpdateMindwaveData(MindwaveDataModel _Data)
	{
		eSenseData[dataCount] = _Data.eSense;
		eegPowerData[dataCount] = _Data.eegPower;
		gameTime[dataCount] = Time.time;
		datetime[dataCount] = DateTime.Now.ToString("yyyyMMdd-HHmmss");

		thinkFocus[dataCount] = Input.GetKey(KeyCode.Space);

		dataCount += 1;
	}

	void OnDisable()
    {
		WriteCsv();
	}

	public void WriteCsv()
	{
		string path = $"./data_{DateTime.Now.ToString("yyyyMMdd - HHmm")}.csv";

		if (!File.Exists(path)) File.Create(path).Dispose();

		using (StreamWriter stream = new StreamWriter(path))
		{
			stream.WriteLine("gameTime,datetime,attention,meditation,Delta,Theta,Low Alpha,High Alpha,Low Beta,High Beta,Low Gamma,High Gamma,FocusInput");

			for (int i = 0; i < dataCount; ++i)
			{
				stream.WriteLine(gameTime[i] + "," + datetime[i] + "," + eSenseData[i].attention + "," + eSenseData[i].meditation
					+ "," + eegPowerData[i].delta + "," + eegPowerData[i].theta + "," + eegPowerData[i].lowAlpha + "," + eegPowerData[i].highAlpha
					 + "," + eegPowerData[i].lowBeta + "," + eegPowerData[i].highBeta + "," + eegPowerData[i].lowGamma + "," + eegPowerData[i].highGamma
					 + "," + thinkFocus[i]);
			}
		}
	}
}
