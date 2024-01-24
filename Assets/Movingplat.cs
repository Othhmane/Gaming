using UnityEngine;

public class Movingplat : MonoBehaviour
{
    public float moveDistance = 10f; // Adjust as needed
    public float speed = 1f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private Rigidbody platformRigidbody;
    private Transform initialParent; // Added variable to store initial parent

    private Vector3 initialPlayerScale;
    private Transform playerTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            initialPlayerScale = playerTransform.localScale; // Store initial scale
            initialParent = playerTransform.parent;

            other.transform.SetParent(transform);

            platformRigidbody = GetComponent<Rigidbody>();
            if (platformRigidbody != null)
            {
                platformRigidbody.isKinematic = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(initialParent); // Restore the initial parent
            playerTransform.localScale = initialPlayerScale;

            // Enable platform's Rigidbody gravity when the player leaves
            if (platformRigidbody != null)
            {
                platformRigidbody.isKinematic = false;
            }

            initialParent = null; // Reset the initial parent
        }
    }

    private void Start()
    {
        initialPosition = transform.position;
        CalculateTargetPosition();
    }

    private void FixedUpdate()
    {
        float step = speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Swap target positions to move back and forth
            CalculateTargetPosition();
        }
    }

    private void CalculateTargetPosition()
    {
        // Swap target positions to move back and forth
        if (targetPosition == initialPosition)
        {
            targetPosition = initialPosition + new Vector3(moveDistance, 0f, 0f);
        }
        else
        {
            targetPosition = initialPosition;
        }
    }
}