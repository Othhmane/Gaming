using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using StarterAssets;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(PlayerInput))]
public class CharacterControls : MonoBehaviour
{
    public float speed = 10.0f;
    public float airVelocity = 8f;
    public float gravity = 30.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 1.0f;
    public float maxFallSpeed = 50.0f;
    public float rotateSpeed = 25f; // Speed the player rotates
    private Vector3 moveDir;

    public GameObject cam;
    private Rigidbody rb;
    

    private float distToGround;

    private bool canMove = true; // If the player is not hit
    private bool isStunned = false;
    private bool wasStunned = false; // If the player was stunned before getting stunned another time
    private float pushForce;
    private Vector3 pushDir;

    public Vector3 checkPoint;
    private bool slide = false;

    private Animator animator;

    private StarterAssetsInputs playerInput;
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "SpeedBoost":
                speed = 25f;
                break;

        }

    }
    void Start()
    {
        // get the distance to the ground
        distToGround = GetComponent<Collider>().bounds.extents.y;
        playerInput = GetComponent<StarterAssetsInputs>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        checkPoint = transform.position;
        Cursor.visible = false;
        animator = GetComponent<Animator>();
        // Hook up input callbacks
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }



    public void OnMove()
    {
        //Vector2 input = context.ReadValue<Vector2>();
        float h = playerInput.move.x;
        float v = playerInput.move.y;

        Vector3 v2 = v * cam.transform.forward; // Vertical axis to which I want to move with respect to the camera
        Vector3 h2 = h * cam.transform.right; // Horizontal axis to which I want to move with respect to the camera
        moveDir = (v2 + h2).normalized; // Global position to which I want to move with magnitude 1
    }

    public void OnJump()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, CalculateJumpVerticalSpeed(), rb.velocity.z);
        }
    }

    void FixedUpdate()
    {
        if (canMove)
        {
            //animator.SetBool("IsMove", moveDir.magnitude > 0.1f);
            if (moveDir.x != 0 || moveDir.z != 0)
            {
                Vector3 targetDir = moveDir; // Direction of the character

                targetDir.y = 0;
                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion tr = Quaternion.LookRotation(targetDir); // Rotation of the character to where it moves
                Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed); // Rotate the character little by little
                transform.rotation = targetRotation;
            }

            if (IsGrounded())
            {
                //animator.SetBool("IsFalling", false);
                // Calculate how fast we should be moving
                Vector3 targetVelocity = moveDir;
                targetVelocity *= speed;

                // Apply a force that attempts to reach our target velocity
                Vector3 velocity = rb.velocity;
                if (targetVelocity.magnitude < velocity.magnitude) // If I'm slowing down the character
                {
                    targetVelocity = velocity;
                    rb.velocity /= 1.1f;
                }
                Vector3 velocityChange = (targetVelocity - velocity);
                velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                velocityChange.y = 0;
                if (!slide)
                {
                    if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                        rb.AddForce(velocityChange, ForceMode.VelocityChange);
                }
                else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                {
                    rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
                }

                // Jump
                if (IsGrounded() && Input.GetButton("Jump"))
                {
                    rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                }
            }
            else
            {
                if (!slide)
                {
                    if (!Input.GetButton("Jump"))
                    {
                        //animator.SetBool("IsFalling", true);
                    }
                        Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
                    Vector3 velocity = rb.velocity;
                    Vector3 velocityChange = (targetVelocity - velocity);
                    velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
                    velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
                    rb.AddForce(velocityChange, ForceMode.VelocityChange);
                    if (velocity.y < -maxFallSpeed)
                        rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
                }
                else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
                {
                    rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
                }
            }
        }
        else
        {
            rb.velocity = pushDir * pushForce;
        }
        // We apply gravity manually for more tuning control
        rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
    }

    float CalculateJumpVerticalSpeed()
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    public void HitPlayer(Vector3 velocityF, float time)
    {
        //animator.SetBool("IsMove", false);
        rb.velocity = velocityF;

        pushForce = velocityF.magnitude;
        pushDir = Vector3.Normalize(velocityF);
        StartCoroutine(Decrease(velocityF.magnitude, time));
    }

    public void LoadCheckPoint()
    {
        transform.position = checkPoint;
    }

    private IEnumerator Decrease(float value, float duration)
    {
        if (isStunned)
            wasStunned = true;
        isStunned = true;
        canMove = false;

        float delta = 0;
        delta = value / duration;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;
            if (!slide) // Reduce the force if the ground isn't slide
            {
                pushForce = pushForce - Time.deltaTime * delta;
                pushForce = pushForce < 0 ? 0 : pushForce;
            }
            rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); // Add gravity
        }

        if (wasStunned)
        {
            wasStunned = false;
        }
        else
        {
            isStunned = false;
            canMove = true;
        }
    }
}