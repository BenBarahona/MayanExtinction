using UnityEngine;
using System.Collections;

public class DominosaPieceScript : MonoBehaviour {

	public bool horizontal;
	public bool placedOnGrid;
	public int value1;
	public int value2;

	public DominosaArrayPosition arrayPosition;

	// Use this for initialization
	void Start () {
		arrayPosition = new DominosaArrayPosition(-1,-1);
		placedOnGrid = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void pieceDidFinishRotate()
	{
		//gameObject.SendMessageUpwards ("resetDominosaGridPosition", gameObject);
		if (horizontal) {
			horizontal = false;
		} else {
			horizontal = true;
			int tempValue = value1;
			value1 = value2;
			value2 = tempValue;
		}
		//gameObject.SendMessageUpwards("dominosaPieceFinishedMoving",gameObject);
	}

	/*
	public void pieceDidFinishMovement()
	{
		gameObject.SendMessageUpwards("dominosaPieceFinishedMoving",gameObject);
	}*/
}
