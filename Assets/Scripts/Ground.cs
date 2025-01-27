using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [Space(10)]
    [Header("All bot spawn point and move point")]
    public List<Transform> allSpawnPoint; // Positions of bot for each ground
    public int botCount; // Total bot needed in the ground
    public List<Transform> botRandomMove;

    [Space(10)]
    [Header("Player spawn point")]
    public Transform playerSpawnPos; // Position fot player for each ground

    [Space(10)]
    [Header("Geound active object")]
    public List<GameObject> ActiveObject; // Object list which should be active in the start of the match

    [Space(10)]
    [Header("Exit gate variables")]
    public GameObject ExitGate; // Exit gate for completing the round
    public Vector3 ExitGateOpenPosition; // Exit gate open position
    public Vector3 ExitGateClosedPosition; // Exit gate closed position
    public bool isOpenDoor; // Find that door is opened or not


    // Start
    private void Start()
    {
        // Activing all object which should show in the ground
        for (int i = 0; i < ActiveObject.Count; i++)
        {
            ActiveObject[i].SetActive(true);
        }
    }

    // Called on active object
    private void OnEnable()
    {
        // Exit gate poition as a closed door
        ExitGate.transform.localPosition = ExitGateClosedPosition;
    }

    private void Update()
    {
        // Find that bool for changing position of the gate
        if (isOpenDoor == true)
        {
            // Changing the position
            ExitGate.transform.localPosition = Vector3.MoveTowards(ExitGate.transform.localPosition, ExitGateOpenPosition, 5 * Time.deltaTime);
            // Turn off the bool when position is setted
            if (ExitGate.transform.localPosition == ExitGateOpenPosition)
            {
                isOpenDoor = false;
            }
        }
    }

}