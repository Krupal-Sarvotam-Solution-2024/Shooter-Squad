using UnityEngine;

public class Weapon : MonoBehaviour
{
    public AudioSource WeaponAudio; // Audiosource for playing sound
    public AudioClip BlastSound; // Audio clip which will play

    private void Start()
    {
        WeaponAudio = GetComponent<AudioSource>(); // Find audio source dynamically
    }
}
