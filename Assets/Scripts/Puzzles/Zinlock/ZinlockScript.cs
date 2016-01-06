using UnityEngine;
using System.Collections;

public class ZinlockScript : Puzzle {

	public GameObject piecePrefab;

	public int gridHeight;
	public int gridWidth;
	public int availablePieces;

	public float horizontalSpace;
	public float verticalSpace;
	
	ArrayList pieceGrid;
	ArrayList piecesToMove;
	ArrayList solutionArray;

	float lastTouch;

	GameObject firstSelected;
	GameObject secondSelected;

	// Use this for initialization
	new void Start () {
		base.Start ();

		firstSelected = null;
		secondSelected = null;

		solutionArray = new ArrayList ();
		//solutionIndexes = new ArrayList();
		pieceGrid = new ArrayList();
		piecesToMove = new ArrayList ();

		lastTouch = 0.0f;

		createSolution ();
		createGridPieces ();
	}
	
	// Update is called once per frame
	void Update () {
		if(this.isFinished)
		{
			//print ("Finished ZINLOCK");
		}
	}

	void LateUpdate () {
		if (Input.touchCount < 2 && lastTouch < 2 && !this.isFinished) 
		{
			if(Input.GetMouseButtonDown(0))
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if(Physics.Raycast(ray, out hit))
				{
					if(hit.collider.CompareTag("ZinlockCube"))
					{
						GameObject selected = hit.collider.gameObject;
						ZinlockPiece script = selected.GetComponent<ZinlockPiece>();

						if(script.hasPiece)
						{
							if(firstSelected != null)
							{
								ZinlockPiece selectedScript = firstSelected.GetComponent<ZinlockPiece>();
								selectedScript.setSelected(!selectedScript.isSelected);
							}

							firstSelected = selected;
							script.setSelected(!script.isSelected);
						}
						else if(firstSelected != null)
						{
							secondSelected = selected;

							movePieces();
						}

					}
				}
			}
		} 
		else 
		{
			lastTouch = Input.touchCount;
		}
	}

	void movePieces()
	{
		ZinlockPiece piece = firstSelected.GetComponent<ZinlockPiece>();
		ZinlockPiece space = secondSelected.GetComponent<ZinlockPiece> ();

		int row1 = Mathf.FloorToInt (piece.index / gridWidth);
		int row2 = Mathf.FloorToInt (space.index / gridWidth);
		
		int column1 = piece.index % gridWidth;
		int column2 = space.index % gridWidth;

		Vector3 direction = Vector3.zero;
		float spaceBetween = 0;
		int spacesToMove = 0;
		
		if (row1 == row2)
		{
			//Getting pieces on each row
			foreach(ZinlockPiece obj in pieceGrid)
			{
				int row = Mathf.FloorToInt(obj.index / gridWidth);
				if(row1 == row)
				{
					piecesToMove.Add (obj);
				}
			}

			direction = column1 < column2 ? Vector3.right : Vector3.left;
			spaceBetween = horizontalSpace;
			spacesToMove = Mathf.Abs(column1 - column2);
		}
		else if(column1 == column2)
		{
			//Getting pieces on each column
			foreach(ZinlockPiece obj in pieceGrid)
			{
				int col = obj.index % gridWidth;
				if(column1 == col)
				{
					piecesToMove.Add (obj);
				}
			}

			direction = row1 < row2 ? Vector3.down : Vector3.up;
			spaceBetween = verticalSpace;
			spacesToMove = Mathf.Abs(row1 - row2);
		}


		foreach(ZinlockPiece square in piecesToMove)
		{
			square.movePiece(spacesToMove , spaceBetween , direction);

			//Resetting new piece index based on movement
			int newIndex = square.index;
			if(direction.Equals(Vector3.left))
			{
				newIndex -= spacesToMove;
				if(newIndex < gridWidth * row1)
				{
					square.spacesLeftAfterWrap = gridWidth * row1 - newIndex - 1;
					newIndex += gridWidth;
				}
				square.index = newIndex;
			} 
			else if(direction.Equals(Vector3.right)) 
			{
				newIndex += spacesToMove;
				if(newIndex >= gridWidth * row1 + gridWidth)
				{
					square.spacesLeftAfterWrap = newIndex - (gridWidth * row1 + gridWidth);
					newIndex -= gridWidth;
				}
			}
			else if(direction.Equals(Vector3.up))
			{
				newIndex -= gridWidth * spacesToMove;
				if(newIndex < 0)
				{
					int newRow = Mathf.FloorToInt(newIndex / gridWidth);
					square.spacesLeftAfterWrap = Mathf.Abs(newRow - 1) - 1;
					newIndex += gridWidth * gridHeight;
				}
			}
			else if(direction.Equals(Vector3.down))
			{
				newIndex += gridWidth * spacesToMove;
				if(newIndex >= gridWidth * gridHeight)
				{
					int newRow = Mathf.FloorToInt(newIndex / gridWidth);
					square.spacesLeftAfterWrap = newRow - gridHeight;
					newIndex -= gridWidth * gridHeight;
				}
			}

			square.index = newIndex;
		}

		piece.setSelected (false);
		firstSelected = null;
		secondSelected = null;
		piecesToMove.Clear ();

		if(checkIfSolved ())
		{
			finishedPuzzle();
			solvePuzzle ();
		}
	}

	void createGridPieces()
	{
		pieceGrid.Clear ();
	
		ArrayList pieceIndexes = new ArrayList ();
		int showedPiecesCounter = 1;
		for(int i = 0; i < availablePieces; i++)
		{
			int random = Random.Range(0, (int)(gridHeight * gridWidth));
			while(pieceIndexes.Contains(random))
			{
				random = Random.Range(0, (int)(gridHeight * gridWidth));
			}
			pieceIndexes.Add(random);
		}

		float zAxis = 0.3f;
		float xAxis = -1.34f;
		float yAxis = 0.87f;

		Vector3 gridSize = new Vector3 (horizontalSpace * gridWidth, verticalSpace * gridHeight, 0.0f);
		for(int i = 0; i < gridHeight; i++)
		{
			for(int j = 0; j < gridWidth ; j++)
			{
				int index = i * gridWidth + j;
				Vector3 pos = new Vector3(xAxis, yAxis, zAxis);
				GameObject piece = (GameObject)Instantiate (piecePrefab, pos, Quaternion.identity);
				piece.transform.parent = this.transform;
				piece.transform.localPosition = pos;

				ZinlockPiece script = piece.GetComponent<ZinlockPiece>();
				script.index = index;
				script.hasPiece = pieceIndexes.Contains (index);
				if(script.hasPiece){
					GameObject skin = (GameObject)Instantiate(Resources.Load("puzzlePieces/PuzzlePiece"+showedPiecesCounter));
					skin.transform.parent = piece.transform;
					skin.transform.localPosition = Vector3.zero;
					skin.transform.eulerAngles = new Vector3(0f,270f,0f);
					skin.transform.localScale = new Vector3(550,175,150);
					script.validIndex = (int)solutionArray[showedPiecesCounter - 1];
					showedPiecesCounter++;
				}else{
					script.validIndex = -1;
				}
				script.gridSize = gridSize;

				xAxis += horizontalSpace;

				pieceGrid.Add (script);
			}

			xAxis = -1.34f;
			yAxis -= verticalSpace;
		}
	}
	
	void createSolution()
	{
		/*
		solutionIndexes.AddRange (new int[]{4, 13, 20, 21, 27, 28, 35, 43, 44});

		for(int i = 0; i < gridHeight; i++)
		{
			for(int j = 0; j < gridWidth ; j++)
			{
				int index = (int)(i * gridHeight + j);
				ZinlockSpace space = ScriptableObject.CreateInstance<ZinlockSpace>();
				space.init (index, solutionIndexes.Contains(index));

				solutionArray.Add (space);
			}
		}
		*/
		solutionArray.AddRange (new int[]{4, 13, 21, 20, 28, 27, 35, 43, 44});
	}

	public bool checkIfSolved()
	{
		int numberCorrect = 0;
		foreach(ZinlockPiece piece in pieceGrid)
		{
			if(piece.hasPiece && piece.index == piece.validIndex/*solutionArray.Contains(piece.index)*/)
			{
				Debug.Log("Found a piece in the correct location");
				numberCorrect++;
			}
		}

		return numberCorrect == solutionArray.Count;
	}

	public override void autoSolvePuzzle ()
	{
		base.autoSolvePuzzle ();

		foreach(ZinlockPiece piece in pieceGrid)
		{
			bool contains = solutionArray.Contains(piece.index);
			piece.hasPiece = contains;
			piece.setDone(contains);
		}
	}

	void finishedPuzzle()
	{
		foreach(ZinlockPiece piece in pieceGrid)
		{
			piece.setDone (true);
		}
	}
}
