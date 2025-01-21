using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Shooting : MonoBehaviour
{
    [SerializeField] private Transform FirePoint;
    [SerializeField] private GameObject prefebBullet;
    [SerializeField] private float bulletSpeed;
    [SerializeField] public LineRenderer Laser;
    [SerializeField] private Button FireButton;
    [SerializeField] private Image FireReloadingImage;
    [SerializeField] private float intervalTime;
    [SerializeField] private bool isInInterval;

    public List<GameObject> bulletAll;
    public List<GameObject> bulletUsed;
    public List<GameObject> bulletUnused;
    public int bulletCount = 0;

    public GameManager GameManager;

    public Player_Manager PlayerManager;

    private void Start()
    {
        PlayerManager = GetComponent<Player_Manager>();
    }

    // Update is called once per frame
    private void Update()
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
        if (GameManager.GamePlay == false)
        {
            return;
        }
        if (isInInterval == true)
            return;

        GameObject bullet = bulletUnused[bulletCount];
        bulletUnused[bulletCount].SetActive(true);
        bulletUsed.Add(bulletUnused[bulletCount]);
        bulletUnused.Remove(bulletUnused[bulletCount]);
        Vector3 direction = transform.forward;
        Rigidbody rb_bullet = bullet.GetComponent<Rigidbody>();
        rb_bullet.linearVelocity = direction * bulletSpeed;
        bullet.GetComponent<Bullet>().bulletPlayer = this.transform.GetComponent<Player_Manager>();
        bullet.transform.parent = null;

        PlayerManager.myWeapon.enabled = true;
        PlayerManager.myWeapon.WeaponAudio.clip = PlayerManager.myWeapon.BlastSound;
        PlayerManager.myWeapon.WeaponAudio.Play();

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
}
