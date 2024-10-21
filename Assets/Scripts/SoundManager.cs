using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource movementSource;
    public AudioSource levelChangeSource;

    public AudioClip movementClip;
    public AudioClip advanceNextLevelClip;
    public AudioClip fallPreviousLevelClip;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public void PlayMovementSound()
    {
        movementSource.PlayOneShot(movementClip);
    }

    public void PlayAdvanceToNextLevelSound()
    {
        levelChangeSource.PlayOneShot(advanceNextLevelClip);
    }

    public void PlayFallPreviousLevelSound()
    {
        levelChangeSource.PlayOneShot(fallPreviousLevelClip);
    }
}
