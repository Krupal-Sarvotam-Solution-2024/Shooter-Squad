using TMPro;
using UnityEngine;

public class Player_Manager : MonoBehaviour
{

    [SerializeField] private float playerHealth; // Player current health
    [SerializeField] private float playerMaxHealth = 100f; // Player max health
    [SerializeField] private float playerHealthIncrement = 1f; // Player health recovery amount
    
    [SerializeField] private int playerScore; // Player current score
    
    [SerializeField] private TextMeshProUGUI textHealth; // Player health amount status
    [SerializeField] private TextMeshProUGUI textScore; // Player score amount status

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("HealthUpgradation", 0, 2);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ScoreTextUpdate()
    {
        textScore.text = playerScore.ToString("00");
    }

    void HealthTextUpdate()
    {
        textHealth.text = playerHealth.ToString("00") + " / " + playerMaxHealth.ToString("00");
    }

    void HealthUpgradation()
    {
        if (playerHealth < playerMaxHealth)
        {
            playerHealth += playerHealthIncrement;
            HealthTextUpdate();
        }
    }

    public void KillPlayer(int ScoreIncrementAmount)
    {
        playerScore += ScoreIncrementAmount;
        ScoreTextUpdate();
    }
}