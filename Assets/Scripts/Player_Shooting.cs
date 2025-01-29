using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Shooting : MonoBehaviour
{
    [Space(10)]
    [Header("Game Manager")]
    public GameManager GameManager; // Access of game manager

    [Space(10)]
    [Header("Player Manager")]
    public Player_Manager PlayerManager; // Accedd of player manager

    [Space(10)]
    [Header("All shooting and reloading variables")]
    [SerializeField] private Transform FirePoint; // Starting point for bullet
    [SerializeField] private float bulletSpeed; // Bullet speed
    [SerializeField] public LineRenderer Laser; // Aim laser
    [SerializeField] private Button FireButton; // Fire button
    [SerializeField] private Image FireReloadingImage; // Fire realoding image
    [SerializeField] private float intervalTime; // Interval timing for next shoot
    [SerializeField] private bool isInInterval; // Find that gun is in interval or not

    [Space(10)]
    [Header("All bullet manage variables")]
    public List<GameObject> bulletAll; // All player bullet
    public List<GameObject> bulletUsed; // All used bullet
    public List<GameObject> bulletUnused; // All unused bullet


    [Space(10)]
    [Header("Damage variables")]
    public int hitDamage = 5; // Bot damage amount

    // Start
    void Start()
    {
        PlayerManager = GetComponent<Player_Manager>();
    }

    // Update
    void Update()
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

        Debug.DrawRay(transform.position, transform.forward * 100, Color.green);
        if (Input.GetKeyDown(KeyCode.K))
        {
            Shoot();
        }
        DrawTrajectory();
    }

    // Bullet shoot
    public void Shoot()
    {
        if (GameManager.GamePlay == false || isInInterval == true || PlayerManager.isDeath == true)
        {
            return;
        }

        GameObject bullet = bulletUnused[0];

        bulletUnused[0].SetActive(true);
        bulletUsed.Add(bulletUnused[0]);
        bulletUnused.Remove(bulletUnused[0]);

        Vector3 direction = transform.forward;

        Rigidbody rb_bullet = bullet.GetComponent<Rigidbody>();
        rb_bullet.linearVelocity = direction * bulletSpeed;
        bullet.GetComponent<Bullet>().bulletPlayer = this.transform.GetComponent<Player_Manager>();
        bullet.GetComponent<Bullet>().damageAmount = hitDamage;
        bullet.transform.parent = null;

        PlayerManager.myWeapon.enabled = true;
        PlayerManager.myWeapon.WeaponAudio.clip = PlayerManager.myWeapon.BlastSound;
        PlayerManager.myWeapon.WeaponAudio.Play();

        if(PlayerManager.player_Movement.playerState == Player_Movement.AnimState.Idle)
        {
            PlayerManager.player_Movement.AnimationController(Player_Movement.AnimState.IdleShoot);
        }

        StartCoroutine(ShootingInterval());
    }

    // Draw laser aim
    public void DrawTrajectory()
    {

        // Calculate the forward direction (XZ plane)
        Vector3 forwardDirection = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        // Starting point of the trajectory
        Vector3 startPoint = transform.position;
        startPoint.y = .75f;
        // Check for collisions using a raycast
        RaycastHit hit;
        Vector3 endPoint;


        if (Physics.Raycast(startPoint, forwardDirection, out hit))
        {
            // Collision detected
            endPoint = hit.point;
        }
        else
        {
            // No collision
            endPoint = startPoint + transform.forward * 100;
        }
        // Apply points to trajectoryLine
        Laser.positionCount = 2; // Start and end points only
        Laser.SetPosition(0, new Vector3(startPoint.x, .75f, startPoint.z)); // Start point
        Laser.SetPosition(1, new Vector3(endPoint.x, .75f, endPoint.z)); // End point
    }

    // Collecting bullet for reseting the player
    public void CollectingBullet()
    {
        for (int i = 0; i < bulletAll.Count; i++)
        {
            bulletAll[i].GetComponent<Bullet>().GoToParent();
        }
        // Deactivate the reload image once the reload is complete
        FireReloadingImage.gameObject.SetActive(false);
        isInInterval = false;
    }

    // Shooting interval
    IEnumerator ShootingInterval()
    {
        isInInterval = true;
        FireButton.interactable = false;
        // Activate the reload image
        FireReloadingImage.gameObject.SetActive(true);

        // Reset the reload image fill amount
        FireReloadingImage.fillAmount = 0f;

        float elapsedTime = 0f;

        // Gradually fill the image over reloadTime
        while (elapsedTime < intervalTime)
        {
            elapsedTime += Time.deltaTime;
            FireReloadingImage.fillAmount = Mathf.Clamp01(elapsedTime / intervalTime);
            yield return null;
        }

        // Deactivate the reload image once the reload is complete
        FireReloadingImage.gameObject.SetActive(false);

        FireButton.interactable = true;
        isInInterval = false;
    }
}
