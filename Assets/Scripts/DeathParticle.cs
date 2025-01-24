using UnityEngine;

public class DeathParticle : MonoBehaviour
{
    Camera cameraMain; // Main camera object for look there

    private void Start()
    {
        cameraMain = Camera.main; // Assigning the value
    }

    private void Update()
    {
        transform.LookAt(cameraMain.transform.position); // Saw the camera continusoly
    }
}
