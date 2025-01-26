using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int score = 0;
    private static ScoreManager instance = null;
    public static ScoreManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
}
