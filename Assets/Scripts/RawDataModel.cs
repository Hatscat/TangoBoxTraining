using System;
using UnityEngine;

[Serializable]
public class RawDataModel : MonoBehaviour
{
	public Tango.TangoUnityImageData[] videoImages;
    public Vector3[][] depthPoints;

	/*
	public static void SaveData (Tango.TangoUnityImageData[] videoImages, Vector3[][] depthPoints)
	{
		RawDataModel rawData = new RawDataModel();

		rawData.videoImages = videoImages;
		rawData.depthPoints = depthPoints;

		string json = JsonUtility.ToJson(rawData);
	}
	*/
}
