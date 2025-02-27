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

    public GameObject ExitGate; // Exit gate for completing the round

    [Space(10)]
    [Header("Entry gate")]
    public GameObject EntryDoorLeft;
    public GameObject EntryDoorRight;
    public bool isOpenEntryDoor;

    public GameObject ExitDoorLeft;
    public GameObject ExitDoorRight;
    public bool isOpenExitDoor;

    private float rotationSpeed = 90f; // Degrees per second
    private float targetLeftRotation = -90f;
    private float targetRightRotation = 90f;


    //Start
    private void Start()
    {
        //Activing all object which should show in the ground
        for (int i = 0; i < ActiveObject.Count; i++)
        {
            ActiveObject[i].SetActive(true);
        }
    }

    private void Update()
    {
        
        // Smoothly rotate the left door if it's not yet at target
        //if (isOpenEntryDoor && EntryDoorLeft.transform.eulerAngles.y != targetLeftRotation)
        //{
        //    // Calculate the step for each frame
        //    float step = rotationSpeed * Time.deltaTime;
        //    float currentRotation = Mathf.MoveTowardsAngle(EntryDoorLeft.transform.eulerAngles.y, targetLeftRotation, step);
        //    EntryDoorLeft.transform.eulerAngles = new Vector3(0, currentRotation, 0);
        //}

        // Smoothly rotate the right door if it's not yet at target
        //if (isOpenEntryDoor && EntryDoorRight.transform.eulerAngles.y != targetRightRotation)
        //{
        //    // Calculate the step for each frame
        //    float step = rotationSpeed * Time.deltaTime;
        //    float currentRotation = Mathf.MoveTowardsAngle(EntryDoorRight.transform.eulerAngles.y, targetRightRotation, step);
        //    EntryDoorRight.transform.eulerAngles = new Vector3(0, currentRotation, 0);
        //}

        // Check if both doors have reached their target rotation
        //if (EntryDoorLeft.transform.eulerAngles.y == targetLeftRotation && EntryDoorRight.transform.eulerAngles.y == targetRightRotation)
        //{
        //    isOpenEntryDoor = false; // Set the bool to false once the doors are fully open
        //}

       

        //if (isOpenExitDoor && ExitDoorLeft.transform.eulerAngles.y != targetRightRotation)
        //{
        //    // Calculate the step for each frame
        //    float step = rotationSpeed * Time.deltaTime;
        //    float currentRotation = Mathf.MoveTowardsAngle(ExitDoorLeft.transform.eulerAngles.y, targetRightRotation, step);
        //    ExitDoorLeft.transform.eulerAngles = new Vector3(0, currentRotation, 0);
        //}

        //// Smoothly rotate the right door if it's not yet at target
        //if (isOpenExitDoor && ExitDoorRight.transform.eulerAngles.y != targetLeftRotation)
        //{
        //    // Calculate the step for each frame
        //    float step = rotationSpeed * Time.deltaTime;
        //    float currentRotation = Mathf.MoveTowardsAngle(ExitDoorRight.transform.eulerAngles.y, targetLeftRotation, step);
        //    ExitDoorRight.transform.eulerAngles = new Vector3(0, currentRotation, 0);
        //}

        // Check if both doors have reached their target rotation
        //if (ExitDoorLeft.transform.eulerAngles.y == targetRightRotation && ExitDoorRight.transform.eulerAngles.y == targetLeftRotation)
        //{
        //    isOpenExitDoor = false; // Set the bool to false once the doors are fully open
        //}
    }

    public void OpenEntryDoor()
    {
        EntryDoorLeft.transform.eulerAngles = new Vector3(0, 0, 0);
    }


    public void Closegate()
    {
        isOpenEntryDoor = false;
        isOpenExitDoor = false;
        EntryDoorLeft.transform.eulerAngles = new Vector3(0, 0, 0);
        EntryDoorRight.transform.eulerAngles = new Vector3(0, 0, 0);
        ExitDoorLeft.transform.eulerAngles = new Vector3(0, 0, 0);
        ExitDoorRight.transform.eulerAngles = new Vector3(0, 0, 0);
    }
}