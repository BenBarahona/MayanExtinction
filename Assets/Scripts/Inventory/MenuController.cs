using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	public GameObject buttonPrefab;
	GameObject canvasMenu;

	bool inventoryHidden;

	// Use this for initialization
	void Start () {
		inventoryHidden = true;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void toggleInventoryPanel()
	{
		print ("Hola Mundo");
		/*
		Vector3 newPos = Vector3.zero;//panelTransform.position;
		
		float oldPosition = newPos.y;
		if (inventoryHidden) {
			//newPos.y = originalX;
			inventoryHidden = false;
		} else {
			newPos.y = 0;
			inventoryHidden = true;
		}
		panelTransform.position = newPos;
		*/
	}
}
