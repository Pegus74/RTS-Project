using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    private AudioSource soldierAttacChannel;
    public AudioClip soldierAttackClip;
    private float soundDelay = 1.0f; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        soldierAttacChannel = gameObject.AddComponent<AudioSource>();
        soldierAttacChannel.volume = 0.1f;
        soldierAttacChannel.playOnAwake = false;
    }

    public void PlaySoldierAttackSound()
    {
        if (!soldierAttacChannel.isPlaying)
        {
            soldierAttacChannel.PlayOneShot(soldierAttackClip);
            StartCoroutine(WaitForSoundToFinish());
        }
    }

    private IEnumerator WaitForSoundToFinish()
    {
        yield return new WaitForSeconds(soundDelay);
    }
}
