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
    [SerializeField] private Vector3 startingEular; // Bot starting rotation
    //[SerializeField] private Vector3 startingScale; // Bot starting scale


    [Space(10)]
    [Header("Nav mesh manager")]
    private bool isFollowing = false; // Find that bot is following the player or not
    private bool movingToTarget = true; // Track if moving to target or back to start
    public Transform RandomMovePos;
    [SerializeField] private bool isInInterval; // Find that gun is in interval or not




    [Header("shild")]
    public GameObject shildeffect;
    public bool shild;
   

    // Update
    void Update()
    {
      //  HealthBar.transform.LookAt(Camera.main.transform.position); // Healthbar saw camera continusoly

        // Return if game play is off ot bot is death
        if (gameManager.GamePlay == false || is_death == true)
        {
          
            return;
        }
   


        if (isOnceInRadius == false)
        {
            // Check if the bot has reached its current destination
            //if (!entity_navAi.pathPending && entity_navAi.remainingDistance <= entity_navAi.stoppingDistance)
            //{
            //    if (!entity_navAi.hasPath || entity_navAi.velocity.sqrMagnitude == 0f)
            //    {
            //        // Switch between the target and start position
            //        if (movingToTarget)
            //        {
            //          //  entity_navAi.SetDestination(starting_pos); // Go back to start
            //        }
            //        else
            //        {
            //         //   entity_navAi.SetDestination(RandomMovePos.position); // Go to the target
            //        }

            //        movingToTarget = !movingToTarget; // Toggle the direction
            //        AnimationController(AnimState.Running);
            //    }
            //}
        }
        if (Enemy == null) // If no enemy is found, move randomly
        {
            if (!isRoaming)
            {
                StartCoroutine(RoamRandomly());
            }
        }
        else
        {
            isRoaming = false; // Stop random roaming when an enemy appears
            StopCoroutine(RoamRandomly());
            // Follow the player and managing the animation
            if (isFollowing && Enemy)
            {
                float distance = Vector3.Distance(Enemy.gameObject.transform.position, this.gameObject.transform.position);
                if (distance > stopDistance)
                {
                    Vector3 directionToPlayer = Enemy.gameObject.transform.position - entity_navAi.transform.position;
                    Vector3 targetPosition = Enemy.gameObject.transform.position - directionToPlayer.normalized * stopDistance;

                    entity_navAi.SetDestination(targetPosition);
                    AnimationController(AnimState.Running);
                }
                else if (distance < stopDistance - 0.5f) // Move backwards if too close (adjust buffer if needed)
                {
                    // Move away from the player
                    Vector3 directionAwayFromPlayer = entity_navAi.transform.position - Enemy.gameObject.transform.position;
                    Vector3 backwardPosition = entity_navAi.transform.position + directionAwayFromPlayer.normalized * 1.5f; // Move back

                    entity_navAi.SetDestination(backwardPosition);
                    AnimationController(AnimState.Running); // Add a backward animation if needed
                }
                else
                {
                    // Stop movement
                    entity_navAi.ResetPath();
                    AnimationController(AnimState.Idle);
                    //entity_audio.Stop();
                }
            }
        }  
        transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
        GetNeartestEnemy();
    }

    public override void GetNeartestEnemy()
    {
        base.GetNeartestEnemy();
        if (Enemy)
        {
            Vector3 targetPosition = new Vector3(Enemy.transform.position.x, transform.position.y, Enemy.transform.position.z);
            transform.LookAt(targetPosition);
            Shoot();
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            isTargeting = true;
            StartFollowing();
        }
       
      
    }
    void AutoTarget()
    {

        if (gameManager.GamePlay == false || is_death)
            return;
        if (gameManager.botAll.Count > 0)
        {
            if (Enemy == null)
            {
                isTargetSelected = false;
            }


            Enemy = null;
            foreach (var item in gameManager.botAll)
            {


                if (Vector3.Distance(this.gameObject.transform.position, item.transform.position) < nearestenemydis &&
                    !item.GetComponent<Entity>().insideGrass &&
                    item.gameObject.activeInHierarchy &&
                    !item.GetComponent<Entity>().is_death &&
                    item != this)
                {
                    nearestenemydis = Vector3.Distance(this.gameObject.transform.position, item.transform.position);
                    Enemy = item;
                }
            }


         
            //player_Shooting.Laser.gameObject.SetActive(true);
           // nearestenemydis = enemyDistance;
        }
        else
        {

            //  player_Shooting.Laser.gameObject.SetActive(false);
            Enemy = null;
            isTargeting = false;
        }
    }

    // On Collide with any object
    void OnCollisionEnter(Collision collision)
    {
        if (gameManager.GamePlay == false || is_death == true)
        {
            return;
        }

      
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (gameManager.GamePlay == false)
        {
            return;
        }

        if (other.GetComponent<Grass>())
        {
            BodyVisibility(false);
            insideGrass = true;
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

        //if (other.gameObject.transform.TryGetComponent<Bullet>(out Bullet bullet))
        //{
        //    if (bullet.entity_holder != null)
        //    {
        //        BulletHitted(bullet);
        //    }
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameManager.GamePlay == false)
        {
            return;
        }

        if (other.GetComponent<Grass>())
        {
            insideGrass = false;
            BodyVisibility(true);
           // BodyVisibility(insideGrass);
        }

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
        if (gameManager.GamePlay == false || is_death == true)
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


    

    //// Player bullet hit
    //void BulletHitted(Bullet bullet)
    //{
    //    ReduceHeath(bullet.damageAmount);
       
    //}

    // Distance check between player and bot
    //void DistanceChecker()
    //{
    //    if (gameManager.GamePlay == false || Player_Manager.is_death)
    //    {
    //        StopFollowing();
    //        return;
    //    }
    //    if (isInRadius)
    //    {
    //        botAnimator.SetBool("Shoot", true);
    //        if (Vector3.Distance(Player_Manager.gameObject.transform.position, this.gameObject.transform.position) > Player_Manager.enemyDistance || Player_Manager.insideGrass == true)// bot is outside the radious
    //        {
    //            isOnceInRadius = false;
                
    //            isInRadius = false;
    //            StopFollowing();
    //        }
           
    //    }
    //    else
    //    {
    //        if (Vector3.Distance(Player_Manager.gameObject.transform.position, this.gameObject.transform.position) <= Player_Manager.enemyDistance && Player_Manager.insideGrass == false)// bot in the radious
    //        {
                
                
              
    //            isInRadius = true;
    //            isOnceInRadius = true;
    //            StartFollowing();
    //        }

    //    }
    //}
    public void OnAnimationEventTriggered() // Method name must match the Animation Event
    {
        Shoot();

        // Your logic (e.g., shaking the health bar)
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
        if (gameManager.GamePlay == false ||
          isInInterval == true ||
          is_death == true )
        {
            return;
        }

        isInInterval = true;
       
        base.Shotting();

        botAnimator.SetBool("Shoot", false);

        // Camera.main.gameObject.GetComponent<Camera_Follower>().Fire();

        // my_wepon.WeaponAudio.Play();

     //   Enemy = null;

        Invoke("ResetShooting",1f); // Adjust timing based on animation length
    }

    void ResetShooting()
    {
        isInInterval = false;
        botAnimator.SetBool("Shoot", false);
    }
    
    // Making bot value defualt
    public override void ResetingGame()
    {
        base.ResetingGame();

        AnimationController(AnimState.Idle);
        StopFollowing();



        // botHealth = botMaxHealth;
        isOnceInRadius = false;

        //  this.transform.position = startingPos;
        this.transform.eulerAngles = startingEular;
        // this.transform.localScale = startingScale;

        AnimatorObject.gameObject.transform.localRotation = Quaternion.identity;

        Player_Manager = gameManager.player;
    }

 
    public override void Death()
    {
        if (!Player_Manager)
        {
            return;
        }

        Player_Manager.Enemy = null;
       // is_death = true;
        ////if (Player_Manager.listEnemy.Contains(this.gameObject))
        ////{
        ////    Player_Manager.listEnemy.Remove(this.gameObject);
        ////    Player_Manager.enemyInRadius--;
        ////}
        
        Player_Manager.killcount++;
        Player_Manager = null;

        //SelectedBot.SetActive(false);

        if (gameManager.botDeath.Contains(this) == false)
        {
            gameManager.botDeath.Add(this);
        }
        gameManager.ShowBlood(transform.position);

        StopFollowing();
        isInRadius = false;
        insideGrass = false;
        base.Death();
        StartCoroutine(DeathPartical());

    }
    [SerializeField] private float roamRadius = 10f; // Radius for random movement
    [SerializeField] private float roamWaitTime = 3f; // Time to wait before picking a new position
    private bool isRoaming = false;
    IEnumerator RoamRandomly()
    {
        isRoaming = true;

        while (Enemy == null) // Keep roaming while no enemy is found
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
            {
                entity_navAi.SetDestination(hit.position);
                AnimationController(AnimState.Running);
            }

            yield return new WaitForSeconds(roamWaitTime);
        }

        isRoaming = false;
    }
    public IEnumerator DeathPartical()
    {

        death_partclesystem.SetActive(true);
        death_partclesystem.GetComponent<ParticleSystem>().Play();
        enity_audio.PlayOneShot(playerDeath);


        yield return new WaitForSeconds(2);
        this.gameObject.SetActive(false);
        BodyVisibility(true);
    }






    // All animation state
    enum AnimState
    {
        Idle,
        Running,
        Turn
    }

}