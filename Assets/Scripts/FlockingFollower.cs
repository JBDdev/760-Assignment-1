using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingFollower : MonoBehaviour
{
    [Header("Boids")]
    [SerializeField] Transform[] otherBoids;
    [SerializeField] Transform leaderBoid;
    [Header("Flocking Weights")]
    [SerializeField] float separationWeight = 1f;
    [SerializeField] float alignmentWeight = 0.15f;
    [SerializeField] float cohesionWeight = 0.8f;

    [Header("Flocking Thresholds")]
    [SerializeField] float separationThreshold = 0.75f;
    [SerializeField] float alignmentThreshold = 0.75f;
    [SerializeField] float cohesionThreshold = 0.75f;


    [Header("Arrive / Align Settings")]
    [SerializeField] float maxAcceleration;
    [SerializeField] float maxSpeed;
    [SerializeField] float targetRadius;
    [SerializeField] float slowRadius;
    [SerializeField] float timeToTarget = 0.1f;
    [SerializeField] float maxRotation;
    [SerializeField] float maxAngularAcceleration;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SteerCharacter();
    }

    void SteerCharacter() 
    {
        Vector3 steeringOutput = (Separation() * separationWeight) + (Alignment() * alignmentWeight) + (Cohesion() * cohesionWeight);
        Vector3 direction = (steeringOutput * maxSpeed) - transform.position;
        float distance = direction.magnitude;

        float targetSpeed = 0.0f;

        if (distance < targetRadius)
            return;

        if (distance > slowRadius)
            targetSpeed = maxSpeed;
        else
            targetSpeed = maxSpeed * distance / slowRadius;

        Vector3 targetVelocity = direction.normalized * targetSpeed;

        Vector3 linearAcceleration = targetVelocity - new Vector3(rb.velocity.x, rb.velocity.y, 0);
        linearAcceleration /= timeToTarget;

        //Comparing to squared magnitude to avoid the slower calculation
        if (linearAcceleration.sqrMagnitude > maxAcceleration * maxAcceleration)
        {
            linearAcceleration.Normalize();
            linearAcceleration *= maxAcceleration;
        }

        rb.AddForce(linearAcceleration, ForceMode2D.Force);
        AlignRotation(direction);
    }

    void AlignRotation(Vector3 steerTarget)
    {
        rb.angularVelocity = 0f;
        //Get the target orientation in degrees
        float targetOrientation = leaderBoid.rotation.eulerAngles.z;
        //Debug.Log("Target Orientation: " + target.rotation.eulerAngles.z + " degrees, " + targetOrientation + " radians");

        //Get our orientation in degrees
        float currentOrientation = transform.rotation.eulerAngles.z;

        //Calculate new rotation in radians
        float newRotation = Mathf.DeltaAngle(targetOrientation, currentOrientation) * Mathf.Deg2Rad;


        float rotationSize = Mathf.Abs(newRotation);

        float goalRotation;

        //Debug.Log(rotationSize);
        if (rotationSize < targetRadius)
            return;

        if (rotationSize > slowRadius)
            goalRotation = maxRotation;
        else
            goalRotation = maxRotation * rotationSize / slowRadius;

        goalRotation *= newRotation / rotationSize;
        float rotationInRadians = currentOrientation * Mathf.Deg2Rad;

        //Debug.Log(rotationInRadians);

        float resultAngular = goalRotation - rotationInRadians;
        resultAngular /= timeToTarget;

        float angularAcceleration = Mathf.Abs(resultAngular);
        //Debug.Log(goalRotation + ", " + resultAngular);

        if (angularAcceleration > maxAngularAcceleration)
        {
            resultAngular /= angularAcceleration;
            resultAngular *= maxAngularAcceleration;
        }
        rb.AddTorque(-resultAngular, ForceMode2D.Force);
    }

    Vector3 Separation() 
    {
        Vector3 result = Vector3.zero;
        foreach (Transform boid in otherBoids) 
        {
            Vector3 direction = boid.position - transform.position;
            float distance = direction.magnitude;
            float strength;
            if (distance < separationThreshold) 
            {
                strength = maxAcceleration * (separationThreshold - distance) / separationThreshold;
                direction.Normalize();
                result += strength * direction;
            }            
        }
        return -result.normalized;
    }

    Vector3 Alignment() 
    {
        Vector3 result = Vector3.zero;
        foreach (Transform boid in otherBoids)
        {
            Vector3 direction = boid.position - transform.position;
            float distance = direction.magnitude;
            float strength;
            if (distance < alignmentThreshold)
            {
                //Instead of applying force in the opposite direction of the boid, apply force in the direction that the boid is facing (using transform.up)
                strength = maxAcceleration * (alignmentThreshold - distance) / alignmentThreshold;
                result += strength * boid.transform.up;
            }
        }
        return result.normalized;
    }

    Vector3 Cohesion() 
    {
        Vector3 result = Vector3.zero;
        foreach (Transform boid in otherBoids)
        {
            Vector3 direction = boid.position - transform.position;
            float distance = direction.magnitude;
            float strength;
            if (distance < cohesionThreshold)
            {
                //Adds nearby boid to generate cohesion point
                strength = maxAcceleration * (cohesionThreshold - distance) / cohesionThreshold;
                result += strength * boid.position;
            }
        }
        //Returning the direction to the midpoint between the point of cohesion and the leader boid (see documentation)
        //Vector3 midpoint = new Vector3((result.x + leaderBoid.position.x) / 2, (result.y + leaderBoid.position.y) / 2, 0f);

        return (result - transform.position).normalized;
    }
}
