using UnityEngine;
using System.Collections;

public class RotateObject : Puzzle {

	public Vector3 solutionRotation;
	public int lockAngle;
	public int lerpSpeed;

	Vector3 previousMousePos = Vector3.zero;
	Vector3 currentMousePos = Vector3.zero;
	bool mouseDownFlag;
	float rotationAmount;

	bool isSmoothing;
	bool timeFlag;
	float timeStartedLocking;

	// Use this for initialization
	new void Start () {
		mouseDownFlag = false;
		rotationAmount = 0.0f;
		base.Start ();
	}
	
	private float ReturnSignedAngleBetweenVectors(Vector3 vectorA, Vector3 vectorB)
	{
		if (vectorA == vectorB)
			return 0f;
		
		// refVector is a 90cw rotation of vectorA
		Vector3 refVector = Vector3.Cross (vectorA, Vector3.up);
		float dotProduct = Vector3.Dot(refVector, vectorB);
		
		if (dotProduct > 0)
			return -Vector3.Angle(vectorA, vectorB);
		else if (dotProduct < 0)
			return Vector3.Angle(vectorA, vectorB);
		else
			throw new System.InvalidOperationException("the vectors are opposite");
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (!this.isFinished) {
			Vector3 mousePos = new Vector3 (Input.mousePosition.x, 0.0f,
		                               Input.mousePosition.y);
		
			if (Input.GetMouseButtonDown (0) && !mouseDownFlag) {
				previousMousePos = mousePos;
				currentMousePos = mousePos;
				mouseDownFlag = true;
				isSmoothing = true;
			} else if (Input.GetMouseButton (0) && mouseDownFlag) {
				previousMousePos = currentMousePos;
				currentMousePos = mousePos;
			} else if (!Input.GetMouseButton (0)) {
				mouseDownFlag = false;
			}
		
			Vector3 screenPosition = Camera.main.WorldToScreenPoint (transform.position);
			Vector3 screenPositionXY = new Vector3 (screenPosition.x, 0.0f, screenPosition.y);
			Vector3 previousPositionVector = previousMousePos - screenPositionXY;
			Vector3 currentPositionVector = currentMousePos - screenPositionXY;
		
			if (previousPositionVector != -currentPositionVector) {
				if (mouseDownFlag) {
					rotationAmount = ReturnSignedAngleBetweenVectors (previousPositionVector, currentPositionVector);
					transform.Rotate (Vector3.up, rotationAmount);
				} else if (isSmoothing) {
					//Slowing down the movement of the table
					float i = Time.deltaTime * lerpSpeed;
					rotationAmount = Mathf.Lerp (rotationAmount, 0.0f, i);
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
						int newAngle = Mathf.RoundToInt (transform.eulerAngles.y / lockAngle) * lockAngle;
						Vector3 newEulerAngle = new Vector3 (0.0f, newAngle, 0.0f);
						transform.eulerAngles = Vector3.Lerp (transform.eulerAngles, newEulerAngle, percentageComplete);
						rotationAmount = 0.0f;

						if (percentageComplete >= 1.0f) {
							transform.eulerAngles = newEulerAngle;
							isSmoothing = false;
							timeFlag = true;

							checkIfSolved ();
						}
					} else {
						transform.Rotate (Vector3.up, rotationAmount);
					}
				}
			}
		}
	}

	void checkIfSolved()
	{
		float y = transform.eulerAngles.y;
		float errorMargin = 0.1f;

		if(y <= solutionRotation.y + errorMargin && y >= solutionRotation.y - errorMargin) {
			solvePuzzle();
		}
	}

	public override void autoSolvePuzzle ()
	{
		if (!this.isFinished) {
			base.autoSolvePuzzle ();
			transform.eulerAngles = solutionRotation;
		}
	}
}