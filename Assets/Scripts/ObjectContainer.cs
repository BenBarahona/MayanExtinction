using UnityEngine;
using System.Collections;

public class ObjectContainer : MonoBehaviour {

	// Use this for initialization
	public bool containsObject;

	public GameObject containedObject;
	public GameObject validObjectToContain;
	
	public Vector3 objectScale;
	public Vector3 objectOffset;
	public Vector3 objectRotation;

	public bool canContainObject(GameObject candidate){
		if (!containsObject && validObjectToContain != null) {
			PickupProperties candidateProperties = candidate.GetComponent<PickupProperties>();
			PickupProperties validProperties = validObjectToContain.GetComponent<PickupProperties>();
			if(validProperties == null || candidateProperties == null){
				return false;
			}else{
				if (candidateProperties.pickupName == validProperties.pickupName) {
					return true;
				} else {
					return false;
				}
			}
		} else {
			return false;
		}
	}	

	public void addObject(GameObject objectToAdd){
		GameObject newGameObject = (GameObject)Instantiate (validObjectToContain);
		newGameObject.transform.parent = transform;
		newGameObject.transform.localScale = objectScale;
		newGameObject.transform.localPosition = objectOffset;
		newGameObject.transform.localEulerAngles = objectRotation;

		containedObject = newGameObject;
		PickupProperties properties = containedObject.GetComponent<PickupProperties> ();
		containsObject = true;
		if (properties != null) {
			properties.placedCorrectly = true;
		}
		SendMessageUpwards ("didPlaceObject", newGameObject,SendMessageOptions.DontRequireReceiver);
	}
}
