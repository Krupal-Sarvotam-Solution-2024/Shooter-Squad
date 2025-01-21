using UnityEngine;

public class Weapon : MonoBehaviour
{
    public AudioSource WeaponAudio;
    public AudioClip BlastSound;

    private void Start()
    {
        WeaponAudio = GetComponent<AudioSource>();
    }
}
