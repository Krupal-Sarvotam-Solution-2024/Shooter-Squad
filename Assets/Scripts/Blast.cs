using System.Collections;
using UnityEngine;

public class Blast : MonoBehaviour
{
    public int damangeamount;

    private void OnEnable()
    {

        StartCoroutine(waitfordeactivate());


    }

    public IEnumerator waitfordeactivate()
    {
        yield return new WaitForSeconds(.2f);
        transform.parent.gameObject.SetActive(false);
    }
    private void OnTriggerStay(Collider collision)
    {
            Debug.Log("trigering");
        if (collision.GetComponent<Player_Manager>())
        {
            collision.GetComponent<Player_Manager>().ReduceHeath(damangeamount);
        }      
    }

    
}
