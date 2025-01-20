using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    [SerializeField] private Animator botAnimator;

    [SerializeField] private Vector3 startingPos;
    [SerializeField] private Vector3 startingEular;
    [SerializeField] private Vector3 startingScale;

    public List<GameObject> bulletAll;
    public List<GameObject> bulletUsed;
    public List<GameObject> bulletUnused;
    public int bulletCount = 0;

    private NavMeshAgent navAgent;
    private bool isFollowing = false;

    public GameManager GameManager;
    public float stopDistance = 2.0f; // Distance to stop away from the player
    private bool isIdle = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = transform.position;
        startingEular = transform.eulerAngles;
        startingScale = transform.localScale;
        InvokeRepeating("HealthUpgradation", 0, 1);
        //Player_Manager = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Manager>();
        navAgent = GetComponent<NavMeshAgent>();
    }


    private void Update()
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

        if (Player_Manager != null)
        {
            DistanceChecker();

            if (isOnceInRadius == true && Player_Manager != null)
            {
                transform.LookAt(Player_Manager.transform.position);
            }
        }

        if (isFollowing && Player_Manager != null)
        {
            if (Vector3.Distance(Player_Manager.gameObject.transform.position, this.gameObject.transform.position) > stopDistance)
            {
                Vector3 directionToPlayer = Player_Manager.gameObject.transform.position - navAgent.transform.position;
                Vector3 targetPosition = Player_Manager.gameObject.transform.position - directionToPlayer.normalized * stopDistance;

                navAgent.SetDestination(targetPosition);
                AnimationController(AnimState.Running);
            }
            else
            {
                AnimationController(AnimState.Idle);
            }
        }
    }

    void FollowPlayer()
    {
        if (isFollowing && Player_Manager != null)
            return;

        // Calculate the direction from the bot to the player
        Vector3 directionToPlayer = (Player_Manager.gameObject.transform.position - transform.position).normalized;

        // Calculate the target position some distance away from the player
        Vector3 targetPosition = Player_Manager.gameObject.transform.position - directionToPlayer * 2f;

        // Set the NavMeshAgent's destination
        navAgent.SetDestination(targetPosition);

        // Check the distance between the bot and the player
        float distanceToPlayer = Vector3.Distance(transform.position, Player_Manager.gameObject.transform.position);

        if (distanceToPlayer <= stopDistance)
        {
            if (!isIdle)
            {
                AnimationController(AnimState.Idle); // Trigger Idle animation
                isIdle = true;
            }
        }
        else
        {
            if (isIdle)
            {
                AnimationController(AnimState.Running); // Trigger Running animation
                isIdle = false;
            }
        }
    }

    // Health increase
    void HealthUpgradation()
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

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

    public void StartFollowing()
    {
        isFollowing = true;
        AnimationController(AnimState.Running);
    }

    public void StopFollowing()
    {
        isFollowing = false;
        navAgent.ResetPath(); // Stops the bot
        AnimationController(AnimState.Idle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

        if (other.gameObject.transform.TryGetComponent<Bullet>(out Bullet bullet))
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
        if (GameManager.GamePlay == false)
        {
            return;
        }
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
                CancelInvoke("Shoot");
                InvokeRepeating("Shoot", shootStartTime, shootWaitTime);
                isInRadius = true;
                isOnceInRadius = true;
                StartFollowing();
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

    void AnimationController(AnimState newState)
    {
        switch (newState)
        {
            case AnimState.Idle:
                botAnimator.SetBool("Idle", true);
                botAnimator.SetBool("Running", false);
                break;
            case AnimState.Running:
                botAnimator.SetBool("Idle", false);
                botAnimator.SetBool("Running", true);
                break;
        }
    }

    enum AnimState
    {
        Idle,
        Running
    }

    public void ResetingGame()
    {
        this.gameObject.SetActive(true);
        AnimationController(AnimState.Idle);
        CancelInvoke();
        StopFollowing();
        CollectingBullet();
        botHealth = botMaxHealth;
        this.transform.position = startingPos;
        this.transform.eulerAngles = startingEular;
        this.transform.localScale = startingScale;
    }

    void CollectingBullet()
    {
        for (int i = 0; i < bulletAll.Count; i++)
        {
            bulletAll[i].GetComponent<Bullet>().GoToParent();
        }
    }

}