using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundManage : MonoBehaviour
{
    [Space(10)]
    [Header("All Sound manager")]
    [SerializeField] private List<AudioSource> WholePlayer; // Player all audiosource
    [SerializeField] private List<BotSounds> AllBots; // All bots all audio source

    // Change the sound volume
    public void SoundOnOff(float volume)
    {
        for (int playerAudios = 0; playerAudios < WholePlayer.Count; playerAudios++)
        {
            WholePlayer[playerAudios].volume = volume;
        }

        for (int botAudios = 0; botAudios < AllBots.Count; botAudios++)
        {
            for (int botAudio1 = 0; botAudio1 < AllBots[botAudios].BotAudiosource.Count; botAudio1++)
            {
                AllBots[botAudios].BotAudiosource[botAudio1].volume = volume;
            }
        }
    }

    // Play or Stop sound
    public void SoundPlayStop(int status)
    {
        if (status == 1)
        {

            for (int playerAudios = 0; playerAudios < WholePlayer.Count; playerAudios++)
            {
                WholePlayer[playerAudios].Play();
            }

            for (int botAudios = 0; botAudios < AllBots.Count; botAudios++)
            {
                for (int botAudio1 = 0; botAudio1 < AllBots[botAudios].BotAudiosource.Count; botAudio1++)
                {
                    AllBots[botAudios].BotAudiosource[botAudio1].Play();
                }
            } 
        }
        else if (status == 0)
        {

            for (int playerAudios = 0; playerAudios < WholePlayer.Count; playerAudios++)
            {
                WholePlayer[playerAudios].Stop();
            }

            for (int botAudios = 0; botAudios < AllBots.Count; botAudios++)
            {
                for (int botAudio1 = 0; botAudio1 < AllBots[botAudios].BotAudiosource.Count; botAudio1++)
                {
                    AllBots[botAudios].BotAudiosource[botAudio1].Stop();
                }
            }
        }
    }

}
[System.Serializable]
public struct BotSounds
{
    public List<AudioSource> BotAudiosource; // Bot audio source list
}

