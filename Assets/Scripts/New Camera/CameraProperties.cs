using UnityEngine;
using System.Collections;

public class CameraProperties : MonoBehaviour {
	
	public float distance = 10f;
	
	public float initialAngleX = 0f;
	public float initialAngleY = 0f;
	
	public bool limitY = true;
	public float yMinLimit = -60f;
	public float yMaxLimit = 60f;
	public float yLimitOffset = 0f;
	
	public bool limitX = false;
	public float xMinLimit = -60f;
	public float xMaxLimit = 60f;
	public float xLimitOffset = 0f;

	public float transitionTime = 1.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
