using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public abstract class GameplayBaseState
{
    public abstract void EnterState(GameplayStateHandler player, PlayerCharacterInput player_input);

    public abstract void UpdateState(GameplayStateHandler player, PlayerCharacterInput player_input);
}
