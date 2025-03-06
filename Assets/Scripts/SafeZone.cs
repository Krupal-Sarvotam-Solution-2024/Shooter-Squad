using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public float speedreduceing;
    public Vector3 startingsclae;
    public bool start;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!start)
            return;

        transform.localScale -= new Vector3(speedreduceing * Time.deltaTime, 0, speedreduceing * Time.deltaTime);
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.GetComponent<Entity>())
        {
            other.GetComponent<Entity>().ReduceHeath(1000,null);
        }

   

    }
}
