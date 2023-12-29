using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InventoryUIHandler : MonoBehaviour
{

    [SerializeField] private int grid_width = 5;
    [SerializeField] private int grid_height = 3;

    private int image_size = 64;
    [SerializeField] private float padding_value = 5f;

    private Vector2 slot_panel_size;
    private Vector2 inventory_panel_size;

    public TextMeshProUGUI inventory_title;
    public GameObject inventory_panel;
    public GameObject slot_panel;
    [SerializeField] private GameObject slot_prefab;
    [SerializeField] private GameObject info_card;

    //list of slots
    public List<GameObject> inventory_slots = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //Create the size of the inventory
        slot_panel_size = new Vector2(((grid_width + 1) * padding_value) + (grid_width * image_size), ((grid_height + 1) * padding_value) + (grid_height * image_size));
        inventory_panel_size = new Vector2((2 * padding_value) + slot_panel_size.x, 
            (2 * padding_value) + slot_panel_size.y + (padding_value + inventory_title.GetComponent<RectTransform>().sizeDelta.y));
        inventory_title.GetComponent<RectTransform>().sizeDelta = new Vector2(slot_panel_size.x/3, inventory_title.GetComponent<RectTransform>().sizeDelta.y);

        //pass that size into the console
        Debug.Log(slot_panel_size);
        Debug.Log(inventory_panel_size);

        //apply the new sizing
        inventory_panel.GetComponent<RectTransform>().sizeDelta = inventory_panel_size;
        slot_panel.GetComponent<RectTransform>().sizeDelta = slot_panel_size;
        info_card.GetComponent<RectTransform>().sizeDelta = new Vector2(info_card.GetComponent<RectTransform>().sizeDelta.x, inventory_panel_size.y);

        //adjust positions due to size adjustment
        inventory_title.GetComponent<RectTransform>().localPosition = new Vector3(0, slot_panel_size.y - (inventory_panel_size.y / 2) + padding_value, 0);
        slot_panel.GetComponent<RectTransform>().localPosition = new Vector3(-slot_panel_size.x/2, -inventory_panel_size.y/2 + padding_value, 0);
        info_card.GetComponent<RectTransform>().localPosition = new Vector3(info_card.GetComponent<RectTransform>().localPosition.x, 
            inventory_panel.GetComponent<RectTransform>().localPosition.y, 0);

        //create grid
        for (int i = 0; i < grid_height; i++)
        {
            for(int j = 0; j < grid_width; j++)
            {
                //create inventory slot
                GameObject slot = Instantiate(slot_prefab, new Vector3(0, 0, 0), Quaternion.identity);
                inventory_slots.Add(slot);
                slot.AddComponent<ItemControl>();
                slot.name = "Slot " + (1 + ((i * grid_width) + j)).ToString();
                slot.transform.SetParent(slot_panel.transform);
                slot.GetComponent<RectTransform>().localPosition = new Vector3(padding_value + j * (image_size + padding_value), 
                    (slot_panel_size.y - padding_value) + (-i * (image_size + padding_value)),0);
            }
        }
    }
}
