using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public int id;
    public AudioSource WeaponAudio; // Audiosource for playing sound
    public AudioClip BlastSound; // Audio clip which will play
    public List<Transform> FirePoints;
    public GameObject showingrange;
    public bool isPlayMultiTime;
    public GameObject bullets;
    public GameManager gameManager;
    private void Awake()
    {
        Debug.Log(gameObject.name);
        WeaponAudio = GetComponent<AudioSource>(); // Find audio source dynamically
        gameManager.Objectpool.CreatePool(bullets.name, bullets, 30,gameManager.BulletsHolder);
    }
}
