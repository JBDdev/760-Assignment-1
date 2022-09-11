using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicMovement : MonoBehaviour
{
    [SerializeField] Transform[] patrolTargets;
    [SerializeField] int currentTarget = 0;
    [SerializeField] float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        steerCharacter();
    }

    void steerCharacter() 
    {
        Vector3 velocity = patrolTargets[currentTarget].position - transform.position;
        Vector3 distance = patrolTargets[currentTarget].position - transform.position;

        velocity.Normalize(); //Normalized Direction to Target
        velocity *= maxSpeed;


        transform.position += velocity;

        if (Mathf.Abs(distance.x) < 0.01 && Mathf.Abs(distance.y) < 0.01) 
        {
           // Debug.Log("Target: " + currentTarget + "Distance: " + distance);

            currentTarget++;
            if (currentTarget >= patrolTargets.Length)
                currentTarget = 0;
        }


        getOrientation(transform.rotation.z, velocity);
    }

    void getOrientation(float previousOrientation, Vector3 velocity) 
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(-velocity.x, velocity.y) * Mathf.Rad2Deg, Vector3.forward);
    }
}
