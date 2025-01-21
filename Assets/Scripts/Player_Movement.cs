using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player_Movement : MonoBehaviour
{

    [SerializeField] public Rigidbody playerRigidbody;

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

    [Header("Radius Ring")]
    [SerializeField] private GameObject RadiusRing;

    public GameManager GameManager;

    public AnimState playerState;

    private Player_Manager player;

    public Vector3 movementDirection;
    public Vector3 newTemp;
    public bool temp;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        player = GetComponent<Player_Manager>();
        StartCoroutine(kinematicSetting());
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
            temp = false;

        }
        else
        {
            if (temp == false)
            {
                newTemp = transform.position;
                temp = true;
            }
            transform.position = newTemp;
        }

        JoystickInput();
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

        // Get input from the player
        float horizontalInput = playerJoystick.Horizontal;
        float verticalInput = playerJoystick.Vertical;

        // Calculate movement direction
        movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        if (movementDirection.magnitude > 0.1f)
        {
            // Normalize movement to maintain consistent speed in all directions
            movementDirection.Normalize();

            // Move the transform smoothly in the direction of input
            transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);

            // Smoothly rotate the player to face the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            if (player.isTargeting == false)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Play running animation
            AnimationController(AnimState.Running);
            if (player.isSoundPlaying == false)
            {
                player.playerAudio.clip = player.runSurface;
                player.playerAudio.Play();
                player.isSoundPlaying = true;
            }
        }
        else
        {
            transform.Translate(Vector3.zero, Space.World);
            // Play idle animation

            AnimationController(AnimState.Idle);
            player.playerAudio.Stop();
            player.isSoundPlaying = false;

        }
    }


    public void AnimationController(AnimState newState)
    {
        switch (newState)
        {
            case AnimState.Idle:
                playerAnimator.SetBool("Idle", true);
                playerAnimator.SetBool("Running", false);
                playerAnimator.SetBool("Death", false);
                break;
            case AnimState.Running:
                playerAnimator.SetBool("Idle", false);
                playerAnimator.SetBool("Running", true);
                playerAnimator.SetBool("Death", false);
                break;
            case AnimState.Death:
                playerAnimator.SetBool("Idle", false);
                playerAnimator.SetBool("Running", false);
                playerAnimator.SetBool("Death", true);
                break;
        }
    }

    public enum AnimState
    {
        Idle,
        Running,
        Death
    }

    IEnumerator kinematicSetting()
    {
        yield return new WaitForSeconds(3);
        if (player.isDeath == false && GameManager.GamePlay == true)
        {
            playerRigidbody.isKinematic = true;
            playerRigidbody.isKinematic = false;
        }
        StartCoroutine(kinematicSetting());
    }

}