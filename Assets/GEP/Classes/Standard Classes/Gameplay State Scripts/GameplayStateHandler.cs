using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameplayStateHandler : MonoBehaviour
{
    public GameplayBaseState current_state;
    public PlayerCharacterInput player_input;
    public GameplayPlayingState PlayingState = new GameplayPlayingState();
    public GameplayInventoryState InventoryState = new GameplayInventoryState();
    public List<GameObject> slots;
    public GameObject inventory_canvas;
    public GameObject inventory_full_text;
    public GameObject inventory_details;

    // Start is called before the first frame update
    void Start()
    {
        inventory_canvas = GameObject.Find("Inventory");
        player_input = GameObject.Find("Player Character").GetComponent<PlayerCharacterInput>();
        slots = GameObject.Find("Inventory").GetComponent<InventoryUIHandler>().inventory_slots;
        current_state = PlayingState;
        current_state.EnterState(this, player_input);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (current_state == PlayingState && other.tag == "Item") PlayingState.OnTriggerEnter(this, other);
    }
    // Update is called once per frame
    void Update()
    {
        current_state.UpdateState(this, player_input);
    }

    public void SwitchState(GameplayBaseState state)
    {
        current_state = state;
        current_state.EnterState(this, player_input);
    }
}
