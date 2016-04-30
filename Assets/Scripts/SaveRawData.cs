using UnityEngine;
using System.Collections.Generic;

using Tango;

public class SaveRawData : MonoBehaviour, ITangoVideoOverlay, ITangoDepth
{
	public TangoPointCloud tangoPointCloud;

	private List<Tango.TangoUnityImageData> videoImages;
	private List<Vector3[]> depthPoints;


	public void StopRecord ()
	{
		Save();
		Reset();
	}

	public void OnTangoImageAvailableEventHandler (Tango.TangoEnums.TangoCameraId cameraId, Tango.TangoUnityImageData imageBuffer)
	{
		videoImages.Add(imageBuffer);	
	}

    public void OnTangoDepthAvailable (TangoUnityDepth tangoDepth)
	{
		if (tangoPointCloud || tangoDepth == null || tangoDepth.m_points == null)
			return;

		depthPoints.Add(tangoPointCloud.m_points);
	}

	private void Start ()
	{
		Reset();
	}

	private void Reset ()
	{
		videoImages = new List<Tango.TangoUnityImageData>();
		depthPoints = new List<Vector3[]>();
	}

	private void Save ()
	{
		RawDataModel rawData = new RawDataModel();

		rawData.videoImages = videoImages.ToArray();
		rawData.depthPoints = depthPoints.ToArray();

		string json = JsonUtility.ToJson(rawData);
	}
}
