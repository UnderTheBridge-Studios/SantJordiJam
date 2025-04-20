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
        }
        else if(clientManager.GetClientCount() == 1)
        {
            AudioManager.instance.SetAmbienceParameter("SoundIntensity", 0.40f);
        }
        else if (clientManager.GetClientCount() == 2)
        {
            AudioManager.instance.SetAmbienceParameter("SoundIntensity", 0.60f);
        }
        else if (clientManager.GetClientCount() == 3)
        {
            AudioManager.instance.SetAmbienceParameter("SoundIntensity", 0.80f);
        }
        else if (clientManager.GetClientCount() == 4)
        {
            AudioManager.instance.SetAmbienceParameter("SoundIntensity", 1f);
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
