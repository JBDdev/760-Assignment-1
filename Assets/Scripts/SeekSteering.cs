using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekSteering : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float maxAcceleration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SteerCharacter();
    }

    void SteerCharacter() 
    {
        //Get Direction to target
        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        transform.GetComponent<Rigidbody2D>().AddForce(direction * maxAcceleration, ForceMode2D.Impulse);

        getOrientation(direction);
    }

    void getOrientation(Vector3 velocity)
    {
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(-velocity.x, velocity.y) * Mathf.Rad2Deg, Vector3.forward);
    }
}
