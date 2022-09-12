using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicWander : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float maxRotation;
    [SerializeField] float rotationTimer;
    [SerializeField] float maxTimer;

    [SerializeField] float boundsTimer = 0.0f;
    [SerializeField] float xBounds;
    [SerializeField] float yBounds;

    // Start is called before the first frame update
    void Start()
    {
        rotationTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        boundsTimer -= Time.deltaTime;
        if (boundsTimer < 0.0f)
        {
            boundsTimer = 0.0f;
        }

        if (Mathf.Abs(transform.position.x) > xBounds || Mathf.Abs(transform.position.y) > yBounds)
            if (boundsTimer == 0.0f)
            {
                Flip();
                boundsTimer = 2.0f;
            }



        Wander();
        rotationTimer += Time.deltaTime;

        if (rotationTimer >= maxTimer)
        {
            changeDirection();
            rotationTimer = 0.0f;
        }


    }

    void Wander()
    {
        Vector3 velocity = maxSpeed * transform.up;
        transform.position += velocity * Time.deltaTime;
    }

    void changeDirection()
    {
        Quaternion newRotation = transform.rotation;
        newRotation.eulerAngles = new Vector3(0, 0, (Random.Range(0.0f, 1.0f) - Random.Range(0.0f, 1.0f)) * maxRotation + transform.rotation.eulerAngles.z);
        transform.rotation = newRotation;
    }

    void Flip()
    {
        Quaternion newRotation = transform.rotation;
        newRotation.eulerAngles = new Vector3(0, 0, 180 + transform.rotation.eulerAngles.z);
        transform.rotation = newRotation;
    }
}
