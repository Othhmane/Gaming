using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{

    // Bumper-specific parameters
    public float JumpForce = 10.0f;


    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a Rigidbody
        Rigidbody otherRigidbody = collision.collider.GetComponent<Rigidbody>();

        if (otherRigidbody != null)
        {
            // Calculate the direction from the bumper to the other object
            Debug.Log(otherRigidbody);
            // Apply force to repel the other object
            otherRigidbody.AddForce(transform.up * JumpForce, ForceMode.VelocityChange);
            otherRigidbody.gameObject.GetComponent<PlayerMovementTutorial>().Stunned = true;
        }
    }
}
