using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConoDeVision : MonoBehaviour
{

    static private Renderer rend; //declarar variable para cambiar componentes de renderer

    [SerializeField]
    static public Color Detected = Color.yellow, Alert = Color.red, Calm = Color.green; //declarar los colores que seran utilizados
    void Start()
    {
        rend = GetComponent<Renderer>(); //se da el componente rendderer
        rend.material.color = Calm; // se pone el color verde como el base
    }
    static public void ColorC() //se inicia el metodo para poner el color verde
    {
        rend.material.color = Calm;
    }
    static public void ColorD() //se inicia el metodo para poner el color amarillo
    {
        rend.material.color = Detected;
    }
    static public void ColorA() //se inicia el metodo para poner el color rojo
    {
        rend.material.color = Alert;
    }

}
