using UnityEngine;
using System.Collections;

public class MainCameraScript : MonoBehaviour {

	// Use this for initialization
	[HideInInspector] public CameraOrbitScript currentCamera;
	private Camera _camera;

	private float changeTime;

	private bool isSmoothChanging;
	
	private	Vector3 startPosition;
	private	Quaternion startRotation;
	private float startFOV;
	private float startOrtho;
	private	float startTime;
	private float startFocalDistance;

	void Start () {

	}

	void Awake() {
		if (GetComponent <Camera>())
		{
			_camera = GetComponent <Camera>();
		}
	}
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate () {
		if (currentCamera && !isSmoothChanging) {
			if(!currentCamera.enabled){
				currentCamera.enabled = true;
			}
			transform.rotation = currentCamera.transform.rotation;
			transform.position = currentCamera.transform.position;
			_camera.fieldOfView = currentCamera._camera.fieldOfView;
		} else {
			if (Time.time < startTime + changeTime)
			{
				transform.position = Toolbox.Lerp (startPosition, currentCamera.transform.position, Toolbox.Interpolate (startTime, changeTime, true)); 
				transform.rotation = Toolbox.Lerp (startRotation, currentCamera.transform.rotation, Toolbox.Interpolate (startTime, changeTime, true));
				
				_camera.fieldOfView = Toolbox.Lerp (startFOV, currentCamera._camera.fieldOfView, Toolbox.Interpolate (startTime, changeTime, true));
				_camera.orthographicSize = Toolbox.Lerp (startOrtho, currentCamera._camera.orthographicSize, Toolbox.Interpolate (startTime, changeTime, true));
			}
			else
			{
				Toolbox.Instance.endTransition();
				isSmoothChanging = false;
				if(currentCamera != null){
					currentCamera.enabled = true;
				}
			}
		}
	}

	public void SetGameCamera(CameraOrbitScript newCamera, float transitionTime){
		if (newCamera == null) {
			return;
		}

		_camera.ResetProjectionMatrix ();
		if (currentCamera != null) {
			currentCamera.enabled = false;
		}
		//newCamera.ResetRotation ();
		currentCamera = newCamera;

		_camera.farClipPlane = currentCamera._camera.farClipPlane;
		_camera.nearClipPlane = currentCamera._camera.nearClipPlane;
		_camera.orthographic = currentCamera._camera.orthographic;

		if (transitionTime > 0f)
		{
			SmoothChange (transitionTime);
		}
		else if (currentCamera != null)
		{
			SnapToAttached ();
		}
	}

	private void SmoothChange(float transitionTime)
	{
		isSmoothChanging = true;

		startTime = Time.time;
		changeTime = transitionTime;
		
		startPosition = transform.position;
		startRotation = transform.rotation;
		startFOV = _camera.fieldOfView;
		startOrtho = _camera.orthographicSize;
	}

	private void SnapToAttached()
	{
		Debug.Log("Snap to Attached");
		
		if (currentCamera && currentCamera._camera) {
			currentCamera.enabled = true;
			isSmoothChanging = false;
			
			_camera.orthographic = currentCamera._camera.orthographic;
			_camera.fieldOfView = currentCamera._camera.fieldOfView;
			_camera.orthographicSize = currentCamera._camera.orthographicSize;
			transform.position = currentCamera.transform.position;
			transform.rotation = currentCamera.transform.rotation;
		}
	}
}