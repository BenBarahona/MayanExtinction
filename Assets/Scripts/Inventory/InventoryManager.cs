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

	public GameObject canvasGameObject;
	public GameObject buttonPrefab;
	public int inventorySlots;
	public CustomCameraScript inventoryCamera;
	public MainCameraScript mainCamera;
	[HideInInspector] public CustomCameraScript originCamera;
	[HideInInspector] public GameObject inventoryGameObject;

	private bool isViewingItem;
	private int[] itemIds;
	private GameObject displayedObj;

	void Awake()
	{
		Debug.Log ("Inv Manager Awake");
	}

	// Use this for initialization
	void Start () {

		Debug.Log ("Inv Manager Start");

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
		
		Vector2 position = new Vector2(btnTransform.sizeDelta.x / 2, btnTransform.sizeDelta.y / 2);
		
		//TODO: Center all items relative to screen height
		for(int i = 0; i < inventorySlots; i++)
		{
			GameObject btn = (GameObject)Instantiate (buttonPrefab, position, Quaternion.identity);
			btn.transform.SetParent(canvasGameObject.transform, false);
			
			InvSlot slot = new InvSlot(i + 1, btn);
			AddBtnListener(slot);
			
			slotList.Add(slot);
			position.y += btnTransform.sizeDelta.y + 20;
		}
		
		Debug.Log ("Slots: " + slotList.Count);
		Toolbox.Instance.inventoryManager = this;
		inventoryGameObject = GameObject.Find ("/Inventory");
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void PickUpGameObject(GameObject gameObject)
	{
		Debug.Log ("Pickup: " + gameObject.name);
		Debug.Log ("Slots: " + slotList.Count);
		InvItem newItem = new InvItem (gameObject);

		foreach(InvSlot slot in slotList)
		{
			Debug.Log ("Item:" + slot.item + " SLOT: " + slot.id);
			if(slot.item == null || (newItem.canStoreMultiple && slot.item.altLabel.Equals(newItem.altLabel)))
			{
				AddItemToSlot(newItem, slot);
				break;
			}
		}
		//gameObject.SetActive (false);
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
			//Debug.Log (slot.item);
			Debug.Log ("Slots: " + slotList.Count);
			if(slot.item == null)
				Debug.Log (slot.id + "- Empty");
			else
				Debug.Log(slot.id + "- " + slot.item.altLabel);

			HandleSlotClick(slot);
		});
	}

	void HandleSlotClick(InvSlot slot)
	{
		if (isViewingItem) {
			isViewingItem = false;
			mainCamera.SetGameCamera(originCamera, 0);
			Toolbox.Instance.SetGameState(GameState.Resumed);
			Destroy(displayedObj);
		}
		else if (slot.item != null) 
		{
			//Debug.Log (slot.item.gameObject);
			isViewingItem = true;
			originCamera = mainCamera.currentCamera;
			mainCamera.SetGameCamera(inventoryCamera, 0);
			Toolbox.Instance.SetGameState(GameState.Inventory);
			displayedObj = (GameObject)Instantiate(slot.item.gameObject, Vector2.zero, Quaternion.identity);
			displayedObj.SetActive(true);
			displayedObj.transform.SetParent(inventoryGameObject.transform, false);
			displayedObj.AddComponent<SpinObject>();
		}
	}
}
