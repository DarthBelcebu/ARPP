using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardia : MonoBehaviour
{
    #region Variables
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

    public GameObject Vision;
    public Transform player, Guard, Base;

    private float rotZ=-90;
    [Range(0f, 360f)]
    public float visionAngle = 53f, visionDistance = 10f;
    
    public float detectionTime=5, AlertTime, speed = 5, RotationSpeed;
    
    public bool detected = false, alert = false;
    #endregion

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        ConoDeVision.ColorC();
        StartCoroutine(Turn());
    }

    private void Update()
    {
        Vector2 playerVector = player.position - Guard.position;
        if (detectionTime >= 0)
        {
            Attack();
            pursuitAction();
            StartCoroutine(Attack());
            ConoDeVision.ColorA();

        }
        if (Vector3.Angle(playerVector.normalized, Guard.right) < visionAngle * 0.5f)
        {
            if (playerVector.magnitude < visionDistance)
            {
                StartCoroutine(Detection());
                StopCoroutine(Turn());
                detected = true;
                ConoDeVision.ColorD();
                Vision.transform.localScale = new Vector3(7.5f,5.5f,1f);
                visionAngle = 70f;
                detectionTime -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        myRigidbody.velocity = Vector3.ClampMagnitude(myRigidbody.velocity, fMaxSpeed);
    }

    public IEnumerator Turn()
    {
        yield return new WaitForSeconds(5.0f);
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
        rotZ *= -1;
        StartCoroutine(Turn());
    }

    public IEnumerator Detection()
    {
        yield return new WaitForSeconds(1.0f);
        if (transform.position == Base.position)
            ExitDetection();
    }
    #region Ataque
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(5.0f);
        ExitDetection();
    }

    #region Pursuit

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
    public void ExitDetection()
    {
        transform.position = Vector2.MoveTowards(transform.position, Base.position, speed * Time.deltaTime);
        detected = false;
        alert = false;
        ConoDeVision.ColorC();
        visionAngle = 53f;
        StartCoroutine(Turn());
    }
    #endregion
    #endregion

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == ("Player"))
        {
            ExitDetection();
        }
    }
    
    private void OnDrawGizmos()
    {
        if (visionAngle <= 0f) return;

        float halfVisionAngle = visionAngle * 0.5f;

        Vector2 p1, p2;

        p1 = PointForAngle(halfVisionAngle, visionDistance);
        p2 = PointForAngle(-halfVisionAngle, visionDistance);

        Gizmos.color = alert ? Color.red : detected ?  Color.yellow : Color.green;
        Gizmos.DrawLine(Guard.position, (Vector2)Guard.position + p1);
        Gizmos.DrawLine(Guard.position, (Vector2)Guard.position + p2);

        Gizmos.DrawRay(Guard.position, Guard.right * 2.0f);
    }

    Vector2 PointForAngle(float angle, float distance)
    {
        return Guard.TransformDirection(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad))) * distance;
    }
}