using UnityEngine;
using System.Collections;

public class GemPuzzle : Puzzle {

	public GameObject gemPrefab;

	public int placedGemsCount;
	public int gemsNeeded;

	// Use this for initialization
	new void Start () {
		base. Start ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void didPlaceObject(GameObject placedObject){
		PickupProperties validProperties  =  gemPrefab.GetComponent<PickupProperties>();
		PickupProperties properties = placedObject.GetComponent<PickupProperties> ();
		if(properties == null){
			return;
		}
		if (properties.pickupName == validProperties.pickupName) {
			placedGemsCount++;
			if(placedGemsCount == gemsNeeded){
				solvePuzzle();
			}
		}
	}

	public override void autoSolvePuzzle ()
	{
		if (!this.isFinished) {
			base.autoSolvePuzzle ();

			GameObject[] pickupsArray = GameObject.FindGameObjectsWithTag ("Pickup");
			GameObject[] containersArray = GameObject.FindGameObjectsWithTag ("Container");

			PickupProperties validProperties  =  gemPrefab.GetComponent<PickupProperties>();

			foreach (GameObject gem in pickupsArray) {
				PickupProperties properties = gem.GetComponent<PickupProperties> ();
				if(properties == null){
					continue;
				}
				if (properties.pickupName == validProperties.pickupName) {
					if (!properties.placedCorrectly) {
						foreach (GameObject container in containersArray) {
							ObjectContainer objectContainer = container.GetComponent<ObjectContainer> ();
							if (objectContainer != null && objectContainer.canContainObject (gem)) {
								Debug.Log("Add gem to container");
								objectContainer.addObject (gem);
							}
						}
						Destroy (gem);
					}
				}
			}

			pickupsArray = null;
			containersArray = null;
		}
	}
}
