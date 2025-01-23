using UnityEngine;

public class Camera_Follower : MonoBehaviour
{
    [SerializeField] private Vector3 Offset;
    [SerializeField] private GameObject PlayerObject;
    public bool shouldFollow;
    private void Update()
    {
        if (PlayerObject != null && shouldFollow == true)
        {

            transform.position = PlayerObject.transform.position - Offset;
            transform.LookAt(PlayerObject.transform.position);
        }
    }
}