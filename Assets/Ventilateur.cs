using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventilateur : MonoBehaviour
{
    public float repelForce = 10f;
    public Transform orientation;
    public ForceMode Forcemod;
    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("Player")) {
            // Check if the collided object has a Rigidbody
            Rigidbody otherRigidbody = collision.GetComponent<Collider>().GetComponent<Rigidbody>();
            otherRigidbody.gameObject.GetComponent<PlayerMovementTutorial>().InWind = true;

            if (otherRigidbody != null)
        {
            // Calculate the direction from the bumper to the other object
            Vector3 repelDirection = (otherRigidbody.position - transform.position).normalized;
            //Vector3 repelDirection = orientation.;
            //Vector3 currentvelocity = otherRigidbody.velocity;
            Vector3 RepelForce = repelDirection * repelForce;
            //Debug.Log(repelDirection + "direction");
            //Debug.Log(currentvelocity + "current v");
            //Debug.Log(repelDirection*repelForce);
            repelDirection.y = 0f;
            otherRigidbody.gameObject.GetComponent<PlayerMovementTutorial>().WindForce = RepelForce;
            // Apply force to repel the other object
            otherRigidbody.AddForce(repelDirection * repelForce, Forcemod);
        }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Check if the collided object has a Rigidbody
            Rigidbody otherRigidbody = collision.GetComponent<Collider>().GetComponent<Rigidbody>();

            otherRigidbody.gameObject.GetComponent<PlayerMovementTutorial>().InWind = false;
        }
    }
}
