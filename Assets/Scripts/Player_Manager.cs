using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Player_Manager : MonoBehaviour
{

    [SerializeField] private float playerHealth; // Player current health
    [SerializeField] private float playerMaxHealth = 100f; // Player max health
    [SerializeField] private float playerHealthIncrement = 1f; // Player health recovery amount
    [SerializeField] private float playerHealthDeductionAmount = 3f; // Player health deduct amount

    [SerializeField] private int playerScore; // Player current score

    [SerializeField] private TextMeshProUGUI textHealth; // Player health amount status
    [SerializeField] private TextMeshProUGUI textScore; // Player score amount status

    public float enemyDistance; // Enemy distance for fight
    public int enemyInRadius; // Enemy count in our radius
    public List<GameObject> listEnemy; // All enemy in that list
    private GameObject targetEnemy;
    private bool isTargetSelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("HealthUpgradation", 0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        AutoTarget();
    }

    void AutoTarget()
    {
        if (listEnemy.Count > 0)
        {
            if (targetEnemy == null)
            {
                isTargetSelected = false;
            }

            if (isTargetSelected == false)
            {
                targetEnemy = listEnemy[Random.Range(0, listEnemy.Count)];
                isTargetSelected = true;
            }

            transform.LookAt(targetEnemy.transform.position);
        }
        else
        {
            targetEnemy = null;
        }
    }

    // Player score update in text
    void ScoreTextUpdate()
    {
        textScore.text = playerScore.ToString("00");
    }

    // Player health update in text
    void HealthTextUpdate()
    {
        textHealth.text = playerHealth.ToString("00") + " / " + playerMaxHealth.ToString("00");
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
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.TryGetComponent<Bullet>(out Bullet bullet))
        {
            if (bullet.bulletBot != null)
            {
                BulletHitted(bullet);
            }
        }
    }

    // Player health increaser
    void HealthUpgradation()
    {
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
}