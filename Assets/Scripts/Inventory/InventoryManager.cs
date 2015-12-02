using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

	/* List containing all inventory slots */
	[HideInInspector] public List<InvSlot> slots;
	/** The inventory item that is currently selected */
	[HideInInspector] public InvItem selectedItem = null;
	/** The inventory item that is currently being highlighted within an MenuInventoryBox element */
	[HideInInspector] public InvItem highlightItem = null;

	public GameObject buttonPrefab;
	public int inventorySlots;

	// Use this for initialization
	void Start () {
		slots = new List<InvSlot>();

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
			btn.transform.parent = transform;

			InvSlot slot = new InvSlot(i + 1, btn);
			slots.Add(slot);

			AddBtnListener(slot);

			position.y += btnTransform.sizeDelta.y + 20;
		}
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if(Input.touchCount > 0){
			ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		}
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			if(hit.collider.CompareTag("Pickup"))
			{

			}
		}
	}

	void AddItemToSlot(InvItem item, InvSlot slot)
	{

	}

	void AddBtnListener(InvSlot slot)
	{
		Button btn = slot.button.GetComponent<Button> ();
		btn.onClick.AddListener (() => {
			Debug.Log("" + slot.id);
		});
	}
}
