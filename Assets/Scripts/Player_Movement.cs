using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public enum MovementType { Rigidbody, Translate, CharacterController }
    public MovementType movementType = MovementType.Rigidbody;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;

    [Header("Joystick Input Settings")]
    [SerializeField] private Joystick playerJoystick;

    [Header("Animations Controller")]
    public Animator playerAnimator;

    [Header("Game Manager")]
    public GameManager GameManager;

    [Header("Current player state")]
    public AnimState playerState;

    private Player_Manager player;
    private Vector3 movementDirection;
    private Vector3 currentVelocity;
    private Rigidbody rb;
    private CharacterController characterController;

    void Start()
    {
        player = GetComponent<Player_Manager>();
        rb = GetComponent<Rigidbody>();
        //characterController = GetComponent<CharacterController>();

        //if (rb != null)
        //{
        //    rb.interpolation = RigidbodyInterpolation.Interpolate;
        //    rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //}
    }
    public void OnAnimationEventTriggered() // Method name must match the Animation Event
    {
        player.Shotting();
        // Your logic (e.g., shaking the health bar)
    }
    void Update()
    {
        if (!GameManager.GamePlay || player.is_death)
        {
            return;
        }
//#if UNITY_EDITOR
//        float horizontalInput = Input.GetAxis("Horizontal");
//        float verticalInput = Input.GetAxis("Vertical");
//#else
        
       float horizontalInput = playerJoystick.Horizontal;
       float verticalInput = playerJoystick.Vertical;
////#endif
        Vector3 targetDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        movementDirection = Vector3.Lerp(movementDirection, targetDirection, Time.deltaTime * acceleration);

        if (movementDirection.magnitude < 0.1f)
        {
            movementDirection = Vector3.zero;

        }
        else
        {
            if (rb.linearVelocity != Vector3.zero && walksoundtime > .25f)
            {
                walksoundtime = 0;
                player.playerAudio.PlayOneShot(player.runSurface);
              
            }
            walksoundtime += Time.deltaTime;
           
        }

        UpdateAnimation(horizontalInput, verticalInput);
    }

    void FixedUpdate()
    {
        if (!GameManager.GamePlay || player.is_death)
        {
            movementDirection = Vector3.zero;
            return;
        }

        switch (movementType)
        {
            case MovementType.Rigidbody:
                MoveWithRigidbody();
                break;
            case MovementType.Translate:
                MoveWithTranslate();
                break;
            case MovementType.CharacterController:
                MoveWithCharacterController();
                break;
        }
    }
    float walksoundtime;
    void MoveWithRigidbody()
    {
        Vector3 targetVelocity = movementDirection * moveSpeed;
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
       
        RotatePlayer();
    }

    void MoveWithTranslate()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position + movementDirection * moveSpeed * Time.fixedDeltaTime, Time.fixedDeltaTime * acceleration);
        RotatePlayer();
    }

    void MoveWithCharacterController()
    {
        if (characterController != null)
        {
            Vector3 smoothedMovement = Vector3.Lerp(characterController.velocity, movementDirection * moveSpeed, Time.fixedDeltaTime * acceleration);
            characterController.Move(smoothedMovement * Time.fixedDeltaTime);
            RotatePlayer();
        }
    }

    void RotatePlayer()
    {
        if (movementDirection.magnitude > 0.1f )
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }

    void UpdateAnimation(float horizontal, float vertical)
    {
        if (!player.Enemy)
        {
            if (movementDirection.magnitude > 0.1f)
            {
                float angle = Vector3.SignedAngle(transform.forward, movementDirection, Vector3.up);

                if (angle > -45f && angle < 45f)
                    AnimationController(AnimState.RunningForward);
                else if (angle >= 45f && angle < 135f)
                    AnimationController(AnimState.RunningRight);
                else if (angle <= -45f && angle > -135f)
                    AnimationController(AnimState.RunningLeft);
                else
                    AnimationController(AnimState.RunningBackward);
            }
            else
            {
                AnimationController(AnimState.Idle);
            }
        }
        else
        {
            Vector3 relativeMovement = transform.InverseTransformDirection(movementDirection);

            if (relativeMovement.z > 0.1f)      // Moving forward relative to target
                AnimationController(AnimState.RunningForward);
            else if (relativeMovement.z < -0.1f)// Moving backward relative to target
                AnimationController(AnimState.RunningBackward);
            else if (relativeMovement.x > 0.1f) // Moving right relative to target
                AnimationController(AnimState.RunningRight);
            else if (relativeMovement.x < -0.1f)// Moving left relative to target
                AnimationController(AnimState.RunningLeft);
            else
                AnimationController(AnimState.Idle);
        }
    }
    

    public void AnimationController(AnimState newState)
    {
        if (playerState == newState) return;

        playerState = newState;
        playerAnimator.SetBool("Idle", newState == AnimState.Idle);
        playerAnimator.SetBool("Running", newState == AnimState.RunningForward);
        playerAnimator.SetBool("Right", newState == AnimState.RunningRight);
        playerAnimator.SetBool("Backward", newState == AnimState.RunningBackward);
        playerAnimator.SetBool("Left", newState == AnimState.RunningLeft);
    }

    public enum AnimState
    {
        Idle,
        RunningForward,
        RunningBackward,
        RunningLeft,
        RunningRight
    }
}
