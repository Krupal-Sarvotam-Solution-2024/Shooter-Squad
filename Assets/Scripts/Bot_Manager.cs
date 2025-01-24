using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Bot_Manager : MonoBehaviour
{


    [SerializeField] private float botHealth; // Bot current health
    [SerializeField] private float botMaxHealth = 100f; // Bot max health
    [SerializeField] private float botHealthIncrement = 1f; // Bot health recovery amount
    [SerializeField] private float botHealthDeductionAmount = 3f; // Bot health deduction amount
    [SerializeField] private int botDeathScore = 10; // Score for increment to the player
    [SerializeField] private GameObject HealthBar, HealthBarFG; // Health bar on the head image
    [SerializeField] private TextMeshPro HealthPerText; // Health percantage text

    public Player_Manager Player_Manager; // Player

    [SerializeField] private bool isInRadius; // Check that it is in radius or not for shooting
    [SerializeField] private bool isOnceInRadius; // Check that it is in radius or not for shooting

    [SerializeField] private Transform FirePoint; // Bullet shooting point
    [SerializeField] private GameObject prefebBullet; // Bullet prefeb
    [SerializeField] private float bulletSpeed; // Bullet speed after shoot
    [SerializeField] private float shootStartTime; // Waiting time for next shot
    [SerializeField] private float shootWaitTime; // Waiting time for next shot

    [SerializeField] private Animator botAnimator; // Animator controller

    [SerializeField] private Vector3 startingPos; // Bot starting pos
    [SerializeField] private Vector3 startingEular; // Bot starting rotation
    [SerializeField] private Vector3 startingScale; // Bot starting scale

    public List<GameObject> bulletAll; // All bullet of bot
    public List<GameObject> bulletUsed; // All used bullet
    public List<GameObject> bulletUnused; // All unused bullet

    private NavMeshAgent navAgent; // Navmesh agent for following the player
    private bool isFollowing = false; // Find that bot is following the player or not
    private bool isRandomMove = false; // Find that it should move random or not
    public Transform randomMovePos;

    public GameManager GameManager;
    public float stopDistance = 2.0f; // Distance to stop away from the player

    private bool isIdle = false; // Find that bot is idle or not
    public GameObject AnimatorObject; // Animator occupied game object

    // Audio managing system
    public AudioSource botAudio; // Audio source which handle player audios
    public AudioClip playerDeath; // All audio clips

    public Weapon myWeapon; // Bot weapon

    [SerializeField] private List<GameObject> botBodyParts; // All body parts for activation and deactivation
    [SerializeField] private GameObject DeathPartcleSystem; // Death particle system
    bool isDeath; // Find that bot is Death or not

    public List<GameObject> DeathIndicatorAll; // All Damage indicator gameobject
    public List<GameObject> DeathIndicatorUsed; // Used damage indicator
    public List<GameObject> DeathIndicatorUnused; // Unused damage indicator


    // Start
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>(); // Assigning the component
        ReassignValue(); // Making bot value as a new bot
    }

    // Update
    void Update()
    {
        HealthBar.transform.LookAt(Camera.main.transform.position); // Healthbar saw camera continusoly

        // Return if game play is off ot bot is death
        if (GameManager.GamePlay == false || isDeath == true)
        {
            return;
        }

        if (isOnceInRadius == false)
        {
            isRandomMove = true;
        }
        else
        {
            isRandomMove = false;
        }

        // Look at the player contusoly
        if (Player_Manager != null) // Find that player is not null
        {
            DistanceChecker(); // Cheking the distance

            if (isOnceInRadius == true && Player_Manager != null)
            {
                transform.LookAt(Player_Manager.transform.position); // After find player it start look the player always
            }
        }

        // Follow the player and managing the animation
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
                botAudio.Stop();
            }
        }
    }

    // On Collide with any object
    void OnCollisionEnter(Collision collision)
    {
        if (GameManager.GamePlay == false || isDeath == true)
        {
            return;
        }

        if (collision.gameObject.transform.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (bullet.bulletPlayer != null)
            {
                BulletHitted(bullet);
            }
        }
    }

    // Follow the player and managin animtion
    void FollowPlayer()
    {
        if (isFollowing && Player_Manager != null)
            return;
        if (GameManager.GamePlay == false || isDeath == true)
        {
            return;
        }

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
                botAudio.Stop();
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
        if (GameManager.GamePlay == false || isDeath == true)
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
        DamgeIndicator((int)botHealthDeductionAmount);
        HealthShow();
    }

    // Health Show
    void HealthShow()
    {
        float healthPercentage = (botHealth / botMaxHealth) * 100f;
        HealthBarFG.transform.localScale = new Vector3(healthPercentage / 100, HealthBarFG.transform.localScale.y, HealthBarFG.transform.localScale.z);
        HealthPerText.text = healthPercentage + "%";
    }

    // Player bullet hit
    void BulletHitted(Bullet bullet)
    {
        HealthDeduction();
        if (botHealth <= 0)
        {
            bullet.bulletPlayer.KillPlayer(botDeathScore);
            /*if (Player_Manager.listEnemy.Contains(this.gameObject))
            {
                Player_Manager.listEnemy.Remove(this.gameObject);
                Player_Manager.enemyInRadius--;
            }
            //Destroy(this.gameObject);
            this.gameObject.SetActive(false);
            CancelInvoke("Shoot");*/
            StartCoroutine(BotDeath());
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

    // Collecting the all used bullet
    void CollectingBullet()
    {
        for (int i = 0; i < bulletAll.Count; i++)
        {
            bulletAll[i].GetComponent<Bullet>().GoToParent();
        }
    }

    // Change a visibility of the body
    void BodyVisibility(bool visibility)
    {
        for (int i = 0; i < botBodyParts.Count; i++)
        {
            botBodyParts[i].gameObject.SetActive(visibility);
        }
    }

    // Damage indicator
    void DamgeIndicator(int damage)
    {

        if (DeathIndicatorUnused.Count == 0)
        {
            DeathIndicatorUsed[0].gameObject.SetActive(false);
            DeathIndicatorUnused.Add(DeathIndicatorUsed[0]);
            DeathIndicatorUsed.Remove(DeathIndicatorUsed[0]);
        }

        GameObject indicator = DeathIndicatorUnused[0];
        indicator.SetActive(true);
        DeathIndicatorUsed.Add(indicator);
        DeathIndicatorUnused.Remove(indicator);

        indicator.GetComponent<TextMeshPro>().text = "-" + damage.ToString();
        StartCoroutine(damageIdicatorInterval(indicator));
    }

    // Controlling animation
    void AnimationController(AnimState newState)
    {
        switch (newState)
        {
            case AnimState.Idle:
                botAnimator.SetBool("Idle", true);
                botAnimator.SetBool("Running", false);
                /*botAnimator.SetBool("Death", false);*/
                break;
            case AnimState.Running:
                botAnimator.SetBool("Idle", false);
                botAnimator.SetBool("Running", true);
                /*botAnimator.SetBool("Death", false);*/
                break;
                /*case AnimState.Death:
                    botAnimator.SetBool("Idle", false);
                    botAnimator.SetBool("Running", false);
                    botAnimator.SetBool("Death", true);
                    break;*/
        }
    }

    // Random movement until catch the player
    void MoveRandom()
    {
        // Random movement code should be here
    }

    // Start follow to player
    public void StartFollowing()
    {
        isFollowing = true;
        AnimationController(AnimState.Running);
    }

    // Stop follow to player
    public void StopFollowing()
    {
        isFollowing = false;
        if (this.gameObject.activeInHierarchy == true && navAgent != null)
        {
            navAgent.ResetPath(); // Stops the bot 
        }
        AnimationController(AnimState.Idle);
        botAudio.Stop();
    }

    // Bullet shoot
    public void Shoot()
    {
        if (Player_Manager == null)
            return;

        GameObject bullet = bulletUnused[0];
        bulletUsed.Add(bulletUnused[0]);
        bulletUnused.Remove(bulletUnused[0]);
        bullet.SetActive(true);
        Vector3 direction = transform.forward;
        Rigidbody rb_bullet = bullet.GetComponent<Rigidbody>();
        rb_bullet.linearVelocity = direction * bulletSpeed;
        bullet.GetComponent<Bullet>().bulletBot = this.transform.GetComponent<Bot_Manager>();
        bullet.transform.parent = null;

        myWeapon.enabled = true;
        myWeapon.WeaponAudio.clip = myWeapon.BlastSound;
        myWeapon.WeaponAudio.Play();
    }

    // Making bot value defualt
    public void ResetingGame()
    {
        isDeath = false;

        CancelInvoke();

        this.gameObject.SetActive(true);

        AnimationController(AnimState.Idle);
        StopFollowing();

        GetComponent<Rigidbody>().isKinematic = false;

        botHealth = botMaxHealth;
        isOnceInRadius = false;

        //this.transform.position = startingPos;
        this.transform.eulerAngles = startingEular;
        this.transform.localScale = startingScale;

        AnimatorObject.gameObject.transform.localRotation = Quaternion.identity;

        Player_Manager = GameManager.player;

        HealthShow();

        CollectingBullet();
    }

    // Reassign the bot tranform
    public void ReassignValue()
    {
        startingPos = transform.position;
        startingEular = transform.eulerAngles;
        startingScale = transform.localScale;
    }

    // On Bot Death
    IEnumerator BotDeath()
    {
        isDeath = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
        BodyVisibility(false);
        DeathPartcleSystem.SetActive(true);
        DeathPartcleSystem.GetComponent<ParticleSystem>().Play();
        if (Player_Manager.listEnemy.Contains(this.gameObject))
        {
            Player_Manager.listEnemy.Remove(this.gameObject);
            Player_Manager.enemyInRadius--;
        }
        CancelInvoke("Shoot");

        Player_Manager = null;
        StopFollowing();

        /*AnimationController(AnimState.Death);*/

        if (GameManager.botDeath.Contains(this) == false)
        {
            GameManager.botDeath.Add(this);
        }

        botAudio.clip = playerDeath;
        botAudio.PlayOneShot(playerDeath);
        yield return new WaitForSeconds(3);
        DeathPartcleSystem.SetActive(false);
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().enabled = true;
        this.gameObject.SetActive(false);
        BodyVisibility(true);
    }

    // Interval for damage interval
    IEnumerator damageIdicatorInterval(GameObject indicator)
    {
        yield return new WaitForSeconds(1.5f);

        indicator.SetActive(false);

        DeathIndicatorUsed.Remove(indicator);
        DeathIndicatorUnused.Add(indicator);
    }

    // All animation state
    enum AnimState
    {
        Idle,
        Running/*,
        Death*/
    }

}