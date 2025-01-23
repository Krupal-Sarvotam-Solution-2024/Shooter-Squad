using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Health_Indocator : MonoBehaviour 
{
    bool isStartGoingUp;
    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        isStartGoingUp = true;
    }

    private void Update()
    {
        if(isStartGoingUp)
        {
            // Move the text upward.
            transform.position += Vector3.up * 1 * Time.deltaTime;
            transform.LookAt(Camera.main.transform.position);
        }
    }


    private void OnDisable()
    {
        isStartGoingUp = false;
        transform.localPosition = Vector3.zero;
    }
}
