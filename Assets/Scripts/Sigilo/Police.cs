using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : MonoBehaviour
{
    #region Variables
   
    public Rigidbody2D myRigidbody;
    public GameObject Vision; //el cono de vision
    public Transform player, Guard, Base; //localizaciones de cosas, base es el lugar donde esta el guardia
    Vector2 dir;//direccion del guardia

    private float rotZ = -90; //se pone la rotacion para que el guardia gire
    [Range(0f, 360f)] //se limita lo de abajo a 0-360
    public float visionAngle, visionDistance; //angulo de vision y distancia 

    public float detectionTime, AlertTime, speed, RotationSpeed; // tiempo de deteccion, de alerte, velocidad del guardia y velocidad de rotacion

    public bool detected = false, alert = false; //bools para saber e que modo esta
    public bool turnAble;//bool para saber si es posible girar

    #endregion

    private void Start()
    {
        turnAble = true;
        myRigidbody = GetComponent<Rigidbody2D>();// rigidbody
        ConoDeVision.ColorC(); //se inicializa el color verde, otra vez
        StartCoroutine(Turn()); //se inicia el turn, es el metodo para que gire
    }

    private void Update()
    {
        Vector2 playerVector = player.position - Guard.position; //se compara la distancia que hay entre el jugador y el guardia

        if (Vector3.Angle(playerVector.normalized, Guard.right) < visionAngle * 0.5f) //se compara la distancia en tre guardia y jugador para saber si esta en el angulo de vision
        {
            if (playerVector.magnitude < visionDistance) //si es menor:
            {
                StartCoroutine(Detection()); //se empieza deteccion
                detectionTime -= Time.deltaTime;// se inicia el contador  para que se active el modo alerta
                if (detectionTime <= 0) //si el tiempo es igual o menor a 0:
                {
                    StartCoroutine(Attack()); //se llama el metodo ataque
                    StopCoroutine(Detection());//se detiene deteccion
                    alert = true;//se pone alerta como activo
                }
                 
            }
        }
    }

    public IEnumerator Turn()//se inicia giro
    {
        Debug.Log("Giro"); //Comprobacion
        if (turnAble)
        {
            Debug.Log("Giro Completato"); //Comprobacion
            turnAble = false;//Se pone como falso para que no se llame muchas veces el metodo
            yield return new WaitForSeconds(5.0f); //se espera 5 seg luego:
            transform.rotation = Quaternion.Euler(0, 0, rotZ); //se rota al valor de rotZ
            rotZ *= -1; //se multiplica por -1 para que se cambie de posicion
            ReinicioGiro(); //Se llama este metodo porque habian errores si no lo hacia asi
        }
    }

    void ReinicioGiro()
    {
        turnAble = true;
        StartCoroutine(Turn()); //se reinicia el metodo
    }

    public IEnumerator Detection() //inicia deteccion
    {

        StopCoroutine(Turn()); //se detiene giro
        detected = true; //se pone como verdadero el bool
        ConoDeVision.ColorD(); //el cono de vision se pone amarillo
        Vision.transform.localScale = new Vector3(7.5f, 5.5f, 1f); //se amplia la escala del cono de vision
        visionAngle = 70f; //se amplia el angulo de vision
        detectionTime -= Time.deltaTime;//tiempo de deteccion se resta al tiempo transurrido, solo si el jugador esta en el vector
        yield return new WaitForSeconds(1.5f); //se espera por 1 segundo
        Guard.position = Vector2.MoveTowards(Guard.position, player.position, speed * Time.deltaTime);//va a la posicion del jugador
        Vector3 direction = player.position - Guard.position; //se busca la direccion a la que debe de ver el guardia para perseguir al espia
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //se realiza el calculo para saber en que angulo debe rotar el guardia
        myRigidbody.rotation = angle; //se rota el guardia
        if (!alert)
        ExitDetection(); //sale de ataque y deteccion
    }

    public IEnumerator Attack() //inicia ataque
    {
        ConoDeVision.ColorA(); //el cono de vision se pone rojo
        Guard.position = Vector2.MoveTowards(Guard.position, player.position, speed * Time.deltaTime);//va a la posicion del jugador
        Vector3 direction = player.position - Guard.position; //se busca la direccion a la que debe de ver el guardia para perseguir al espia
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //se realiza el calculo para saber en que angulo debe rotar el guardia
        myRigidbody.rotation = angle; //se rota el guardia
        yield return new WaitForSeconds(5.0f); //Se espera por 5 segundos y:
        ExitDetection();//sale de ataque y deteccion
    }

    public void ExitDetection()
    {
        transform.position = Vector2.MoveTowards(Guard.position, Base.position, speed * Time.deltaTime); //se traslada a la posicion original
        Vector3 direction = Base.position - Guard.position; //se busca la direccion a la que debe de ver el guardia para perseguir al espia
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //se realiza el calculo para saber en que angulo debe rotar el guardia
        myRigidbody.rotation = angle; //se rota el guardia
        Vision.transform.localScale = new Vector3(5.0f, 5.5f, 1f); //se disminuye la escala del cono de vision
        detected = false; //se ponen bools como falsos
        alert = false;
        ConoDeVision.ColorC(); //cono de vision de color verde
        visionAngle = 53f; //se disminuye el campo de vision
        detectionTime = 1; //Se reinicia el valor
        transform.rotation = Quaternion.Euler(0, 0, rotZ); //se rota al valor de rotZ
        StartCoroutine(Turn()); //se reinicia giro
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == ("Player") && alert)//si se coliciona conn el jugador y esta en alerta
        {
            ExitDetection(); //se vuelve a modo normal
        }
    }

    private void OnDrawGizmos()
    {
        if (visionAngle <= 0f) return;

        float halfVisionAngle = visionAngle * 0.5f; //se divide /2 el campo de vision para trabajar con 180 y -180

        Vector2 p1, p2; //se ponen los vectores p1 y p2 para las 2 mitades de 360 grados

        p1 = PointForAngle(halfVisionAngle, visionDistance); //mitad 1
        p2 = PointForAngle(-halfVisionAngle, visionDistance); //mitad 2

        Gizmos.color = alert ? Color.red : detected ? Color.yellow : Color.green;//se cambian de color los gizmos dependiendo el estado que tenga el guardia
        Gizmos.DrawLine(Guard.position, (Vector2)Guard.position + p1); //se dibuja la linea en el lado izquierdo del guardia
        Gizmos.DrawLine(Guard.position, (Vector2)Guard.position + p2); //lo mismo pero en derecha

        Gizmos.DrawRay(Guard.position, Guard.right * 2.0f); //se dibujael rayo central
    }

    Vector2 PointForAngle(float angle, float distance)
    {
        //se hace el calculo para que se hagan los rayos fdel gizmo de la distancia debida
        return Guard.TransformDirection(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad))) * distance;
    }
}