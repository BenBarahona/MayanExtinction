using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraZoom : MonoBehaviour {
	
	public GameObject environmentTarget;
	//public GameObject cameraObj;

	ObjectProperties targetProperties;
	GameObject target;

	Stack<GameObject> parentStack;

	float speedMultiplier;
	float zoomStartTime;
	int holdTimer;
	Vector3 zoomStartPos;
	Vector3 zoomEndPos;
	Vector3 moveStartPos;

	Toolbox toolbox;

	public static MainCameraScript mainCamera;

	// Use this for initialization
	void Start () {
		target = null;
		mainCamera = this.GetComponent<MainCameraScript> ();
		parentStack = new  Stack<GameObject> ();
		//parentStack.Push (environmentTarget);

		zoomIn (environmentTarget, false);
		//setNewTarget (environmentTarget, false);
	}
	

	void Awake() {
		holdTimer = 0;
		toolbox = Toolbox.Instance;

	}

	void Update () {
		/*if(toolbox.animatingZoom)
		{
			float deltaTime = Time.time - zoomStartTime;
			float percentage = deltaTime / 1.0f;

			cameraObj.transform.position = Vector3.Lerp (moveStartPos, target.transform.position, percentage);

			Vector3 negDistance = new Vector3(0, 0, -targetProperties.distance);
			Vector3 newPosition = transform.localRotation * negDistance;
			transform.localPosition = Vector3.Lerp(zoomStartPos, newPosition, percentage);

			if(percentage >= 1.0f)
			{
				transform.localPosition = newPosition;
				cameraObj.transform.position = target.transform.position;
				toolbox.animatingZoom = false;
			}
		}*/
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//Pinch control vars
		Vector2 currentDistance = new Vector2(0, 0);
		Vector2 previousDistance = new Vector2(0, 0);
		float speedTouch0 = 0.0f;
		float speedTouch1 = 0.0f;
		float touchDelta = 0.0f;
		float minPinchSpeed = 10.0f;
		float varianceInDistances = 5.0f;

		//Debug.Log (Toolbox.Instance.touchManager.GetMouseState().ToString());

		if (!isOverUI() && !toolbox.isTransitioning()) {

			// On non touch devices, zoom is left click, zoom out is right click
			#if UNITY_WEBPLAYER || UNITY_STANDALONE || UNITY_EDITOR
			if(Input.GetMouseButton(0))
				holdTimer++;

			if (Input.GetMouseButtonUp (0) && holdTimer < 8) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit2;
				if (Physics.Raycast (ray, out hit2)) {
					if (hit2.collider != null && hit2.collider.gameObject != target && hit2.collider.CompareTag ("Dbl-Clickable")) {

						if (hit2.collider != null && hit2.collider.CompareTag ("Dbl-Clickable")) {
							if(targetProperties.onlyAllowChildColliders){
								if(hit2.collider.gameObject.transform.IsChildOf(target.transform)){
									zoomIn(hit2.collider.gameObject, true);
								}
							}else{
								zoomIn(hit2.collider.gameObject, true);
							}
						}//setNewTarget (hit2.collider.gameObject, true);
					}
				}
			} else if (Input.GetMouseButtonDown (1) && target != environmentTarget) {
				zoomOut();
				/*
				if (parentStack.Peek () == environmentTarget) {
					setNewTarget (parentStack.Peek (), true);
				} else {
					setNewTarget (parentStack.Pop (), true);
				}*/
			}

			//Reset timer when user releases mouse btn
			if(Input.GetMouseButtonUp(0))
				holdTimer = 0;

			#endif

			// For Touch devices, double click and zoom logic
			if (!toolbox.isTransitioning()) {
				if (Input.touchCount == 1) {
					Touch touch = Input.GetTouch (0);
					Ray ray = Camera.main.ScreenPointToRay (touch.position);
					RaycastHit hit;
					if (touch.tapCount == 2 && Physics.Raycast (ray, out hit)) {
						if (hit.collider != null && hit.collider.CompareTag ("Dbl-Clickable")) {
							Debug.Log("Hit a collider");
							if(targetProperties.onlyAllowChildColliders){
								Debug.Log("Only child colliders!");
								if(hit.collider.gameObject.transform.IsChildOf(target.transform)){
									zoomIn(hit.collider.gameObject, true);
								}
							}else{
								Debug.Log("Any colliders");
								zoomIn(hit.collider.gameObject, true);
							}
							/*
							if (hit.collider.gameObject == target) {
								setNewTarget (parentStack.Pop (), true);
							} else {
								setNewTarget (hit.collider.gameObject, true);
							}*/
						}
					}
				} else if (Input.touchCount == 2 && target != environmentTarget) {
					Touch touch1 = Input.GetTouch (0);
					Touch touch2 = Input.GetTouch (1);
					if (touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved) {
						//current distance between finger touches
						currentDistance = Input.GetTouch (0).position - Input.GetTouch (1).position;
						//difference in previous locations using delta positions
						previousDistance = ((Input.GetTouch (0).position - Input.GetTouch (0).deltaPosition) - (Input.GetTouch (1).position - Input.GetTouch (1).deltaPosition));
						touchDelta = currentDistance.magnitude - previousDistance.magnitude;
						speedTouch0 = Input.GetTouch (0).deltaPosition.magnitude / Input.GetTouch (0).deltaTime;
						speedTouch1 = Input.GetTouch (1).deltaPosition.magnitude / Input.GetTouch (1).deltaTime;
					
						//Pinch (Zoom out)
						if ((touchDelta + varianceInDistances <= 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed)) {
							zoomOut();
						}

						//Pan ("Zoom in" gesture)
						if ((touchDelta + varianceInDistances > 1) && (speedTouch0 > minPinchSpeed) && (speedTouch1 > minPinchSpeed)) {

						}
					}
				}
			}
		}
	}

	void zoomIn(GameObject newTarget, bool isTransition){
		if (newTarget != null) {
			ObjectProperties newObjectProperties = newTarget.GetComponent<ObjectProperties>();
			if(newObjectProperties != null && newObjectProperties.customCamera != null){
				//Pushes current target to the stack
				parentStack.Push (target);
				//Disables current target's puzzle
				if(target != null && targetProperties != null && targetProperties.isPuzzle) {
					target.SendMessage("EnablePuzzle", false);
				}

				//Disabling new target collider to detect children colliders
				setObjectColliderEnabled (newTarget, false);

				if(target){
					target.SendMessage("beganCameraTransition", SendMessageOptions.DontRequireReceiver);
				}

				//Setting new target to local var
				target = newTarget;
				targetProperties = newObjectProperties;
				
				//Setting global vars to new values
				toolbox.currentTargetProperties.setValuesFromProperties(targetProperties, newTarget.transform);

				if(isTransition)
				{
					mainCamera.SetGameCamera(targetProperties.customCamera, targetProperties.transitionTime);
					
					toolbox.beginTransition();
				}else{
					mainCamera.SetGameCamera(targetProperties.customCamera, 0);
				}
				
				//If the gameobject has a puzzle, call method to enable
				if(targetProperties.isPuzzle){
					newTarget.SendMessage("EnablePuzzle", true);
				}
			}
		}
	}

	void zoomOut(){
		float transitionTime = targetProperties.transitionTime;
		GameObject newTarget;
		if (parentStack.Peek ().GetInstanceID () == environmentTarget.GetInstanceID ()) {
			newTarget = parentStack.Peek ();
		} else {
			newTarget = parentStack.Pop();
		}

		ObjectProperties newObjectProperties = newTarget.GetComponent<ObjectProperties>();


		//Disables current target's puzzle
		if (target != null && targetProperties != null && targetProperties.isPuzzle) {
			target.SendMessage ("EnablePuzzle", false);
		}
		//Enables current target's collider
		setObjectColliderEnabled (target, true);

		if(target){
			target.SendMessage("beganCameraTransition", SendMessageOptions.DontRequireReceiver);
		}

		target = newTarget;
		targetProperties = newObjectProperties;

		toolbox.currentTargetProperties.setValuesFromProperties(targetProperties, newTarget.transform);
		
		if(transitionTime > 0)
		{
			mainCamera.SetGameCamera(targetProperties.customCamera, transitionTime);
			toolbox.beginTransition();
		}else{
			mainCamera.SetGameCamera(targetProperties.customCamera, 0);
		}
		
		//If the gameobject has a puzzle, call method to enable
		if(targetProperties.isPuzzle){
			newTarget.SendMessage("EnablePuzzle", true);
		}

	}

	void setNewTarget(GameObject newTarget, bool isTransition)
	{
		if (newTarget != null) {
			//if old target exists, and is a puzzle, disable it
			ObjectProperties newObjectProperties = newTarget.GetComponent<ObjectProperties>();
			if(newObjectProperties != null && newObjectProperties.customCamera != null)
			{
				if(target != null && targetProperties != null && targetProperties.isPuzzle) {
					target.SendMessage("EnablePuzzle", false);
				}

				//enabling old target collider and parent s, as long as it is NOT a parent of the new target
				if (target != null && !newTarget.transform.IsChildOf (target.transform)) {
					//Second parameter is to enable just the object (false) or parents of obj too (true)
					Debug.Log("Enabling parents!");
					enableObjectAndParentColliders (target, !target.transform.IsChildOf (newTarget.transform));
					//They are brothers!
					//if(newTarget.transform.IsChildOf(target.transform.parent)){
						parentStack.Push(target);
					//}
				}else {
					parentStack.Push (target);
				}

				//Disabling new target collider to detect children colliders
				setObjectColliderEnabled (newTarget, false);

				//Setting new target to local var
				target = newTarget;
				targetProperties = newObjectProperties;

				//Setting global vars to new values
				toolbox.currentTargetProperties.setValuesFromProperties(targetProperties, newTarget.transform);

				if(isTransition)
				{
					mainCamera.SetGameCamera(targetProperties.customCamera, targetProperties.transitionTime);
					//Setting vars for zoom lerp
					//zoomStartTime = Time.time;
					//zoomStartPos = transform.localPosition;
					//moveStartPos = cameraObj.transform.position;

					toolbox.beginTransition();
				}else{
					mainCamera.SetGameCamera(targetProperties.customCamera, 0);
				}

				//If the gameobject has a puzzle, call method to enable
				if(targetProperties.isPuzzle){

					newTarget.SendMessage("EnablePuzzle", true);
				}
			}

		}
	}

	void enableObjectAndParentColliders(GameObject obj, bool enableParents)
	{
		setObjectColliderEnabled(obj, true);
		if (obj.transform.parent != null && enableParents) 
		{
			GameObject parent = obj.transform.parent.gameObject;
			enableObjectAndParentColliders (parent, enableParents);
		}
	}

	void setObjectColliderEnabled(GameObject obj, bool enableCollider)
	{
		Puzzle puzzle = obj.GetComponent<Puzzle> ();
		Collider collider = obj.GetComponent<Collider> ();

		if (collider != null) {
			if(!puzzle)
			{
				collider.enabled = enableCollider;
			}
			else if(!(puzzle.disableAfterSolve && puzzle.isFinished))
			{
				collider.enabled = enableCollider;
			}
		}
	}

	private bool isOverUI(){
		if (UnityEngine.EventSystems.EventSystem.current) {
			if (Input.touchCount > 0) {
				int pointerID = Input.touches [0].fingerId;
				return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject (pointerID);
			} else {
				return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ();
			}
		} else {
			return false;
		}
	}

	public void zoomOutToParent()
	{
		if (parentStack.Peek () == environmentTarget) {
			setNewTarget (parentStack.Peek (), true);
		} else {
			setNewTarget (parentStack.Pop (), true);
		}
	}
}
