using UnityEngine;
using System.Collections;

public class Toolbox : Singleton<Toolbox> {

	protected Toolbox() {}

	public ObjectProperties currentTargetProperties;

	public bool animatingZoom;
	public bool animatingHorizontal;
	public bool animatingVertical;
	public Puzzle currentPuzzle;
	
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
	private Vector2 oldDragPosition = Vector2.zero;
	private float dragSpeed = 0f;
	private Vector2 dragVector;
	private float touchTime = 0f;
	private float touchThreshold = 0.2f;

	// Draggable
	private float cameraInfluence = 100000f;
	private Vector2 lastMousePosition;
	private Vector3 lastCameraPosition;
	private Vector3 dragForce;
	private Vector2 deltaDragMouse;

	private int holdTimer = 0;

	void Awake () {
		GameObject main = GameObject.Find ("/MainGameObj");
		currentTargetProperties = main.AddComponent<ObjectProperties>();
		//scene = main.GetComponent <TutorialSceneScript>();

		animatingZoom = animatingHorizontal = animatingVertical = false;
	}

	void Update () {
		if (!isTransitioning ()) {
			UpdateInput ();
		}
	}

	public void SetUpForLevelBegin()
	{
		GameObject main = GameObject.Find ("/MainGameObj");
		currentTargetProperties = main.AddComponent<ObjectProperties>();
		//scene = main.GetComponent <TutorialSceneScript>();
		
		animatingZoom = animatingHorizontal = animatingVertical = false;
	}

	public void beginTransition()
	{
		animatingZoom = true;
		animatingHorizontal = true;
		animatingVertical = true;

		mouseState = MouseState.Normal;
		dragState = DragState.None;
		dragVector = Vector2.zero;
		ResetClick ();
		ResetDoubleClick ();
	}

	public void endTransition()
	{
		animatingZoom = false;
		animatingHorizontal = false;
		animatingVertical = false;

		mouseState = MouseState.Normal;
		dragState = DragState.None;
		dragVector = Vector2.zero;
		ResetClick ();
		ResetDoubleClick ();
	}
	
	public bool isTransitioning()
	{
		return animatingZoom || animatingHorizontal || animatingVertical;
	}

	// function to clamp the angle based on given min and max
	public static float ClampAngle (float angle, float min,float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}

	/**
		 * <summary>Lerps from one float to another over time.</summary>
		 * <param name = "from">The initial value</param>
		 * <param name = "to">The final value</param>
		 * <param name = "t">The time value.  If greater than 1, the result will overshoot the final value</param>
		 * <returns>The lerped float</returns>
		 */
	public static float Lerp (float from, float to, float t)
	{
		if (t <= 1)
		{
			return Mathf.Lerp (from, to, t);
		}
		
		return from + (to-from)*t;
	}
	
	
	/**
		 * <summary>Lerps from one Vector3 to another over time.</summary>
		 * <param name = "from">The initial value</param>
		 * <param name = "to">The final value</param>
		 * <param name = "t">The time value.  If greater than 1, the result will overshoot the final value</param>
		 * <returns>The lerped Vector3</returns>
		 */
	public static Vector3 Lerp (Vector3 from, Vector3 to, float t)
	{
		if (t <= 1)
		{
			return Vector3.Lerp (from, to, t);
		}
		
		return from + (to-from)*t;
	}
	
	
	/**
		 * <summary>Lerps from one Quaternion to another over time.</summary>
		 * <param name = "from">The initial value</param>
		 * <param name = "to">The final value</param>
		 * <param name = "t">The time value.  If greater than 1, the result will overshoot the final value</param>
		 * <returns>The lerped Quaternion</returns>
		 */
	public static Quaternion Lerp (Quaternion from, Quaternion to, float t)
	{
		if (t <= 1)
		{
			return Quaternion.Lerp (from, to, t);
		}
		
		Vector3 fromVec = from.eulerAngles;
		Vector3 toVec = to.eulerAngles;
		
		if (fromVec.x - toVec.x > 180f)
		{
			toVec.x -= 360f;
		}
		else if (fromVec.x - toVec.x > 180f)
		{
			toVec.x += 360;
		}
		if (fromVec.y - toVec.y < -180f)
		{
			toVec.y -= 360f;
		}
		else if (fromVec.y - toVec.y > 180f)
		{
			toVec.y += 360;
		}
		if (fromVec.z - toVec.z > 180f)
		{
			toVec.z -= 360f;
		}
		else if (fromVec.z - toVec.z > 180f)
		{
			toVec.z += 360;
		}
		
		return Quaternion.Euler (Lerp (fromVec, toVec, t));
	}
	
	
	/**
		 * <summary>Interpolates a float over time, according to various interpolation methods.</summary>
		 * <param name = "startT">The starting time</param>
		 * <param name = "deltaT">The time difference</param>
		 * <param name = "moveMethod">The method of interpolation (Linear, Smooth, Curved, EaseIn, EaseOut, Curved)</param>
		 * <param name = "timeCurve">The AnimationCurve to interpolate against, if the moveMethod = MoveMethod.Curved</param>
		 * <returns>The interpolated float</returns>
		 */
	public static float Interpolate (float startT, float deltaT, bool smooth)
	{
		if (smooth) {
			return -0.5f * (Mathf.Cos (Mathf.PI * (Time.time - startT) / deltaT) - 1f);
		}
		return ((Time.time - startT) / deltaT);
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
	
	
	private void ResetDoubleClick ()
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