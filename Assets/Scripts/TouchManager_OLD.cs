using UnityEngine;
using System.Collections;

public class TouchManager_OLD : MonoBehaviour 
{	
	private MouseState mouseState = MouseState.Normal;
	private DragState dragState = DragState.None;
	
	/** The minimum duration, in seconds, that can elapse between mouse clicks */
	public float clickDelay = 1.0f;
	/**<The maximum duration, in seconds, between two successive mouse clicks to register a "double-click" */
	public float doubleClickDelay = 0.4f;
	
	private float clickTime = 0f;
	private float doubleClickTime = 0f;
	private bool hasUnclickedSinceClick = false;
	private bool lastClickWasDouble = false;

	// Controller movement
	private Vector2 mousePosition;
	
	// Touch-Screen movement
	private Vector2 dragStartPosition = Vector2.zero;
	private float dragSpeed = 0f;
	private Vector2 dragVector;
	
	// Draggable
	private Vector2 lastMousePosition;
	private Vector2 deltaDragMouse;

	private int holdTimer = 0;

	// Use this for initialization
	void Start () {
		ResetClick ();
		ResetDoubleClick ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!Toolbox.Instance.isTransitioning ()) {
			UpdateInput ();
			if(dragState != DragState.Inventory)
			{
				CheckForClicks ();
			}
		}

