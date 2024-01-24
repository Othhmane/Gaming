using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Lumin;

#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
#endif

public class PlayerMovementTutorial : MonoBehaviour
{
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _rotationVelocity;
        private float _verticalVelocity;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDHanging;

        private Rigidbody _rb;
        private Animator _animator;
        //private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        private bool CanMove = true;

        public bool InWind;

        bool hanging;


        [Header("Movement")]
        public Vector3 CurrentSpeed;
        public float moveSpeed;
        public float SprintSpeed = 5.335f;
        public float rotationSpeed = 45f;
        public float groundDrag;
        public bool Stunned; 
        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;
        bool readyToJump;

        [HideInInspector] public float walkSpeed;
        [HideInInspector] public float sprintSpeed;

        [Header("Keybinds")]
        public KeyCode jumpKey = KeyCode.Space;

        [Header("Ground Check")]
        public float playerHeight;
        public LayerMask whatIsGround;
        public bool grounded;

        public Transform orientation;

        float horizontalInput;
        float verticalInput;

        Vector3 moveDirection;

    private Rigidbody rb;

    // cinemachine
    private bool IsCurrentDeviceMouse = true;


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

    private void PlayerStunHandle()
    {
        if (Stunned && !grounded)
        {
            CanMove = false;
        }
        else if (Stunned && grounded)
        {
            CanMove = true;
            Stunned = false;
        }
    }

    private void playerfollowCam()
    {
        rb.transform.right = orientation.right;
    }

    private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _input = GetComponent<StarterAssetsInputs>();
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            readyToJump = true;
        }

        private void Update()
        {
            // ground check
            grounded = Physics.Raycast(transform.position, Vector3.down, whatIsGround);

            MyInput();
            SpeedControl();
            _input.jump = false;

            // handle drag
            if (grounded)
                rb.drag = groundDrag;
            else
                rb.drag = 0;

        }

        private void FixedUpdate()
        {
        PlayerStunHandle();
        if (CanMove)
        {
        MovePlayer();
        }
        }

        private Vector3 RotateTo = Vector3.zero;


        private void LateUpdate()
        {
            CameraRotation();
    }

        
        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                // Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }


        private void MyInput()
        {
            horizontalInput = _input.move.x;
            verticalInput = _input.move.y;

            // when to jump
            if (_input.jump && readyToJump && grounded)
            {
                readyToJump = false;

                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    public Vector3 CheckPoint;    

        public void TeleportToCheckpoint()
    {
        rb.velocity = Vector3.zero;
        rb.MovePosition(CheckPoint);
    }
        
        private void MovePlayer()
        {
            // calculate movement direction
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.y = 0f;
        // rb.MoveRotation(Quaternion.LookRotation(moveDirection));
        Vector3 v0 = new Vector3(0.001f, 0f, 0.001f);
        if (moveDirection.magnitude >= v0.magnitude)
        {
            // Calculate the target rotation based on the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

            // Use Quaternion.RotateTowards to smoothly rotate towards the target rotation
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime));
        }
        // on ground
        if (grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * Time.deltaTime, ForceMode.VelocityChange);

            // in air
            else if (!grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier * Time.deltaTime, ForceMode.VelocityChange);
        }
        public Vector3 WindForce;
        private void SpeedControl()
        {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed && !InWind)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                CurrentSpeed = rb.velocity;
        }
        else if (flatVel.magnitude > moveSpeed && InWind){

            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            Debug.Log(limitedVel+"limit");
            Vector3 limitedWindVel = limitedVel + WindForce;
            Debug.Log(limitedWindVel + "limit wind");
            Debug.Log(rb.velocity + "speed");
            rb.velocity = new Vector3(limitedWindVel.x, rb.velocity.y, limitedWindVel.z);
            CurrentSpeed = rb.velocity;

        }

        }


        private void Jump()
        {
            // reset y velocity
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
        }
        private void ResetJump()
        {
            readyToJump = true;
        }
    
}