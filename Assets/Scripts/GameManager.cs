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

    public List<Transform> botSpawnPoint;

    private void Start()
    {
        Time.timeScale = 1f;
        RestartGame(true);
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
        StartCoroutine(StartGameAnim());
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public void RestartGame(bool first)
    {
        Time.timeScale = 1f;
        //for (int i = 0;i < botAll.Count;i++)
        //{
        //    botAll[i].gameObject.SetActive(true);
        //    botAll[i].GetComponent<Bot_Manager>().ResetingGame();
        //}
        for(int i = 0;i < botSpawnPoint.Count;i++)
        {
            botAll[i].gameObject.transform.position = botSpawnPoint[i].transform.position;
            botAll[i].gameObject.SetActive(true);
            botAll[i].GetComponent<Bot_Manager>().ResetingGame();
            botAll[i].GetComponent<Bot_Manager>().ReassignValue();
        }
        player.ResettingGame();
        if (first != true)
        {
            StartGame(); 
        }
    }

    IEnumerator StartGameAnim()
    {
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
        GamePlay = true;
    }

    void BotCount()
    {
        int tempBotCounter = 0;
        for(int i = 0;i < botAll.Count;i++)
        {
            if (botAll[i].gameObject.activeInHierarchy == true)
            {
                tempBotCounter++; 
            }
        }

        if(tempBotCounter == 0)
        {
            RestartGame(false);
        }
    }
}
