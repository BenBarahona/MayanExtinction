using UnityEngine;
using System.Collections;

public class ObjectProperties : MonoBehaviour {

	//Min and max values for camera movement
	
	public bool isPuzzle;

	public CustomCameraScript customCamera;

	public float transitionTime = 1f;

	public bool onlyAllowChildColliders = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setValuesFromProperties(ObjectProperties newProp, Transform newTarget)
	{

		this.customCamera = newProp.customCamera;
		this.transitionTime = newProp.transitionTime;

		float offsetHorizontal = 0.0f;
		float offsetVertical = 0.0f;
		if (newTarget.parent) {
			offsetHorizontal = newTarget.parent.eulerAngles.y;
			offsetVertical = newTarget.parent.eulerAngles.x;
		}
	}
}
