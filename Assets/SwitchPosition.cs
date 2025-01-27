using UnityEngine;
using UnityEngine.InputSystem;
public class SwitchPosition : MonoBehaviour
{
    public Transform roomPosition;
    public Transform externalPosition;
    private bool isInRoom = true;
    
    public InputActionReference action;

    void Update()
    {
        // Check for button press (e.g., "Fire1" for the primary button)
        action.action.Enable();
        action.action.performed += (ctx) =>
        {   
           SwitchPlayerPosition();
        };
    }

    void SwitchPlayerPosition()
    {
        if (isInRoom)
        {
            transform.position = externalPosition.position;
            transform.rotation = externalPosition.rotation;
        }
        else
        {
            transform.position = roomPosition.position;
            transform.rotation = roomPosition.rotation;
        }
        isInRoom = !isInRoom;
    }
}