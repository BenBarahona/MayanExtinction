using UnityEngine;
using System.Collections;

public class ZinlockPiece : MonoBehaviour {

	public bool hasPiece;
	public bool isSelected;
	public Texture selectedTexture;
	public Texture deselectedTexture;
	public int index;
	public Vector3 gridSize;
	public int spacesLeftAfterWrap;
	public int validIndex;

	private MeshRenderer mRenderer;
	private Vector3 newPosition;

	//Movement vars
	Vector3 startPosition;
	Vector3 endPosition;
	Vector3 moveDirection;
	float timeSinceMove;
	float timeToLerp = 1.0f;
	float movementSpeed = 0.25f;
	bool isMoving;

	//Temp var
	Color blueColor;

	public GameObject indexText;

	TextMesh indexTextMesh;

	// Use this for initialization
	void Start () {
		isSelected = false;
		mRenderer = GetComponentInChildren<MeshRenderer> ();
		//blueColor = mRenderer.material.GetColor("_Color");

		isMoving = false;
		indexTextMesh = indexText.GetComponent<TextMesh> ();

	}

	// Update is called once per frame
	void Update () {
		if (mRenderer != null) {
			mRenderer.enabled = hasPiece;
		}

		if (isMoving) {
			float deltaTime = Time.time - timeSinceMove;
			float percentage = deltaTime / timeToLerp;

			transform.localPosition = Vector3.Lerp (startPosition, endPosition, percentage);

			if(percentage > 1.0f)
			{
				isMoving = false;
				spacesLeftAfterWrap = 0;
			}
		}
	}

	public void setSelected(bool selected)
	{
		isSelected = selected;

		//Texture newTexture = new Text
		//Texture newTexture = selected ? selectedTexture : deselectedTexture;
		//mRenderer.material.SetTexture ("_MainTex", newTexture);

		//TODO:Remove when provided final textures
		//Color newColor = selected ? Color.red : blueColor;
		//mRenderer.material.SetColor ("_Color", newColor);
	}

	public void setDone(bool done)
	{
		Color newColor = done ? Color.yellow : Color.blue;
		//mRenderer.material.SetColor ("_Color", newColor);
	}

	public void movePiece(int spaces, float spaceBetweenSquares, Vector3 direction)
	{
		Vector3 end = direction * spaces * spaceBetweenSquares;

		moveDirection = direction;
		timeToLerp = movementSpeed * spaces;
		timeSinceMove = Time.time;
		startPosition = transform.localPosition;
		endPosition = transform.localPosition + end;
		isMoving = true;
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Border")) {
			isMoving = false;

			Vector3 difference = endPosition - transform.localPosition;
			//print ("WRAP: " + difference);
			Vector3 newPos = transform.localPosition;

			if(moveDirection.Equals(Vector3.left))
			{
				newPos.x += gridSize.x;
			} 
			else if(moveDirection.Equals(Vector3.right)) 
			{
				newPos.x -= gridSize.x;
			}
			else if(moveDirection.Equals(Vector3.up))
			{
				newPos.y -= gridSize.y;
			}
			else if(moveDirection.Equals(Vector3.down))
			{
				newPos.y += gridSize.y;
			}

			timeToLerp = movementSpeed * spacesLeftAfterWrap;
			timeSinceMove = Time.time;
			transform.localPosition = newPos;
			endPosition = newPos + difference;
			startPosition = newPos;
			isMoving = true;
		}
	}
}
