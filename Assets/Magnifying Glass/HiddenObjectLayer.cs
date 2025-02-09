using UnityEngine;

public class HiddenObjectLayer : MonoBehaviour
{
    void Start()
    {
        // Set the object to only be visible to the magnifying glass camera
        gameObject.layer = LayerMask.NameToLayer("HiddenObjects");
    }
}