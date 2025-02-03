using TMPro;
using UnityEngine;
using System.Collections;

public class Camera_Follower : MonoBehaviour
{
    [Space(10)]
    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    [Space(10)]
    [Header("Follow speed")]
    public float movespeed;

    [Space(10)]
    [Header("Follow manager variable")]
    [SerializeField] private Vector3 minOffset; // Minimum offset value for the camera
    [SerializeField] private Vector3 maxOffset; // Maximum offset value for the camera
    [SerializeField] private GameObject playerObject; // Player object which the camera will follow
    [SerializeField] private float maxDistance = 20f; // Maximum distance to normalize the offset calculation 
    public bool shouldFollow = true; // Whether the camera should follow the player
    private Vector3 currentOffset; // Current offset dynamically calculated

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.2f;

    private Vector3 originalPosition;

    private void Update()
    {
        // Ensure the player and exit gate are not null and the camera should follow
        if (playerObject != null && shouldFollow)
        {
            // Calculate the distance between the player and the exit gate
            float distanceToGate = gameManager.playerDistance;

            // Normalize the distance (invert it so closer means smaller offset)
            float t = Mathf.Clamp01(1 - (distanceToGate / maxDistance));

            // Interpolate between maxOffset and minOffset based on the normalized distance
            currentOffset = Vector3.Lerp(maxOffset, minOffset, t);

            // Calculate the target position for the camera
            Vector3 targetPosition = playerObject.transform.position - currentOffset;

            // Move the camera towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movespeed);

            // Ensure the camera is looking at the player
            transform.LookAt(playerObject.transform.position);
        }
    }

    public void Fire()
    {
        Debug.Log("Firing!");
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        shouldFollow = false;
        originalPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        shouldFollow = true;
    }
}