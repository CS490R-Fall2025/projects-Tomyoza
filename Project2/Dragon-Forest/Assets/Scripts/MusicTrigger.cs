using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    public bool isBossScene = false;

    void Start()
    {
        if (AudioManager.Instance == null) return;

        if (isBossScene)
            AudioManager.Instance.PlayMusic(AudioManager.Instance.bossMusic);
        else
            AudioManager.Instance.PlayMusic(AudioManager.Instance.forestMusic);
    }
}