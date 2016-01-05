using UnityEngine;
using System.Collections;

[System.Serializable]
public class InvItem
{
	/* The item's Editor name */
	public string label;
	/* A unique identifier */
	public int id;
	/* The item's in-game name, if not label */
	public string altLabel;
	/* Item image */
	public Sprite image;
	/* If multiple instances of the item can be stored */
	public bool canStoreMultiple;
	/* Item GameObject */
	public GameObject gameObject;

	/**
		 * <summary>The default Constructor.</summary>
		 * <param name = "idArray">An array of already-used ID numbers, so that a unique ID number can be assigned</param>
		 */
	public InvItem (/*int[] idArray, */GameObject itemObject)
	{
		id = 0;
		
		label = "Inventory item " + (id + 1).ToString ();
		altLabel = "";
		canStoreMultiple = false;

		PickupProperties properties = itemObject.GetComponent<PickupProperties> ();
		if (properties != null) {
			gameObject = properties.prefab;
			image = properties.image;
			altLabel = properties.pickupName;
		}
	}
	
	
	/*
		* <summary>A Constructor that sets all it's values by copying another InvItem.</summary>
		* <param name = "assetItem">The InvItem to copy</param>
	*/
	public InvItem (InvItem assetItem)
	{
		id = assetItem.id;
		label = assetItem.label;
		altLabel = assetItem.altLabel;
		image = assetItem.image;
		canStoreMultiple = assetItem.canStoreMultiple;
	}
}