		Debug.Log (mouseState);
	}

	public void ResetForTransition()
	{
		SetDragState (DragState.None);
		mouseState = MouseState.Normal;
		dragVector = Vector2.zero;
		
		ResetClick ();
		ResetDoubleClick ();
	}

	private void UpdateDrag ()
	{
		// Calculate change in mouse position
		if (dragState != DragState.None) {
			deltaDragMouse = ((Vector2)mousePosition - lastMousePosition) / Time.deltaTime;
			lastMousePosition = mousePosition;
		}
	}
	
	public DragState GetDragState ()
	{
		return dragState;
	}

	public MouseState GetMouseState()
	{
		return mouseState;
	}
	
	public Vector2 GetDragVector ()
	{
		if (dragState == DragState._Camera)
		{
			return deltaDragMouse;
		}
		return dragVector;
	}
	
	public void SetDragState (DragState newState)
	{
		dragState = newState;
		//Debug.Log (dragState.ToString());
	}
	
	/**
		 * <summary>Gets the y-inverted cursor position. This is useful because Menu Rects are drawn upwards, while screen space is measured downwards.</summary>
		 * <returns>Gets the y-inverted cursor position. This is useful because Menu Rects are drawn upwards, while screen space is measured downwards.</returns>
		 */
	public Vector2 GetInvertedMouse ()
	{
		return new Vector2 (GetMousePosition ().x, Screen.height - GetMousePosition ().y);
	}
	
	public Vector2 GetMousePosition ()
	{
		return mousePosition;
	}
	
	public void UpdateInput ()
	{
		//Debug.Log ("C: "+clickTime);
		//Debug.Log ("D: "+doubleClickTime);
		if (clickTime > 0f)
		{
			clickTime -= 4f * GetDeltaTime ();
		}
		if (clickTime < 0f)
		{
			clickTime = 0f;
		}
		
		if (doubleClickTime > 0f)
		{
			//Debug.Log ("" + doubleClickDelay * GetDeltaTime());
			doubleClickTime -= 4f * GetDeltaTime ();
		}
		if (doubleClickTime < 0f)
		{
			doubleClickTime = 0f;
		}
		
		if (mouseState == MouseState.Normal)
		{
			SetDragState(DragState.None);
		}
		
		
		if (Input.GetMouseButton (0)) {
			holdTimer++;
		}
		if (Input.GetMouseButtonUp (0)) {
			holdTimer = 0;
		}

		//First time click
		if (Input.GetMouseButtonDown (0))
		{
			if (mouseState == MouseState.Normal)
			{
				if (CanDoubleClick ())
				{
					mouseState = MouseState.DoubleClick;
					ResetClick ();
				}
				else if (CanClick ())
				{
					/*
					if(holdTimer == 0){
						dragVector = Vector2.zero;
					}*/
					dragStartPosition = GetInvertedMouse ();
					
					mouseState = MouseState.SingleClick;
					ResetClick ();
					ResetDoubleClick ();
				}
			}
		}
		//Continous click (hold down)
		else if (Input.GetMouseButton (0))
		{
			mouseState = MouseState.HeldDown;
			GameState gameState = Toolbox.Instance.gameState;
			if(gameState == GameState.Resumed) {
				SetDragState (DragState._Camera);
			}
			else if(gameState == GameState.Inventory)
			{
				SetDragState (DragState.Inventory);
			}

			if (deltaDragMouse.magnitude * Time.deltaTime <= 1f && (GetInvertedMouse () - dragStartPosition).magnitude < 10f) {
				SetDragState(DragState.None);
			}
		}
		//Release click
		else
		{
			if (mouseState == MouseState.HeldDown && CanClick ())
			{
				mouseState = MouseState.LetGo;
			}
			else
			{
				ResetMouseClick ();
			}
		}
		
		SetDoubleClickState ();
		
		mousePosition = Input.mousePosition;
		/*
		if (Input.touchCount > 0) {
			Touch t = Input.GetTouch (0);
			if (t.phase == TouchPhase.Moved && Input.touchCount == 1){
				mousePosition += t.deltaPosition * Time.deltaTime / t.deltaTime;
			}
		}*/
		
		if (mouseState == MouseState.Normal && !hasUnclickedSinceClick)
		{
			hasUnclickedSinceClick = true;
		}
		
		UpdateDrag ();
		
		if (dragState != DragState.None)
		{
			dragVector = GetInvertedMouse () - dragStartPosition;
			dragSpeed = dragVector.magnitude;
		}
		else
		{
			dragSpeed = 0f;
		}
	}

	private void CheckForClicks()
	{
		if (mouseState == MouseState.SingleClick) 
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Input.touchCount > 0) {
				ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
			}
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				if (hit.collider.CompareTag ("Pickup")) {

					/*
					GameObject canvas = GameObject.Find ("Canvas");
					InventoryManager manager = canvas.GetComponent<InventoryManager>();
					manager.PickUpGameObject (hit.collider.gameObject);
					*/
					Toolbox.Instance.inventoryManager.PickUpGameObject (hit.collider.gameObject);
				}
			}
		}
	}
	
	private float GetDeltaTime ()
	{
		if (Time.deltaTime == 0f)
		{
			return 0.02f;
		}
		return Time.deltaTime;
	}
	
	public bool CanDoubleClick ()
	{
		return doubleClickTime > 0f && clickTime == 0f;
	}
	
	/**
		 * Records the current click time, so that another click will not register for the duration of clickDelay.
		 */
	public void ResetClick ()
	{
		clickTime = clickDelay;
		hasUnclickedSinceClick = false;
	}
	
	
	public void ResetDoubleClick ()
	{
		doubleClickTime = doubleClickDelay;
	}
	
	/**
		 * <summary>Checks if a mouse click will be registered.</summary>
		 * <returns>True if a mouse click will be registered</returns>
		 */
	public bool CanClick ()
	{
		return clickTime == 0f;
	}
	
	/**
		 * Resets the mouse click so that nothing else will be affected by it this frame.
		 */
	public void ResetMouseClick ()
	{
		mouseState = MouseState.Normal;
	}
	
	private void SetDoubleClickState ()
	{
		if (mouseState == MouseState.DoubleClick)
		{
			lastClickWasDouble = true;
		}
		else if (mouseState == MouseState.SingleClick || mouseState == MouseState.RightClick || mouseState == MouseState.LetGo)
		{
			lastClickWasDouble = false;
		}
	}

}
