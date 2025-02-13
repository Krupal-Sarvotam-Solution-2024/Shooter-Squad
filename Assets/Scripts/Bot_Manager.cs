using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bot_Manager : Entity
{

    [Space(10)]
    [Header("Bot health system managing variables")]
    [SerializeField] private float botHealthIncrement = 1f; // Bot health recovery amount
    [SerializeField] private GameObject HealthBar, HealthBarFG; // Health bar on the head image
    [SerializeField] private TextMeshPro HealthPerText; // Health percantage text

    [Space(10)]
    [Header("Bot death score and damage manager")]
    [SerializeField] private int botDeathScore = 10; // Score for increment to the player
    [SerializeField] private int botHitDamage = 3; // Health decrement amount from the player

    [Space(10)]
    [Header("Player Manager")]
    public Player_Manager Player_Manager; // Player
    public GameObject SelectedBot; // Red Target image

    [Space(10)]
    [Header("Player following variables")]
    [SerializeField] private bool isInRadius; // Check that it is in radius or not for shooting
    [SerializeField] private bool isOnceInRadius; // Check that it is in radius or not for shooting
    public float stopDistance = 2.0f; // Distance to stop away from the player


    [Space(10)]
    [Header("All Shooting varables")]
    [SerializeField] private Transform FirePoint; // Bullet shooting point
    [SerializeField] private GameObject prefebBullet; // Bullet prefeb
    [SerializeField] private float bulletSpeed; // Bullet speed after shoot
    [SerializeField] private float shootStartTime; // Waiting time for next shot
    [SerializeField] private float shootWaitTime; // Waiting time for next shot
    [SerializeField] private ParticleSystem ShootParicle;

    [Space(10)]
    [Header("Animation manager variables")]
    [SerializeField] private Animator botAnimator; // Animator controller
    private bool isIdle = false; // Find that bot is idle or not
    public GameObject AnimatorObject; // Animator occupied game object

    [Space(10)]
    [Header("Bot default value variables")]
    [SerializeField] private Vector3 startingPos; // Bot starting pos
    [SerializeField] private Vector3 startingEular; // Bot starting rotation
    //[SerializeField] private Vector3 startingScale; // Bot starting scale

    [Space(10)]
    [Header("All bullet managing variables")]
    public List<GameObject> bulletAll; // All bullet of bot
    public List<GameObject> bulletUsed; // All used bullet
    public List<GameObject> bulletUnused; // All unused bullet

    [Space(10)]
    [Header("Nav mesh manager")]
    private bool isFollowing = false; // Find that bot is following the player or not
    private bool movingToTarget = true; // Track if moving to target or back to start
    public Transform RandomMovePos;


    [Space(10)]
    [Header("Game manager")]
    public GameManager GameManager;

  

    [Space(10)]
    [Header("Weapon manager")]
    public Weapon myWeapon; // Bot weapon


    [SerializeField] private Color InsideGrass, outsidegrass;

    [Space(10)]
    [Header("Player death manager")]
    public bool isDeath; // Find that bot is Death or not
    public List<GameObject> DeathIndicatorAll; // All Damage indicator gameobject
    public List<GameObject> DeathIndicatorUsed; // Used damage indicator
    public List<GameObject> DeathIndicatorUnused; // Unused damage indicator
    [SerializeField] private bool insidegrass;

    [Header("shild")]
    public GameObject shildeffect;
    public bool shild;
    // Start
    void OnEnable()
    {
       // navAgent = GetComponent<NavMeshAgent>(); // Assigning the component
     //   ReassignValue(); // Making bot value as a new bot
        startingPos = transform.position;
    }

    // Update
    void Update()
    {
      //  HealthBar.transform.LookAt(Camera.main.transform.position); // Healthbar saw camera continusoly

        // Return if game play is off ot bot is death
        if (GameManager.GamePlay == false || isDeath == true)
        {
            if(isDeath)
            SelectedBot.SetActive(false);
            return;
        }
        else
        {
        //    SelectedBot.SetActive(true);
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

        if (isOnceInRadius == false)
        {
            // Check if the bot has reached its current destination
            if (!entity_navAi.pathPending && entity_navAi.remainingDistance <= entity_navAi.stoppingDistance)
            {
                if (!entity_navAi.hasPath || entity_navAi.velocity.sqrMagnitude == 0f)
                {
                    // Switch between the target and start position
                    if (movingToTarget)
                    {
                        entity_navAi.SetDestination(startingPos); // Go back to start
                    }
                    else
                    {
                        entity_navAi.SetDestination(RandomMovePos.position); // Go to the target
                    }

                    movingToTarget = !movingToTarget; // Toggle the direction
                    AnimationController(AnimState.Running);
                }
            }
        }

        // Follow the player and managing the animation
        if (isFollowing && Player_Manager != null)
        {
            if (Vector3.Distance(Player_Manager.gameObject.transform.position, this.gameObject.transform.position) > stopDistance)
            {
                Vector3 directionToPlayer = Player_Manager.gameObject.transform.position - entity_navAi.transform.position;
                Vector3 targetPosition = Player_Manager.gameObject.transform.position - directionToPlayer.normalized * stopDistance;

                entity_navAi.SetDestination(targetPosition);
                AnimationController(AnimState.Running);
            }
            else
            {
                AnimationController(AnimState.Idle);
                enity_audio.Stop();
            }
        }

        transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);

    }
   
    // On Collide with any object
    void OnCollisionEnter(Collision collision)
    {
        if (GameManager.GamePlay == false || isDeath == true)
        {
            return;
        }

      
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

        if (other.GetComponent<Grass>())
        {
            //Debug.Log("TEXT");
            //for (int i = 0; i < botBodyParts.Count; i++)
            //{
            //    botBodyParts[i].gameObject.SetActive(false);
            //    //if (botBodyParts[i].GetComponent<MeshRenderer>())
            //    //{
            //    //    botBodyParts[i].GetComponent<MeshRenderer>().material.color = InsideGrass;
            //    //}
            //}
        }

        if (other.gameObject.transform.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (bullet.bulletPlayer != null)
            {
                BulletHitted(bullet);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

        //if (other.GetComponent<Grass>())
        //{
        //    for (int i = 0; i < botBodyParts.Count; i++)
        //    {
        //        botBodyParts[i].gameObject.SetActive(true);
        //    }
        //}

    }
    public IEnumerator shildAcitavte()
    {
        //acriavter shild
        shild = true;
        shildeffect.SetActive(true);
        //showing effect
        yield return new WaitForSeconds(3);
        shildeffect.SetActive(false);
        shild = false;
        //deaactivate shild
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
        entity_navAi.SetDestination(targetPosition);

        // Check the distance between the bot and the player
        float distanceToPlayer = Vector3.Distance(transform.position, Player_Manager.gameObject.transform.position);

        if (distanceToPlayer <= stopDistance)
        {
            if (!isIdle)
            {
                AnimationController(AnimState.Idle); // Trigger Idle animation
                enity_audio.Stop();
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

        //if (botHealth < botMaxHealth)
        //{
        //    botHealth += botHealthIncrement;
        //}
    }

    // Health deduct on player hit
    public void HealthDeduction(int DamageAmount)
    {
     //   botHealth -= DamageAmount;
        DamgeIndicator((int)DamageAmount);
      //  HealthShow();
        //if (botHealth <= 0)
        //{
        //   // bullet.bulletPlayer.KillPlayer(botDeathScore);
        //    StartCoroutine(BotDeath());
        //}
    }

    // Health Show
    //void HealthShow()
    //{
    //   // float healthPercentage = (botHealth / botMaxHealth) * 100f;
    //    HealthBarFG.transform.localScale = new Vector3(healthPercentage / 100, HealthBarFG.transform.localScale.y, HealthBarFG.transform.localScale.z);
    //    HealthPerText.text = healthPercentage + "%";
    //}

    // Player bullet hit
    void BulletHitted(Bullet bullet)
    {
        ReduceHeath(bullet.damageAmount);
       
    }

    // Distance check between player and bot
    void DistanceChecker()
    {
        if (GameManager.GamePlay == false || Player_Manager.isDeath)
        {
            StopFollowing();
            return;
        }
        if (isInRadius)
        {
            if (Vector3.Distance(Player_Manager.gameObject.transform.position, this.gameObject.transform.position) > Player_Manager.enemyDistance || Player_Manager.insidegrass == true)// bot is outside the radious
            {
                isOnceInRadius = false;
                CancelInvoke("Shoot");
                isInRadius = false;
                StopFollowing();
                Player_Manager.listEnemy.Remove(this.gameObject);
                Player_Manager.enemyInRadius--;
                //SelectedBot.SetActive(false);
            }
           
        }
        else
        {
            if (Vector3.Distance(Player_Manager.gameObject.transform.position, this.gameObject.transform.position) <= Player_Manager.enemyDistance && Player_Manager.insidegrass == false)// bot in the radious
            {
                Debug.Log("Keep shooting");
                CancelInvoke("Shoot");
                InvokeRepeating("Shoot", shootStartTime, shootWaitTime);
                isInRadius = true;
                isOnceInRadius = true;
                StartFollowing();
                Player_Manager.listEnemy.Add(this.gameObject);
                Player_Manager.enemyInRadius++;
            }
            else 
            {

            }
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
                botAnimator.SetBool("Turn", false);
                break;
            case AnimState.Running:
                botAnimator.SetBool("Idle", false);
                botAnimator.SetBool("Running", true);
                botAnimator.SetBool("Turn", false);
                break;
            case AnimState.Turn:
                botAnimator.SetBool("Idle", false);
                botAnimator.SetBool("Running", false);
                botAnimator.SetBool("Turn", true);
                break;
        }
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
        if (this.gameObject.activeInHierarchy  && entity_navAi != null)
        {
            entity_navAi.ResetPath(); // Stops the bot 
        }
        AnimationController(AnimState.Idle);
    //    enity_audio.Stop();
        //botAudio.Stop();
    }

    // Bullet shoot
    public void Shoot()
    {
        if (Player_Manager == null || Player_Manager.isDeath)
            return;
        for (int i = 0; i < myWeapon.FirePoints.Count; i++)
        {


            GameObject bullet = bulletUnused[0];
            bulletUsed.Add(bulletUnused[0]);
            bulletUnused.Remove(bulletUnused[0]);
            bullet.SetActive(true);
            bullet.transform.position =myWeapon.FirePoints[i].position;
            bullet.transform.eulerAngles = myWeapon.FirePoints[i].eulerAngles;
            Vector3 direction = bullet.transform.forward;
            Rigidbody rb_bullet = bullet.GetComponent<Rigidbody>();
            rb_bullet.linearVelocity = direction * bulletSpeed;
            bullet.GetComponent<Bullet>().bulletBot = this.transform.GetComponent<Bot_Manager>();
            bullet.GetComponent<Bullet>().damageAmount = botHitDamage;
            bullet.transform.parent = null;

            ShootParicle.Play();

            if (myWeapon.isPlayMultiTime == true)
            {
                myWeapon.enabled = true;
               myWeapon.WeaponAudio.clip = myWeapon.BlastSound;
            }
        }
       // Camera.main.gameObject.GetComponent<Camera_Follower>().Fire();

        myWeapon.WeaponAudio.Play();

        if (myWeapon.isPlayMultiTime == false)
        {
            myWeapon.enabled = true;
            myWeapon.WeaponAudio.clip = myWeapon.BlastSound;
        }

        botAnimator.SetBool("Shoot", true);
        Invoke("ResetShooting", 0.2f); // Adjust timing based on animation length
    }
  

    void ResetShooting()
    {
        botAnimator.SetBool("Shoot", false);
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

       // botHealth = botMaxHealth;
        isOnceInRadius = false;

      //  this.transform.position = startingPos;
        this.transform.eulerAngles = startingEular;
       // this.transform.localScale = startingScale;

        AnimatorObject.gameObject.transform.localRotation = Quaternion.identity;

        Player_Manager = GameManager.player;

        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;

   //     HealthShow();

      //  CollectingBullet();
    }

    // Reassign the bot tranform
    public void ReassignValue()
    {
        startingPos = transform.position;
        startingEular = transform.eulerAngles;
    //    startingScale = transform.localScale;
    }

    // On Bot Death
    public override IEnumerator Death()
    {
        isDeath = true;
        if (Player_Manager.listEnemy.Contains(this.gameObject))
        {
            Player_Manager.listEnemy.Remove(this.gameObject);
            Player_Manager.enemyInRadius--;
        }
        CancelInvoke("Shoot");
        Player_Manager.killcount++;
        Player_Manager = null;

        //SelectedBot.SetActive(false);

        if (GameManager.botDeath.Contains(this) == false)
        {
            GameManager.botDeath.Add(this);
        }
        GameManager.ShowBlood(transform.position);

     //   yield return new WaitForSeconds(3);
        //  DeathPartcleSystem.SetActive(false);
  
        //    this.gameObject.SetActive(false);
        StopFollowing();
        return base.Death();


    }


        

   
      //  BodyVisibility(true);
    

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
        Running,
        Turn
    }

}