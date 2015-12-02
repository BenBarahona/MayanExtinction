using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour 
{	
	private MouseState mouseState = MouseState.Normal;
	private DragState dragState = DragState.None;
	
	/** The minimum duration, in seconds, that can elapse between mouse clicks */
	public float clickDelay = 0.1f;
	/**<The maximum duration, in seconds, between two successive mouse clicks to register a "double-click" */
	public float doubleClickDelay = 0.2f;
	
	private float clickTime = 0f;
	private float doubleClickTime = 0;
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
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!Toolbox.Instance.isTransitioning ()) {
			UpdateInput ();
		}
	}

	public void ResetForTransition()
	{
		mouseState = MouseState.Normal;
		dragState = DragState.None;
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
	
	public Vector2 GetDragVector ()
	{
		if (dragState == DragState._Camera)
		{
			return deltaDragMouse;
		}
		return dragVector;
	}
	
	private void SetDragState ()
	{
		dragState = DragState._Camera;
		if (deltaDragMouse.magnitude * Time.deltaTime <= 1f && (GetInvertedMouse () - dragStartPosition).magnitude < 10f)
		{
			dragState = DragState.None;
		}
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
			doubleClickTime -= 4f * GetDeltaTime ();
		}
		if (doubleClickTime < 0f)
		{
			doubleClickTime = 0f;
		}
		
		if (mouseState == MouseState.Normal)
		{
			dragState = DragState.None;
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
			SetDragState ();
		}
		//Release click
		else
		{
			if (mouseState == MouseState.HeldDown && dragState == DragState.None && CanClick ())
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
		if (doubleClickTime > 0f && clickTime == 0f)
		{
			return true;
		}
		
		return false;
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
		if (clickTime == 0f)
		{
			return true;
		}
		
		return false;
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
