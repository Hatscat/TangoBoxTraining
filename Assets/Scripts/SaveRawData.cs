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
	public float frameTimeIntervale = 0.2f; // sec
	[Range(0.1f, 40f)]
	public float timerMax = 15f; // sec

	private Tango.TangoUnityImageData lastVideoImages;
	private List<FrameData> framesData;
	private bool record;
	private float timer;
	private string json = null;
	private string error = null;

	public void StopRecord ()
	{
		Save();
		Reset();
	}

	public void OnTangoImageAvailableEventHandler (Tango.TangoEnums.TangoCameraId cameraId, Tango.TangoUnityImageData imageBuffer)
	{
        if (imageBuffer != null)
            lastVideoImages = imageBuffer;	
	}

	private void Awake ()
	{
		//DontDestroyOnLoad(gameObject);
		Reset();
	}

	private void OnGUI ()
	{
		if (GUI.Button (new Rect(Screen.width * 0.1f, Screen.height * 0.8f, 150, 100), record ? "Stop" : "Record"))
		{
			record = !record;

			if (record)
				Capture();
			else
				Save();
		}

		if (json != null)
			GUI.Label(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.1f, Screen.height * 0.1f), json);
		else if (error != null)
			GUI.Label(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.1f, Screen.height * 0.1f), error);
	}

	private void Reset ()
	{
		lastVideoImages = null;
		framesData = new List<FrameData>();
		record = false;
		timer = 0f;
	}

	private void Save ()
	{
		RawDataModel rawData = new RawDataModel(framesData);

		File.WriteAllText(Application.persistentDataPath + "/tessst2.json", "{'test': 2}");

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

	private IEnumerator Capture ()
	{
		while (record)
		{
			if (framesData != null && tangoPointCloud != null && tangoPointCloud.m_points != null && lastVideoImages != null && lastVideoImages.data != null)
				framesData.Add(new FrameData(timer, tangoPointCloud.m_points, lastVideoImages.data, lastVideoImages.width));
			yield return new WaitForSeconds(frameTimeIntervale);
		}
	}
}
