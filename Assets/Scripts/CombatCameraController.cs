using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCameraController : MonoBehaviour
{

    public Transform player1;
    public Transform player2;

   
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 midpoint = (player1.position + player2.position) / 2f;

        // Set the position of the midpoint object to the calculated midpoint
        transform.position = midpoint;

        Vector3 direction = player2.position - player1.position;

        // Set the rotation of the midpoint object to face the two game objects
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
