using UnityEngine;
using System.Collections;

struct DominosaRect
{
	public float originX;
	public float originY;
	public float width;
	public float height;

	public DominosaRect(float x, float y, float width, float height){
		originX = x;
		originY = y;
		this.width = width;
		this.height = height;
	}


	public DominosaRect(GameObject piece){
		if (piece != null) {
			Vector3 size = piece.GetComponentInChildren<Renderer> ().bounds.size;
			Vector3 center = piece.transform.localPosition;
			originX = center.x + size.x / 2.0f;
			originY = center.y + size.y / 2.0f;
			width = size.x;
			height = size.y;
		} else {
			originX = originY = width = height = 0f;
		}
	}

	public DominosaRect(GameObject piece, bool inverted){
		Vector3 size =  piece.GetComponentInChildren<Renderer>().bounds.size;
		Vector3 center = piece.transform.localPosition;
		if (inverted) {
			originY = center.y + size.x / 2.0f;
			originX = center.x + size.y / 2.0f;
			height = size.x;
			width = size.y;
		} else {
			originX = center.x + size.x / 2.0f;
			originY = center.y + size.y / 2.0f;
			width = size.x;
			height = size.y;
		}
	}
}

struct DominosaGridPosition
{
	public DominosaRect rect;
	public bool used;
	public int placedValue;

	public DominosaGridPosition(DominosaRect rect){
		this.rect = rect;
		used = false;
		placedValue = -1;
	}	
}

public struct DominosaArrayPosition
{
	public int i;
	public int j;

	public DominosaArrayPosition(int i, int j){
		this.i = i;
		this.j = j;
	}
}

public class DominosaScript : Puzzle {
	public bool useSmoothing;
	public int lerpSpeed;
	private GameObject rotatingObject;
	private GameObject movingObject;
	private float rotationStartTime;
	private float movementStartTime;
	private Vector3 finalRotation;
	private Vector3 finalPosition;
	
	private int[,] puzzleValues; 
	private DominosaGridPosition[,] dominosaGrid; 
	private DominosaRect draggingLimits;
	private GameObject draggedObject;

	bool mouseDownFlag;
	bool dragging;
	Vector3 previousMousePos = Vector3.zero;
	Vector3 currentMousePos = Vector3.zero;

	new void Start () {
		base.Start ();


		puzzleValues = new int[,] {{0,0,1,0,3},
									{0,2,2,2,3},
									{2,0,1,3,1},
									{2,3,3,1,1}};
									
		dominosaGrid = new DominosaGridPosition[4, 5];
		float width = 0.4f;
		float height = 0.4f;
		float originX = 1.08f;
		float originY = 0.8f;
		for (int i = 0; i < 4; i++) {
			originX = 1.08f;
			for(int j = 0; j < 5; j++){
				DominosaRect rect = new DominosaRect(originX,originY,width,height);
				dominosaGrid[i,j] = new DominosaGridPosition(rect); 
				originX -= (width + 0.03f);
			}
			originY -= (height + 0.03f);
		}

		draggingLimits = new DominosaRect (1.485f, 1.215f, 2.97f, 2.43f);

		mouseDownFlag = false;
		dragging = false;
		draggedObject = null;

		movingObject = null;
		rotatingObject = null;
	}

