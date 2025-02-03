using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{
    [Space(10)]
    [Header("Player Rigidbody")]
    [SerializeField] public Rigidbody playerRigidbody;

    [Space(10)]
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed = 10f;

    [Space(10)]
    [Header("Joystick Input Settings")]
    [SerializeField] private Joystick playerJoystick;

    [Space(10)]
    [Header("Animations Controller")]
    public Animator playerAnimator;

    [Space(10)]
    [Header("Radius Ring")]
    [SerializeField] private GameObject RadiusRing;

    [Space(10)]
    [Header("Game Manager")]
    public GameManager GameManager;

    [Space(10)]
    [Header("Current player state")]
    public AnimState playerState;

    [Space(10)]
    [Header("Player manager")]
    private Player_Manager player;

    [Space(10)]
    [Header("Movement manager")]
    public Vector3 movementDirection;
    public Vector3 newTemp;
    public bool temp;
    private Vector3 velocity;

    [SerializeField] GameObject PredicatedPosShower;
    [SerializeField] GameObject PredicatedPosShowerParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        player = GetComponent<Player_Manager>();
        //StartCoroutine(kinematicSetting());
        //playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((GameManager.GamePlay == false) || (player.isDeath == true))
        {
            return;
        }



        // Get input from the player
        float horizontalInput = playerJoystick.Horizontal;
        float verticalInput = playerJoystick.Vertical;

        // Calculate movement direction
        movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        if (movementDirection.magnitude > 0.1f)
        {
/*            temp = false;
*/            PredicatedPosShowerParent.transform.position = transform.position;
            PredicatedPosShower.SetActive(true);
            PredicatedPosShower.transform.localPosition = movementDirection.normalized * 1.5f;
        }
        else
        {
            PredicatedPosShower.SetActive(false);
/*            if (temp == false)
            {
                newTemp = transform.position;
                temp = true;
            }
            transform.position = newTemp;
*/        }

        //if (transform.position.y < 0)
        //{
        //    transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        //    // Move the transform smoothly in the direction of input
        //    transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);
        //}
        JoystickInput();

        if (player.enemyInRadius > 0)
        {
            if (verticalInput > 0 && Mathf.Abs(horizontalInput) < 0.5f)
            {
                AnimationController(AnimState.RunningForward);
            }
            else if (verticalInput < 0 && Mathf.Abs(horizontalInput) < 0.5f)
            {
                AnimationController(AnimState.RunningBackward);
            }
            else if (horizontalInput > 0 && Mathf.Abs(verticalInput) < 0.5f)
            {
                AnimationController(AnimState.RunningRight);
            }
            else if (horizontalInput < 0 && Mathf.Abs(verticalInput) < 0.5f)
            {
                AnimationController(AnimState.RunningLeft);
            }
            else if (horizontalInput == 0 && Mathf.Abs(verticalInput) == 0)
            {
                AnimationController(AnimState.Idle);
            }
        }

        RadiusRing.transform.eulerAngles = Vector3.zero;
    }


    void JoystickInput()
    {
        if (player.isDeath == true)
        {
            player.playerAudio.Stop();
            player.isSoundPlaying = false;
            return;
        }

        // Get playerJoystick input
        Vector3 input = new Vector3(playerJoystick.Horizontal, 0f, playerJoystick.Vertical);

        //if (input.magnitude > 0.1f)
        //{
        //    // Accelerate towards target velocity
        //    velocity = Vector3.Lerp(velocity, input.normalized * moveSpeed, moveSpeed * Time.deltaTime);

        //    if (player.isTargeting == false)
        //    {
        //        // Rotate player towards movement direction
        //        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(input), Time.deltaTime * moveSpeed);
        //    }

        //    AnimationController(AnimState.RunningForward);
        //    if (player.isSoundPlaying == false)
        //    {
        //        player.playerAudio.clip = player.runSurface;
        //        player.playerAudio.Play();
        //        player.isSoundPlaying = true;
        //    }
        //}
        //else
        //{
        //    // Gradually slow down when no input
        //    velocity = Vector3.Lerp(velocity, Vector3.zero, (moveSpeed / 2) * Time.deltaTime);
        //    AnimationController(AnimState.Idle);
        //    player.playerAudio.Stop();
        //    player.isSoundPlaying = false;
        //}
        if (movementDirection.magnitude > 0.1f)
        {
            movementDirection.Normalize();
            velocity = Vector3.Lerp(velocity, movementDirection * moveSpeed, moveSpeed * Time.deltaTime);
            playerRigidbody.linearVelocity = Vector3.SmoothDamp(playerRigidbody.linearVelocity, new Vector3(velocity.x, playerRigidbody.linearVelocity.y, velocity.z), ref velocity, 0.1f);

            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            if (!player.isTargeting)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            AnimationController(AnimState.RunningForward);
            if (player.isSoundPlaying == false)
            {
                player.playerAudio.clip = player.runSurface;
                player.playerAudio.Play();
                player.isSoundPlaying = true;
            }
        }
        else
        {
            velocity = Vector3.Lerp(velocity, Vector3.zero, moveSpeed * Time.deltaTime);
            AnimationController(AnimState.Idle);
            player.playerAudio.Stop();
            player.isSoundPlaying = false;
        }
    }
    void FixedUpdate()
    {
        // Apply movement
        playerRigidbody.linearVelocity = new Vector3(velocity.x, playerRigidbody.linearVelocity.y, velocity.z);
    }


    public void AnimationController(AnimState newState)
    {
        playerState = newState;
        switch (newState)
        {
            case AnimState.Idle:
                playerAnimator.SetBool("Idle", true);
                playerAnimator.SetBool("Running", false);
                playerAnimator.SetBool("Right", false);
                playerAnimator.SetBool("Left", false);
                playerAnimator.SetBool("Backward", false);
                break;
            case AnimState.RunningForward:
                playerAnimator.SetBool("Idle", false);
                playerAnimator.SetBool("Running", true);
                playerAnimator.SetBool("Right", false);
                playerAnimator.SetBool("Left", false);
                playerAnimator.SetBool("Backward", false);
                break;
            case AnimState.RunningBackward:
                playerAnimator.SetBool("Idle", false);
                playerAnimator.SetBool("Running", true);
                playerAnimator.SetBool("Right", false);
                playerAnimator.SetBool("Left", false);
                playerAnimator.SetBool("Backward", true);
                break;
            case AnimState.RunningLeft:
                playerAnimator.SetBool("Idle", false);
                playerAnimator.SetBool("Running", true);
                playerAnimator.SetBool("Right", false);
                playerAnimator.SetBool("Left", true);
                playerAnimator.SetBool("Backward", false);
                break;
            case AnimState.RunningRight:
                playerAnimator.SetBool("Idle", false);
                playerAnimator.SetBool("Running", true);
                playerAnimator.SetBool("Right", true);
                playerAnimator.SetBool("Left", false);
                playerAnimator.SetBool("Backward", false);
                break;
            case AnimState.IdleShoot:
                playerAnimator.SetBool("Idle", true);
                playerAnimator.SetBool("Running", false);
                playerAnimator.SetBool("Right", false);
                playerAnimator.SetBool("Left", false);
                playerAnimator.SetBool("Backward", false);
                break;
        }
    }

    public enum AnimState
    {
        Idle,
        RunningForward,
        RunningRight,
        RunningLeft,
        RunningBackward,
        IdleShoot
    }

    IEnumerator kinematicSetting()
    {
        yield return new WaitForSeconds(3);
        if (player.isDeath == false && GameManager.GamePlay == true)
        {
            playerRigidbody.isKinematic = true;
            playerRigidbody.isKinematic = false;
        }
    }

}