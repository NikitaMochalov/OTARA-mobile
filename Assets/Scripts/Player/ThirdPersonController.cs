using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    private Controlls playerActionsAsset;
    private InputAction move;

    private Rigidbody rb;
    [SerializeField]
    private float movementForce = 1f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float maxSpeed = 5f;
    private Vector3 forceDirection = Vector3.zero;
    [SerializeField]
    private Camera playerCamera;
    private Animator animator;
    private AudioSource bark;
    bool canBark = true;

    //ForAndroid
    [SerializeField] private FixedJoystick joystick;
    Vector2 jsVect;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerActionsAsset = new Controlls();
        animator = this.GetComponent<Animator>();
        bark = this.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        playerActionsAsset.Player.Jump.started += DoJump;
        playerActionsAsset.Player.Bark.started += DoBark;
        move = playerActionsAsset.Player.Move;
        playerActionsAsset.Player.Enable();
    }

    private void OnDisable()
    {
        playerActionsAsset.Player.Jump.started -= DoJump;
        playerActionsAsset.Player.Bark.started -= DoBark;
        playerActionsAsset.Player.Disable();
    }

    private void FixedUpdate()
    {
        /*forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f)
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        LookAt();
        Debug.Log(IsGrounded()); */

        forceDirection += joystick.Horizontal * GetCameraRight(playerCamera) * movementForce;
        forceDirection += joystick.Vertical * GetCameraForward(playerCamera) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if (rb.velocity.y < 0f)
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime;

        Vector3 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

        Vector3 direction = rb.velocity;
        direction.y = 0f;
        jsVect = new Vector2(joystick.Horizontal, joystick.Vertical);
        if(jsVect.sqrMagnitude > 0.05f && direction.sqrMagnitude > 0.05f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    public void DoJump(InputAction.CallbackContext obj)
    {
        JumpJump();
    }

    public void JumpJump()
    {
        if (IsGrounded())
        {
            forceDirection += Vector3.up * jumpForce;
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.8f))
            return true;
        else
            return false;
    }

    public void DoBark(InputAction.CallbackContext obj)
    {
        BarkBark();
    }

    public void BarkBark()
    {
        if (canBark)
        {
            canBark = false;
            animator.SetTrigger("bark");
            StartCoroutine(Bark());
            Debug.Log("Bark!");
        }
    }

    IEnumerator Bark()
    {
        bark.Play();
        yield return new WaitForSeconds(0.25f);
        bark.Play();
    }

    public void EnableBarking()
    {
        canBark = true;
    }
}