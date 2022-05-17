using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static void ChangeScene()
    {

    }

    public static void StartGame(string levelName, int dif)
    {
        Level.S.ReadLevel(levelName, dif);

        SceneManager.LoadScene(SCENE.GAME_SCENE);
    }

    public static void ResultScene()
    {
        SceneManager.LoadScene(SCENE.RESULT_SCENE);

        //GameInfo.S.ShowResult();
    }
}
