using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    //heath 
    [Space(10)]
    [Header("Heath Manager")]
    private float currentHealth;
    [SerializeField]protected float MaxHealth;
    private bool is_death;
    public AudioClip playerDeath;


    //physics
    protected Rigidbody entity_rb;
    protected Collider entity_colider;
    protected NavMeshAgent entity_navAi;


    [Space(10)]
    [Header("Audio manager")]
    protected AudioSource enity_audio; // Audio source which handle player audios

    //parts
    [Space(10)]
    [Header("Whole body object manager")]
    [SerializeField] private List<GameObject> body_parts; // All body parts for activation and deactivation
    [SerializeField] private GameObject death_partclesystem; // Death particle system

    //valus

    public virtual void Start()
    {
        currentHealth = MaxHealth;
        enity_audio = GetComponent<AudioSource>();
        entity_rb = GetComponent<Rigidbody>();
        entity_navAi = GetComponent<NavMeshAgent>();
        entity_colider = GetComponent<Collider>();

        for (int i = 0; i < transform.childCount-1; i++)
        {

            body_parts.Add(transform.GetChild(i).gameObject);
        }
    }

    public void ReduceHeath(float damage)
    {
        currentHealth -= damage;


        if (currentHealth <= 0)
        {
            currentHealth = 0;
            // death
            StartCoroutine(Death());

        }
    }

    public void IncreaseHeath(float value)
    {
        if(currentHealth < MaxHealth)
        {
            currentHealth += value;
        }

        currentHealth = currentHealth > MaxHealth ? MaxHealth : currentHealth;
    }

    public virtual IEnumerator Death()
    {
        //entity is dead
        is_death = true;
        if (entity_rb) entity_rb.isKinematic = true;
        if (entity_colider) entity_colider.enabled = false;
        if (entity_navAi) entity_navAi.enabled = false;
        BodyVisibility(false);
        death_partclesystem.SetActive(true);
        death_partclesystem.GetComponent<ParticleSystem>().Play();
        enity_audio.PlayOneShot(playerDeath);

        yield return new WaitForSeconds(2);
        this.gameObject.SetActive(false);
        BodyVisibility(true);
    }


    public virtual void ResetingGame()
    {
        //Restart the game
   


    }

    // Change a visibility of the body
    public virtual void BodyVisibility(bool visibility)
    {
        for (int i = 0; i < body_parts.Count; i++)
        {
            body_parts[i].gameObject.SetActive(visibility);
        }
    }

    // Collecting the all used bullet
    void CollectingBullet()
    {
        //for (int i = 0; i < bulletAll.Count; i++)
        //{
        //    bulletAll[i].GetComponent<Bullet>().GoToParent();
        //}
    }

}
