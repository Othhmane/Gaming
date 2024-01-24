using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BumperScript : MonoBehaviour
{
    [Tooltip("Force with which the bumper repels other objects")]
    public float repelForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Your code here for when the collider interacts with a GameObject with the "Player" tag
        // Check if the collided object has a Rigidbody
        Rigidbody otherRigidbody = collision.collider.GetComponent<Rigidbody>();

        if (otherRigidbody != null)
        {
            // Calculate the direction from the bumper to the other object
            Vector3 repelDirection = (otherRigidbody.position - transform.position).normalized;
            Debug.Log(otherRigidbody);
            repelDirection.y = 0.5f;
            // Apply force to repel the other object
            otherRigidbody.velocity = Vector3.zero;
            otherRigidbody.AddForce(repelDirection * repelForce, ForceMode.Impulse);
            otherRigidbody.gameObject.GetComponent<PlayerMovementTutorial>().Stunned = true;
            }
    }
        }
}

