using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{

    public float speed;
    public int startingPoint;
    public Transform[] checkpoints;
    private int i;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = checkpoints[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, checkpoints[i].position) < 0.02f)
        {
            i++;
            if(i == checkpoints.Length)
            {
                i = 0;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, checkpoints[i].position, speed * Time.deltaTime);
    }
}
