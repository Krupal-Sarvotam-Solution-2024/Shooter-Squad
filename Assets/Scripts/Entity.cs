using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

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
    public float killcount;
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
    public GameObject shildeffect,speedeffect,passthroughEffect;
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
    

        if (other.GetComponent<Weapon>())
        {
            my_wepon.gameObject.SetActive(false);
            my_wepon = my_wepon.gameObject.transform.parent.GetChild(other.GetComponent<Weapon>().id).GetComponent<Weapon>();
            my_wepon.enity = this;
            my_wepon.gameObject.SetActive(true);
        }

        if (other.GetComponent<Powerups>() && other.GetComponent<Reactivate>()|| other.GetComponent<Weapon>() && other.GetComponent<Reactivate>())
        {
            StartCoroutine(other.GetComponent<Reactivate>().reacrivate());
        }
        //trigering powerups

    }
    #endregion
    public void ReduceHeath(float damage,Entity gethitfrom)
    {
        if (shild)
            return;
        currentHealth -= damage;
        GameObject indicator = gameManager.Objectpool.GetFromPool("DamageIndicator", this.transform.position, Quaternion.identity);
        indicator.GetComponent<TextMeshPro>().text = "-" + damage.ToString();
        StartCoroutine(DeactivatingObject(indicator));
        HealthShow();//showing the current Health
        // Show damage indicator
        if (currentHealth <= 0 )
        {
            if(gethitfrom)
            gethitfrom.killcount += 1;
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
              
                HealthShow();
            }

 
            yield return new WaitForSeconds(value);
            currentHealth = currentHealth > MaxHealth ? MaxHealth : currentHealth;
        }
    }
    public GameObject dropingwepon;
    public virtual void Death()
    {

        //   Enemy.ki++;
        gameManager.BotCount();
        Enemy = null;
        Debug.Log("dead");
        Healthbarmain.SetActive(false);
        is_death = true;
        if (entity_rb) entity_rb.isKinematic = true;
        if (entity_colider) entity_colider.enabled = false;
        if (entity_navAi) entity_navAi.enabled = false;
        BodyVisibility(false);
        dropingwepon.gameObject.SetActive(true);
        dropingwepon.transform.GetChild(my_wepon.id).gameObject.SetActive(true);

        //gameManager.BotCount();
        //  StartCoroutine(DeathPartical());
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3);
        dropingwepon.transform.GetChild(my_wepon.id).gameObject.SetActive(false);
        transform.position =starting_pos;
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
    public bool isInInterval;
    public virtual void Shotting()
    {
       
    
        for (int i = 0; i < my_wepon.FirePoints.Count; i++)
        {

            //.LookAt(Enemy.transform.position);
           
           
            Debug.Log("fireing");
            Bullet spawnedBullets = gameManager.Objectpool.GetFromPool(my_wepon.bullets.name, my_wepon.FirePoints[i].transform.position, Quaternion.Euler(0, my_wepon.FirePoints[i].transform.eulerAngles.y, 0)).GetComponent<Bullet>();
            spawnedBullets.entity_holder = this;
           // spawnedBullets.BulletFire();
          //  isInInterval = false;

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

    public IEnumerator Invisible()
    {
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Collider>().isTrigger=true ;
        passthroughEffect.SetActive(true);
        yield return new WaitForSeconds(4);
        passthroughEffect.SetActive(false);
        GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().useGravity = true;
    }
    public SkinnedMeshRenderer[] allmaterial;
    public Material mat;
    public IEnumerator Hide()
    {
        insideGrass = true;
        for (int i = 0; i < allmaterial.Length; i++)
        {
            SetTransparent(allmaterial[i].material);

            // SetMaterialTransparent(allmaterial[i].material);
            //    yield return StartCoroutine(FadeToTransparent(allmaterial[i].material, 3));
            //need to make it trasparent
      
        }
            yield return new WaitForSeconds(10);

        insideGrass = false;
        for (int i = 0; i < allmaterial.Length; i++)
        {
           

            // SetMaterialTransparent(allmaterial[i].material);
            //    yield return StartCoroutine(FadeToTransparent(allmaterial[i].material, 3));
         
            SetOpaque(allmaterial[i].material);
        }
    }
    private IEnumerator FadeToTransparent(Material mat, float duration)
    {
        Color color = mat.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, time / duration);
            mat.color = color;
            yield return null;
        }
    }
    void SetTransparent(Material targetMaterial)
    {
        targetMaterial.SetFloat("_Surface", 1); // Transparent
        targetMaterial.renderQueue = (int)RenderQueue.Transparent; // Move to Transparent queue
        targetMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        targetMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        targetMaterial.SetFloat("_ZWrite", 0); // Disable ZWrite for proper transparency
        targetMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        
    }

    void SetOpaque(Material targetMaterial)
    {
        targetMaterial.SetFloat("_Surface", 0); // Opaque
        targetMaterial.renderQueue = (int)RenderQueue.Geometry; // Move to Opaque queue
        targetMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
        targetMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
        targetMaterial.SetFloat("_ZWrite", 1); // Enable ZWrite
        targetMaterial.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
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
            
                if (distace < nearestenemydis && distace < shooting_radious && !item.insideGrass)
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
