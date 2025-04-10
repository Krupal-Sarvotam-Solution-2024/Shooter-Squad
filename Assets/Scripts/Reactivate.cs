using UnityEngine;
using System.Collections;
public class Reactivate : MonoBehaviour
{
    public GameManager manager;
    public Vector3 minimum, maximum;
    bool insidesafezone;
    public IEnumerator reacrivate()
    {
        yield return new WaitForSeconds(Random.Range(0,5));

        this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
        this.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("triger entered");
        if (other.GetComponent<SafeZone>())
        {
            insidesafezone = true;
        }

        if(!insidesafezone)
            this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
        if (other.GetComponent<Bullet>())
            return;
        
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<SafeZone>())
        {
            insidesafezone = false;
            this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
        }
    }
}
