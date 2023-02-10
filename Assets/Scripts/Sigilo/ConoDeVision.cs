using UnityEngine;

public class ConoDeVision : MonoBehaviour
{

    static private Renderer rend;

    [SerializeField]
    static public Color Detected = Color.yellow, Alert = Color.red, Calm = Color.green;
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = Calm;

    }
    static public void ColorC()
    {
        rend.material.color = Calm;
    }
    static public void ColorD()
    {
        rend.material.color = Detected;
    }
    static public void ColorA()
    {
        rend.material.color = Alert;
    }

}
