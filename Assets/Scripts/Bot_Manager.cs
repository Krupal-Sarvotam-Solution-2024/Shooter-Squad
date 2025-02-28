using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Bot_Manager : Entity
{


    [Space(10)]
    [Header("Player following variables")]
   
    public float stopDistance = 2.0f; // Distance to stop away from the player


    [Space(10)]
    [Header("Animation manager variables")]
    private bool isIdle = false; // Find that bot is idle or not
    public GameObject AnimatorObject; // Animator occupied game object

    [Space(10)]
    [Header("Nav mesh manager")]
    private bool isFollowing = false; // Find that bot is following the player or not
    public Transform RandomMovePos;
    [SerializeField] private bool isInInterval; // Find that gun is in interval or not




    [Header("shild")]
    public GameObject shildeffect;
    public bool shild;
    public float acceleration;
    private Vector3 movementDirection;
    public float horizotalinput = 0;
    public float verticalinput = 0;
    // Update

    public override void Start()
    {
        base.Start();
        StartCoroutine(RandomMovement());
    }
    void Update()
    {
      //  HealthBar.transform.LookAt(Camera.main.transform.position); // Healthbar saw camera continusoly

        // Return if game play is off ot bot is death
        if (gameManager.GamePlay == false || is_death == true)
        {
          
            return;
        }

        if (Enemy == null) // If no enemy is found, move randomly
        {
            if (!isRoaming)
            {
                Vector3 targetDirection = new Vector3(horizotalinput, 0,verticalinput).normalized;

                movementDirection = Vector3.Lerp(movementDirection, targetDirection, Time.deltaTime * acceleration);

                if (movementDirection.magnitude < 0.1f)
                {
                    movementDirection = Vector3.zero;

                }
                else
                {
                    //if (entity_rb.linearVelocity != Vector3.zero)
                    //{
                    // //   walksoundtime = 0;
                    //  //  player.playerAudio.PlayOneShot(player.runSurface);

                    //}
                    //walksoundtime += Time.deltaTime;

                }
                //  StartCoroutine(RoamRandomly());
            }
        }
        else
        {
            isRoaming = false; // Stop random roaming when an enemy appears
            // StopCoroutine(RoamRandomly());
            // Follow the player and managing the animation
            //if (isFollowing && Enemy)
            //{
            //    float distance = Vector3.Distance(Enemy.gameObject.transform.position, this.gameObject.transform.position);
            //    if (distance > stopDistance)
            //    {
            //        Vector3 directionToPlayer = Enemy.gameObject.transform.position - entity_navAi.transform.position;
            //        Vector3 targetPosition = Enemy.gameObject.transform.position - directionToPlayer.normalized * stopDistance;

            //        entity_navAi.SetDestination(targetPosition);
            //        AnimationController(AnimState.Running);
            //    }
            //    else if (distance < stopDistance - 0.5f) // Move backwards if too close (adjust buffer if needed)
            //    {
            //        // Move away from the player
            //        Vector3 directionAwayFromPlayer = entity_navAi.transform.position - Enemy.gameObject.transform.position;
            //        Vector3 backwardPosition = entity_navAi.transform.position + directionAwayFromPlayer.normalized * 1.5f; // Move back

            //        entity_navAi.SetDestination(backwardPosition);
            //        AnimationController(AnimState.Running); // Add a backward animation if needed
            //    }
            //    else
            //    {
            //        // Stop movement
            //        entity_navAi.ResetPath();
            //        AnimationController(AnimState.Idle);
            //        //entity_audio.Stop();
            //    }
            //}
        }  
        transform.eulerAngles = new Vector3 (0, transform.eulerAngles.y, 0);
        GetNeartestEnemy();
    }
    void FixedUpdate()
    {
        if (!gameManager.GamePlay || is_death)
        {
            movementDirection = Vector3.zero;
            return;
        }

     
        MoveWithRigidbody();
         
    }

    void MoveWithRigidbody()
    {
        Vector3 targetVelocity = movementDirection * 7;
        entity_rb.linearVelocity = Vector3.Lerp(entity_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
        if(!Enemy)
        RotatePlayer();
    }

   
    void RotatePlayer()
    {
        if (movementDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 90);
        }
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

    private void OnTriggerStay(Collider other)
    {
        if (gameManager.GamePlay == false)
        {
            return;
        }

        if (other.GetComponent<Grass>())
        {
            BodyVisibility(false);
            insideGrass = true;
            
        }
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
        if (gameManager.GamePlay == false || is_death == true)
        {
            return;
        }

        // Calculate the direction from the bot to the player
        Vector3 directionToPlayer = (Enemy.gameObject.transform.position - transform.position).normalized;

        // Calculate the target position some distance away from the player
        Vector3 targetPosition = Enemy.gameObject.transform.position - directionToPlayer * 2f;

        // Set the NavMeshAgent's destination
        entity_navAi.SetDestination(targetPosition);

        // Check the distance between the bot and the player
        float distanceToPlayer = Vector3.Distance(transform.position, Enemy.transform.position);

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
                entity_animator.SetBool("Idle", true);
                entity_animator.SetBool("Running", false);
                entity_animator.SetBool("Turn", false);
                break;
            case AnimState.Running:
                entity_animator.SetBool("Idle", false);
                entity_animator.SetBool("Running", true);
                entity_animator.SetBool("Turn", false);
                break;
            case AnimState.Turn:
                entity_animator.SetBool("Idle", false);
                entity_animator.SetBool("Running", false);
                entity_animator.SetBool("Turn", true);
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

        entity_animator.SetBool("Shoot", false);

        // Camera.main.gameObject.GetComponent<Camera_Follower>().Fire();

        // my_wepon.WeaponAudio.Play();

     //   Enemy = null;

        Invoke("ResetShooting",1f); // Adjust timing based on animation length
    }

    void ResetShooting()
    {
        isInInterval = false;
        entity_animator.SetBool("Shoot", false);
    }
    IEnumerator RandomMovement()
    {
        while (true)
        {
            // Generate random movement input values between -1 and 1
            horizotalinput = Random.Range(-1f, 1f);
            verticalinput = Random.Range(-1f, 1f);

            // Normalize the movement direction to ensure consistent speed
            Vector3 targetDirection = new Vector3(horizotalinput, 0, verticalinput).normalized;

            // Apply movement
            movementDirection = targetDirection;

            // Wait for a few seconds before changing direction
            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        }
    }

    // Making bot value defualt
    public override void ResetingGame()
    {
        base.ResetingGame();

        AnimationController(AnimState.Idle);
        StopFollowing();



        // botHealth = botMaxHealth;
       // isOnceInRadius = false;

        //  this.transform.position = startingPos;
     
        // this.transform.localScale = startingScale;

        AnimatorObject.gameObject.transform.localRotation = Quaternion.identity;

        //Player_Manager = gameManager.player;
    }

 
    public override void Death()
    {
       
      
        //SelectedBot.SetActive(false);

        //if (gameManager.botDeath.Contains(this) == false)
        //{
        //    gameManager.botDeath.Add(this);
        //}
        gameManager.ShowBlood(transform.position);

        StopFollowing();
        insideGrass = false;
        base.Death();
        StartCoroutine(DeathPartical());

    }
    [SerializeField] private float roamRadius = 10f; // Radius for random movement
    [SerializeField] private float roamWaitTime = 3f; // Time to wait before picking a new position
    private bool isRoaming = false;
   
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