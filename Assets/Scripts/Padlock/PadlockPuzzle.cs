using UnityEngine;
using System.Collections;

public class PadlockPuzzle : Puzzle {

	public GameObject top;
	public bool isActivated;

	public PadlockRotate cyllinder1;
	public PadlockRotate cyllinder2;
	public PadlockRotate cyllinder3;

	public int solution1;
	public int solution2;
	public int solution3;

	bool padlockAnimation;
	Quaternion startRotation;
	Quaternion endRotation;
	float timeAtStart;

	// Use this for initialization
	new void Start () {
		base.Start ();
		isActivated = false;
		//TODO: Temp code
		GameObject cube = GameObject.Find ("Padlock/Cube");
	}
	
	// Update is called once per frame
	void Update () {
		if (padlockAnimation) {
			float deltaTime = Time.time - timeAtStart;
			float percentage = deltaTime / 1.0f;
			top.transform.rotation = Quaternion.Lerp(startRotation, endRotation, percentage);

			if(percentage >= 1.0f)
			{
				print ("Finished padlock animation");
				padlockAnimation = false;
			}
		}
	}

	public void activatePadlock()
	{
		//TODO: Animation when texture is done
		if (!isActivated) {
			isActivated = true;
			padlockAnimation = true;
			startRotation = top.transform.rotation;

			Quaternion finalRotation = Quaternion.identity;
			finalRotation.eulerAngles = new Vector3(140f,0f,0f);
			endRotation = finalRotation;
			timeAtStart = Time.time;
		}
	}

	public override void EnablePuzzle (bool enable)
	{
		base.EnablePuzzle (enable);
		if (isActivated) {
			cyllinder1.enabled = cyllinder2.enabled = cyllinder3.enabled = enable;
		} else {
			cyllinder1.enabled = cyllinder2.enabled = cyllinder3.enabled = false;
		}
	}

	public override void autoSolvePuzzle ()
	{
		if (!this.isFinished && this.isActivated) {
			base.autoSolvePuzzle ();
		}
	}

	public void cyllinderFinishedRotating()
	{

		int value1 = cyllinder1.selectedValue ();
		int value2 = cyllinder2.selectedValue ();
		int value3 = cyllinder3.selectedValue ();

		Debug.Log ("Selected values: " + value1 + "-" + value2 + "-"+value3);
		if (value1 == solution1 && value2 == solution2 && value3 == solution3) {
			this.solvePuzzle();
			cyllinder1.enabled = false;
			cyllinder2.enabled = false;
			cyllinder3.enabled = false;
		}
	}
}
