using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ItemSciptableObjects")]
public class ItemScriptableObject : ScriptableObject
{
    [Header("Prefabs")]
    public Sprite icon;
    public GameObject model;

    [Header("General Variables")]
    public string item_name;
    public string item_description;
    public int maximum_stack_size;
}
