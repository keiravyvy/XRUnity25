using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Liht : MonoBehaviour
{
    public Light nLight;
void Start()
{
    nLight = GetComponent<Light>();
    if (Input.GetKeyDown("tab")) {
        nLight.color = new Color(69, 68, 69);
    }
}

}
