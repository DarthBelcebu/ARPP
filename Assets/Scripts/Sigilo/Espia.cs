using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Espia : MonoBehaviour
{
    public float speed = 5.0f; //se declara la velocidad

    private Vector3 target; //se pone el vector para poner la posicion

    void Start()
    {
        target = transform.position; //se pone la posicion inicial para que no hayan problemas
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) //al momento que se click izquierdo:
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition); //se localiza el lugar del click
            target.z = transform.position.z; //se deja la z en 0 para no joderla 
        }
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime); //se mueve hacia donde se hizo el click
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == ("Guardia")) //si se coliciona con guardia:
        {
            Respawn();
        }
    }
    void Respawn()
    {
        transform.position = new Vector3(-11, -5, 0); //se transportara hacia la esquina inferior derecha
    }
}