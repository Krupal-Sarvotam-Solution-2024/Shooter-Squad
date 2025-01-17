using System.Collections.Generic;
using UnityEngine;

public class Bot_Manager : MonoBehaviour
{
    [SerializeField] private float botHealth; // Bot current health
    [SerializeField] private float botMaxHealth = 100f; // Bot max health
    [SerializeField] private float botHealthIncrement = 1f; // Bot health recovery amount
    [SerializeField] private float botHealthDeductionAmount = 3f; // Bot health deduction amount
    [SerializeField] private int botDeathScore = 10; // Score for increment to the player

    [SerializeField] private Player_Manager Player_Manager; // Player

    [SerializeField] private bool isInRadius; // Check that it is in radius or not for shooting
    [SerializeField] private bool isOnceInRadius; // Check that it is in radius or not for shooting

    [SerializeField] private Transform FirePoint; // Bullet shooting point
    [SerializeField] private GameObject prefebBullet; // Bullet prefeb
    [SerializeField] private float bulletSpeed; // Bullet speed after shoot
    [SerializeField] private float shootStartTime; // Waiting time for next shot
    [SerializeField] private float shootWaitTime; // Waiting time for next shot

    public List<GameObject> bulletAll;
    public List<GameObject> bulletUsed;
    public List<GameObject> bulletUnused;
    public int bulletCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //InvokeRepeating("HealthUpgradation", 0, 1);
        Player_Manager = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Manager>();
    }

    private void Update()
    {
        if (Player_Manager != null)
        {
            DistanceChecker();

            if (isOnceInRadius == true && Player_Manager != null)
            {
                transform.LookAt(Player_Manager.transform.position);
            }
        }
    }

    // Health increase
    void HealthUpgradation()
    {
        if (botHealth < botMaxHealth)
        {
            botHealth += botHealthIncrement;
        }
    }

    // Health deduct on player hit
    void HealthDeduction()
    {
        botHealth -= botHealthDeductionAmount;
    }

    // Player bullet hit
    void BulletHitted(Bullet bullet)
    {
        HealthDeduction();
        if (botHealth <= 0)
        {
            bullet.bulletPlayer.KillPlayer(botDeathScore);
            if (Player_Manager.listEnemy.Contains(this.gameObject))
            {
                Player_Manager.listEnemy.Remove(this.gameObject);
                Player_Manager.enemyInRadius--;
            }
            //Destroy(this.gameObject);
            this.gameObject.SetActive(false);
            CancelInvoke("Shoot");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (bullet.bulletPlayer != null)
            {
                BulletHitted(bullet);
            }
        }
    }

    // Distance check between player and bot
    void DistanceChecker()
    {
        if (isInRadius)
        {
            if (Vector3.Distance(Player_Manager.gameObject.transform.position, this.gameObject.transform.position) > Player_Manager.enemyDistance)
            {
                //CancelInvoke("Shoot");
                isInRadius = false;
                Player_Manager.listEnemy.Remove(this.gameObject);
                Player_Manager.enemyInRadius--;
            }
        }
        else
        {
            if (Vector3.Distance(Player_Manager.gameObject.transform.position, this.gameObject.transform.position) <= Player_Manager.enemyDistance)
            {
                InvokeRepeating("Shoot", shootStartTime, shootWaitTime);
                isInRadius = true;
                isOnceInRadius = true;
                Player_Manager.listEnemy.Add(this.gameObject);
                Player_Manager.enemyInRadius++;
            }
        }
    }

    // Bullet shoot
    public void Shoot()
    {
        if (Player_Manager == null)
            return;

        GameObject bullet = bulletUnused[bulletCount];
        bulletUsed.Add(bulletUnused[bulletCount]);
        bulletUnused.Remove(bulletUnused[bulletCount]);
        bullet.SetActive(true);
        Vector3 direction = transform.forward;
        Rigidbody rb_bullet = bullet.GetComponent<Rigidbody>();
        rb_bullet.linearVelocity = direction * bulletSpeed;
        bullet.GetComponent<Bullet>().bulletBot = this.transform.GetComponent<Bot_Manager>();
        bullet.transform.parent = null;
    }
}