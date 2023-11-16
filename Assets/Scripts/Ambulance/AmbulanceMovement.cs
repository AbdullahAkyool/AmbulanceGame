using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbulanceMovement : MonoBehaviour
{
    public Rigidbody rb;
    public FloatingJoystick joystick;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    private Vector3 moveVector;

    public bool isDriveable = true;

    private void OnDisable()
    {
        Debug.Log("disable");
    }
    
    private void OnDestroy()
    {
        Debug.Log("destroy");
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (isDriveable)
        {
            moveVector = Vector3.zero;
            moveVector.x = joystick.Horizontal * moveSpeed * Time.deltaTime;
            moveVector.z = joystick.Vertical * moveSpeed * Time.deltaTime;

            if (joystick.Horizontal != 0 || joystick.Vertical != 0)
            {
                Vector3 direction = Vector3.RotateTowards(transform.forward, moveVector, rotateSpeed * Time.deltaTime, 0f);
                transform.rotation = Quaternion.LookRotation(direction);
            }

            else if(joystick.Horizontal == 0 || joystick.Vertical == 0)
            {
                rb.velocity = Vector3.zero;
            }

            rb.MovePosition(rb.position + moveVector);
        }
    }
}