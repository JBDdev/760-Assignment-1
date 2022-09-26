using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveAlign : MonoBehaviour
{
    [SerializeField] Transform target;
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
        //rb.AddTorque(.5f, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SteerCharacter();
    }

    void SteerCharacter() 
    {
        Vector3 direction = target.position - transform.position;
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

        rb.AddForce(linearAcceleration, ForceMode2D.Impulse);
        AlignRotation();
    }

    void AlignRotation() 
    {
        rb.angularVelocity = 0f;
        //Get the target orientation in degrees
        float targetOrientation = target.rotation.eulerAngles.z;
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

        Debug.Log(rotationInRadians);

        float resultAngular = goalRotation - rotationInRadians;
        resultAngular /= timeToTarget;

        float angularAcceleration = Mathf.Abs(resultAngular);
        //Debug.Log(goalRotation + ", " + resultAngular);

        if (angularAcceleration > maxAngularAcceleration) 
        {
            resultAngular /= angularAcceleration;
            resultAngular *= maxAngularAcceleration;
        }

        //Debug.Log("Re-maxed angular: " + resultAngular);
        //Use resultAngular to add a torque to turn the boid
        Debug.Log(resultAngular);

        //For whatever reason, the aligning boid was reversed in orientation and i could not figure out why, but reversing the sign on the result angular fixed the issue 
        rb.AddTorque(-resultAngular, ForceMode2D.Force);

        //Debug.Log(rotationSize);

        //lerp method of adjusting rotation that i messed with while being frustrated with the pseudocode implementation not working for me :)

        //A: Current Rotation
        //B: Goal Rotation
        //T: ratio between rotations, where 0 < t <= maxRotation
        //float targetT;

        //if (distanceToTarget > slowRadius)
        //    targetT = maxRotation;
        //else
        //    targetT = maxRotation * distanceToTarget / slowRadius;


        ////Generate the Quaternion from the Lerp function that will represent the rotation acheived in this loop
        //Quaternion newRotation = Quaternion.Slerp(transform.rotation, target.rotation, targetT);

        //transform.rotation = newRotation ;
    }
}
