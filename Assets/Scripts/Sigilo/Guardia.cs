using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardia : MonoBehaviour
{
    public GameObject Vision;
    public Transform player, Guard, Base;

    private float rotZ=-90;
    [Range(0f, 360f)]
    public float visionAngle = 53f, visionDistance = 10f;
    
    public float detectionTime, AlertTime, speed = 5, RotationSpeed;
    
    public static bool detected = false, alert = false;

    private void Start()
    {
        StartCoroutine(Turn());
    }

    private void Update()
    {
        Vector2 playerVector = player.position - Guard.position;

        if(Vector3.Angle(playerVector.normalized, Guard.right) < visionAngle * 0.5f)
        {
            if (playerVector.magnitude < visionDistance)
            {
                StartCoroutine(Detection());
                StopCoroutine(Turn());
                detected = true;
                ConoDeVision.ColorD();
                Vision.transform.localScale = new Vector3(7.5f,5.5f,1f);
                visionAngle = 70f;
            }
        }
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
        Arrive();
    }
    
    public void Arrive()
    {
        transform.position = Vector2.MoveTowards(transform.position, Base.position, speed * Time.deltaTime);
        if (transform.position == Base.position)
            ExitDetection();
    }

    private void ExitDetection()
    {
        detected = false;
        alert = false;
        ConoDeVision.ColorC();
        visionAngle = 53f;
        StartCoroutine(Turn());
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
