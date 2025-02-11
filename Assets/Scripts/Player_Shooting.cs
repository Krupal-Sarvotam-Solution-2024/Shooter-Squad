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
    public FixedJoystick Shottingjoystick;

    [Space(10)]
    [Header("All shooting and reloading variables")]
    [SerializeField] private Transform FirePoint; // Starting point for bullet
    [SerializeField] private float bulletSpeed; // Bullet speed
    [SerializeField] public LineRenderer Laser; // Aim laser
    [SerializeField] private Button FireButton; // Fire button
    [SerializeField] private Image FireReloadingImage; // Fire realoding image
    [SerializeField] private float intervalTime; // Interval timing for next shoot
    [SerializeField] private bool isInInterval; // Find that gun is in interval or not
    [SerializeField] private ParticleSystem ShootParticle;
    [SerializeField] private GameObject shootingdirection;
    
    [Space(10)]
    [Header("All bullet manage variables")]
    public List<GameObject> bulletAll; // All player bullet
    public List<GameObject> bulletUsed; // All used bullet
    public List<GameObject> bulletUnused; // All unused bullet


    [Space(10)]
    [Header("Damage variables")]
    public int hitDamage = 5; // Bot damage amount

    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.2f;
    private Vector3 originalPosition;

    // Start
    void Start()
    {
        PlayerManager = GetComponent<Player_Manager>();
    }

    // Update
    void Update()
    {
        if (GameManager.GamePlay == false || PlayerManager.isDeath)
        {
            return;
        }
 //       DirectionShoot();
        //Debug.DrawRay(transform.position, transform.forward * 100, Color.green);
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    Shoot();
        //}
        //DrawTrajectory();
    }

    Vector3 joystickDirection;
    private bool shooting,shooted;
    float shooting_direction;
    float stading_driection;
    public void DirectionShoot()
    {

        joystickDirection = new Vector2(Shottingjoystick.Horizontal, Shottingjoystick.Vertical);

        if (joystickDirection.magnitude > 0.1f) // To avoid shooting in place
        {
            shootingdirection.SetActive(true);
            shooting = true;   
            float angle = Mathf.Atan2(joystickDirection.x, joystickDirection.y) * Mathf.Rad2Deg;
            shooting_direction = angle;
            shootingdirection.transform.rotation = Quaternion.Euler(0, angle, 0); // Rotate player or gun

        }else if (shooting && !shooted && joystickDirection.magnitude < 0.1f)
        {
            Debug.Log("SHOTING");
            Shottingjoystick.gameObject.SetActive(false);
            shooted = true;
            shootingdirection.SetActive(false);
            
           // StartCoroutine(Shoot());
        }
        
    }

    // Bullet shoot
    public void Shoot()
    {
        if (GameManager.GamePlay == false || isInInterval == true || PlayerManager.isDeath == true)
        {
           return;
        }
        stading_driection = transform.eulerAngles.y;
     //   transform.rotation = Quaternion.Euler(0, shooting_direction, 0); // Rotate player or gun
         PlayerManager.player_Movement.playerAnimator.SetBool("Shoot_Idle", true);
        for (int i = 0; i < PlayerManager.myWeapon.FirePoints.Count; i++)
        {


            GameObject bullet = bulletUnused[0];
           

          
            bulletUnused[0].SetActive(true);
            bulletUsed.Add(bulletUnused[0]);
            bulletUnused.Remove(bulletUnused[0]);

            ShootParticle.Play();

            bullet.transform.position = PlayerManager.myWeapon.FirePoints[i].position;
            bullet.transform.eulerAngles = PlayerManager.myWeapon.FirePoints[i].eulerAngles;

            Vector3 direction = bullet.transform.forward;

            Rigidbody rb_bullet = bullet.GetComponent<Rigidbody>();
            rb_bullet.linearVelocity = direction * bulletSpeed;
            bullet.GetComponent<Bullet>().bulletPlayer = this.transform.GetComponent<Player_Manager>();
            bullet.GetComponent<Bullet>().damageAmount = hitDamage;
            bullet.transform.parent = null;


            if(PlayerManager.myWeapon.isPlayMultiTime == true)
            {
                PlayerManager.myWeapon.enabled = true;
                PlayerManager.myWeapon.WeaponAudio.clip = PlayerManager.myWeapon.BlastSound;
            }
        }
        Camera.main.gameObject.GetComponent<Camera_Follower>().Fire();
        
        PlayerManager.myWeapon.WeaponAudio.Play();

        if (PlayerManager.myWeapon.isPlayMultiTime == false)
        {
            PlayerManager.myWeapon.enabled = true;
            PlayerManager.myWeapon.WeaponAudio.clip = PlayerManager.myWeapon.BlastSound;
        }

     //  yield return new WaitForSeconds(.5f);
       

        //transform.rotation = Quaternion.Euler(0,stading_driection, 0); // Rotate player or gun
       // Shottingjoystick.gameObject.SetActive(true);
        //shooting = false;
        //shooted = false;
        Invoke("ResetShooting", 0.2f); // Adjust timing based on animation length

        StartCoroutine(ShootingInterval());

    }



    void ResetShooting()
    {
        PlayerManager.player_Movement.playerAnimator.SetBool("Shoot_Idle", false);
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
       // yield return new WaitForSeconds(1f);
       //transform.rotation = Quaternion.Euler(0, stading_driection, 0); // Rotate player or gun
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
