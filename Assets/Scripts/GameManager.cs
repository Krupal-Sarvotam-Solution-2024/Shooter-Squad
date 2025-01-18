using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool GamePlay;

    public GameObject panelStart, panelPause;

    public List<Bot_Manager> botAll;
    public Player_Manager player;

    public Text GameStartAnimText;

    private void Start()
    {
        Time.timeScale = 1f;
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

    public void RestartGame()
    {
        Time.timeScale = 1f;
        for (int i = 0;i < botAll.Count;i++)
        {
            botAll[i].GetComponent<Bot_Manager>().ResetingGame();
        }
        player.ResettingGame();
        StartGame();
        StartCoroutine(StartGameAnim());
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
            RestartGame();
            Debug.Log(tempBotCounter + " IF");
        }
        else
        {
            Debug.Log(tempBotCounter);
        }
    }
}
