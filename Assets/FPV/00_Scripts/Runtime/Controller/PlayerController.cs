using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using FPV;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerController : MonoBehaviour
{
    private Vector2 moveVector;
    private Vector3 moveDir, slopeMoveDir;

    private bool isGrounded;
    private Rigidbody rb;

    [Header("Movement Settings")]
    public float speed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private Transform feet;
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private float airMultiplier = 0.4f;
    [SerializeField] private float groundDrag, airDrag;
    [SerializeField] private float maxAddedGravity, speedAddedGravity;
    private float currentAddedGravity;
    private RaycastHit slopeHit;
    [SerializeField] private float coyoteTime = 0.2f;
    private float currentFallTime;

    [Header("Animation")] 
    [SerializeField] private Animator animator;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //ground check
        isGrounded = Physics.Raycast(feet.position, -transform.up, out RaycastHit hit, 0.5f);
        if (hit.point != Vector3.zero)
        {
            Debug.DrawLine(feet.position, hit.point, Color.green);
        }
        else
        {
            Debug.DrawRay(feet.position, -transform.up * 0.5f, Color.red);
        }
        
        
        slopeMoveDir = Vector3.ProjectOnPlane(moveDir, slopeHit.normal);

        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        
        moveDir = forward.normalized * moveVector.y + right * moveVector.x;
        
        if (isGrounded && rb.linearVelocity.y < 0)
        {
            currentFallTime = 0;
        }
        
        ControlDrag();

        AnimatorUpdate();
    }

    private void FixedUpdate()
    {
        Move();
        RotateModel();
        
        if (!isGrounded)
        {
            currentAddedGravity = Mathf.SmoothStep(currentAddedGravity, maxAddedGravity, speedAddedGravity * Time.deltaTime);

            if (currentFallTime < coyoteTime + 0.1f)
            {
                currentFallTime += Time.deltaTime;
            }
        }
        else if (currentAddedGravity != 0)
        {
            currentAddedGravity = 0;
        }
        
        
    }

    

    void Move()
    {
        //Extra gravity
        rb.AddForce(Vector3.down * currentAddedGravity);
        
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDir.normalized * speed, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDir.normalized * speed, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDir.normalized * speed * airMultiplier, ForceMode.Acceleration);
        }
        
    }

    private Vector3 targetRotation;
    [SerializeField] private Transform objectToRotate;
    private void RotateModel()
    {
        Vector3 camRotation = Camera.main.transform.eulerAngles;
        targetRotation = Vector3.Lerp(objectToRotate.localEulerAngles, 
            new Vector3(objectToRotate.rotation.eulerAngles.x, camRotation.y - 90, objectToRotate.rotation.eulerAngles.z), 
            1f);
        
        objectToRotate.localEulerAngles = targetRotation;
    }

    void ControlDrag()
    {
        if (OnSlope() && moveDir.magnitude <= 0.1f)
        {
            rb.linearDamping = 30;
        }
        else if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }

    void Jump()
    {
        if (currentFallTime < coyoteTime)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            
            //Appel Audio Saut
            AudioManager.PlayOneShotAttached(FMODEvents.Instance.jump, feet.gameObject);

            currentFallTime = coyoteTime + 1;
        }
    }
    
    private void AnimatorUpdate()
    {
        Vector3 currentSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        animator.SetFloat("Speed", currentSpeed.magnitude);
    }

    public void Footsteps()
    {
        AudioManager.PlayOneShotAttached(FMODEvents.Instance.footSteep, feet.gameObject);
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(feet.position, Vector3.down, out slopeHit, 0.2f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
        {
            Jump();
        }
    }
}
