using UnityEngine;

public class Loockat : MonoBehaviour
{
    public Transform cameraTransform;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        Vector3 targetPosition = transform.position + cameraTransform.forward;
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.right);
    }
}
