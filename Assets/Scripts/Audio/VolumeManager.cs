using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    [Header("Parameter Change")]
    [SerializeField] private ClientManager clientManager;
    [SerializeField] private GameManager gameManager;

    private void Update()
    {
        // People
        if(clientManager.GetClientCount() == 0)
        {
            AudioManager.instance.SetAmbienceParameter("SoundIntensity", 0.20f);
            AudioManager.instance.SetMusicParameter("SoundIntensity 2", 1f);
        }
        else if(clientManager.GetClientCount() == 1)
        {
            AudioManager.instance.SetAmbienceParameter("SoundIntensity", 0.40f);
            AudioManager.instance.SetMusicParameter("SoundIntensity 2", 0.8f);
        }
        else if (clientManager.GetClientCount() == 2)
        {
            AudioManager.instance.SetAmbienceParameter("SoundIntensity", 0.60f);
            AudioManager.instance.SetMusicParameter("SoundIntensity 2", 0.6f);
        }
        else if (clientManager.GetClientCount() == 3)
        {
            AudioManager.instance.SetAmbienceParameter("SoundIntensity", 0.80f);
            AudioManager.instance.SetMusicParameter("SoundIntensity 2", 0.4f);
        }
        else if (clientManager.GetClientCount() == 4)
        {
            AudioManager.instance.SetAmbienceParameter("SoundIntensity", 1f);
            AudioManager.instance.SetMusicParameter("SoundIntensity 2", 0.2f);
        }

        // Music
        if (gameManager.currentDayTime == DayTime.day)
        {
            AudioManager.instance.SetMusicTime("Day");
        }
        else if (gameManager.currentDayTime == DayTime.night)
        {
            AudioManager.instance.SetMusicTime("Night");
        }
    }


}
