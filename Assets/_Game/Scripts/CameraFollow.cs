using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float speed = 20f;
    public Vector3 rotationAngle = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<PlayerMovement>().transform;
        transform.rotation = Quaternion.Euler(rotationAngle);
    }

    // Update is called once per frame
    void FixedUpdate()
    {    
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
    }
}
