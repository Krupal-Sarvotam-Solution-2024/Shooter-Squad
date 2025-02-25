using System.Collections;
using UnityEngine;

public class Camera_Follower : MonoBehaviour
{
    public Transform player; // Reference to the player
    public Vector3 offset; // Camera offset from player
    public float minX, maxX, minY, maxY; // Min/Max positions for camera movement
    public float maxSpeed = 10f; // Max speed for the camera movement
    public float slowDownDistance = 3f; // How close the player needs to be to the edge for slowdown effect

    private Vector3 targetPosition; // The target position for the camera

    [Header("Camera Shake")]
    public float shakeDuration = 0.25f;
    public float shakeMagnitude = 0.1f;

    private Vector3 shakeOffset = Vector3.zero;
    private bool isShaking = false;

    public void Fire()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsed = 0f;
        isShaking = true;

        while (elapsed < shakeDuration)
        {
            // Use Perlin Noise for smooth shake effect
            float x = (Mathf.PerlinNoise(Time.time * 20f, 0f) * 2f - 1f) * shakeMagnitude;
            float y = (Mathf.PerlinNoise(0f, Time.time * 20f) * 2f - 1f) * shakeMagnitude;

            shakeOffset = new Vector3(x, y, 0); // Apply shake as an offset
            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero; // Reset shake effect
        isShaking = false;
    }

    void LateUpdate()
    {
        // Calculate the target position based on the player's position + offset
        targetPosition = player.position + offset;

        // Clamp the target position to stay within the defined bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minY, maxY);

        // Calculate the speed: slower when near the edges, faster in the center
        float speed = Mathf.Lerp(0, maxSpeed, 1 - Mathf.InverseLerp(minX + slowDownDistance, maxX - slowDownDistance, targetPosition.x));

        // Apply smooth movement
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Apply camera shake AFTER movement calculation
        if (isShaking)
        {
            transform.position += shakeOffset;
        }
    }
}
