using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour
{ 
    public int current_stack_size = 0;
    public ItemScriptableObject configuration;
    private float y_rotation;

    private void Update()
    {
        if(tag == "Item")
        {
            y_rotation += 0.5f;
            Vector3 spin_angle = new Vector3(0f, y_rotation, 0f);
            transform.eulerAngles = spin_angle;
        }
    }

}
