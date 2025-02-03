using NUnit.Framework;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Player_Manager : MonoBehaviour
{
    [Space(10)]
    [Header("All Health managing variable")]
    [SerializeField] private float playerHealth; // Player current health
    [SerializeField] private float playerMaxHealth = 100f; // Player max health
    [SerializeField] private float playerHealthIncrement = 1f; // Player health recovery amount
    [SerializeField] private TextMeshProUGUI textHealth, textHealth1; // Player health amount status
    [SerializeField] private Image HealthBarSlider;
    [SerializeField] private GameObject HealthBar, HealthBarFG;
    [SerializeField] private TextMeshPro HealthPerText;
    [SerializeField] private ParticleSystem HealthIncreaserParticle;

    [Space(10)]
    [Header("Player Score Managing variable")]
    [SerializeField] private int playerScore; // Player current score
    [SerializeField] private TextMeshProUGUI textScore, textScore1; // Player score amount status

    [Space(10)]
    [Header("Auto aim & enemy managing variables")]
    public float enemyDistance; // Enemy distance for fight
    public int enemyInRadius; // Enemy count in our radius
    public List<GameObject> listEnemy; // All enemy in that list
    private GameObject targetEnemy; // Target enemy for auto aim
    private bool isTargetSelected; // Check that tarhet is selected or not
    public bool isTargeting; // Check that is finding target or not

    [Space(10)]
    [Header("Other player scripts")]
    public Player_Movement player_Movement; // Player movement script access
    public Player_Shooting player_Shooting; // Player shooting script access

    [Space(10)]
    [Header("Player default transform")]
    [SerializeField] private Vector3 startingPos; // Player start position
    [SerializeField] private Vector3 startingEular; // Player start eular (rotation)
    [SerializeField] private Vector3 startingScale; // Player start scale

    [Space(10)]
    [Header("Animations and Death ")]
    [SerializeField] private List<GameObject> botBodyParts;
    [SerializeField] GameObject DeathPartcleSystem;
    public bool isDeath = false; // finding that player is dead or not

    [Space(10)]
    [Header("Audio managing system")]
    public AudioSource playerAudio; // Audio source which handle player audios
    public AudioClip runSurface, runRamp, playerDeath; // All audio clips
    public List<Weapon> allWeapon;
    public Weapon myWeapon;
    public bool isSoundPlaying;

    [Space(10)]
    [Header("Game manager access")]
    public GameManager GameManager; // Game manager access

    [Space(10)]
    [Header("All damage indicator variables")]
    public List<GameObject> healthIndicatorAll;
    public List<GameObject> healthIndicatorUsed;
    public List<GameObject> healthIndicatorUnused;
    public int healthIndicatorCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ReassignValue();
        player_Movement = GetComponent<Player_Movement>();
        player_Shooting = GetComponent<Player_Shooting>();
        
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.transform.LookAt(Camera.main.transform.position);
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
            if (targetEnemy == null)
            {
                isTargetSelected = false;
            }

            if (isTargetSelected == false)
            {
                targetEnemy = listEnemy[Random.Range(0, listEnemy.Count)];
                isTargetSelected = true;
            }
            if (Vector3.Distance(this.gameObject.transform.position, targetEnemy.transform.position) > enemyDistance || targetEnemy.gameObject.activeInHierarchy == false || targetEnemy.GetComponent<Bot_Manager>().isDeath == true)
            {
                targetEnemy = listEnemy[Random.Range(0, listEnemy.Count)];
                isTargetSelected = true;
            }

            //for (int i = 0; i < GameManager.botAll.Count; i++)
            //{
            //    GameManager.botAll[i].SelectedBot.gameObject.SetActive(false);
            //}
            //targetEnemy.GetComponent<Bot_Manager>().SelectedBot.SetActive(true);
            transform.LookAt(targetEnemy.transform.position);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            isTargeting = true;
            player_Shooting.Laser.gameObject.SetActive(true);
        }
        else
        {
            player_Shooting.Laser.gameObject.SetActive(false);
            targetEnemy = null;
            isTargeting = false;
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
        if (playerHealth >= 0)
        {
            textHealth.text = playerHealth.ToString("00") + " / " + playerMaxHealth.ToString("00");
            textHealth1.text = playerHealth.ToString("00") + " / " + playerMaxHealth.ToString("00");
            HealthBarSlider.fillAmount = playerHealth / 100;
            float healthPercentage = (playerHealth / playerMaxHealth) * 100f;
            HealthBarFG.transform.localScale = new Vector3(healthPercentage / 100, HealthBarFG.transform.localScale.y, HealthBarFG.transform.localScale.z);
            HealthPerText.text = healthPercentage + "%";
        }
        else
        {
            textHealth.text = "00" + " / " + playerMaxHealth.ToString("00");
            textHealth1.text = "00" + " / " + playerMaxHealth.ToString("00");
            float healthPercentage = 0;
            HealthBarFG.transform.localScale = new Vector3(healthPercentage / 100, HealthBarFG.transform.localScale.y, HealthBarFG.transform.localScale.z);
            HealthPerText.text = healthPercentage + "%";
        }
    }

    // Health deduct on player hit
    public void HealthDeduction(int DamageAmount)
    {
        playerHealth -= DamageAmount;
        DamgeIndicator((int)DamageAmount);
        HealthTextUpdate();
        if (playerHealth <= 0)
        {
            //Destroy(this.gameObject); 
            //this.gameObject.SetActive(false);
            StartCoroutine(PlayerDeath());
        }
    }



    // Player bullet hit
    void BulletHitted(Bullet bullet)
    {
        HealthDeduction(bullet.damageAmount);
        /*        if (playerHealth <= 0)
                {
                    //Destroy(this.gameObject); 
                    //this.gameObject.SetActive(false);
                    StartCoroutine(PlayerDeath());
                }
        */
    }

    IEnumerator PlayerDeath()
    {
        isDeath = true;
        player_Movement.playerRigidbody.isKinematic = true;
        //player_Movement.AnimationController(Player_Movement.AnimState.Death);
        BodyVisibility(false);
        DeathPartcleSystem.SetActive(true);
        DeathPartcleSystem.GetComponent<ParticleSystem>().Play();
        for (int i = 0; i < GameManager.botAll.Count; i++)
        {
            GameManager.botAll[i].Player_Manager = null;
            GameManager.botAll[i].StopFollowing();
        }
        playerAudio.clip = playerDeath;
        playerAudio.PlayOneShot(playerDeath);
        isSoundPlaying = true;
        yield return new WaitForSeconds(4);
        DeathPartcleSystem.SetActive(false);
        GameManager.RestartGame();

        isDeath = false;
    }

    /* private void OnTriggerEnter(Collider other)
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



     }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.GamePlay == false)
        {
            return;
        }

        if (collision.gameObject.transform.TryGetComponent<Bullet>(out Bullet bullet))
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
        //StartCoroutine(HealthIncrease());
    }

    IEnumerator HealthIncrease()
    {
        yield return new WaitForSeconds(5);
        if (GameManager.GamePlay == false || isDeath == true)
        {
            yield return null;
        }

        HealthIncreaserParticle.Play();

        if (playerHealth < playerMaxHealth)
        {
            playerHealth += playerHealthIncrement;
            HealthTextUpdate();
        }
        yield return new WaitForSeconds(1);
        if (playerHealth < playerMaxHealth)
        {
            playerHealth += playerHealthIncrement;
            HealthTextUpdate();
        }
        yield return new WaitForSeconds(1);
        if (playerHealth < playerMaxHealth)
        {
            playerHealth += playerHealthIncrement;
            HealthTextUpdate();
        }

        HealthIncreaserParticle.Stop();

        StartCoroutine(HealthIncrease());
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
        BodyVisibility(true);

        player_Movement.AnimationController(Player_Movement.AnimState.Idle);
        player_Movement.movementDirection = new Vector3(0, 0, 0);
        player_Movement.temp = true;
        player_Movement.newTemp = startingPos;
        player_Movement.playerRigidbody.isKinematic = true;

        playerHealth = playerMaxHealth;
        playerScore = 0;

        HealthTextUpdate();
        ScoreTextUpdate();

        player_Shooting.CollectingBullet();
        isSoundPlaying = false;
        enemyInRadius = 0;
        listEnemy.Clear();

        AssignMyWeapone();

        this.transform.position = startingPos;
        this.transform.eulerAngles = startingEular;
        this.transform.localScale = startingScale;

        StartCoroutine(HealthIncrease());
    }

    public void AssignMyWeapone()
    {
        for (int i = 0; i < allWeapon.Count; i++)
        {
            allWeapon[i].gameObject.SetActive(false);
        }
        myWeapon = allWeapon[Random.Range(0, allWeapon.Count)];
        myWeapon.gameObject.SetActive(true);
    }

    void BodyVisibility(bool visibility)
    {
        for (int i = 0; i < botBodyParts.Count; i++)
        {
            botBodyParts[i].gameObject.SetActive(visibility);
        }
    }

    public void ReassignValue()
    {
        startingPos = transform.position;
        startingEular = transform.eulerAngles;
        startingScale = transform.localScale;
        player_Movement.newTemp = startingPos;
    }

    void DamgeIndicator(int damage)
    {

        if (healthIndicatorUnused.Count == 0)
        {
            healthIndicatorUsed[0].gameObject.SetActive(false);
            healthIndicatorUnused.Add(healthIndicatorUsed[0]);
            healthIndicatorUsed.Remove(healthIndicatorUsed[0]);
        }

        GameObject indicator = healthIndicatorUnused[healthIndicatorCount];
        indicator.SetActive(true);
        healthIndicatorUsed.Add(indicator);
        healthIndicatorUnused.Remove(indicator);

        indicator.GetComponent<TextMeshPro>().text = "-" + damage.ToString();
        StartCoroutine(healthIndocatorInterval(indicator));
    }

    IEnumerator healthIndocatorInterval(GameObject indicator)
    {
        yield return new WaitForSeconds(1.5f);

        indicator.SetActive(false);

        healthIndicatorUsed.Remove(indicator);
        healthIndicatorUnused.Add(indicator);
    }
}