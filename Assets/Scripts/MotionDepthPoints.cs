using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tango;

public class MotionDepthPoints : MonoBehaviour
{
	public TangoPointCloud tangoPointCloud;
	public GameObject sphereModel;
	[Range(0.01f, 2f)]
	public float frameTimeInterval = 0.2f; // sec
	[Range(0.1f, 40f)]
	public float timerMax = 15f; // sec
	public Material mat;
	private TangoApplication tangoApplication;

	private List<Vector3[]> depthPointsList;
	private List<GameObject> spheres;
	private bool recording;
	private float timer;
	private string error = null;
	private string msg = null;

	void Awake ()
	{
		tangoApplication = FindObjectOfType<TangoApplication>();
		DontDestroyOnLoad(gameObject);
		Reset();
	}

	private void OnGUI ()
	{
		if (GUI.Button(new Rect(Screen.width * 0.02f, Screen.height * 0.4f, Screen.width * 0.2f, Screen.height * 0.2f), recording ? "<size=40>Stop</size>" : "<size=40>Record</size>"))
		{
			tangoApplication.m_enableDepth = tangoPointCloud.enabled = tangoPointCloud.GetComponent<MeshRenderer>().enabled = recording = !recording;

			if (recording)
			{
				StartCoroutine(Record());
			}
		}
		// Debug
		if (error != null)
			GUI.Label(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f), error);
		else if (msg != null)
			GUI.Label(new Rect(Screen.width * 0.1f, Screen.height * 0.1f, Screen.width * 0.8f, Screen.height * 0.8f), msg);
	}

	private void OnPostRender ()
	{
		if (recording || depthPointsList == null || depthPointsList.Count == 0)
			return;
	
		GL.PushMatrix();
		mat.SetPass(0);
		//GL.LoadOrtho();
		GL.Begin(GL.LINES);

		for (int i = 0; i < depthPointsList.Count; i++)
		{
			GL.Color(Color.red);
			for (int j = 0; j < depthPointsList[i].Length; j++)
					GL.Vertex(depthPointsList[i][j]);
		}

		GL.End();
		GL.PopMatrix();	
	}

	private void Reset ()
	{
		depthPointsList = new List<Vector3[]>();
		recording = false;
		timer = 0f;

		if (spheres != null)
			for (int i = 0; i < spheres.Count; ++i)
				Destroy(spheres[i]);
		spheres = new List<GameObject>();
	}

	private void setMeshRender ()
	{
		for (int i = 0; i < depthPointsList.Count; ++i)
		{
			Color col = Color.Lerp(Color.blue, Color.red, (float)i / (float)depthPointsList.Count);
			for (int j = 0; j < depthPointsList[i].Length; ++j)
			{
				GameObject newSphere = Instantiate(sphereModel, depthPointsList[i][j], Quaternion.identity) as GameObject;
				newSphere.GetComponent<Renderer>().material.color = col;
				spheres.Add(newSphere);
				
			}
		}
	}

	private IEnumerator Record ()
	{
		while (recording && timer < timerMax)
		{
			if (tangoPointCloud != null && tangoPointCloud.enabled && tangoPointCloud.m_points != null)
				depthPointsList.Add(tangoPointCloud.m_points);

			yield return new WaitForSeconds(frameTimeInterval);
			timer += frameTimeInterval;
			msg = "TIMER: " + timer;
		}

		Reset();
	}
}
