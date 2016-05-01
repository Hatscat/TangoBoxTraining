using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Tango;

public class SaveRawData : MonoBehaviour, ITangoVideoOverlay
{
	public TangoPointCloud tangoPointCloud;
	[Range(0.01f, 2f)]
	public float frameTimeInterval = 0.2f; // sec
	[Range(0.1f, 40f)]
	public float timerMax = 15f; // sec

	private TangoApplication tangoApplication;
	private TangoUnityImageData lastVideoImages;
	private List<FrameData> framesData;
	private bool recording;
	private float timer;
	private string json = null;
	private string error = null;


	public void OnTangoImageAvailableEventHandler (TangoEnums.TangoCameraId cameraId, TangoUnityImageData imageBuffer)
	{
		lastVideoImages = imageBuffer;	
	}

	private void Awake ()
	{
		tangoApplication = FindObjectOfType<TangoApplication>();
		tangoApplication.Register(this);
		Reset();
	}

	private void OnGUI ()
	{
		if (GUI.Button(new Rect(Screen.width * 0.02f, Screen.height * 0.4f, Screen.width * 0.2f, Screen.height * 0.2f), recording ? "<size=40>Stop</size>" : "<size=40>Record</size>"))
		{
			recording = !recording;

			if (recording)
				StartCoroutine(Record());
			else
				StopRecord();
		}
		// Debug
		if (json != null)
			GUI.Label(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f), json);
		else if (error != null)
			GUI.Label(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f), error);
	}

	private void Reset ()
	{
		lastVideoImages = null;
		framesData = new List<FrameData>();
		recording = false;
		timer = 0f;
	}

	private void Save ()
	{
		// too heavy to save....
	
		RawDataModel rawData = new RawDataModel(framesData);

		try
		{
			json = JsonUtility.ToJson(rawData);
		}
		catch (Exception e)
		{
			error = e.ToString();
		}

        File.WriteAllText(Application.persistentDataPath + "/rawData.json", json);
	}

	private void StopRecord ()
	{
		Save();
		Reset();
	}

	private IEnumerator Record ()
	{
		while (recording && timer < timerMax)
		{
			if (framesData != null && tangoPointCloud != null && tangoPointCloud.m_points != null && lastVideoImages != null && lastVideoImages.data != null)
				framesData.Add(new FrameData(timer, tangoPointCloud.m_points, lastVideoImages.data, lastVideoImages.width));
			else
				error = "frameData: " + (framesData != null) +
					"\ntangoPointCloud:" + (tangoPointCloud != null) +
					"\ntangoPointCloud.m_points:" + (tangoPointCloud != null && tangoPointCloud.m_points != null) +
					"\nlastVideoImages:" + (lastVideoImages != null) +
					"\nlastVideoImages.data:" + (lastVideoImages != null && lastVideoImages.data != null);

			yield return new WaitForSeconds(frameTimeInterval);
			timer += frameTimeInterval;
		}
	}
}
