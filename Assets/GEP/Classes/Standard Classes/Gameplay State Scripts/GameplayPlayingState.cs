using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayPlayingState : GameplayBaseState
{
    private bool inventory_full = false;
    private float inventory_full_timer = 0;

    public override void EnterState(GameplayStateHandler player, PlayerCharacterInput player_input)
    {
        Debug.Log("Entering Play State");
        player_input.lock_inputs = false;
  
    }

    public override void UpdateState(GameplayStateHandler player, PlayerCharacterInput player_input)
    {
        if (player_input.toggleInventory)
        {
            player_input.move = new Vector2(0, 0);
            player.SwitchState(player.InventoryState);
        }

        if(inventory_full)
        {
            inventory_full_timer += Time.deltaTime;

            if(inventory_full_timer >= 1.0)
            {
                Debug.Log("stop text");
                inventory_full_timer = 0;
                inventory_full = false;
                player.inventory_full_text.SetActive(false);
            }
        }
    }

    public void OnTriggerEnter(GameplayStateHandler player, Collider collision)
    {
        Debug.Log("near item");

        //Adds item picked up to the back of the list
        GameObject temp_item = collision.gameObject;
        int item_total = TotalElements(player);

        //destroys item
        Object.Destroy(collision.gameObject);

        //if the inventory already has some items.
        if(player.slots.Count != 0)
        {
            //check if item already exists in the inventory, and creates a list of all its locations if so
            List<int> other_stack_locations = ItemAlreadyExists(temp_item.GetComponent<ItemControl>().configuration, player.slots);
            
            //fills up existing stacks and returns the leftovers in the variable stack size.
            temp_item.GetComponent<ItemControl>().current_stack_size = CalculateStacks(temp_item.GetComponent<ItemControl>().configuration, other_stack_locations, player.slots, temp_item.GetComponent<ItemControl>().current_stack_size);
            Debug.Log(temp_item.GetComponent<ItemControl>().current_stack_size);
        }

        //if we still have some of the item left to add
        if (temp_item.GetComponent<ItemControl>().current_stack_size >= 1)
        {
            int index = 0;
            //checks if there is a free slot
            if(SlotAvailable(player.slots, ref index))
            {
                //takes icon from data and adds to ui inventory
                Debug.Log("Add to visible inventory");
                AddToInventory(temp_item, index, player.slots);
                //update stack size
                //player.slots[item_total].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = temp_item.GetComponent<ItemControl>().current_stack_size.ToString();

            }
            else
            {
                Debug.Log("Inventory Full");
                player.InventoryState.SpawnSelectedItem(temp_item, player.transform, 0f, 1f);
                inventory_full = true;
                player.inventory_full_text.SetActive(true);
                ///
                /// Add a visual on screen to say inventory full
                ///

            }
            //Debug.Log(slots[0]);
        }
    }

    //count of list
    int TotalElements(GameplayStateHandler player)
    {
        int total = 0;
        for (int i = 0; i < player.slots.Count; i++) if (player.slots[i].GetComponent<ItemControl>().current_stack_size != 0) total += 1;
        Debug.Log(total.ToString() + "elemnts");
        return total;
    }

    //checks if their is an available slot in the inventory
    bool SlotAvailable(List<GameObject> slots, ref int index)
    {
        bool availability = false;

        for(int i = 0; i < slots.Count; i++)
        {
            if (slots[i].GetComponent<ItemControl>().configuration.maximum_stack_size == 0)
            {
                index = i;
                availability = true;
                Debug.Log("Inventory has space");
                break;
            }
        }

        return availability;
    }

    //adds an item to the visible inventory
    void AddToInventory(GameObject item, int index, List<GameObject> slots)
    {
        //adding the required data to the slots children, image (icon) and text (stack_size)
        //updates meain data
        slots[index].GetComponent<ItemControl>().configuration = item.GetComponent<ItemControl>().configuration;
        slots[index].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = item.GetComponent<ItemControl>().configuration.icon;
        //updates item count
        slots[index].GetComponent<ItemControl>().current_stack_size = item.GetComponent<ItemControl>().current_stack_size;
        slots[index].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.GetComponent<ItemControl>().current_stack_size.ToString();
    }

    //checks if an item already exists, so we can maybe add this new stack to the old stack
    List<int> ItemAlreadyExists(ItemScriptableObject item, List<GameObject> slots)
    {
        List<int> locations = new List<int>();
        //if items icon == an image in the inventory
        for (int i = 0; i < slots.Count; i++)
        {
            if (item.icon == slots[i].transform.GetChild(0).gameObject.GetComponent<Image>().sprite)    
            {
                locations.Add(i);
            }
        }
        return locations;
    }

    //Calculates and updates stacks and returns the remainder
    int CalculateStacks(ItemScriptableObject item, List<int> slot_position, List<GameObject> slots, int stack_size)
    {
        //stack of item being added
        int remaining_stack = stack_size;
        for(int i = 0; i < slot_position.Count; i++)
        {
            //add new stack to other existing ones
            int temporary_stack_size = slots[slot_position[i]].GetComponent<ItemControl>().current_stack_size + remaining_stack;
            //gives us the remaining amount of stack after adding them
            remaining_stack = temporary_stack_size - item.maximum_stack_size;

            if (temporary_stack_size > item.maximum_stack_size)
            {
                slots[slot_position[i]].GetComponent<ItemControl>().current_stack_size = item.maximum_stack_size;
                slots[slot_position[i]].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.maximum_stack_size.ToString();
            }
            else
            {
                slots[slot_position[i]].GetComponent<ItemControl>().current_stack_size = temporary_stack_size;
                slots[slot_position[i]].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = temporary_stack_size.ToString();
            }
        }
        return remaining_stack;
    }
}
