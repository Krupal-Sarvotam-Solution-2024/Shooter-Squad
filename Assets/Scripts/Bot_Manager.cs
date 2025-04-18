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





    [Header("shild")]
    //public GameObject shildeffect;
    //public bool shild;
    public float acceleration;
    private Vector3 movementDirection;
    public float horizotalinput = 0;
    public float verticalinput = 0;
    // Update
    public Vector3 minimum, maximum;

    public override void Start()
    {
        base.Start();
       
       
    }
    public Transform BOTMOVELOCATION;
    void Update()
    {
      //  HealthBar.transform.LookAt(Camera.main.transform.position); // Healthbar saw camera continusoly

        // Return if game play is off ot bot is death
        if (gameManager.GamePlay == false || is_death == true)
        {
          
            return;
        }
        Vector3 targetDirection = new Vector3(horizotalinput, 0, verticalinput).normalized;
        minimum.x += Time.deltaTime;
        minimum.z += Time.deltaTime;
        maximum.z -= Time.deltaTime;
        maximum.x -= Time.deltaTime;
        movementDirection = Vector3.Lerp(movementDirection, targetDirection, Time.deltaTime * acceleration);
        AnimationController(AnimState.RunningForward);
        entity_navAi.SetDestination(BOTMOVELOCATION.position);
        if (movementDirection.magnitude < 0.1f)
        {
            movementDirection = Vector3.zero;

        }
       
     //  UpdateAnimation();
        GetNeartestEnemy();
    }
    void FixedUpdate()
    {
        if (!gameManager.GamePlay || is_death)
        {
            movementDirection = Vector3.zero;
            return;
        }

        
       // MoveWithRigidbody();
         
    }
    bool interval;
    void MoveWithRigidbody()
    {
        Vector3 targetVelocity = movementDirection * 7;
        entity_rb.linearVelocity = Vector3.Lerp(entity_rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * acceleration);
        if(!Enemy)
        RotatePlayer();
        RaycastHit hit;
        if(Physics.SphereCast(transform.position,1f,transform.forward,out hit))
        {
          
            if (hit.transform.name.Contains("Wall")|| hit.transform.name.Contains("water") || hit.transform.name.Contains("Box") || hit.transform.name.Contains("Bamboo")
                || hit.transform.name.Contains("Barrel"))
            {

                // Generate random movement input values between -1 and 1
                horizotalinput = Random.Range(-1f, 1f);
                verticalinput = Random.Range(-1f, 1f);

                // Normalize the movement direction to ensure consistent speed
                Vector3 targetDirection = new Vector3(horizotalinput, 0, verticalinput).normalized;

                // Apply movement
                movementDirection = targetDirection;
            }
        }

        //if(transform.position.x < minimum.x || transform.position.z< minimum.z || transform.position.x>maximum.x || transform.position.z > maximum.z)
        //{
            if (!interval)
            {
                interval = true;
                StartCoroutine(changeDirection());
            }
        //}
    }
    IEnumerator changeDirection()
    {
        interval = true;
        horizotalinput = Random.Range(-1f, 1f);
        verticalinput = Random.Range(-1f, 1f);

        // Normalize the movement direction to ensure consistent speed
        Vector3 targetDirection = new Vector3(horizotalinput, 0, verticalinput).normalized;

        // Apply movement
        movementDirection = targetDirection;
        yield return new WaitForSeconds(1);
        interval = false;
    }
    void UpdateAnimation()
    {
        if (!Enemy)
        {

            if (movementDirection.magnitude > 0.1f)
            {
                AnimationController(AnimState.RunningForward);
            }
            else
            {
                AnimationController(AnimState.Idle);
            }
        }
        else
        {
            Vector3 relativeMovement = transform.InverseTransformDirection(movementDirection);

            if (relativeMovement.z > 0.1f)      // Moving forward relative to target
                AnimationController(AnimState.RunningForward);
            else if (relativeMovement.z < -0.1f)// Moving backward relative to target
                AnimationController(AnimState.RunningBackward);
            else if (relativeMovement.x > 0.1f) // Moving right relative to target
                AnimationController(AnimState.RunningRight);
            else if (relativeMovement.x < -0.1f)// Moving left relative to target
                AnimationController(AnimState.RunningLeft);
            else
                AnimationController(AnimState.Idle);
        }
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
                    item.gameObject.activeInHierarchy &&
                    !item.GetComponent<Entity>().is_death &&
                    item != this && !item.insideGrass)
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

 
    private void OnTriggerStay(Collider other)
    {
        if (gameManager.GamePlay == false)
        {
            return;
        }

        if (other.GetComponent<Grass>())
        {
           // BodyVisibility(false);
       //     insideGrass = true;
            
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
          //  insideGrass = false;
           // BodyVisibility(true);
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

    
    public void OnAnimationEventTriggered() // Method name must match the Animation Event
    {
        Debug.Log("Event Trigered from playermanager1 time");
        Shotting();

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
            case AnimState.RunningForward:
                entity_animator.SetBool("Idle", false);
                entity_animator.SetBool("Running", true);
                entity_animator.SetBool("Turn", false);
                break;
     
        }
    }


    // Start follow to player
    public void StartFollowing()
    {
        isFollowing = true;
        AnimationController(AnimState.RunningForward);
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
       
       // base.Shotting();

        entity_animator.SetBool("Shoot", true);
        entity_animator.SetFloat("Shooting Speed", my_wepon.firerate *10);
        Invoke("ResetShooting", .1f);
        // Adjust timing based on animation length
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
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }

    // Making bot value defualt
    public override void ResetingGame()
    {
        base.ResetingGame();
        isInInterval = false;
        AnimationController(AnimState.Idle);
        StopFollowing();
        AnimatorObject.gameObject.transform.localRotation = Quaternion.identity;
        StartCoroutine(RandomMovement());
        //Player_Manager = gameManager.player;
    }

 
    public override void Death()
    {
       

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
        death_partclesystem.SetActive(false);
        //this.gameObject.SetActive(false);
        //BodyVisibility(true);
    }

}