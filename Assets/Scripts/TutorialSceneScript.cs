using UnityEngine;
using System.Collections;


public class TutorialSceneScript : MonoBehaviour {

	//Stage 1 (Table puzzles)
	GemPuzzle gemPuzzle;
	RotateObject rotateTablePuzzle;
	PadlockPuzzle padlockPuzzle;

	//Stage 2 (Box puzzles)
	ZinlockScript zinlockPuzzle;
	DominosaScript dominosaPuzzle;

	//Stage 3 (Pedestal & Skull puzzles
	//PedestalScript pedestalPuzzle;
	//PedestalRotatorScript skullPuzzle;

	
	public GameObject skullPrefab;
	public GameObject tableCenter;
	public GameObject box;
	public GameObject portal;
	public GameObject portalCenter;


	bool shownBoxAnimation;
	bool endingCenterAnimation;
	bool showingPortalAnimation;

	//Lerp related variables
	public float tableCenterAnimationDuration = 2.0f;
	public float boxAnimationDuration = 2.0f;
	public float distanceToMoveTableCenter = 3.0f;
	public float distanceToMoveBox = 1.0f;//7.0f;
	float lerpStartTime;
	bool movingTableCenter;
	bool movingBox;
	bool movingPortalCenter1;
	bool movingPortalCenter2;
	bool rotatingPortalCenter;

	Vector3 tableCenterOrigin;
	Vector3 tableCenterDestination;
	Vector3 boxOrigin;
	Vector3 boxDestination;
	Vector3 portalCenterOrigin;
	Vector3 portalCenterDestination;

	private bool openBox;
	private bool skullActivated;
	private bool padlockActivated;


	Toolbox toolbox;

	// Use this for initialization
	void Start () {
		Debug.Log("Starting tutorial level");
		shownBoxAnimation = false;
		endingCenterAnimation = false;
		showingPortalAnimation = false;

		toolbox = Toolbox.Instance;
		toolbox.SetUpForLevelBegin ();

		GameObject tableObj = GameObject.Find ("Table");
		GameObject rotateMiddleObj = GameObject.Find ("Middle_Rotate");
		GameObject padlockObj = GameObject.Find ("Padlock");
		GameObject zinlockObj = GameObject.Find ("Zinlock");
		GameObject dominosaObj = GameObject.Find ("Dominosa");
		GameObject pedestal = GameObject.Find ("Pedestal");
		GameObject pedestalRotator = GameObject.Find ("Pedestal Rotator");

		gemPuzzle = tableObj.GetComponent<GemPuzzle>();
		rotateTablePuzzle = rotateMiddleObj.GetComponent<RotateObject> ();
		padlockPuzzle = padlockObj.GetComponent<PadlockPuzzle> ();
		dominosaPuzzle = dominosaObj.GetComponent<DominosaScript> ();
		zinlockPuzzle = zinlockObj.GetComponent<ZinlockScript> ();
		//pedestalPuzzle = pedestal.GetComponent<PedestalScript> ();
		//skullPuzzle = pedestalRotator.GetComponent<PedestalRotatorScript> ();

		openBox = false;
		padlockActivated = false;
		skullActivated = false;
	}
	
	// Update is called once per frame
	void Update () {
		/* Puzzle order logic:
		 * same number means they can be solved at any order, all iterations need to be solved for next number
			1a. Gem placement
			1b. table rotate
			
			2a. Padlock
			
			3a. Zinlock
			3b. Dominosa

			4. Pedestal

			5. Pedestal Rotation

			6. Portal
		*/
		if (gemPuzzle.isFinished && rotateTablePuzzle.isFinished)
		{
			if(!padlockActivated){
				padlockActivated = true;
				padlockPuzzle.activatePadlock();
			}

			if(padlockPuzzle.isFinished)
			{
				if(!shownBoxAnimation){
					showBox();
				}

				if(dominosaPuzzle.isFinished && zinlockPuzzle.isFinished)
				{
					if(!openBox){
						openBox = true;
						openBoxAnimation();
					}
					/*
					if(pedestalPuzzle.isFinished){
						if(!skullActivated){
							skullActivated = true;
							skullPuzzle.activateRotator();
						}

						if(skullPuzzle.isFinished){
							if(!showingPortalAnimation){
								portalAnimationPart1();
							}
						}
					}*/
				}

			}
		}
	}

