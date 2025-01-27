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

    // Called on activation of object
    void OnEnable()
    {
        StartCoroutine(OffBullet());
    }

    // Turning off bullet if no object collide
    IEnumerator OffBullet()
    {
        yield return new WaitForSeconds(5f);
        GoToParent();
    }

    // On Collide with any object
    void OnCollisionEnter(Collision collision)
    {
        this.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        if (bulletPlayer != null && collision.transform.name.Contains("Player") == true)
        {
            // Player Shoot
            StartCoroutine(GoParentAfterParticle(playerHitParicle));
        }
        else if (bulletBot != null && collision.transform.name.Contains("Player") == false)
        {
            // Bot Shoot
            StartCoroutine(GoParentAfterParticle(playerHitParicle));
        }
        else
        {
            // else
            StartCoroutine(GoParentAfterParticle(wallHitParticle));
        }
    }

    // Go to parent timing over of bullet
    public void GoToParent()
    {
        this.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        this.gameObject.SetActive(false);
        this.transform.parent = Parent.transform;
        this.transform.localPosition = previousPosition;
        this.transform.localScale = previousScale;
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
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        this.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        particleType.Play();
        yield return new WaitForSeconds(2f);
        GoToParent();
    }
}