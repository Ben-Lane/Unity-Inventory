using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GameplayInventoryState : GameplayBaseState
{
    //General Variables
    private int selected_item_position = -1;
    private GraphicRaycaster ui_raycaster;

    private PointerEventData click_data;
    private List<RaycastResult> click_results;

    public override void EnterState(GameplayStateHandler player, PlayerCharacterInput player_input)
    {
        Debug.Log("Entering Inventory State");
        player_input.lock_inputs = true;

        //clears selected item position
        selected_item_position = -1;

        ui_raycaster = player.inventory_canvas.GetComponent<GraphicRaycaster>();
        click_data = new PointerEventData(EventSystem.current);
        click_results = new List<RaycastResult>();
    }

    public override void UpdateState(GameplayStateHandler player, PlayerCharacterInput player_input)
    {
        //exit inventory
        if (!player_input.toggleInventory)
        {
            if(selected_item_position >= 0) player.slots[selected_item_position - 1].GetComponent<Image>().color = Color.red;
            player.inventory_details.SetActive(false);
            player.SwitchState(player.PlayingState);
        }
           

        //if the player clicks the mouse and is within the bounds of a slot, set selected_item_position to the position in the inventory(list position)

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            //get mouse position
            click_data.position = Mouse.current.position.ReadValue();

            //clear previous list of objects
            click_results.Clear();

            //casts raycast and adds every UI collision
            ui_raycaster.Raycast(click_data, click_results);


            foreach(RaycastResult result in click_results)
            {
                //if an item is a slot set selected slot to the right value
                if (result.gameObject.tag == "Slot")
                {
                    player_input.drop_item = false;
                    if (selected_item_position >= 0) player.slots[selected_item_position - 1].GetComponent<Image>().color = Color.red;
                    selected_item_position = int.Parse(result.gameObject.name.Replace("Slot ", ""));
                    result.gameObject.GetComponent<Image>().color = Color.green;

                    if (player.slots[selected_item_position - 1].GetComponent<ItemControl>().current_stack_size > 0)
                    {
                        player.inventory_details.SetActive(true);
                        SetDetails(player.inventory_details, player.slots[selected_item_position - 1].GetComponent<ItemControl>());
                    }
                    else
                    {
                        player.inventory_details.SetActive(false);
                    }

                    Debug.Log(selected_item_position);
                    break;
                }
            }
        }

        
        if(selected_item_position >= 0)
        {
            //if player presses the drop key and there is something in the selected slot
            if (player_input.drop_item && (player.slots[selected_item_position - 1].GetComponent<ItemControl>().current_stack_size != 0))
            {

                SpawnSelectedItem(player.slots[selected_item_position - 1], player.transform, 0.5f, 1f);
                DeleteFromInventory(player.slots[selected_item_position - 1], player);
                //reset button to allow for more drops
                player.slots[selected_item_position - 1].GetComponent<Image>().color = Color.red;
                player_input.drop_item = false;
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
