using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioCuacker : MonoBehaviour
{
    [SerializeField] private AudioClip[] trucoSounds;
    [SerializeField] private AudioClip[] comboSounds;
    [SerializeField]AudioSource audioSource;
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

    public void PlayTrucoSound()
    {
        audioSource.PlayOneShot(trucoSounds[Random.Range(0, trucoSounds.Length)]);
    }

    public void PlayComboSound()
    {
        audioSource.PlayOneShot(comboSounds[Random.Range(0, comboSounds.Length)]);
    }

}
