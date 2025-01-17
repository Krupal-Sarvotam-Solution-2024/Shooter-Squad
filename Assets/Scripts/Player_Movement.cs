using UnityEngine;

public class Player_Movement : MonoBehaviour
{

    [SerializeField] private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Keyboard Input Settings")]
    [SerializeField] private string horizontalAxis = "Horizontal"; // Default to "Horizontal" input axis
    [SerializeField] private string verticalAxis = "Vertical";     // Default to "Vertical" input axis

    [Header("Joystick Input Settings")]
    [SerializeField] private Joystick playerJoystick;

    [Header("Animations Controller")]
    [SerializeField] private Animator playerAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyboardInput();
        JoystickInput();
    }

    // Player movement through keyboard input
    void KeyboardInput()
    {
        // Get input from the player
        float horizontalInput = Input.GetAxis(horizontalAxis);
        float verticalInput = Input.GetAxis(verticalAxis);

        // Calculate movement direction
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        if (movementDirection.magnitude > 0.1f)
        {
            // Normalize movement to maintain consistent speed in all directions
            movementDirection.Normalize();

            // Calculate target position based on movement direction
            Vector3 targetPosition = rb.position + movementDirection * moveSpeed * Time.fixedDeltaTime;

            // Move the Rigidbody to the target position
            rb.MovePosition(targetPosition);

            // Smoothly rotate the player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            AnimationController(AnimState.Running);
        }
        else
        {
            AnimationController(AnimState.Idle);
        }
    }

    void JoystickInput()
    {
        // Get input from the player
        float horizontalInput = playerJoystick.Horizontal;
        float verticalInput = playerJoystick.Vertical;

        // Calculate movement direction
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        if (movementDirection.magnitude > 0.1f)
        {
            // Normalize movement to maintain consistent speed in all directions
            movementDirection.Normalize();

            // Calculate target position based on movement direction
            Vector3 targetPosition = rb.position + movementDirection * moveSpeed * Time.fixedDeltaTime;

            // Move the Rigidbody to the target position
            rb.MovePosition(targetPosition);

            // Smoothly rotate the player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            AnimationController(AnimState.Running);
        }
        else
        {
            AnimationController(AnimState.Idle);
        }
    }

    void AnimationController(AnimState newState)
    {
        switch (newState)
        {
            case AnimState.Idle:
                playerAnimator.SetBool("Idle", true);
                playerAnimator.SetBool("Running", false);
                break;
            case AnimState.Running:
                playerAnimator.SetBool("Idle", false);
                playerAnimator.SetBool("Running", true);
                break;
        }
    }

    enum AnimState
    {
        Idle,
        Running
    }


}