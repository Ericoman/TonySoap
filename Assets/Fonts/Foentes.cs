using System;
using UnityEngine;
using UnityEngine.UI;

public class Foentes : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private GameObject scoreUIContainer;

    [SerializeField] private GameObject fontPrefab;
    
    private static Foentes instance = null;
    public static Foentes Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        
    }

    public Sprite GetSprite(int index)
    {
        return sprites[index];
    }
    public Sprite GetSprite(string index)
    {
        return sprites[int.Parse(index)];
    }

    public void AddScore(int score)
    {
        foreach (Transform child in scoreUIContainer.transform)
        {
            Destroy(child.gameObject);
        }
        string scoreString = score.ToString();
        char[] scoreSplit = scoreString.ToCharArray();
        foreach (char c in scoreSplit)
        {
            Image image = Instantiate(fontPrefab,scoreUIContainer.transform).GetComponent<Image>();
            image.transform.SetAsLastSibling();
            image.sprite = GetSprite(c.ToString());
        }
    }
}
