using UnityEngine;

public class Loockat : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform target;
    public float ypos;
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
        transform.position = target.position + new Vector3(0, ypos, 0);
        transform.rotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.right);
    }
}
