using UnityEngine;
using System.Collections;
public class Reactivate : MonoBehaviour
{
    public GameManager manager;
    public Vector3 minimum, maximum;
    public IEnumerator reacrivate()
    {
        yield return new WaitForSeconds(Random.Range(3,15));

        this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
        this.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("triger entered");

        this.transform.position = new Vector3(Random.Range(minimum.x, maximum.x), transform.position.y, Random.Range(minimum.z, maximum.z));
    }
}
