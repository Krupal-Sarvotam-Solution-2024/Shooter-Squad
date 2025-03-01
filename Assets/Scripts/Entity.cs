using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;


public class Entity : MonoBehaviour
{

    [Header("Coponets")]
    public GameManager gameManager;
    public Entity Enemy; // All enemy in that list
    protected Rigidbody entity_rb;
    protected Collider entity_colider;
    protected NavMeshAgent entity_navAi;
    protected Animator entity_animator;
    protected AudioSource enity_audio; // Audio source which handle player audios

    protected float currentHealth { get; private set; }
    protected float nearestenemydis = 1000;
    [Space(10)]
    [Header("Heath Manager")]
    [SerializeField]protected float MaxHealth;
    [SerializeField]private bool Healing;
    public bool is_player;
    public bool is_death {get; private set; }
    protected float killcount;
    public float moveSpeed =5f;
    public float rotationSpeed = 10f;
    public Vector3 startingpostion;

    [Space(10)]
    [Header("Audio manager")]
    [SerializeField]protected AudioClip playerDeath;

    //parts
    [Space(10)]
    [Header("Whole body object manager")]
    [SerializeField] private List<GameObject> body_parts; // All body parts for activation and deactivation
    [SerializeField] protected GameObject death_partclesystem; // Death particle system

    //valus
    public Vector3 starting_pos;
    [SerializeField] private GameObject HealthBarFG; // Health bar on the head image
    [SerializeField] private GameObject Healthbarmain;
    [SerializeField] private TextMeshPro HealthPerText; // Health percantage text
    public bool isTargetSelected; // Check that tarhet is selected or not
    public bool isTargeting; // Check that is finding target or not
    //shooting
    
