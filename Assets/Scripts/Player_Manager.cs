using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player_Manager : MonoBehaviour
{

    [SerializeField] private float playerHealth; // Player current health
    [SerializeField] private float playerMaxHealth = 100f; // Player max health
    [SerializeField] private float playerHealthIncrement = 1f; // Player health recovery amount
    [SerializeField] private float playerHealthDeductionAmount = 3f; // Player health deduct amount

    [SerializeField] private int playerScore; // Player current score

    [SerializeField] private TextMeshProUGUI textHealth, textHealth1; // Player health amount status
    [SerializeField] private TextMeshProUGUI textScore, textScore1; // Player score amount status

    [SerializeField] private Image HealthBarSlider;

    public float enemyDistance; // Enemy distance for fight
    public int enemyInRadius; // Enemy count in our radius
    public List<GameObject> listEnemy; // All enemy in that list
    private GameObject targetEnemy;
    private bool isTargetSelected;
    public bool isTargeting;

    public Player_Movement player_Movement;
    public Player_Shooting player_Shooting;


    [SerializeField] private Vector3 startingPos;
    [SerializeField] private Vector3 startingEular;
    [SerializeField] private Vector3 startingScale;

    public GameManager GameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startingPos = transform.position;
        startingEular = transform.eulerAngles;
        startingScale = transform.localScale;
        InvokeRepeating("HealthUpgradation", 0, 2);
        player_Movement = GetComponent<Player_Movement>();
        player_Shooting = GetComponent<Player_Shooting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

        AutoTarget();
    }

    void AutoTarget()
    {

        if (listEnemy.Count > 0)
        {
            //if (targetEnemy == null)
            //{
            //    isTargetSelected = false;
            //}

            //if (isTargetSelected == false)
            //{
            //    targetEnemy = listEnemy[Random.Range(0, listEnemy.Count)];
            //    isTargetSelected = true;
            //}

            //transform.LookAt(targetEnemy.transform.position);
            //isTargeting = true;
            player_Shooting.Laser.gameObject.SetActive(true);
        }
        else
        {
            player_Shooting.Laser.gameObject.SetActive(false);
            //targetEnemy = null;
            //isTargeting = false;
        }
    }

    // Player score update in text
    void ScoreTextUpdate()
    {
        textScore.text = playerScore.ToString("00");
        textScore1.text = playerScore.ToString("00");
    }

    // Player health update in text
    void HealthTextUpdate()
    {
        textHealth.text = playerHealth.ToString("00") + " / " + playerMaxHealth.ToString("00");
        textHealth1.text = playerHealth.ToString("00") + " / " + playerMaxHealth.ToString("00");
        HealthBarSlider.fillAmount = playerHealth/100;
    }

    // Health deduct on player hit
    void HealthDeduction()
    {
        playerHealth -= playerHealthDeductionAmount;
        HealthTextUpdate();
    }

    // Player bullet hit
    void BulletHitted(Bullet bullet)
    {
        HealthDeduction();
        if (playerHealth <= 0)
        {
            //Destroy(this.gameObject); 
            this.gameObject.SetActive(false);
            GameManager.RestartGame();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

        if (other.gameObject.transform.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (bullet.bulletBot != null)
            {
                bullet.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                BulletHitted(bullet);
            }
        }

        

    }

    // Player health increaser
    void HealthUpgradation()
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

        if (playerHealth < playerMaxHealth)
        {
            playerHealth += playerHealthIncrement;
            HealthTextUpdate();
        }
    }

    // Kill player
    public void KillPlayer(int ScoreIncrementAmount)
    {
        playerScore += ScoreIncrementAmount;
        ScoreTextUpdate();
    }

    public void ResettingGame()
    {
        CancelInvoke();
        this.gameObject.SetActive(true);
        player_Movement.AnimationController(Player_Movement.AnimState.Idle);
        player_Movement.movementDirection = new Vector3(0,0,0);
        player_Movement.temp = true;
        player_Movement.newTemp = new Vector3(0,0,0);
        player_Movement.playerRigidbody.isKinematic = true;
        playerHealth = playerMaxHealth;
        playerScore = 0;
        this.transform.position = startingPos;
        this.transform.eulerAngles = startingEular;
        this.transform.localScale = startingScale;
        HealthTextUpdate();
        ScoreTextUpdate();
        player_Shooting.CollectingBullet();
        
    }

   

}