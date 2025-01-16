using UnityEngine;

public class Camera_Follower : MonoBehaviour
{
    [SerializeField] private Vector3 Offset;
    [SerializeField] private GameObject PlayerObject;
    private void Update()
    {
        transform.position = PlayerObject.transform.position - Offset;
        transform.LookAt(PlayerObject.transform.position);
    }


}