    public Weapon my_wepon;
    public bool insideGrass;
    public Grass EnteredGrass;
    public Color insidegrass, outsidegrass;
    public float shooting_radious;
    [Header("powerups")]
    public GameObject shildeffect,speedeffect;
    public bool shild;
    #region MonoMethods
    public virtual void Awake()
    {

        enity_audio = GetComponent<AudioSource>();
        entity_rb = GetComponent<Rigidbody>();
        entity_navAi = GetComponent<NavMeshAgent>();
        entity_colider = GetComponent<Collider>();
        entity_animator = GetComponent<Animator>();

    }
    public virtual void Start()
    {
       // starting_pos = transform.position;
        currentHealth = MaxHealth;
        for (int i = 0; i < transform.childCount-1; i++)
        {
            body_parts.Add(transform.GetChild(i).gameObject);
        }
        gameManager.SoundLoad();
    }
    public  virtual void Update()
    {

        insideGrass = EnteredGrass == null ? false : true;

    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (gameManager.GamePlay == false)
        {
            return;
        }
        Debug.Log("Trigering");
        if (other.GetComponent<Grass>())
        {

            insideGrass = true;

        }

        if (other.GetComponent<Weapon>())
        {
            my_wepon.gameObject.SetActive(false);
            my_wepon = my_wepon.gameObject.transform.parent.GetChild(other.GetComponent<Weapon>().id).GetComponent<Weapon>();
            my_wepon.gameObject.SetActive(true);
        }
        //trigering powerups

    }
    #endregion
    public void ReduceHeath(float damage)
    {
        if (shild)
            return;
        currentHealth -= damage;
        GameObject indicator = gameManager.Objectpool.GetFromPool("DamageIndicator", this.transform.position, Quaternion.identity);
        indicator.GetComponent<TextMeshPro>().text = "-" + damage.ToString();
        StartCoroutine(DeactivatingObject(indicator));
        HealthShow();//showing the current Health
        // Show damage indicator
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Death();
        }
    }
    public void ResetingHealth()
    {
        currentHealth = MaxHealth;
        HealthShow();
    }
    public void HealthShow()
    {
        float healthPercentage = (currentHealth / MaxHealth) * 100f;
        if (healthPercentage >= 0)
        {
            HealthBarFG.transform.localScale = new Vector3(healthPercentage / 100, HealthBarFG.transform.localScale.y, HealthBarFG.transform.localScale.z);
        }
        else
        {
            HealthBarFG.transform.localScale = new Vector3(0, HealthBarFG.transform.localScale.y, HealthBarFG.transform.localScale.z);
        }
        HealthPerText.text = healthPercentage.ToString("00") + "%";

    }
    public virtual IEnumerator IncreaseHeath(float value)
    {
        while (Healing)
        {

            if (currentHealth < MaxHealth)
            {
                currentHealth += value;
                Debug.Log("health is increaing");
                HealthShow();
            }

            Debug.Log("coming inside the health");
            yield return new WaitForSeconds(value);
            currentHealth = currentHealth > MaxHealth ? MaxHealth : currentHealth;
        }
    }
    public GameObject dropingwepon;
    public virtual void Death()
    {
        if (Enemy)
        {
            Enemy.Enemy = null;
            Enemy.killcount++;
        }
        //   Enemy.ki++;
        Enemy = null;
        Debug.Log("dead");
        Healthbarmain.SetActive(false);
        is_death = true;
        if (entity_rb) entity_rb.isKinematic = true;
        if (entity_colider) entity_colider.enabled = false;
        if (entity_navAi) entity_navAi.enabled = false;
        BodyVisibility(false);
        dropingwepon.transform.GetChild(my_wepon.id).gameObject.SetActive(true);

        //gameManager.BotCount();
        //  StartCoroutine(DeathPartical());
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3);
        dropingwepon.transform.GetChild(my_wepon.id).gameObject.SetActive(false);
        transform.position = startingpostion;
        if (entity_rb) entity_rb.isKinematic = false;
        if (entity_colider) entity_colider.enabled = true;
        if (entity_navAi) entity_navAi.enabled = true;
        Healthbarmain.SetActive(true);
        BodyVisibility(true);
        is_death = false;
        currentHealth = MaxHealth;
        HealthShow();

    }


    public virtual void ResetingGame()
    {
        //Restart the game
        is_death = false;
        currentHealth = MaxHealth;
        Healthbarmain.SetActive(true);
        HealthShow();
        transform.rotation = Quaternion.identity;
        StopAllCoroutines();
        CancelInvoke();
        BodyVisibility(true);
        my_wepon.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
        if(entity_rb)entity_rb.isKinematic = false;
        if(entity_colider)entity_colider.enabled = true;
        if(entity_navAi)entity_navAi.enabled = true;
        this.transform.position = starting_pos;
        transform.rotation = Quaternion.identity;//making 000
        StartCoroutine(IncreaseHeath(1));
    }

    // Change a visibility of the body
    public virtual void BodyVisibility(bool visibility)
    {
        for (int i = 0; i < body_parts.Count; i++)
        {
            body_parts[i].gameObject.SetActive(visibility);
        }
    }

    public virtual void BodyVisibility(Color color)
    {
        for (int i = 0; i < body_parts.Count; i++)
        {
            body_parts[i].GetComponentInChildren<MeshRenderer>().material.color = color;
        }
    }

    IEnumerator DeactivatingObject(GameObject objects,float time =1.5f)
    {
        yield return new WaitForSeconds(time);
        gameManager.Objectpool.ReturnToPool("DamageIndicator", objects);
    }

    //shooting

    public virtual void Shotting()
    {
        if (gameManager.GamePlay == false ||is_death)
        {
            return;
        }
        Debug.Log("Shooting" +this.gameObject.name);
        for (int i = 0; i < my_wepon.FirePoints.Count; i++)
        {

            //.LookAt(Enemy.transform.position);
            if (Enemy)
            {
                Vector3 targetPosition = new Vector3(Enemy.transform.position.x, transform.position.y, Enemy.transform.position.z);
                my_wepon.FirePoints[i].LookAt(targetPosition);
                Bullet spawnedBullets = gameManager.Objectpool.GetFromPool(my_wepon.bullets.name, my_wepon.FirePoints[i].transform.position, Quaternion.Euler(0, my_wepon.FirePoints[i].transform.eulerAngles.y, 0)).GetComponent<Bullet>();
                spawnedBullets.entity_holder = this;
                spawnedBullets.BulletFire();

                my_wepon.WeaponAudio.PlayOneShot(my_wepon.BlastSound);
            }
        }

    }

    public void HealthBarShake()
    {
        StartCoroutine(ShakeHealthBar());
    }

    private IEnumerator ShakeHealthBar()
    {
        Vector3 originalPos = HealthBarFG.transform.localPosition;

        float duration = 0.5f;
        float elapsed = 0f;
        float magnitude = 5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Random.Range(-magnitude, magnitude);
            float offsetY = Random.Range(-magnitude, magnitude);

            HealthBarFG.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }

        HealthBarFG.transform.localPosition = originalPos;
    }
    public IEnumerator RemovingBullets(GameObject shootingpartical)
    {
        yield return new WaitForSeconds(.5f);
        gameManager.Objectpool.ReturnToPool("ShootingPartical", shootingpartical);
        

    }
    #region Powerups
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
    bool speedbosting;
    public IEnumerator SpeedBost()
    {
        if (!speedbosting)
        {
            speedbosting = true;

            float tempspeed = 0;
            tempspeed = moveSpeed;
            speedeffect.SetActive(true);
            moveSpeed *= 2;
            yield return new WaitForSeconds(4);
            speedeffect.SetActive(false);
            moveSpeed = tempspeed;
            speedbosting = false;
        }
        else
        {

            yield return new WaitForSeconds(.1f);
           
        }
       
    }
    #endregion
    public virtual void GetNeartestEnemy()
    {
        Enemy = null;
        foreach (var item in gameManager.botAll)
        {
            if(!item.is_death && item != this && item.gameObject.activeInHierarchy )
            {
                float distace = Vector3.Distance(this.transform.position, item.transform.position);
            
                if (distace < nearestenemydis && distace < shooting_radious)
                {
                    nearestenemydis = distace;
                    Enemy = item;
                }

            }

        }
        if(Enemy == null)
        {
            nearestenemydis = shooting_radious;
          
        }
    }
}
