using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardia : MonoBehaviour
{
    #region Variables
    //variables del pursuit
    GameObject PursuitTarget = null;
    Rigidbody PursuitTargetRB = null;
    public Rigidbody myRigidbody = null;
    public float fMaxSpeed = 1.0f;
    public float fMaxForce = 0.5f;
    public float fPredictionSteps = 10.0f;
    public bool bUseArrive = true;
    public float fArriveRadius = 3.0f;
    public enum SteeringB {none, Seek, Pursue}
    public SteeringB currentBehavior = SteeringB.none;
    Vector3 v3TargetPosition = Vector3.zero;
    
    public GameObject Vision; //el cono de vision
    public Transform player, Guard, Base; //localizaciones de cosas, base es el lugar donde esta el guardia
    Vector2 dir;//direccion del guardia

    private float rotZ=-90; //se pone la rotacion para que el guardia gire
    [Range(0f, 360f)] //se limita lo de abajo a 0-360
    public float visionAngle = 53f, visionDistance = 10f; //angulo de vision y distancia 
    
    public float detectionTime=5, AlertTime, speed = 5, RotationSpeed; // tiempo de deteccion, de alerte, velocidad del guardia y velocidad de rotacion
    
    public bool detected = false, alert = false; //bools para saber e que modo esta
    #endregion

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();// rigidbody
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
            }
        }
    }

    public IEnumerator Turn()//se inicia giro
    {
        yield return new WaitForSeconds(5.0f); //se espera 5 seg luego:
        transform.rotation = Quaternion.Euler(0, 0, rotZ); //se rota al valor de rotZ
        rotZ *= -1; //se multiplica por -1 para que se cambie de posicion
        StartCoroutine(Turn()); //se reinicia el metodo
    }

    public IEnumerator Detection() //inicia deteccion         **OCUPA CORRECCIONES**
    {

        StopCoroutine(Turn()); //se detiene giro
        detected = true; //se pone como verdadero el bool
        ConoDeVision.ColorD(); //el cono de vision se pone amarillo
        Vision.transform.localScale = new Vector3(7.5f, 5.5f, 1f); //se amplia la escala del cono de vision
        visionAngle = 70f; //se amplia el angulo de vision
        detectionTime -= Time.deltaTime;//tiempo de deteccion se resta al tiempo transurrido, solo si el jugador esta en el vector
        yield return new WaitForSeconds(1.0f); //se espera por 1 segundo
        Guard.position = Vector2.MoveTowards(Guard.position, player.position, speed * Time.deltaTime);
        dir = player.position - Guard.position;
        /*player.up = dir;
        yield return new WaitForSeconds(3.0f);
        ExitDetection();*/
        yield return new WaitForSeconds(5.0f); //se espera por 5 segundos y
        ExitDetection(); //sale de ataque y deteccion
    }
    //                       ****ataque comentado para probar cosas****
    #region Ataque
    /*
    //se inicia pursuit, no se bien como jala xd         ***QUITAR COMENTARIO CUANDO ENTREGUEMOS***
    #region Pursuit

    private void FixedUpdate()
    {
        myRigidbody.velocity = Vector3.ClampMagnitude(myRigidbody.velocity, fMaxSpeed);
    }

    private void pursuitAction()
    {
        currentBehavior = SteeringB.Pursue;
    }

    float ArriveFunction(Vector3 in_v3DesiredDirection)
    {

        float fDistance = in_v3DesiredDirection.magnitude;
        float fDesiredMagnitude = fMaxSpeed;

        if (fDistance < fArriveRadius)
        {
            fDesiredMagnitude = Mathf.InverseLerp(0.0f, fArriveRadius, fDistance);
        }
        return fDesiredMagnitude;

    }
    Vector3 Seek(Vector3 in_v3TargetPosition)
    {
        // Dirección deseada es punta ("a dónde quiero llegar") - cola (dónde estoy ahorita)
        Vector3 v3DesiredDirection = in_v3TargetPosition - transform.position;
        float fDesiredMagnitude = fMaxSpeed;
        if (bUseArrive)
        {
            fDesiredMagnitude = ArriveFunction(v3DesiredDirection);
        }

        Vector3 v3DesiredVelocity = v3DesiredDirection.normalized * fDesiredMagnitude;

        Vector3 v3SteeringForce = v3DesiredVelocity - myRigidbody.velocity;
        // Igual aquí, haces este normalized*maxSpeed para que la magnitud de la
        // fuerza nunca sea mayor que la maxSpeed.
        v3SteeringForce = Vector3.ClampMagnitude(v3SteeringForce, fMaxForce);
        return v3SteeringForce;
    }

    void SetPursueTarget()
    {
        Debug.Log("entre a setPursueTarget");

        PursuitTarget = GameObject.Find("Espia");
        if (PursuitTarget == null)
        {
            //entonces no encontro dicho objeto, es un error
            Debug.LogError("no PursuitTarget gameobject found in scene");
            return;
        }
        PursuitTargetRB = PursuitTarget.GetComponent<Rigidbody>();
        if (PursuitTargetRB == null)
        {
            Debug.LogError("no rigidbody present on GameObject PursuitTarget but it should");
            return;
        }
    }

    Vector3 Pursuit(Rigidbody in_target)
    {
        //Es importante que hagamos una copia de la posicion del objetivo para no
        //modificarla directamente
        Vector3 v3TargetPosition = in_target.transform.position;
        //Añadimos a dicha posicion el movimiento a
        //fPredictionSteps - veces el deltaTime. Es decir, n-cuadros en el futuro
        v3TargetPosition += in_target.velocity * Time.fixedDeltaTime * fPredictionSteps;

        return Seek(v3TargetPosition);
    }

    void OnValidate()
    {
        if (currentBehavior == SteeringB.Pursue)
        {
            SetPursueTarget();
        }
    }

    #endregion
    */
    #endregion
    
    public void ExitDetection()
    {
        transform.position = Vector2.MoveTowards(Guard.position, Base.position, speed * Time.deltaTime); //se traslada a la posicion original
        Vision.transform.localScale = new Vector3(5.0f, 5.5f, 1f); //se disminuye la escala del cono de vision
        detected = false; //se ponen bools como falsos
        alert = false; 
        ConoDeVision.ColorC(); //cono de vision de color verde
        visionAngle = 53f; //se disminuye el campo de vision
        StartCoroutine(Turn()); //se reinicia giro
    }


    void OnCollisionEnter(Collision collision)
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

        Gizmos.color = alert ? Color.red : detected ?  Color.yellow : Color.green;//se cambian de color los gizmos dependiendo el estado que tenga el guardia
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