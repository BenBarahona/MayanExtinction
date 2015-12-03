using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

	/* List containing all inventory slots */
	[HideInInspector] public List<InvSlot> slotList;
	/** The inventory item that is currently selected */
	[HideInInspector] public InvItem selectedItem = null;
	/** The inventory item that is currently being highlighted within an MenuInventoryBox element */
	[HideInInspector] public InvItem highlightItem = null;

	public GameObject buttonPrefab;
	public int inventorySlots;

	private int[] itemIds;

	// Use this for initialization
	void Start () {
		slotList = new List<InvSlot>();

		/* Creating inventory slots
		 * Slots squares that are 7% of the screen's width
		 */
		float idealSize = Screen.width * 0.07f;

		//Checking if idealSize for all slots fits screen height
		float slotsHeight = 20 + (idealSize + 20) * inventorySlots;
		while (slotsHeight > Screen.height) {
			idealSize -= 3;
			slotsHeight = 20 + (idealSize + 20) * inventorySlots;
		}

		RectTransform btnTransform = buttonPrefab.GetComponent<RectTransform>();
		btnTransform.sizeDelta = new Vector2 (idealSize, idealSize);

		Vector2 position = new Vector2(20 + btnTransform.sizeDelta.x / 2, 20 + btnTransform.sizeDelta.y / 2);

		//TODO: Center all items relative to screen height
		for(int i = 0; i < inventorySlots; i++)
		{
			GameObject btn = (GameObject)Instantiate (buttonPrefab, position, Quaternion.identity);
			btn.transform.SetParent(transform);

			InvSlot slot = new InvSlot(i + 1, btn);
			slotList.Add(slot);

			AddBtnListener(slot);

			position.y += btnTransform.sizeDelta.y + 20;
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void PickUpGameObject(GameObject gameObject)
	{
		Debug.Log ("Pickup: " + gameObject.name);
		InvItem newItem = new InvItem (gameObject);

		foreach(InvSlot slot in slotList)
		{
			if(slot.item == null || (newItem.canStoreMultiple && slot.item.altLabel.Equals(newItem.altLabel)))
			{
				AddItemToSlot(newItem, slot);
				break;
			}
		}

		Destroy (gameObject);
	}

	void AddItemToSlot(InvItem item, InvSlot slot)
	{
		slot.item = item;
		if (slot.image != null) {
			slot.image.sprite = item.image;
		}
	}

	void AddBtnListener(InvSlot slot)
	{
		Button btn = slot.button.GetComponent<Button> ();
		btn.onClick.AddListener (() => {
			if(slot.item == null)
				Debug.Log (slot.id + "- Empty");
			else
				Debug.Log(slot.id + "- " + slot.item.altLabel);

			handleSlotClick(slot);
		});
	}

	void HandleSlotClick(InvSlot slot)
	{
		if (slot.item != null) {

		}
	}
}
