using UnityEngine;
using System.Collections;

public class Toolbox : Singleton<Toolbox> {

	protected Toolbox() {}

	public ObjectProperties currentTargetProperties;

	public TouchManager touchManager;
	public InventoryManager inventoryManager;

	public GameState gameState;

	public bool animatingZoom;
	public bool animatingHorizontal;
	public bool animatingVertical;
	public Puzzle currentPuzzle;

	void Awake () {
		Debug.Log ("Creating Toolbox");
		//GameObject main = GameObject.Find ("/MainGameObj");
		//scene = main.GetComponent <TutorialSceneScript>();
		currentTargetProperties = gameObject.AddComponent<ObjectProperties>();
		touchManager = gameObject.AddComponent<TouchManager>();

		animatingZoom = animatingHorizontal = animatingVertical = false;

		gameState = GameState.Resumed;
	}

	void Update () {
		
		animatingZoom = animatingHorizontal = animatingVertical = false;
	}
	
	public void SetUpForLevelBegin()
	{
		//GameObject main = GameObject.Find ("/MainGameObj");
		//scene = main.GetComponent <TutorialSceneScript>();
		
		currentTargetProperties = gameObject.AddComponent<ObjectProperties>();
		touchManager = gameObject.AddComponent<TouchManager>();
		
		animatingZoom = animatingHorizontal = animatingVertical = false;

		gameState = GameState.Resumed;
	}

	public void beginTransition()
	{
		animatingZoom = true;
		animatingHorizontal = true;
		animatingVertical = true;

		touchManager.ResetForTransition ();
	}

	public void endTransition()
	{
		animatingZoom = false;
		animatingHorizontal = false;
		animatingVertical = false;

		touchManager.ResetForTransition ();
	}
	
	public bool isTransitioning()
	{
		return animatingZoom || animatingHorizontal || animatingVertical;
	}

	public void SetGameState(GameState newState)
	{
		gameState = newState;
		if (newState == GameState.Inventory) {
			touchManager.SetDragState (DragState.Inventory);
		} else {
			touchManager.ResetClick();
			touchManager.ResetDoubleClick();
		}
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
}