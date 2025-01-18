using System.Collections;
using System.Runtime.InteropServices;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Player_Manager bulletPlayer;
    public Bot_Manager bulletBot;
    public GameObject Parent;
    [SerializeField] private Vector3 previousScale;
    [SerializeField] private Vector3 previousPosition;

    private void OnEnable()
    {
        //Destroy(gameObject,5f);
        StartCoroutine(OffBullet());
    }

    IEnumerator OffBullet()
    {
        yield return new WaitForSeconds(5f);
        GoToParent();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Destroy(gameObject);
        if (bulletPlayer != null && other.transform.name.Contains("Player") == false)
        {
            GoToParent();
        }

        else
        {
            GoToParent();
        }

    }

    public void GoToParent()
    {
        this.transform.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        this.gameObject.SetActive(false);
        this.transform.parent = Parent.transform;
        this.transform.localPosition = previousPosition;
        this.transform.localScale = previousScale;


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
}
