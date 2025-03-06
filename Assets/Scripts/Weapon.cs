
using UnityEngine;
using System.Collections.Generic;

public class Weapon : MonoBehaviour
{
    /// <summary>
    /// Unique identifier for this weapon
    /// </summary>
    public int id = 0;

    /// <summary>
    /// Audio source component for weapon sound effects
    /// </summary>
    public AudioSource WeaponAudio;

    [SerializeField]
    private AudioClip blastSound;

    public List<Transform> FirePoints = new List<Transform>();

    [SerializeField]
    private GameObject rangeIndicator;

    [SerializeField]
    private bool canPlayMultipleTimes;

    public GameObject bullets;
    public Entity enity;
    [SerializeField]
    private GameManager gameManager;
    public float firerate;
    private void Awake()
    {
        if (gameManager == null || bullets == null)
        {
            Debug.LogError($"{gameObject.name}: Required references missing");
            enabled = false;
            return;
        }
       // FirePoints.Add(transform.GetChild(0));
        Debug.Log($"{gameObject.name} initialized");
     //   weaponAudio = GetComponent<AudioSource>();

        //if (weaponAudio == null)
        //{
        //    Debug.LogWarning($"{gameObject.name}: No AudioSource found");
        //}

        gameManager.Objectpool.CreatePool(bullets.name, bullets, 30, gameManager.BulletsHolder);
    }
    private void Update()
    {
        if (enity && enity.Enemy)
        {
            for (int i = 0; i < FirePoints.Count; i++)
            {
                Vector3 targetPosition = new Vector3(enity.Enemy.transform.position.x, transform.position.y, enity.Enemy.transform.position.z)+Vector3.forward;
                FirePoints[i].LookAt(targetPosition);
            }
          
        }
    }
}