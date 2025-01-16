using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Player_Manager bulletPlayer;
    public Bot_Manager bulletBot;

    private void Start()
    {
        Destroy(gameObject,5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
