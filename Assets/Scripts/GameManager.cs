using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;

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
    [Space(10)]
    [Header("Player managing variables")]
    public Player_Manager player; // The main player gameobject
    public float playerDistance; // Distance bewtween player and exit gate
    public float shotingInterval;
    public bool redyforBrustShooting;
    public Weapon[] Allwepons;
    [Space(10)]
    [Header("Game start anim")]
    public Text GameStartAnimText; // Game start count down text
    public TextMeshProUGUI[] remaning_bot;
    public TextMeshProUGUI[] remaning_bot2;
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
    public GameObject gameWinpanel;
    public GameObject gameLosspanel;

    [Space(10)]
    [Header("Blood Particle Effrcts")]
    public List<ParticleSystem> AllBloodParticles;
    public int BloodParticleCount = 0;
    [Space(10)]
    [Header("powerups")]
    public GameObject[] allPowerups;
    public Transform[] allPowerupsPostion;

    [Header("pooling")]
    public GameObject damage_indicator;
    public GameObject ShootPartical;
    public Transform damageIndicaterHolder,shootingparticalHolder,BulletHolder;


    public Vector3 safeZoneminmum, safeZonemaximum;
    // Start
    private void Start()
    {
        Objectpool.CreatePool("DamageIndicator", damage_indicator, 10,damageIndicaterHolder);
        Objectpool.CreatePool("ShootingPartical", ShootPartical, 40,shootingparticalHolder);
        Time.timeScale = 1f; // Make game continue
        SoundLoad();
     
        
    }
    public TextMeshProUGUI timeText,timeText2;
    public float remainingTime;
    private void UpdateTimeDisplay()
    {
        if (!timeText) return;

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
       
        killcounttext.text = player.killCount.ToString();
        killcounttext2.text = player.killCount.ToString();

        timeText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        timeText2.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        remainingTime -= Time.deltaTime;

    }

    public TextMeshProUGUI killcounttext;
    public TextMeshProUGUI killcounttext2;
    IEnumerator GameCompleted()
    {
        remainingTime = 120;

        yield return new WaitForSeconds(remainingTime);
     
        //Entity higestscroe = it;
        //foreach (var item in allcharacter)
        //{ 

        //}
        //      if (allcharacter[0].)
        
        if (allcharacter[0].gameObject.name.Contains("You"))
        {
            gameWinpanel.SetActive(true);
            //winner
        }
        else
        {
            gameLosspanel.SetActive(true);
            //loser
        }
        for (int i = 0; i < allcharacter.Length; i++)
        {
           
           
            if (allcharacter[i].name.Contains("You"))
            {
                allcharacter[i].GetComponent<Entity>().powerupsConter.fillAmount = 0;
            }
        }
        BotCount();
        for (int i = 0; i < allcharacter.Length; i++)
        {
       

            if (allcharacter[i].name.Contains("You"))
            {
                allcharacter[i].GetComponent<Entity>().powerupsConter.fillAmount = 0;
            }
        }
        timeText.text = "00 : 00";
        timeText2.text = "00 : 00";
        Time.timeScale = 0;
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
        if (GamePlay == false)
            return;
        UpdateTimeDisplay();

        if( player.is_death)
            return;
        
        if (redyforBrustShooting)
        {
            holdtime += Time.deltaTime;
            gunfiller.fillAmount = holdtime;
            Debug.Log(holdtime + "hold time");
        }
       
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

        timeText.text = "02 : 00";
        timeText2.text = "02 : 00";
        Application.targetFrameRate = -60;
        panelPause.SetActive(false);
        panelStart.SetActive(false);
        gamecompletePnale.SetActive(false);
        SetGround();
        StartCoroutine(StartGameAnim());
        StartCoroutine(GameCompleted());

       
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


        Objectpool.Retrunalltopool("DamageIndicator", damageIndicaterHolder);
        Objectpool.Retrunalltopool("ShootingPartical",  shootingparticalHolder);

        gameWinpanel.SetActive(false);
        gameLosspanel.SetActive(false);
        StopAllCoroutines();
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

        yield return new WaitForSeconds(1);
        Time.timeScale = 1f;
        GameStartAnimText.gameObject.SetActive(false);
        //StartCoroutine(Powerupsspwn());
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
    public int botcount;
    // Counting bot for game end
    public void BotCount()
    {
        botcount--;

        // Sort characters based on kill count in descending order
        allcharacter = allcharacter.OrderByDescending(c => c.GetComponent<Entity>().killCount).ToArray();

        // Update UI text fields with animation
        for (int i = 0; i < remaning_bot.Length; i++)
        {
            if (i < allcharacter.Length)
            {
                string playerName = allcharacter[i].name;
                string killCount = allcharacter[i].GetComponent<Entity>().killCount.ToString();
                string newText = $"{playerName}  {killCount}";

                // Animate text change
                AnimateTextChange(remaning_bot[i], newText);
                AnimateTextChange(remaning_bot2[i], newText);
               // remaning_bot2[i].transform.localScale = remaning_bot[i].transform.localScale;
              //  AnimateTextChange(remaning_bot2[i], newText);
            }
        }
    }

    private void AnimateTextChange(TextMeshProUGUI uiText, string newText)
    {
        // Fade out old text
        uiText.DOFade(0, 0.2f).OnComplete(() =>
        {
            uiText.text = newText; // Change text
            uiText.DOFade(1, 0.2f); // Fade in new text
        });

        uiText.transform.DOScale(1.2f, 0.2f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            uiText.transform.localScale = Vector3.one; // Ensure final scale is (1,1,1)
        });
    }
    public GameObject[] allcharacter;
    public void playersize(float value)
    {
        foreach (var item in allcharacter)
        {
            item.transform.localScale = new Vector3(value, value, value);
        }
    }

    public static bool testc;
    // Set ground, bots and player
    void SetGround()
    {
        // Select ground
        remainingTime = 120;
        currentGround = null;
        //for (int i = 0; i < Holder.childCount; i++)
        //{
        //    Holder.GetChild(i).gameObject.SetActive(false);
        //}
        for (int i = 0; i < grounds.Count; i++)
        {
            grounds[i].gameObject.SetActive(false);
        }

        activeGround = Random.Range(0, grounds.Count);

        currentGround = grounds[activeGround];
        currentGround.SetActive(true);

        // Declare ground variable
        Ground groundSctipt = currentGround.GetComponent<Ground>();
        botcount = groundSctipt.botCount;
        //for (int i = 0; i < remaning_bot.Length; i++)
        //{
        //    remaning_bot[i].text = allcharacter[i].GetComponent<Entity>().killcount.ToString();
        //}
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

        foreach (var item in botAll)
        {
            if(item.gameObject.activeInHierarchy)
                item.ResetingGame();

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