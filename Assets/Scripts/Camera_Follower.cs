using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Camera_Follower : MonoBehaviour
{
    //[Space(10)]
    //[Header("Game Manager")]
    //[SerializeField] private GameManager gameManager;

    //[Space(10)]
    //[Header("Follow speed")]
    //public float movespeed;

    //[Space(10)]
    //[Header("Follow manager variable")]
    //[SerializeField] private Vector3 minOffset; // Minimum offset value for the camera
    //[SerializeField] private Vector3 maxOffset; // Maximum offset value for the camera
    //[SerializeField] private GameObject playerObject; // Player object which the camera will follow
    //[SerializeField] private float maxDistance = 20f; // Maximum distance to normalize the offset calculation 
    //public bool shouldFollow = true; // Whether the camera should follow the player
    //private Vector3 currentOffset; // Current offset dynamically calculated

    //private void Update()
    //{
    //    // Ensure the player and exit gate are not null and the camera should follow
    //    if (playerObject != null && shouldFollow)
    //    {
    //        // Calculate the distance between the player and the exit gate
    //        float distanceToGate = gameManager.playerDistance;

    //        // Normalize the distance (invert it so closer means smaller offset)
    //        float t = Mathf.Clamp01(1 - (distanceToGate / maxDistance));

    //        // Interpolate between maxOffset and minOffset based on the normalized distance
    //        currentOffset = Vector3.Lerp(maxOffset, minOffset, t);

    //        // Calculate the target position for the camera
    //        Vector3 targetPosition = playerObject.transform.position - currentOffset;

    //        // Move the camera towards the target position
    //        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movespeed);

    //        // Ensure the camera is looking at the player
    //        ///stransform.LookAt(playerObject.transform.position);
    //    }
    //}

    public Transform player; // Reference to the player
    public Vector3 offset; // Camera offset from player
    public float minX, maxX, minY, maxY; // Min/Max positions for camera movement
    public float maxSpeed = 10f; // Max speed for the camera movement
    public float slowDownDistance = 3f; // How close the player needs to be to the edge for slowdown effect

    private Vector3 targetPosition; // The target position for the camera

    public void Fire()
    {

    }

    void LateUpdate()
    {
        // Calculate the target position based on the player's position + offset
        targetPosition = player.position + offset;

        Debug.Log("Camera updating" + targetPosition);
        // Clamp the target position to stay within the defined bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minY, maxY);

        // Calculate the distance between the camera and the target position
        float distance = Vector3.Distance(transform.position, targetPosition);

        // Calculate the speed: slower when near the edges, faster in the center
        float speed = Mathf.Lerp(0, maxSpeed, 1 - Mathf.InverseLerp(minX + slowDownDistance, maxX - slowDownDistance, targetPosition.x));

        // Apply the smooth movement with a speed factor
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }
}