using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AudioCuacker : MonoBehaviour
{
    [SerializeField] private AudioClip[] saltoSounds;
    [SerializeField] private AudioClip[] comboSounds;
    [SerializeField] private AudioClip specialWomboComboSound;
    [SerializeField] private AudioClip trailSound;
    [SerializeField] private AudioClip pumSound;
    [SerializeField]AudioSource playerAudioSource;
    [SerializeField]AudioSource comboAudioSource;
    private static AudioCuacker instance = null;
    public static AudioCuacker Instance => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerAudioSource.clip = trailSound;
        playerAudioSource.loop = true;
        playerAudioSource.Play();
    }

    public void PauseTrail(bool pause)
    {
        if (pause)
        {
            playerAudioSource.Stop();
        }
        else
        {
            if (!playerAudioSource.isPlaying)
            {
                playerAudioSource.Play();
            }
        }
    }
    public void PlaySaltoSound()
    {
        playerAudioSource.PlayOneShot(saltoSounds[Random.Range(0, saltoSounds.Length)]);
    }

    public void PlayComboSound(bool special = false)
    {
        if (!special)
        {
            comboAudioSource.PlayOneShot(comboSounds[Random.Range(0, comboSounds.Length)]);
        }
        else
        {
            comboAudioSource.PlayOneShot(specialWomboComboSound);
        }
    }

    public void PlayPumSound()
    {
        playerAudioSource.PlayOneShot(pumSound);
    }

}
