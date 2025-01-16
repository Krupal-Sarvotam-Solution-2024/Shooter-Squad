using UnityEngine;

public class Player_Shooting : MonoBehaviour
{
    [SerializeField] private Transform FirePoint;
    [SerializeField] private GameObject prefebBullet;
    [SerializeField] private float bulletSpeed;

    private void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * 100, Color.green);
        if (Input.GetKeyDown(KeyCode.K))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        GameObject bullet = Instantiate(prefebBullet, FirePoint.position, Quaternion.identity, transform);
        Vector3 direction = transform.forward;
        Rigidbody rb_bullet = bullet.GetComponent<Rigidbody>();
        rb_bullet.linearVelocity = direction * bulletSpeed;
        bullet.GetComponent<Bullet>().bulletPlayer = this.transform.GetComponent<Player_Manager>();
    }
}
