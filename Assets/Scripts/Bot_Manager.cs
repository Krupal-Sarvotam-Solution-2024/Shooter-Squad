using UnityEngine;

public class Bot_Manager : MonoBehaviour
{
    [SerializeField] private float botHealth; // Bot current health
    [SerializeField] private float botMaxHealth = 100f; // Bot max health
    [SerializeField] private float botHealthIncrement = 1f; // Bot health recovery amount
    [SerializeField] private float botHealthDeductionAmount = 3f; // Bot health deduction amount
    [SerializeField] private int botDeathScore = 10; // Score for increment to the player

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //InvokeRepeating("HealthUpgradation", 0, 1);
    }

    private void Update()
    {
        GameObject PlayerObject = GameObject.FindGameObjectWithTag("Player");
        transform.LookAt(PlayerObject.transform.position);
    }

    void HealthUpgradation()
    {
        if (botHealth < botMaxHealth)
        {
            botHealth += botHealthIncrement;
        }
    }

    void HealthDeduction()
    {
        botHealth -= botHealthDeductionAmount;
    }

    void BulletHitted(Bullet bullet)
    {
        if(botHealth <= 0)
        {
            bullet.bulletPlayer.KillPlayer(botDeathScore);
            Destroy(this.gameObject);
        }
        else
        {
            HealthDeduction();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Bullet"))
        {
            if(collision.gameObject.transform.TryGetComponent<Bullet>(out Bullet bullet))
            {
                if(bullet.bulletPlayer != null)
                {
                    BulletHitted(bullet);
                }
            }
        }
    }
}
