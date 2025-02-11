using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    public AudioSource WeaponAudio; // Audiosource for playing sound
    public AudioClip BlastSound; // Audio clip which will play
    public List<Transform> FirePoints;
    public int Damage = 5;
    public float range;
    public GameObject showingrange;
    public bool isPlayMultiTime;

    private void Start()
    {
        WeaponAudio = GetComponent<AudioSource>(); // Find audio source dynamically
    }
}