	void LateUpdate () {
		if (!isFinished) {
			Vector3 touchPosition = Input.mousePosition;
			#if UNITY_IOS || UNITY_ANDROID
			if (Input.touches.Length >= 1) {
				touchPosition = Input.touches [0].position;
			}
			#endif


			if (Input.GetMouseButtonDown (0) && !mouseDownFlag) {
				Ray ray = Camera.main.ScreenPointToRay (touchPosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) {
					if (hit.collider.gameObject.tag == "DominosaPiece") {
						draggedObject = hit.collider.gameObject;
						previousMousePos = touchPosition;
						currentMousePos = touchPosition;
						mouseDownFlag = true;
					}
				}
			} else if (Input.GetMouseButton (0) && mouseDownFlag) {
				previousMousePos = currentMousePos;
				currentMousePos = touchPosition;
				if (!dragging) {
					if (currentMousePos != previousMousePos) {
						dragging = true;
					}
				}
			} else if (Input.GetMouseButtonUp (0)) {
				mouseDownFlag = false;

				if (dragging) {
					Debug.Log("Dominosa piece finished moving 1");
					dominosaPieceFinishedMoving (draggedObject);
				} else {
					if (draggedObject != null) {
						if(useSmoothing){
							if(rotatingObject != null && rotatingObject.GetInstanceID() == draggedObject.GetInstanceID()){
								rotatingObject.transform.localEulerAngles = finalRotation;
								rotatingObject = null;
							}
							beginSmoothRotationForObject(draggedObject,90);
						}else{
							draggedObject.transform.Rotate (Vector3.forward, 90);
						}
						resetDominosaGridPosition (draggedObject);
						draggedObject.SendMessage ("pieceDidFinishRotate");
						Debug.Log("Dominosa piece finished moving 2");
						dominosaPieceFinishedMoving (draggedObject);
					}
				}
				draggedObject = null;
			} else {
				dragging = false;
				mouseDownFlag = false;
				draggedObject = null;
			}

			if (toolbox.isTransitioning ()) {

				if (draggedObject) {
					Debug.Log("Dominosa piece finished moving 3");
					dominosaPieceFinishedMoving (draggedObject);
				}
				mouseDownFlag = false;
				dragging = false;
				draggedObject = null;
			}

			if (dragging && mouseDownFlag) {
				if(movingObject!=null && movingObject.GetInstanceID() == draggedObject.GetInstanceID()){
					movingObject = null;
				}

				Vector3 screenPosition = Camera.main.WorldToScreenPoint (transform.position);
				Vector3 previousMouseWorldPoint = Camera.main.ScreenToWorldPoint (new Vector3 (previousMousePos.x, previousMousePos.y, screenPosition.z));
				Vector3 currentMouseWorldPoint = Camera.main.ScreenToWorldPoint (new Vector3 (currentMousePos.x, currentMousePos.y, screenPosition.z));

				Vector3 movement = currentMouseWorldPoint - previousMouseWorldPoint;
				movement.z = 0;
				draggedObject.transform.localPosition = newPositionAfterMovement (draggedObject, movement);			//}
			}
		}

		if (movingObject != null) {
			if(movementStartTime == 0){
				movementStartTime = Time.time;
			}
			float timeSinceStarted = Time.time - movementStartTime;
			float percentageComplete = timeSinceStarted/0.5f;

			movingObject.transform.localPosition = Vector3.Lerp(movingObject.transform.localPosition,finalPosition,percentageComplete);

			if(percentageComplete >= 1.0f)
			{
				StartCoroutine(resetZPositionAfterMovement(movingObject));
				movingObject = null;
			}
		}
		
		if (rotatingObject != null) {
			if(rotationStartTime == 0){
				rotationStartTime = Time.time;
			}
			float timeSinceStarted = Time.time - rotationStartTime;
			float percentageComplete = timeSinceStarted/0.5f;
			rotatingObject.transform.eulerAngles = Vector3.Lerp(rotatingObject.transform.eulerAngles,finalRotation,percentageComplete);
			if(percentageComplete >= 1.0f)
			{
				StartCoroutine(resetZPositionAfterRotation(rotatingObject));
				rotatingObject = null;
			}
		}
	}

	bool checkPuzzleSolution()
	{
		DominosaGridPosition gridPosition;
		for (int i = 0; i < 4; i++) {
			for(int j = 0; j <5; j++){
				gridPosition = dominosaGrid[i,j];
				if(gridPosition.placedValue != puzzleValues[i,j]){
					return false;
				}
			}
		}
		return true;
	}

	/*
	public override void EnablePuzzle(bool enable)
	{
		print ("ENABLE DOMINOSA: " + enable);
		this.enabled = enable;
		toolbox.currentPuzzle = this;
		//dominosaLimits.SendMessage("EnablePuzzle", enable);
	}
	*/

