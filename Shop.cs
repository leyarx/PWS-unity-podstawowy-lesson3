using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private static Shop _instance;
    public static Shop Instance { get { return _instance; } }

    public InventoryComponent playerInventory;
    public InventoryComponent sellerInventory;

    public Canvas playerInventoryCanvas;
    public Canvas sellerInventoryCanvas;

    public Text itemTitle;
    public Text itemDescription;
    public Text itemPrice;

    public Button BuyButton;
    public Button SellButton;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    shopItem selectedItem;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();

        UpdateInventoryCanvas();

        BuyButton.onClick.AddListener(BuyButtonClick);
        SellButton.onClick.AddListener(SellButtonClick);
    }

    void SetShopItems(InventoryComponent inventory, Canvas inventoryCanvas)
    {
        foreach (Transform child in inventoryCanvas.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in inventory.items)
        {
            Text newItem = new GameObject().AddComponent<Text>();
            shopItem newShopItem = newItem.gameObject.AddComponent<shopItem>();
            newShopItem.item = item;
            newShopItem.inventory = inventory;

            newItem.transform.SetParent(inventoryCanvas.transform, false);
            newItem.text = item.title;
            newItem.color = Color.black;
            newItem.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        }
    }

    void UpdateInventoryCanvas()
    {
        selectedItem = null;
        itemTitle.text = "";
        itemDescription.text = "";
        itemPrice.text = "";

        SetShopItems(playerInventory, playerInventoryCanvas);
        SetShopItems(sellerInventory, sellerInventoryCanvas);
    }

    public void BuyButtonClick()
    {
        Debug.Log("Buy");
        Shop.Instance.OnBuyButtonClick();
    }

    public void SellButtonClick()
    {
        Debug.Log("Sell");
        Shop.Instance.OnSellButtonClick();
    }

    public void OnBuyButtonClick()
    {
        if(selectedItem && sellerInventory == selectedItem.inventory)
        {
            playerInventory.BuyItem(selectedItem.item);
            sellerInventory.SellItem(selectedItem.item);
            UpdateInventoryCanvas();
        }
    }

    public void OnSellButtonClick()
    {
        if(selectedItem && playerInventory == selectedItem.inventory)
        {
            playerInventory.SellItem(selectedItem.item);
            sellerInventory.BuyItem(selectedItem.item);
            UpdateInventoryCanvas();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if the left Mouse button is clicked
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(m_PointerEventData, results);

            foreach (RaycastResult result in results)
            {
                shopItem trySelectItem = result.gameObject.GetComponent<shopItem>();

                if(trySelectItem)
                {
                    if(selectedItem)
                    {
                        selectedItem.GetComponent<Text>().color = Color.black;
                    }

                    selectedItem = trySelectItem;
                    selectedItem.GetComponent<Text>().color = Color.red;
                    itemTitle.text = selectedItem.item.title;
                    itemDescription.text = selectedItem.item.description;
                    itemPrice.text = selectedItem.item.price.ToString();
                }
            }
        }
    }
}
