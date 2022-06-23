using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager S = null;

    private GameObject coverCanvas;
    [SerializeField] private GameObject coverObject;

    [SerializeField] private GameObject quitPopup;

    private void Awake()
    {
        S = this;
        //DontDestroyOnLoad(this.gameObject);
        coverCanvas = transform.GetChild(0).gameObject;
        FadeOut(Color.black, 1.5f);



        Application.wantsToQuit += asdf();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Debug.Log("Quit");
            Application.Quit();

            Image popup = Instantiate(quitPopup).GetComponent<Image>();

            popup.transform.SetParent(coverCanvas.transform);

            popup.transform.position = new Vector2(Screen.width, Screen.height) * 0.5f;

            popup.transform.localScale = Vector2.one;
        }
    }

    private bool asdf()
    {
        return true;
    }

    private void OnApplicationQuit()
    {
        //Application.CancelQuit();

        Image popup = Instantiate(quitPopup).GetComponent<Image>();

        popup.transform.SetParent(coverCanvas.transform);

        popup.transform.position = new Vector2(Screen.width, Screen.height) * 0.5f;

        popup.transform.localScale = Vector2.one;
    }

    public void ChangeScene()
    {

    }
    
    public void MainMenu()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE.MAIN_MENU);
        FadeIn(Color.black, 1, () => { UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE.MAIN_MENU); });
        
    }

    public void StartGame(string levelName, int dif)
    {
        Level.S.ReadLevel(levelName, dif);

        //UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE.GAME_SCENE);

        FadeIn(Color.black, 1, () => { UnityEngine.SceneManagement.SceneManager.LoadScene(SCENE.GAME_SCENE); });
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

    public void FadeIn(Color color, float duration, Action func)
    {
        StartCoroutine(_FadeIn(color, duration, func));
    }
    
    private IEnumerator _FadeIn(Color color, float duration, Action func)
    {
        float t = 0;

        Image cover = InstantiateCoverImage();
        
        cover.color = Utility.SetColorAlpha(color, 0);

        while(t <= 1)
        {
            t += Time.deltaTime / duration;

            cover.color = Utility.SetColorAlpha(cover.color, t);

            yield return null;
        }

        func();
        //Destroy(cover.gameObject);
    }

    public void FadeOut(Color color, float duration)
    {
        StartCoroutine(_FadeOut(color, duration));
    }

    private IEnumerator _FadeOut(Color color, float duration)
    {
        float t = 0;

        Image cover = InstantiateCoverImage();

        cover.color = Utility.SetColorAlpha(color, 1);

        while (t <= 1)
        {
            t += Time.deltaTime / duration;

            cover.color = Utility.SetColorAlpha(cover.color, 1 - t);

            yield return null;
        }

        Destroy(cover.gameObject);
    }

    private Image InstantiateCoverImage()
    {
        Image cover = Instantiate(coverObject).GetComponent<Image>();
        cover.transform.SetParent(coverCanvas.transform);

        cover.transform.position = new Vector2(Screen.width, Screen.height) * 0.5f;
        cover.transform.localScale = Vector2.one * 2;

        return cover;
    }
}
