using TMPro;
using UnityEngine;

public class Camera_Follower : MonoBehaviour
{
    [SerializeField] private Vector3 Offset; // Offset distance which camera maintain from the player
    [SerializeField] private GameObject PlayerObject; // Player object which camera will follow

    public bool shouldFollow; // Find that camera should follow that or not

    public bool isArriveOrignalPos = false; // Find that camera has reached it's target position or noted

    // Update
    private void Update()
    {
        // Can continue if player is not null and camera should follow
        if (PlayerObject != null && shouldFollow == true)
        {
            // If statement for checking it's arrive orignal position or not
            if (isArriveOrignalPos == true)
            {
                transform.position = PlayerObject.transform.position - Offset; // Changing position to follow the player
                transform.LookAt(PlayerObject.transform.position); // Camera should saw the player
            }
            else
            {
                Vector3 playerPos = PlayerObject.transform.position - Offset; // Target position which camera has to arrive
                transform.position = Vector3.MoveTowards(transform.position, playerPos, 5 * Time.deltaTime); // Move the camera towards the target position
                transform.LookAt(PlayerObject.transform.position); // Camera shold saw the player
                if (transform.position == (PlayerObject.transform.position - Offset))
                {
                    isArriveOrignalPos = true;
                }
            }
        }
    }
}