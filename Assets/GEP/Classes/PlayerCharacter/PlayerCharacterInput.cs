using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterInput : MonoBehaviour
{

    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    public bool analogMovement;

    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public bool drop_item = false;

    public bool toggleInventory = false;
    public GameObject inventory;

    public bool lock_inputs = false;

    public void OnToggle()
    {
        toggleInventory = !toggleInventory;
        inventory.SetActive(toggleInventory);
        Cursor.visible = toggleInventory;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnDropItem()
    {
        if (!drop_item)
        {
            drop_item = true;
            Debug.Log("q pressed");
        }
    }

    public void OnMove(InputValue value)
    {
        if(!lock_inputs) MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            if (!lock_inputs)  LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        if (!lock_inputs) JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        if (!lock_inputs)  SprintInput(value.isPressed);
    }

    public void MoveInput(Vector2 newMoveDirection)
    {
        if (!lock_inputs)  move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        if (!lock_inputs)  look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        if (!lock_inputs)  jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        if (!lock_inputs)  sprint = newSprintState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!lock_inputs)  SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        if (!lock_inputs)  Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
