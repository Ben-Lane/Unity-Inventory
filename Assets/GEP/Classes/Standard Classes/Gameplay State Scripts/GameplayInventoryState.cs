using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class GameplayInventoryState : GameplayBaseState
{
    //General Variables
    private int selected_item_position = -1;
    private GraphicRaycaster ui_raycaster;

    private PointerEventData click_data;
    private List<RaycastResult> click_results;

    private bool being_dragged;
    private GameObject drag_icon;

    //variables for storing data
    private Sprite initial_icon;
    private int initial_stack;
    private Sprite new_icon;
    private int new_stack;

    private List<int> slot_index = new List<int>();

    public override void EnterState(GameplayStateHandler player, PlayerCharacterInput player_input)
    {
        Debug.Log("Entering Inventory State");
        player_input.lock_inputs = true;

        //clears selected item position
        selected_item_position = -1;

        ui_raycaster = player.inventory_canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();

        if (drag_icon == null) drag_icon = GameObject.Find("Canvas").transform.GetChild(0).gameObject;
    }

    public override void UpdateState(GameplayStateHandler player, PlayerCharacterInput player_input)
    {
        if (being_dragged)
        {
            drag_icon.transform.position = new Vector3(Mouse.current.position.x.magnitude, Mouse.current.position.y.magnitude, 0f);
        }

        //initial click of drag
        if (Mouse.current.leftButton.wasPressedThisFrame && !being_dragged)
        {
            GetSlot(ref initial_icon, ref initial_stack);
            if (initial_icon != null)
            {
                Debug.Log("Slot stack size: " + initial_stack);
                if (initial_icon != null)
                {
                    being_dragged = true;

                    drag_icon.GetComponent<Image>().sprite = initial_icon;
                    player.slots[slot_index[0]].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    player.slots[slot_index[0]].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";

                    drag_icon.SetActive(true);
                }
            }

        }
        
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            GetSlot(ref new_icon, ref new_stack);
            if(new_icon != null && being_dragged)
            {
                Debug.Log("Adjust slots");

                Debug.Log("updated slot " + slot_index[1]);
                player.slots[slot_index[1]].transform.GetChild(0).GetComponent<Image>().sprite = initial_icon;
                player.slots[slot_index[1]].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = initial_stack.ToString();
                player.slots[slot_index[1]].GetComponent<ItemControl>().current_stack_size = initial_stack;
                //player.slots[slot_index[1]].GetComponent<ItemControl>().configuration = tempNewSlot.configuration;

                Debug.Log("adjusted slot " + slot_index[0]);
                player.slots[slot_index[0]].transform.GetChild(0).GetComponent<Image>().sprite = new_icon;
                player.slots[slot_index[0]].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = new_stack.ToString();
                player.slots[slot_index[0]].GetComponent<ItemControl>().current_stack_size = new_stack;
                //player.slots[slot_index[0]].GetComponent<ItemControl>().configuration = tempOriginalSlot.configuration;


            }
            else if(new_icon == null && being_dragged)
            {
                Debug.Log("Reset slot " + slot_index[0]);
                player.slots[slot_index[0]].transform.GetChild(0).GetComponent<Image>().sprite = initial_icon;
                player.slots[slot_index[0]].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = initial_stack.ToString();
                player.slots[slot_index[0]].GetComponent<ItemControl>().current_stack_size = new_stack;
                //player.slots[slot_index[0]].GetComponent<ItemControl>().configuration = initial_slot.GetComponent<ItemControl>().configuration;
            }
            slot_index.Clear();
            being_dragged = false;
            drag_icon.SetActive(false);
            Debug.Log("DroppedItem");
        }

        //exit inventory
        if (!player_input.toggleInventory)
        {
            drag_icon.SetActive(false);
            if (selected_item_position >= 0) player.slots[selected_item_position - 1].GetComponent<Image>().color = Color.red;
            player.inventory_details.SetActive(false);
            player.SwitchState(player.PlayingState);
        }
    }

    void GetSlot(ref Sprite sprite, ref int stack)
    {
        //get mouse position
        click_data.position = Mouse.current.position.ReadValue();

        //clear previous list of objects
        click_results.Clear();

        //casts raycast and adds every UI collision
        ui_raycaster.Raycast(click_data, click_results);
        foreach (RaycastResult result in click_results)
        {
            if (result.gameObject.tag == "Slot")
            {
                string index = "";
                for (int i = result.gameObject.name.Length - 1; i > -1; i--)
                {
                    Debug.Log(i);
                    if (result.gameObject.name[i] == ' ')
                    {
                        break;
                    }
                    index = result.gameObject.name[i] + index;
                }
                Debug.Log(index);
                slot_index.Add(int.Parse(index) - 1);
                Debug.Log("Valid slot");
                sprite = result.gameObject.GetComponent<ItemControl>().configuration.icon;
                stack = result.gameObject.GetComponent<ItemControl>().current_stack_size;
            }
        }
    }

    public void SpawnSelectedItem(GameObject item, Transform tform, float boost, float height)
    {
        Debug.Log("drop item");
        Debug.Log(item);

        //spawn object
        GameObject spawned_object = Object.Instantiate(item.GetComponent<ItemControl>().configuration.model,
            new Vector3(tform.position.x + (tform.forward.x * tform.localScale.x),
            tform.position.y + height, tform.position.z + (tform.forward.z)), Quaternion.identity);

        //Give it item data
        spawned_object.name = item.GetComponent<ItemControl>().configuration.item_name;
        spawned_object.tag = "Item";
        spawned_object.GetComponent<ItemControl>().current_stack_size = item.GetComponent<ItemControl>().current_stack_size;

        //Impulse
        spawned_object.GetComponent<Rigidbody>().velocity = tform.forward * boost;
    }

    void DeleteFromInventory(GameObject item, GameplayStateHandler player)
    {
        //deletes image
        item.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = null;
        //deletes stack sized
        item.gameObject.GetComponent<ItemControl>().current_stack_size = 0;
        item.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
        player.inventory_details.SetActive(false);
    }

    void SetDetails(GameObject details, ItemControl item)
    {
        details.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.configuration.item_name;
        details.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.configuration.item_description;
        details.transform.GetChild(2).GetComponent<Image>().sprite = item.configuration.icon;
    }
}
