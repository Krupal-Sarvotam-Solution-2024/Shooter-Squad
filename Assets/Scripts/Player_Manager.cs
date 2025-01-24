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
    // All Health managing variable
    [SerializeField] private float playerHealth; // Player current health
    [SerializeField] private float playerMaxHealth = 100f; // Player max health
    [SerializeField] private float playerHealthIncrement = 1f; // Player health recovery amount
    [SerializeField] private float playerHealthDeductionAmount = 3f; // Player health deduct amount
    [SerializeField] private TextMeshProUGUI textHealth, textHealth1; // Player health amount status
    [SerializeField] private Image HealthBarSlider;
    [SerializeField] private GameObject HealthBar, HealthBarFG;
    [SerializeField] private TextMeshPro HealthPerText;

    // Player Score Managing variable
    [SerializeField] private int playerScore; // Player current score
    [SerializeField] private TextMeshProUGUI textScore, textScore1; // Player score amount status

    // Auto aim & enemy managing variables
    public float enemyDistance; // Enemy distance for fight
    public int enemyInRadius; // Enemy count in our radius
    public List<GameObject> listEnemy; // All enemy in that list
    private GameObject targetEnemy; // Target enemy for auto aim
    private bool isTargetSelected; // Check that tarhet is selected or not
    public bool isTargeting; // Check that is finding target or not

    // Other player scripts
    public Player_Movement player_Movement; // Player movement script access
    public Player_Shooting player_Shooting; // Player shooting script access

    // Player default transform
    [SerializeField] private Vector3 startingPos; // Player start position
    [SerializeField] private Vector3 startingEular; // Player start eular (rotation)
    [SerializeField] private Vector3 startingScale; // Player start scale

    // Animations and Death 
    //[SerializeField] private AnimationClip deathClip; // Death animation clip for length
    [SerializeField] private List<GameObject> botBodyParts;
    [SerializeField] GameObject DeathPartcleSystem;
    public bool isDeath = false; // finding that player is dead or not

    // Audio managing system
    public AudioSource playerAudio; // Audio source which handle player audios
    public AudioClip runSurface, runRamp, playerDeath; // All audio clips
    public Weapon myWeapon;
    public bool isSoundPlaying;

    // Game manager access
    public GameManager GameManager; // Game manager access

    public List<GameObject> healthIndicatorAll;
    public List<GameObject> healthIndicatorUsed;
    public List<GameObject> healthIndicatorUnused;
    public int healthIndicatorCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ReassignValue();
        InvokeRepeating("HealthUpgradation", 0, 1);
        player_Movement = GetComponent<Player_Movement>();
        player_Shooting = GetComponent<Player_Shooting>();
        AssignMyWeapone();
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
            if (Vector3.Distance(this.gameObject.transform.position, targetEnemy.transform.position) > enemyDistance || targetEnemy.gameObject.activeInHierarchy == false)
            {
                targetEnemy = listEnemy[Random.Range(0, listEnemy.Count)];
                isTargetSelected = true;
            }

            transform.LookAt(targetEnemy.transform.position);
            transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
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
    void HealthDeduction()
    {
        playerHealth -= playerHealthDeductionAmount;
        DamgeIndicator(10);
        HealthTextUpdate();
    }

    // Player bullet hit
    void BulletHitted(Bullet bullet)
    {
        HealthDeduction();
        if (playerHealth <= 0)
        {
            //Destroy(this.gameObject); 
            //this.gameObject.SetActive(false);
            StartCoroutine(PlayerDeath());
        }
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
        if (GameManager.GamePlay == false || isDeath == true)
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

        this.transform.position = startingPos;
        this.transform.eulerAngles = startingEular;
        this.transform.localScale = startingScale;

        InvokeRepeating("HealthUpgradation", 0, 2);
    }

    public void AssignMyWeapone()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).TryGetComponent<Weapon>(out Weapon myweapon))
            {
                myWeapon = myweapon;
            }

        }
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

        if(healthIndicatorUnused.Count == 0)
        {
            healthIndicatorUsed[0].gameObject.SetActive(false);
            healthIndicatorUnused.Add(healthIndicatorUsed[0]);
            healthIndicatorUsed.Remove(healthIndicatorUsed[0]);
        }

        GameObject indicator = healthIndicatorUnused[healthIndicatorCount];
        indicator.SetActive(true);
        healthIndicatorUsed.Add(indicator);
        healthIndicatorUnused.Remove(indicator);

        indicator.GetComponent<TextMeshPro>().text = "-" +damage.ToString();
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