	public void dominosaPieceFinishedMoving(GameObject piece)
	{
		DominosaRect pieceRect = new DominosaRect (piece);
		if (rotatingObject != null) {
			if(piece.GetInstanceID() == rotatingObject.GetInstanceID()){
				//Debug.Log("Old Rect: O - " + pieceRect.originX + "," + pieceRect.originY + " S - " + pieceRect.width + "," + pieceRect.height);
				pieceRect = new DominosaRect (piece ,true);
				//Debug.Log("New Rect: O - " + pieceRect.originX + "," + pieceRect.originY + " S - " + pieceRect.width + "," + pieceRect.height);

			}
		}
		Vector3 newPiecePosition;

		DominosaPieceScript pieceScript = piece.GetComponent<DominosaPieceScript>();

		resetDominosaGridPosition (piece);

		for (int i = 0; i < 4; i++) {
			bool shouldBreak = false;
			for(int j = 0; j < 5; j++){
				DominosaGridPosition gridPosition = dominosaGrid[i,j];
				bool overlapsOccurs = doRectsOverlap(pieceRect, gridPosition.rect);
				if(overlapsOccurs){
					bool canPlaceIt = false;
					if(gridPosition.used){
						canPlaceIt = false;
					}else{
						if(pieceScript != null){
							if(i == 3){
								if(j == 4){
									canPlaceIt = false;
								}else{
									if(pieceScript.horizontal){
										canPlaceIt = true;
									}else{
										canPlaceIt = false;
									}
								}
							}else{
								if(j == 4){
									if(pieceScript.horizontal){
										canPlaceIt = false;
									}else{
										canPlaceIt = true;
									}
								}else{
									if(pieceScript.horizontal){
										DominosaGridPosition nextPosition = dominosaGrid[i,j+1];
										if(nextPosition.used){
											canPlaceIt = false;
										}else{
											canPlaceIt = true;
										}
									}else{
										DominosaGridPosition nextPosition = dominosaGrid[i + 1,j];
										if(nextPosition.used){
											canPlaceIt = false;
										}else{
											canPlaceIt = true;
										}
									}
								}
							}
						}else{
							canPlaceIt = false;
						}
					}
					if(canPlaceIt){
						pieceScript.placedOnGrid = true;
						pieceScript.arrayPosition = new DominosaArrayPosition(i,j);
						newPiecePosition = new Vector3(gridPosition.rect.originX - pieceRect.width/2.0f,gridPosition.rect.originY - pieceRect.height/2.0f,piece.transform.localPosition.z);
						if(useSmoothing){
							beginSmoothMovementForObject(piece,newPiecePosition);
						}else{
							newPiecePosition.z = -0.3f;
							piece.transform.localPosition = newPiecePosition;
						}
						gridPosition.used = true;
						gridPosition.placedValue = pieceScript.value1;
						dominosaGrid[i,j] = gridPosition;
						if(pieceScript.horizontal){
							DominosaGridPosition nextPosition = dominosaGrid[i,j+1];
							nextPosition.used = true;
							nextPosition.placedValue = pieceScript.value2;
							dominosaGrid[i,j+1] = nextPosition;
						}else{
							DominosaGridPosition nextPosition = dominosaGrid[i+1,j];
							nextPosition.used = true;
							nextPosition.placedValue = pieceScript.value2;
							dominosaGrid[i+1,j] = nextPosition;
						}
						shouldBreak = true;
						break;
					}
				}
			}
			if(shouldBreak){
				break;
			}
		}
		if (checkPuzzleSolution ()) {
			this.solvePuzzle();
			//changePiecesColor();
			Debug.Log("Puzzle solved!!");
		}
	}

	public void resetDominosaGridPosition(GameObject piece){
		DominosaPieceScript pieceScript = piece.GetComponent<DominosaPieceScript>();
		
		if(pieceScript.placedOnGrid){
			DominosaArrayPosition position = pieceScript.arrayPosition;
			DominosaGridPosition gridPosition = dominosaGrid[position.i,position.j];
			gridPosition.used = false;
			gridPosition.placedValue = -1;
			dominosaGrid[position.i,position.j] = gridPosition;
			if(pieceScript.horizontal){
				DominosaGridPosition nextPosition = dominosaGrid[position.i,position.j + 1];
				nextPosition.used = false;
				nextPosition.placedValue = -1;
				dominosaGrid[position.i, position.j + 1] = nextPosition;
			}else{
				DominosaGridPosition nextPosition = dominosaGrid[position.i + 1,position.j];
				nextPosition.used = false;
				nextPosition.placedValue = -1;
				dominosaGrid[position.i + 1, position.j] = nextPosition;
			}
		}
		pieceScript.placedOnGrid = false;
		pieceScript.arrayPosition = new DominosaArrayPosition (-1, -1);
	}


	bool doRectsOverlap(DominosaRect rect1, DominosaRect rect2)
	{
		bool overlapsX1 = (rect1.originX <= rect2.originX && rect1.originX >= (rect2.originX - rect2.width));
		bool overlapsX2 = (rect2.originX <= rect1.originX && rect2.originX >= (rect1.originX - rect1.width));

		bool overlapsX = overlapsX1 || overlapsX2;

		bool overlapsY1 = (rect1.originY <= rect2.originY && rect1.originY >= (rect2.originY - rect2.height));
		bool overlapsY2 = (rect2.originY <= rect1.originY && rect2.originY >= (rect1.originY - rect1.height));

		bool overlapsY = overlapsY1 || overlapsY2;

		return overlapsX && overlapsY;
	}

