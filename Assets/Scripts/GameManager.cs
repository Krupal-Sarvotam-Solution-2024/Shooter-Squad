using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool GamePlay;

    public GameObject panelStart, panelPause;

    public List<Bot_Manager> botAll;
    public List<Bot_Manager> botDeath;
    public Player_Manager player;

    public Text GameStartAnimText;

    public List<GameObject> grounds;
    public GameObject currentGround;
    [Range(1f, 5f)]
    public int activeGround;

    public List<Transform> botSpawnPoint;
    public Transform playerSpawnPoint;

    public bool Sound = true;
    public Image SoundButtonImage;
    public Sprite SoundOn, SoundOff;

    private void Start()
    {
        Time.timeScale = 1f;
        if(Sound == true)
        {
            SoundButtonImage.sprite = SoundOn;
        }
        else
        {
            SoundButtonImage.sprite = SoundOff;
        }
    }

    private void Update()
    {
        if (GamePlay == false)
            return;

        BotCount();
    }

    public void PauseGame()
    {
        GamePlay = false;
        Time.timeScale = 0f;
        panelPause.SetActive(true);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        panelPause.SetActive(false);
        GamePlay = true;
    }

    public void StartGame()
    {
        panelPause.SetActive(false);
        panelStart.SetActive(false);
        SetGround();
        StartCoroutine(StartGameAnim());
    }

    public void EndGame()
    {
        Application.Quit();
    }

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

    public void SoundOnOffClick()
    {
        if (Sound == true)
        {
            SoundButtonImage.sprite = SoundOff;
            Sound = false;
        }
        else
        {
            SoundButtonImage.sprite = SoundOn;
            Sound = true;
        }
    }

    IEnumerator StartGameAnim()
    {
        Camera.main.gameObject.GetComponent<Camera_Follower>().shouldFollow = false;
        Camera.main.transform.position = new Vector3(0,20,-45);
        Camera.main.transform.eulerAngles = new Vector3(30,0,0);
        GamePlay = false;
        GameStartAnimText.gameObject.SetActive(true);
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
        player.player_Movement.playerRigidbody.isKinematic = false;
        Camera.main.gameObject.GetComponent<Camera_Follower>().shouldFollow = true;
        GamePlay = true;
    }

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
            RestartGame();
        }
    }

    void SetGround()
    {
        // Select ground
        currentGround = null;

        for (int i = 0; i < grounds.Count; i++)
        {
            grounds[i].gameObject.SetActive(false);
        }

        currentGround = grounds[activeGround - 1];
        currentGround.SetActive(true);


        // Declare ground variable
        Ground groundSctipt = currentGround.GetComponent<Ground>();

        // Assign player spawn position
        playerSpawnPoint = groundSctipt.playerSpawnPos;

        // Assign all the bot spawn position
        botSpawnPoint.Clear();
        for (int i = 0; i < groundSctipt.botCount; i++)
        {
            botSpawnPoint.Add(groundSctipt.allSpawnPoint[i]);
        }

        // Set the player 
        player.ResettingGame();
        player.gameObject.transform.position = playerSpawnPoint.transform.position;
        player.ReassignValue();

        // Set the bot
        for (int i = 0; i < groundSctipt.botCount; i++)
        {
            botAll[i].gameObject.SetActive(false);

            botAll[i].transform.position = botSpawnPoint[i].transform.position;

            botAll[i].GetComponent<Bot_Manager>().ResetingGame();

            botAll[i].GetComponent<Bot_Manager>().ReassignValue();

            botAll[i].gameObject.SetActive(true);
        }
    }

}
