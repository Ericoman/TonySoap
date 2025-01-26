using System;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    [SerializeField]
    CanvasGroup cg = null;
    [SerializeField]
    AudioSource audioSource = null;
    [SerializeField]
    AudioClip feedbackClip = null;
    private void Start()
    {
        if (cg != null)
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
            cg.alpha = 0;
        }
    }

    public void ShowHelp(bool show)
    {
        PlayFeedback();
        if (cg != null)
        {
            cg.interactable = show;
            cg.blocksRaycasts = show;
            cg.alpha = show ? 1 : 0;
        }
    }
    public void StartGame()
    {
        PlayFeedback();
        MenuManager.Instance.StartGame();
    }

    public void ExitGame()
    {
        PlayFeedback();
        MenuManager.Instance.QuitGame();
    }

    public void PlayFeedback()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(feedbackClip);
        }
    }
}
