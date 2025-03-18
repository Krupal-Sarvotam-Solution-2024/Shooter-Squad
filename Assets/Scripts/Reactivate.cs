using UnityEngine;
using System.Collections;
public class Reactivate : MonoBehaviour
{
    public GameManager manager;
    public Vector3 minimum, maximum;
    public IEnumerator reacrivate()
    {
        yield return new WaitForSeconds(Random.Range(0,5));

        this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
        this.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("triger entered");
        if (other.GetComponent<Bullet>())
            return;
        this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
    }
}
