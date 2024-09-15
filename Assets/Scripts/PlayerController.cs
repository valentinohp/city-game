using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSmoothTime = 0.1f;
    public float idleCheckDelay = 0.2f;
    private Animator animatorController;
    private Camera mainCamera;
    private bool isMoving;
    private bool isMovementInput = false; // Track if movement input is detected
    private Coroutine idleCheckCoroutine;  // Store reference to the idle check coroutine
    private float targetAngle;
    private float angle;
    private Rigidbody rigidbody;

    [Header("Debug")]
    public float horizontalInput;
    public float verticalInput;
    public float turnSmoothVelocity;


    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animatorController = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        horizontalInput = horizontal;
        verticalInput = vertical;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        isMovementInput = direction.magnitude >= 0.1f;

        if (isMovementInput)
        {

            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            animatorController.SetBool("isMoving", true);
            animatorController.speed = moveSpeed / 75f;
            isMoving = true;

            if (idleCheckCoroutine != null)
            {
                StopCoroutine(idleCheckCoroutine);
                idleCheckCoroutine = null;
            }
        }
        else
        {
            idleCheckCoroutine = StartCoroutine(CheckIfIdle());
        }

        if (isMoving)
        {
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Vector3 velocity = moveDir.normalized * moveSpeed;
            velocity.y = rigidbody.velocity.y;
            rigidbody.velocity = velocity;
        }
        else
        {
            rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f);
        }
    }

    private IEnumerator CheckIfIdle()
    {
        yield return new WaitForSeconds(idleCheckDelay);

        if (!isMovementInput)
        {
            animatorController.SetBool("isMoving", false);
            isMoving = false;
        }

        idleCheckCoroutine = null;
    }
}