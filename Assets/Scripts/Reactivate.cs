using UnityEngine;
using System.Collections;
public class Reactivate : MonoBehaviour
{
    public GameManager manager;
    public Vector3 minimum, maximum;
    bool insidesafezone;
    public bool wepon;
    public IEnumerator reacrivate()
    {
        
        yield return new WaitForSeconds(Random.Range(0,5));

        this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
        this.gameObject.SetActive(true);
    }

    public void Relocating()
    {
        this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<SafeZone>())
        {
            return;
        }

        if (other.GetComponent<Bullet>())
        return;

        if (insidesafezone)
        {
            if (this.wepon && other.GetComponent<Entity>())
                return;

           this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<SafeZone>())
        {
            insidesafezone = true;
        }
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
