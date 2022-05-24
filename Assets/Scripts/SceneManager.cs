using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager S = null;

    private void Awake()
    {
        S = this;
    }

    public void ChangeScene()
    {

    }

    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE.MAIN_MENU);
    }

    public void StartGame(string levelName, int dif)
    {
        Level.S.ReadLevel(levelName, dif);

        UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE.GAME_SCENE);
    }

    public void ResultScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE.RESULT_SCENE);

        //GameInfo.S.ShowResult();
    }

    public void Retry()
    {
        StartGame(Level.S.levelName, Level.S.levelDifficulty);
    }
}
