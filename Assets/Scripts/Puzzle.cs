using UnityEngine;
using System.Collections;

public class Puzzle : MonoBehaviour {

	public bool isFinished;
	public string puzzleName;
	public bool disableAfterSolve;
	public bool zoomOutOnSolve;
	public bool alwaysActive;
	protected Toolbox toolbox;

	// Use this for initialization
	protected virtual void Start () {
		//print ("Starting puzzle: " + this.puzzleName);
		toolbox = Toolbox.Instance;
		EnablePuzzle (false);
	}
	
	// Update is called once per frame
	void Update () {
	 
	}

	public virtual void EnablePuzzle(bool enable)
	{
		if (!alwaysActive) {
			this.enabled = enable;
		}
		if (enable) {
			toolbox.currentPuzzle = this;
		} else {
			toolbox.currentPuzzle = null;
		}
	}

	public virtual void solvePuzzle()
	{
		print ("Solve Puzzle: " + this.puzzleName);
		isFinished = true;

		if (disableAfterSolve) {
			Collider collider = this.GetComponent<Collider> ();
			if (collider != null) {
				collider.enabled = false;
			}
		}
		if(zoomOutOnSolve) {
			GameObject camera = GameObject.Find ("Main Camera");
			CameraZoom script = camera.GetComponent<CameraZoom>();
			script.zoomOutToParent();
		}
	}

	public virtual void autoSolvePuzzle()
	{
		solvePuzzle ();
	}

	public override string ToString()
	{
		return this.puzzleName;
	}
}
