using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public void StartGame()
    {
        MenuManager.Instance.StartGame();
    }

    public void ExitGame()
    {
        MenuManager.Instance.QuitGame();
    }
}
