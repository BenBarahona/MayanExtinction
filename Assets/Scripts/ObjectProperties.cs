using UnityEngine;
using System.Collections;

public class ObjectProperties : MonoBehaviour {

	//Min and max values for camera movement
	
	public bool isPuzzle;

	public CustomCameraScript customCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setValuesFromProperties(ObjectProperties newProp, Transform newTarget)
	{

		this.customCamera = newProp.customCamera;

		float offsetHorizontal = 0.0f;
		float offsetVertical = 0.0f;
		if (newTarget.parent) {
			offsetHorizontal = newTarget.parent.eulerAngles.y;
			offsetVertical = newTarget.parent.eulerAngles.x;
		}

		this.customCamera = newProp.customCamera;
	}
}
