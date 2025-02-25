using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Space(10)]
    [Header("Base need boolean")]
    public bool GamePlay; // Check that game is running or not

    [Space(10)]
    [Header("All panels")]
    public GameObject panelStart, panelPause; // All panels
    public Image gunfiller;


    [Space(10)]
    [Header("All bot managing variables")]
    public ObjectPoolManager Objectpool;
    public List<Entity> botAll; // All bot gameobject
    public List<Bot_Manager> botDeath; // All bot gameobject which is death
    [Space(10)]
    [Header("Player managing variables")]
    public Player_Manager player; // The main player gameobject
    public float playerDistance; // Distance bewtween player and exit gate
    public float shotingInterval;
    public bool redyforBrustShooting;
    [Space(10)]
    [Header("Game start anim")]
    public Text GameStartAnimText; // Game start count down text
    public TextMeshProUGUI remaning_bot;
    [Space(10)]
    [Header("Ground manager")]
    public List<GameObject> grounds; // All ground
    public GameObject currentGround; // Ground on using
    [Range(1f, 5f)]
    public int activeGround; // Number of ground on using
    public SafeZone zome;
    [Space(10)]
    [Header("Sound manager")]
    public bool Sound = true; // bool which find sound should play or not
    public Image SoundButtonImage; // Sound button image
    public Sprite SoundOn, SoundOff; // Sound on and off sprite
    public SoundManage SoundManage; // Sound manager

    [Space(10)]
    [Header("Blood Particle Effrcts")]
    public List<ParticleSystem> AllBloodParticles;
    public int BloodParticleCount = 0;
    public Transform BulletsHolder;
    [Space(10)]
    [Header("powerups")]
    public GameObject[] allPowerups;
    public Transform[] allPowerupsPostion;

    [Header("pooling")]
    public GameObject damage_indicator;
    public GameObject ShootPartical;
    public Transform Holder;
    // Start
    private void Start()
    {
        Objectpool.CreatePool("DamageIndicator", damage_indicator, 10,Holder);
        Objectpool.CreatePool("ShootingPartical", ShootPartical, 40,Holder);
        Time.timeScale = 1f; // Make game continue
        SoundLoad();
     

    }

    public void SoundLoad()
    {
        // If statements for checking sound setting
        if (PlayerPrefs.GetString("Sound", "true") == "true")
        {
            SoundManage.SoundOnOff(1);
            SoundButtonImage.sprite = SoundOn;
            Sound = true;
            PlayerPrefs.SetString("Sound", "true");
        }
        else
        {
            SoundManage.SoundOnOff(0);
            SoundButtonImage.sprite = SoundOff;
            Sound = false;
            PlayerPrefs.SetString("Sound", "false");

        }
    }
    [SerializeField] private TextMeshProUGUI fpsTest;
    private float deltaTime = 0.0f;

    // Update
    private void Update()
    {

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsTest.text = "FPS: " + Mathf.Ceil(fps).ToString();
        if (GamePlay == false || player.is_death)
            return;


        if (redyforBrustShooting)
        {
            holdtime += Time.deltaTime;
            gunfiller.fillAmount = holdtime;
            Debug.Log(holdtime + "hold time");
        }
        BotCount();
        //playerDistance = Vector3.Distance(player.transform.position, currentGround.GetComponent<Ground>().ExitGate.transform.position);
    }

    // Game will paused through it
    public void PauseGame()
    {
        GamePlay = false;
        Time.timeScale = 0f;
        panelPause.SetActive(true);
    }

    // Game will continue run
    public void ContinueGame()
    {
        Time.timeScale = 1f;
        panelPause.SetActive(false);
        GamePlay = true;
    
    }

    float holdtime = 0;
    public void onBulletHolding()
    {
        redyforBrustShooting = true;
      
       

    }

    public void OnBulletHoldingRemove()
    {


        redyforBrustShooting = false;
        if(holdtime >=1)
        {
            shotingInterval = .01f;
            StartCoroutine(burstshooting());
        }

    }

    public IEnumerator burstshooting()
    {
        yield return new WaitForSeconds(1);
        gunfiller.fillAmount = 0;
        holdtime = 0;
        shotingInterval = 1;
    }
    
    // Start the game
    public void StartGame()
    {
        Application.targetFrameRate = 60;
        panelPause.SetActive(false);
        panelStart.SetActive(false);
        gamecompletePnale.SetActive(false);
        SetGround();
        StartCoroutine(StartGameAnim());
    }

    // Quit the game
    public void EndGame()
    {
        Application.Quit();
    }


    // Restart current game
    public void RestartGame()
    {
        Time.timeScale = 1f;
       // currentGround.GetComponent<Ground>().Closegate();
       

        
        StartGame();
    }

    // Sound on and off
    public void SoundOnOffClick()
    {
        if (Sound == true)
        {
            SoundManage.SoundOnOff(0);
            SoundButtonImage.sprite = SoundOff;
            Sound = false;
            PlayerPrefs.SetString("Sound", "false");
        }
        else
        {
            SoundManage.SoundOnOff(1);
            SoundButtonImage.sprite = SoundOn;
            Sound = true;
            PlayerPrefs.SetString("Sound", "true");
        }
    }

    // Count down animation for starting the game
    IEnumerator StartGameAnim()
    {

        //Camera.main.gameObject.GetComponent<Camera_Follower>().shouldFollow = false;
        //Camera.main.transform.position = new Vector3(0, 20, -45);
        //Camera.main.transform.eulerAngles = new Vector3(30, 0, 0);
        GamePlay = false;
        GameStartAnimText.gameObject.SetActive(true);
      //  SoundManage.SoundPlayStop(0);
        GameStartAnimText.text = "3";
        yield return new WaitForSeconds(1);
        GameStartAnimText.text = "2";
        yield return new WaitForSeconds(1);
        GameStartAnimText.text = "1";
        yield return new WaitForSeconds(1);
        GameStartAnimText.text = "Go!";
        currentGround.GetComponent<Ground>().isOpenEntryDoor = true;
        currentGround.GetComponent<Ground>().isOpenExitDoor= false;
        yield return new WaitForSeconds(1);
        Time.timeScale = 1f;
        GameStartAnimText.gameObject.SetActive(false);
       // StartCoroutine(Powerupsspwn());
        //  player.player_Movement.playerRigidbody.isKinematic = false;
        //Camera.main.gameObject.GetComponent<Camera_Follower>().shouldFollow = true;
        //Camera.main.gameObject.GetComponent<Camera_Follower>().isArriveOrignalPos = false;
        GamePlay = true;
        SoundManage.GetComponent<AudioSource>().Play();
    }

    public IEnumerator Powerupsspwn()
    {
        while (true)
        {
            //getting all powerups 
            Vector3 spawnpostion = allPowerupsPostion[Random.Range(0, allPowerupsPostion.Length)].position;
            Debug.Log(spawnpostion);
            //getting all powerups positon
            GameObject powerups = allPowerups[Random.Range(0, allPowerups.Length)];
            powerups.gameObject.SetActive(true);
            powerups.transform.position = spawnpostion;
            yield return new WaitForSeconds(Random.Range(5, 10));

        }

        //sellecting random powerups and its positon

        //activating powerups


    }
    public GameObject gamecompletePnale;

    // Counting bot for game end
    void BotCount()
    {
        if (currentGround.GetComponent<Ground>().isOpenExitDoor)
            return;
        int tempBotCounter = 0;
        for (int i = 0; i < botAll.Count; i++)
        {
            if (botAll[i].gameObject.activeInHierarchy == true)
            {
                tempBotCounter++;
               
            }
        }
        remaning_bot.text = "Remaning :" + tempBotCounter;
        if (tempBotCounter == 0)
        {
            currentGround.GetComponent<Ground>().isOpenExitDoor = true;


            gamecompletePnale.SetActive(true);
        }
    }
    public GameObject[] allcharacter;
    public void playersize(float value)
    {
        foreach (var item in allcharacter)
        {
            item.transform.localScale = new Vector3(value, value, value);
        }
    }
    // Set ground, bots and player
    void SetGround()
    {
        // Select ground
        currentGround = null;
        for (int i = 0; i < Holder.childCount; i++)
        {
            Holder.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < grounds.Count; i++)
        {
            grounds[i].gameObject.SetActive(false);
        }

        activeGround = Random.Range(0, grounds.Count);

        currentGround = grounds[activeGround];
        currentGround.SetActive(true);

        // Declare ground variable
        Ground groundSctipt = currentGround.GetComponent<Ground>();
        //groundSctipt.ExitDoorLeft.transform.eulerAngles = new Vector3(0, 0, 0);
        //groundSctipt.ExitDoorRight.transform.eulerAngles = new Vector3(0, 0, 0);
        //groundSctipt.isOpenExitDoor = false;
        //groundSctipt.EntryDoorLeft.transform.eulerAngles = new Vector3(0, 0, 0);
        //groundSctipt.EntryDoorRight.transform.eulerAngles = new Vector3(0, 0, 0);
        //groundSctipt.isOpenEntryDoor = false;

        zome.transform.localScale = zome.startingsclae;
        zome.start = true;
        // Set the player 
        player.ResetingGame();
        player.gameObject.transform.position =new Vector3(groundSctipt.playerSpawnPos.transform.position.x, groundSctipt.playerSpawnPos.transform.position.y+.6f, groundSctipt.playerSpawnPos.transform.position.z);
        player.ReassignValue();

        // Set the bot
        for (int i = 0; i < groundSctipt.botCount; i++)
        {
            botAll[i].gameObject.SetActive(false);

            botAll[i].transform.position = groundSctipt.allSpawnPoint[i].position;
            botAll[i].starting_pos = groundSctipt.botRandomMove[i].parent.position;
            botAll[i].ResetingGame();
           // botAll[i].RandomMovePos = groundSctipt.botRandomMove[i];

       

            botAll[i].gameObject.SetActive(true);
        }

        player.gameObject.SetActive(true);
    }

    // Show blood particles effect
    public void ShowBlood(Vector3 posPlay)
    {
        if(BloodParticleCount >= AllBloodParticles.Count - 1)
        {
            BloodParticleCount--;
        }
        ParticleSystem currentParticle = AllBloodParticles[BloodParticleCount];
        AllBloodParticles[BloodParticleCount].transform.position = posPlay;
        AllBloodParticles[BloodParticleCount].Play();
        float startLifetime = AllBloodParticles[BloodParticleCount].main.startLifetime.constant;
        BloodParticleCount++;
        StartCoroutine(ResetBloodarticle(startLifetime));
    }

    IEnumerator ResetBloodarticle(float time)
    {
        yield return new WaitForSeconds(time);
        BloodParticleCount--;
    }

}