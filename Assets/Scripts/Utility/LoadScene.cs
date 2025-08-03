using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void LoadWinScreen()
    {
        SceneManager.LoadScene("WinScreen");
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
