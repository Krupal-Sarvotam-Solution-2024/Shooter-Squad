using UnityEngine;

public class DeathParticle : MonoBehaviour
{
    Camera cameraMain;

    private void Start()
    {
        cameraMain = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(cameraMain.transform.position);
    }
}
