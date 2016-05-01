using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vec3
{
	public float x, y, z;

	public Vec3 (float _x, float _y, float _z)
	{
		x = _x;
		y = _y;
		z = _z;
	}
}

[Serializable]
public class FrameData
{
	public double timestamp;
	public Vec3[] depthPoints;
	public byte[] colorPixels;
	public uint imageWidth;

	public FrameData (double _timestamp, Vector3[] _depthPoints, Byte[] _colorPixels, uint _imageWidth)
	{
		timestamp = _timestamp;
		depthPoints = new Vec3[_depthPoints.Length];
		colorPixels = _colorPixels;
		imageWidth = _imageWidth;

		for (int i = 0; i < _depthPoints.Length; ++i)
		{
			depthPoints[i] = new Vec3(_depthPoints[i].x, _depthPoints[i].y, _depthPoints[i].z);		
		}
	}
}

[Serializable]
public class RawDataModel : MonoBehaviour
{
	public FrameData[] framesData;

	public RawDataModel(List<FrameData> _framesData)
	{
		framesData = _framesData.ToArray();
	}
}
