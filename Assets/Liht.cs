using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;

public class Liht : MonoBehaviour
{
    public Light nLight;
    
    public InputActionReference action;
void Start()
{
    nLight = GetComponent<Light>();
    action.action.Enable();
        action.action.performed += (ctx) =>
        {   
        nLight.color = new Color(Random.Range(0,1f), Random.Range(0,1f), Random.Range(0,1f));
        }; 
    
}
}
