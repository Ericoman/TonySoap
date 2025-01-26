using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public float maxGameTime = 120.0f;
    [SerializeField]private float gameTime = 0.0f;
        
    public Image timeSlider;
    
    private bool isGameOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameTime = maxGameTime;
        
        if (timeSlider != null)
        {
            timeSlider.fillAmount = maxGameTime; // Set slider max value
            //timeSlider.value = maxGameTime;   // Set it to start full
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            gameTime -= Time.deltaTime; // Decrease the timer

            // Clamp gameTime to prevent negative numbers
            gameTime = Mathf.Clamp(gameTime, 0.0f, maxGameTime);

            // Update the slider value if it is assigned
            if (timeSlider != null)
            {
                timeSlider.fillAmount = gameTime / maxGameTime; // This normalizes the fill between 0 and 1
            }
        }

        if (gameTime <= 0.0f)
        {
            MenuManager.Instance.EndGame();
            isGameOver = true;
        }
    }

    public void AddTime()
    {
        gameTime += 2.0f;
    }
}
