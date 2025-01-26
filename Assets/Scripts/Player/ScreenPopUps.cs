using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ScreenPopUps : MonoBehaviour
{
    [SerializeField] public GameObject duckLeftGreat;
    [SerializeField] public GameObject duckRightGreat;
    [SerializeField] public GameObject duckRightNice;
    [SerializeField] public GameObject duckLeftNice;
    [SerializeField] public GameObject duckRightQuaktastik;
    [SerializeField] public GameObject duckLeftQuaktastik;

    private Image duckRightQuaktastikImage;
    private Image duckLeftQuaktastikImage;
    
    [SerializeField] private Sprite duckFrame1;
    [SerializeField] private Sprite duckFrame2;
    
    
    private bool isAnimating = false;

    [SerializeField]public int currentState = 0;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        duckRightQuaktastikImage = duckRightQuaktastik.GetComponent<Image>();
        duckLeftQuaktastikImage = duckLeftQuaktastik.GetComponent<Image>();
        
        duckLeftGreat.SetActive(false);
        duckRightGreat.SetActive(false);
        duckRightNice.SetActive(false);
        duckLeftNice.SetActive(false);
        duckRightQuaktastik.SetActive(false);
        duckLeftQuaktastik.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        

        if (currentState == 0)
        {
            duckLeftGreat.SetActive(false);
            duckRightGreat.SetActive(false);
            duckRightNice.SetActive(false);
            duckLeftNice.SetActive(false);
            duckRightQuaktastik.SetActive(false);
            duckLeftQuaktastik.SetActive(false);
            StopAnimation();
        }
    }

    public void ComboThreshold()
    {
        if (currentState < 3)
        {
            currentState++;
        }
        else
        {
            currentState = 0;
        }
        
        UpdateImages();
    }

    private void UpdateImages()
    {
        switch (currentState)
        {
            case 1:
                duckLeftGreat.SetActive(true);
                duckRightGreat.SetActive(true);
                duckRightNice.SetActive(false);
                duckLeftNice.SetActive(false);
                duckRightQuaktastik.SetActive(false);
                duckLeftQuaktastik.SetActive(false);
                break;
            
            case 2:
                duckLeftGreat.SetActive(false);
                duckRightGreat.SetActive(false);
                duckRightNice.SetActive(true);
                duckLeftNice.SetActive(true);
                duckRightQuaktastik.SetActive(false);
                duckLeftQuaktastik.SetActive(false);
                break;
            
            case 3:
                duckLeftGreat.SetActive(false);
                duckRightGreat.SetActive(false);
                duckRightNice.SetActive(false);
                duckLeftNice.SetActive(false);
                duckRightQuaktastik.SetActive(true);
                duckLeftQuaktastik.SetActive(true);
                StartAnimation();
                break;
            default:
                
               
                break;
        }
    }
    public void StartAnimation()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            StartCoroutine(SwapSpritesContinuously());
        }
    }

    public void StopAnimation()
    {
        isAnimating = false;
        StopCoroutine(SwapSpritesContinuously()); // Stops any ongoing coroutine
    }

    private IEnumerator SwapSpritesContinuously()
    {
        while (isAnimating)
        {
            // Swap to frame 1
            duckLeftQuaktastikImage.sprite = duckFrame1;
            duckRightQuaktastikImage.sprite = duckFrame1;

            // Wait for 0.5 seconds
            yield return new WaitForSeconds(0.2f);

            // Swap to frame 2
            duckLeftQuaktastikImage.sprite = duckFrame2;
            duckRightQuaktastikImage.sprite = duckFrame2;

            // Wait for 0.5 seconds
            yield return new WaitForSeconds(0.2f);
        }
    }
}
