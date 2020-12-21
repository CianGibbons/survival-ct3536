﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject target;
    public static CameraController instance;
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //Initial Position - wont change until player is present
        this.transform.position = new Vector3(88.7f, 54.6f, -131.8168f);
        cam.orthographicSize = 50f;

        // Find the player gameObject
        target = GameObject.FindWithTag("Player");

        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null)
        {
            target = GameObject.FindWithTag("Player");
        } else
        {
            cam.orthographicSize = 20f;
            Vector3 PlayerPosition = target.transform.position;
            PlayerPosition.z = transform.position.z; // same as default camera z

           
           
            // getting the direction we have to move the camera towards
            Vector3 moveCameraDirection = (PlayerPosition - transform.position).normalized;
            // getting the distance the camera has to move
            float distanceToMove = Vector3.Distance(PlayerPosition, transform.position);

            float speed = 2f;
            // cam position is the camera position plus the direction to move to move the camera towards the player
            // then multiply it by the distance to move so that the camera goes faster the further behind it is
            // then multiply it by the speed we want the camera to move and finally by Time.deltaTime so it happens in real time rather than in "frame time"/"update time"
            transform.position = transform.position + moveCameraDirection * distanceToMove * speed * Time.deltaTime; ;

        }
    }

    public static void SetCameraPosition(Vector3 position)
    {
        instance.transform.position = position;
    }
}
