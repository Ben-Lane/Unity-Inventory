using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class ItemHandler : MonoBehaviour
{
    //data from slot being dragged 
    private int stack_size = 0;
    private Vector3 old_position;
       //, IBeginDragHandler, IDragHandler, IEndDragHandler

    /**
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Started to move item");

        //store data locally
        stack_size = this.GetComponent<ItemControl>().current_stack_size;
        old_position = transform.GetChild(0).transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Moving Item");
        if (stack_size > 0)
        {
            Vector3 mouse_position = new Vector3(eventData.position.x, eventData.position.y, 10);
            transform.GetChild(0).transform.position = mouse_position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Stopped moving item");

    }
    **/
}