	Vector3 newPositionAfterMovement(GameObject movedObject, Vector3 movement)
	{
		Vector3 size = movedObject.GetComponentInChildren<MeshRenderer> ().bounds.size;
		Vector3 newPosition = movedObject.transform.localPosition + movement;

		newPosition.z = -0.24f;

		float difference = 0f;

		float leftSide = newPosition.x + size.x / 2.0f;
		float rightSide = newPosition.x - size.x / 2.0f;
		if (leftSide > draggingLimits.originX) {
			difference = leftSide -  draggingLimits.originX;
			newPosition.x = newPosition.x - difference;
		} else {
			float rightLimit = draggingLimits.originX - draggingLimits.width;
			if(rightSide < rightLimit){
				difference = rightLimit - rightSide;
				newPosition.x = newPosition.x + difference;
			}
		}

		float topSide = newPosition.y + size.y / 2.0f;
		float bottomSide = newPosition.y - size.y / 2.0f;
		if (topSide > draggingLimits.originY) {
			difference = topSide -  draggingLimits.originY;
			newPosition.y = newPosition.y - difference;
		} else {
			float bottomLimit = draggingLimits.originY - draggingLimits.height;
			if(bottomSide < bottomLimit){
				difference = bottomLimit - bottomSide;
				newPosition.y = newPosition.y + difference;
			}
		}
		return newPosition;
	}

	void beginSmoothRotationForObject(GameObject objectToRotate, int rotation)
	{
		rotationStartTime = 0;
		rotatingObject = objectToRotate;
		Vector3 rotationVector = new Vector3 (0, 0, rotation);
		finalRotation = objectToRotate.transform.eulerAngles + rotationVector;
	}

	void beginSmoothMovementForObject(GameObject objectToMove, Vector3 position)
	{
		movementStartTime = 0;
		movingObject = objectToMove;
		finalPosition = position;
	}

	public override void autoSolvePuzzle ()
	{
		if (!this.isFinished) {
			base.autoSolvePuzzle ();

			GameObject[] dominosaPieces = GameObject.FindGameObjectsWithTag("DominosaPiece");

			int count = 0;
			int i = 0;
			int j = 0;
			foreach(GameObject piece in dominosaPieces){
				/*
				DominosaPieceScript pieceScript = piece.GetComponent<DominosaPieceScript>();
				pieceScript.horizontal = false;
				pieceScript.placedOnGrid = true;
				 */

				piece.transform.eulerAngles = Vector3.zero;
				j = count % 5;
				if(count < 5){
					i = 0;
				}else{
					i = 2;
				}
				DominosaRect pieceRect = new DominosaRect(piece);
				DominosaGridPosition gridPosition = dominosaGrid[i,j];
				Vector3 newPiecePosition = new Vector3(gridPosition.rect.originX - pieceRect.width/2.0f,gridPosition.rect.originY - pieceRect.height/2.0f,piece.transform.localPosition.z);

				piece.transform.localPosition = newPiecePosition;
				count++;
			}
			//changePiecesColor();
		}
	}

	public void changePiecesColor()
	{
		GameObject[] dominosaPieces = GameObject.FindGameObjectsWithTag("DominosaPiece");
		foreach (GameObject piece in dominosaPieces) {
			MeshRenderer renderer = piece.GetComponentInChildren<MeshRenderer>();
			renderer.material.color = Color.red;
		}
	}

	IEnumerator resetZPositionAfterRotation(GameObject piece)
	{
		yield return new WaitForSeconds (0.05f);
		if (piece != null) {
			piece.transform.localPosition = new Vector3 (piece.transform.localPosition.x, piece.transform.localPosition.y, -0.3f);
		} else {
			Debug.Log("No object!");
		}
	}

	IEnumerator resetZPositionAfterMovement(GameObject piece)
	{
		yield return new WaitForSeconds (0.05f);
		if (piece != null) {
			piece.transform.localPosition = new Vector3 (piece.transform.localPosition.x, piece.transform.localPosition.y, -0.3f);
		} else {
			Debug.Log("No object!");
		}
	}

}
