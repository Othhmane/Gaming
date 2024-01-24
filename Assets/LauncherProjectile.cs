using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherProjectile : BaseLauncher
{

    Vector3 direction;
    bool fired;

    public override void FireProjectile(GameObject head, GameObject target, Vector3 Force)
    {
        if(head && target)
        {
            direction = (target.transform.position - head.transform.position).normalized;
            fired = true;
        }
    }

    [Tooltip("Force with which the bumper repels other objects")]
    public float repelForce = 10f;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Check if the collided object has a Rigidbody
            Rigidbody otherRigidbody = collision.GetComponent<Collider>().GetComponent<Rigidbody>();

            if (otherRigidbody != null)
            {
                // Calculate the direction from the bumper to the other object
                //Vector3 repelDirection = (otherRigidbody.position - transform.position).normalized;
                Vector3 repelDirection = (otherRigidbody.position - transform.position).normalized;
                repelDirection.y = 0.1f;
                // Apply force to repel the other object
                otherRigidbody.AddForce(repelDirection * repelForce, ForceMode.Impulse);
                otherRigidbody.gameObject.GetComponent<PlayerMovementTutorial>().Stunned = true;
            }
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fired)
        {
            transform.position += direction * (speed * Time.deltaTime);
        }


    }
}
