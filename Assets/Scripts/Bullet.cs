using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Space(10)]
    [Header("Player manager")]
    public Player_Manager bulletPlayer; // Shooter player

    [Space(10)]
    [Header("Bot manager")]
    public Bot_Manager bulletBot; // Shooter bot

    [Space(10)]
    [Header("Bullet parent")]
    public GameObject Parent; // Bullet parent

    [Space(10)]
    [Header("Deafult value")]
    public Vector3 previousScale; // Starting scale
    public Vector3 previousPosition; // Starting position

    [Space(10)]
    [Header("Paricle systems")]
    [SerializeField] private ParticleSystem wallHitParticle; // Particle for playing when hit wall
    [SerializeField] private ParticleSystem playerHitParicle; // Particle for playing when hit player/bot

    [Space(10)]
    [Header("Damage for player/bot")]
    public int damageAmount; // Damage to player or bot

    public AudioSource hitaudio;
    public AudioClip obsticlehit, playerhit;
    bool ended;
    // Called on activation of object
    void OnEnable()
    {
        ended = false;
        StartCoroutine(OffBullet());
    }
    private void Update()
    {
        //distace check from player 
        //if (bulletPlayer)
        //{
        //    float distace = Vector3.Distance(bulletPlayer.transform.position, transform.position);
        //    if (distace > bulletPlayer.myWeapon.range && !ended)
        //    {
        //        ended = true;
        //        this.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        //        //     ContactPoint points = collision.contacts[0];
        //        Vector3 pos = transform.position;
        //        //   Debug.Log(pos);
        //        playerHitParicle.transform.parent = null;
        //        playerHitParicle.transform.position = pos;
        //        wallHitParticle.transform.parent = null;
        //        wallHitParticle.transform.position = pos;
        //        StartCoroutine(GoParentAfterParticle(wallHitParticle));
        //    }
        //}


    }
    // Turning off bullet if no object collide
    IEnumerator OffBullet()
    {
        yield return new WaitForSeconds(5f);
        GoToParent();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject == Parent.gameObject || collision.transform.GetComponent<Bullet>() || collision.gameObject.name == "Magic circle")
            return;
        this.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        
        Vector3 pos = collision.gameObject.transform.position;
       //   Debug.Log(pos);
        playerHitParicle.transform.parent = null;
        playerHitParicle.transform.position = pos;
        wallHitParticle.transform.parent = null;
        wallHitParticle.transform.position = pos;

        Debug.Log(collision.gameObject.name);
        /*if (bulletPlayer == null && collision.transform.name.Contains("Player") == true)
        {
            // Player Shoot
            StartCoroutine(GoParentAfterParticle(playerHitParicle));
        }
        else if (bulletBot == null && collision.transform.name.Contains("Bot") == false)
        {
            // Bot Shoot
            StartCoroutine(GoParentAfterParticle(playerHitParicle));
        }*/
        if (collision.transform.name.Contains("Player") || collision.transform.name.Contains("Bot"))
        {
            hitaudio.clip = playerhit;
            StartCoroutine(GoParentAfterParticle(playerHitParicle));
        }
        else
        {
            // else
            hitaudio.clip = obsticlehit;
            StartCoroutine(GoParentAfterParticle(wallHitParticle));
        }
    }
    // On Collide with any object
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject == Parent.gameObject || collision.transform.GetComponent<Bullet>())
            return;
        this.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        ContactPoint points = collision.contacts[0];
        Vector3 pos = points.point;
     //   Debug.Log(pos);
        playerHitParicle.transform.parent = null;
        playerHitParicle.transform.position = pos;
        wallHitParticle.transform.parent = null;
        wallHitParticle.transform.position = pos;

        Debug.Log(collision.gameObject.name);
        /*if (bulletPlayer == null && collision.transform.name.Contains("Player") == true)
        {
            // Player Shoot
            StartCoroutine(GoParentAfterParticle(playerHitParicle));
        }
        else if (bulletBot == null && collision.transform.name.Contains("Bot") == false)
        {
            // Bot Shoot
            StartCoroutine(GoParentAfterParticle(playerHitParicle));
        }*/
        if (collision.transform.name.Contains("Player") || collision.transform.name.Contains("Bot"))
        {
            hitaudio.clip = playerhit;
            StartCoroutine(GoParentAfterParticle(playerHitParicle));
        }
        else
        {
            // else
            hitaudio.clip = obsticlehit;
            StartCoroutine(GoParentAfterParticle(wallHitParticle));
        }
    }

    // Go to parent timing over of bullet
    public void GoToParent()
    {
        this.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        this.gameObject.SetActive(false);
        this.transform.parent = Parent.transform;
        this.transform.position = Vector3.zero;
       // this.transform.localPosition = previousPosition;
       // this.transform.localScale = previousScale;
        //this.transform.eulerAngles = Vector3.zero;
       // this.transform.localEulerAngles = Vector3.zero;

        wallHitParticle.transform.parent = this.transform;
        playerHitParicle.transform.parent = this.transform;

        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        if (bulletPlayer != null)
        {
            Player_Shooting player_Shooting = bulletPlayer.gameObject.transform.GetComponent<Player_Shooting>();

            if (player_Shooting.bulletUsed.Contains(this.gameObject))
            {
                player_Shooting.bulletUsed.Remove(this.gameObject);
            }

            if (player_Shooting.bulletUnused.Contains(this.gameObject) == false)
            {
                player_Shooting.bulletUnused.Add(this.gameObject);
            }

        }

        if (bulletBot != null)
        {

            if (bulletBot.bulletUsed.Contains(this.gameObject))
            {
                bulletBot.bulletUsed.Remove(this.gameObject);
            }

            if (bulletBot.bulletUnused.Contains(this.gameObject) == false)
            {
                bulletBot.bulletUnused.Add(this.gameObject);
            }
        }
    }

    // Playing particle when hit anything
    IEnumerator GoParentAfterParticle(ParticleSystem particleType)
    {
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        hitaudio.Play();
        particleType.Play();
        yield return new WaitForSeconds(2f);
        GoToParent();
    }
}