	void FixedUpdate()
	{
		if (movingTableCenter) {
			float timeSinceStarted = Time.time - lerpStartTime;
			float percentageComplete = timeSinceStarted/tableCenterAnimationDuration;

			tableCenter.transform.localPosition = Vector3.Lerp(tableCenterOrigin,tableCenterDestination,percentageComplete);
			if(percentageComplete >= 1f){
				movingTableCenter = false;
				if(!endingCenterAnimation){
					endTableCenterAnimation();
					startBoxAnimation();
				}
			}
		}

		if (movingBox) {
			float timeSinceStarted = Time.time - lerpStartTime;
			float percentageComplete = timeSinceStarted/boxAnimationDuration;

			box.transform.localPosition = Vector3.Lerp(boxOrigin,boxDestination,percentageComplete);
			if(percentageComplete >= 1f){
				movingBox = false;
			}
		}

		if (movingPortalCenter1) {
			float timeSinceStarted = Time.time - lerpStartTime;
			float percentageComplete = timeSinceStarted/2.0f;

			portalCenter.transform.localPosition = Vector3.Lerp(portalCenterOrigin,portalCenterDestination,percentageComplete);
			if(percentageComplete >= 1f){
				movingPortalCenter1 = false;
				portalAnimationPart2();
			}
		}

		if (movingPortalCenter2) {
			float timeSinceStarted = Time.time - lerpStartTime;
			float percentageComplete = timeSinceStarted/3.0f;

			portalCenter.transform.localPosition = Vector3.Lerp(portalCenterOrigin,portalCenterDestination,percentageComplete);
			if(percentageComplete >= 1f){
				movingPortalCenter2 = false;
				portalAnimationPart3();
			}
		}
	}

	void showBox()
	{
		shownBoxAnimation = true;
		startTableCenterAnimation ();
	}

	void startTableCenterAnimation()
	{
		movingTableCenter = true;
		lerpStartTime = Time.time;
		tableCenterOrigin = tableCenter.transform.localPosition;
		tableCenterDestination = tableCenterOrigin + Vector3.down * distanceToMoveTableCenter;
	}

	void endTableCenterAnimation()
	{
		endingCenterAnimation = true;
		movingTableCenter = true;
		lerpStartTime = Time.time;
		tableCenterOrigin = tableCenter.transform.localPosition;
		tableCenterDestination = tableCenterOrigin + Vector3.up * distanceToMoveTableCenter;
	}

	void startBoxAnimation()
	{
		movingBox = true;
		lerpStartTime = Time.time;
		boxOrigin = box.transform.localPosition;
		boxDestination = boxOrigin + Vector3.up * distanceToMoveBox;
	}

	void openBoxAnimation()
	{
		Animator animator = box.GetComponentInChildren<Animator>();
		if (animator) {
			Debug.Log("Found Animator!");
			animator.SetBool("isFinished", true);
			//StartCoroutine(disableBoxPuzzle(2));
		}
	}
	


	void portalAnimationPart1()
	{
		showingPortalAnimation = true;
		movingPortalCenter1 = true;
		lerpStartTime = Time.time;
		portalCenterOrigin = portalCenter.transform.localPosition;
		portalCenterDestination = portalCenterOrigin + Vector3.right * 1.0f;
	}

	void portalAnimationPart2()
	{
		movingPortalCenter2 = true;
		lerpStartTime = Time.time;
		portalCenterOrigin = portalCenter.transform.localPosition;
		portalCenterDestination = portalCenterOrigin + Vector3.forward * 12.0f;
	}

	void portalAnimationPart3()
	{
		Animator animator = portal.GetComponentInChildren<Animator> ();
		if (animator) {
			Debug.Log("Found Animator");
			animator.SetBool("isActivated",true);
		}
	}

	IEnumerator disableBoxPuzzle(float time)
	{
		yield return new WaitForSeconds(time);
		Debug.Log("Going to disable zinlock and dominosa");
		GameObject zinlockObj = GameObject.Find ("Zinlock");
		GameObject dominosaObj = GameObject.Find ("Dominosa");
		
		zinlockObj.SetActive(false);
		dominosaObj.SetActive(false);
	}
}
