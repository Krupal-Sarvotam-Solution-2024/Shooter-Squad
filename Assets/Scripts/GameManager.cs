using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class GameManager : MonoBehaviour
{
    [Space(10)]
    [Header("Base need boolean")]
    public bool GamePlay; // Check that game is running or not

    [Space(10)]
    [Header("All panels")]
    public GameObject panelStart, panelPause; // All panels

    [Space(10)]
    [Header("All bot managing variables")]
    public List<Bot_Manager> botAll; // All bot gameobject
    public List<Bot_Manager> botDeath; // All bot gameobject which is death
    [Space(10)]
    [Header("Player managing variables")]
    public Player_Manager player; // The main player gameobject
    public float playerDistance; // Distance bewtween player and exit gate

    [Space(10)]
    [Header("Game start anim")]
    public Text GameStartAnimText; // Game start count down text

    [Space(10)]
    [Header("Ground manager")]
    public List<GameObject> grounds; // All ground
    public GameObject currentGround; // Ground on using
    [Range(1f, 5f)]
    public int activeGround; // Number of ground on using

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

    // Start
    private void Start()
    {
        Time.timeScale = 1f; // Make game continue

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

    // Update
    private void Update()
    {
        if (GamePlay == false)
            return;

        BotCount();
        playerDistance = Vector3.Distance(player.transform.position, currentGround.GetComponent<Ground>().ExitGate.transform.position);
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

    // Start the game
    public void StartGame()
    {
        panelPause.SetActive(false);
        panelStart.SetActive(false);
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

        /*for (int i = 0; i < botSpawnPoint.Count; i++)
        {
            botAll[i].gameObject.transform.position = botSpawnPoint[i].transform.position;
            botAll[i].gameObject.SetActive(true);
            botAll[i].GetComponent<Bot_Manager>().ResetingGame();
            botAll[i].GetComponent<Bot_Manager>().ReassignValue();
        }
        player.ResettingGame();*/
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
        Camera.main.gameObject.GetComponent<Camera_Follower>().shouldFollow = false;
        Camera.main.transform.position = new Vector3(0, 20, -45);
        Camera.main.transform.eulerAngles = new Vector3(30, 0, 0);
        GamePlay = false;
        GameStartAnimText.gameObject.SetActive(true);
        SoundManage.SoundPlayStop(0);
        GameStartAnimText.text = "3";
        yield return new WaitForSeconds(1);
        GameStartAnimText.text = "2";
        yield return new WaitForSeconds(1);
        GameStartAnimText.text = "1";
        yield return new WaitForSeconds(1);
        GameStartAnimText.text = "Go!";
        currentGround.GetComponent<Ground>().isOpenEntryDoor = true;
        yield return new WaitForSeconds(1);
        Time.timeScale = 1f;
        GameStartAnimText.gameObject.SetActive(false);
        player.player_Movement.playerRigidbody.isKinematic = false;
        Camera.main.gameObject.GetComponent<Camera_Follower>().shouldFollow = true;
        //Camera.main.gameObject.GetComponent<Camera_Follower>().isArriveOrignalPos = false;
        GamePlay = true;
    }

    // Counting bot for game end
    void BotCount()
    {
        int tempBotCounter = 0;
        for (int i = 0; i < botAll.Count; i++)
        {
            if (botAll[i].gameObject.activeInHierarchy == true)
            {
                tempBotCounter++;
            }
        }

        if (tempBotCounter == 0)
        {
            currentGround.GetComponent<Ground>().isOpenExitDoor = true;
        }
    }

    // Set ground, bots and player
    void SetGround()
    {
        // Select ground
        currentGround = null;

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


        // Set the player 
        player.ResettingGame();
        player.gameObject.transform.position = groundSctipt.playerSpawnPos.transform.position;
        player.ReassignValue();

        // Set the bot
        for (int i = 0; i < groundSctipt.botCount; i++)
        {
            botAll[i].gameObject.SetActive(false);

            botAll[i].transform.position = groundSctipt.allSpawnPoint[i].position;

            botAll[i].ResetingGame();

            botAll[i].ReassignValue();

            botAll[i].RandomMovePos = groundSctipt.botRandomMove[i];

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