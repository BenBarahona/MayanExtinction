using UnityEngine;
using System.Collections;

[System.Serializable]
public class InvSlot 
{
	/* The category's editor name */
	public string label;
	/* A unique identifier */
	public int id;
	/* Item currently in slot */
	public InvItem item;
	/* The slot gameobject button */
	public GameObject button;

	/**
		 * <summary>The default Constructor.</summary>
		 * <param name = "idArray">An array of already-used ID numbers, so that a unique one can be generated</param>
		 * <param name = "id"> unique ID </param>
		 * <param name = "gameObject"> The button on the screen </param>
		 */
	public InvSlot (int id, GameObject gameObject)
	{
		this.id = id;
		this.button = gameObject;
		this.label = "Slot " + id.ToString();
		this.item = null;
	}
	/*
	public InvSlot (int[] idArray)
	{
		id = 0;
		
		foreach (int _id in idArray)
		{
			if (id == _id)
			{
				id ++;
			}
		}
		
		label = "Category " + (id + 1).ToString ();
	}
	*/
}
