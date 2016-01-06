using UnityEngine;
using System.Collections;

public class PadlockRotate : MonoBehaviour {

	public int lockAngle;
	public int lerpSpeed;
	
	Vector3 previousMousePos = Vector3.zero;
	Vector3 currentMousePos = Vector3.zero;

	Vector3 rotation;
	bool mouseDownFlag;
	float rotationAmount;
	
	bool isSmoothing;
	bool timeFlag;
	float timeStartedLocking;
	
	// Use this for initialization
	void Start () {
		mouseDownFlag = false;
		rotationAmount = 0.0f;
		rotation = new Vector3(0f,0f,90f);
	}
	
	private float ReturnSignedAngleBetweenVectors(Vector3 vectorA, Vector3 vectorB)
	{
		if (vectorA == vectorB)
			return 0f;
		
		// refVector is a 90cw rotation of vectorA
		Vector3 refVector = Vector3.Cross (vectorA, Vector3.up);
		float dotProduct = Vector3.Dot(refVector, vectorB);
		
		if (dotProduct > 0)
			return -Vector3.Angle (vectorA, vectorB);
		else if (dotProduct < 0)
			return Vector3.Angle (vectorA, vectorB);
		else
			throw new System.InvalidOperationException("the vectors are opposite");
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 mousePos;
		if (Input.touches.Length >= 1) {
			mousePos = new Vector3(0.0f, 0.0f, Input.touches[0].position.y);
		}else{
			mousePos =  new Vector3(0.0f, 0.0f, Input.mousePosition.y);
		}

		if (Input.GetMouseButtonDown(0) && !mouseDownFlag)
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				if (hit.collider.gameObject.GetInstanceID() == gameObject.GetInstanceID()) {
					previousMousePos = mousePos;
					currentMousePos = mousePos;
					
					mouseDownFlag = true;
					isSmoothing = true;
				}
			}
		}
		else if (Input.GetMouseButton(0) && mouseDownFlag)
		{
			previousMousePos = currentMousePos;
			currentMousePos = mousePos;
		}
		else if (!Input.GetMouseButton(0))
		{
			mouseDownFlag = false;
		}
		
		Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 screenPositionXY = new Vector3(screenPosition.x, 0.0f, screenPosition.y);
		Vector3 previousPositionVector = previousMousePos - screenPositionXY;
		Vector3 currentPositionVector = currentMousePos - screenPositionXY;
		
		if (previousPositionVector != -currentPositionVector)
		{
			if (mouseDownFlag) {
				rotationAmount = ReturnSignedAngleBetweenVectors (previousPositionVector, currentPositionVector);
				transform.Rotate (Vector3.down, rotationAmount);
				rotation = new Vector3(rotation.x + rotationAmount, rotation.y,rotation.z);
				if(rotation.x > 360) {
					rotation.x -=360;
				}else {
					if(rotation.x < -360){
						rotation.x += 360;
					}
				}
			} else if (isSmoothing) {
				//Slowing down the movement of the table
				float i = Time.deltaTime * lerpSpeed;
				rotationAmount = Mathf.Lerp (rotationAmount, 0.0f, i);
				rotation = new Vector3(rotation.x + rotationAmount, rotation.y,rotation.z);
				if(rotation.x > 360) {
					rotation.x -=360;
				}else {
					if(rotation.x < -360){
						rotation.x += 360;
					}
				}
				//When variable reaches this value, it means it is slowing down
				if (rotationAmount < 1 && rotationAmount > -1) {
					if (timeFlag) {
						timeFlag = false;
						timeStartedLocking = Time.time;
					}
					float timeSinceStarted = Time.time - timeStartedLocking;
					//0.5 is the time it should take to complete this movement
					float percentageComplete = timeSinceStarted / 0.5f;
					
					//Animating to lock to the nearest angle defined on the lockAngle var
					int newAngle = Mathf.RoundToInt (rotation.x / lockAngle) * lockAngle;
					Vector3 newEulerAngle = new Vector3 (newAngle, 0.0f, 90.0f);
					Quaternion newQuaternion = Quaternion.identity;
					newQuaternion.eulerAngles = newEulerAngle;
					transform.rotation = Quaternion.Lerp(transform.rotation, newQuaternion, percentageComplete);
					rotationAmount = 0.0f;
					
					if (percentageComplete >= 1.0f) {
						rotation = newEulerAngle;
						if(rotation.x > 360) {
							rotation.x -=360;
						}else {
							if(rotation.x < -360){
								rotation.x += 360;
							}
						}
						transform.rotation = newQuaternion;
						isSmoothing = false;
						timeFlag = true;
						
						this.SendMessageUpwards("cyllinderFinishedRotating");
					}
				} else {
					transform.Rotate (Vector3.down, rotationAmount);
				}
			}
		}
	}

	public int selectedValue()
	{
		Debug.Log ("Rotation: " + rotation.x);
		float angle = 0;
		if (rotation.x > 0) {
			angle = 360 - rotation.x;
		} else {
			if(rotation.x == -360){
				rotation.x = 0;
			}else{
				angle = -rotation.x;
			}
		}

		if (angle < 36) {
			return 0;
		} else if (angle < 72) {
			return 1;
		} else if (angle < 108) {
			return 2;
		} else if (angle < 144) {
			return 3;
		} else if (angle < 178) {
			return 4;
		} else if (angle < 216) {
			return 5;
		} else if (angle < 252) {
			return 6;
		} else if (angle < 288) {
			return 7;
		} else if (angle < 324) {
			return 8;
		} else {
			return 9;
		}
	}
}
