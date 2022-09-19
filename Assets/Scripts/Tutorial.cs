using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    [SerializeField] private GameObject[] tutorialPref;

    private GameObject curPage;

    private void Start()
    {
        //GetComponent<Button>().onClick.AddListener(FirstPage);
    }

    public void FirstPage()
    {
        Debug.Log("1");
        GameObject go = Instantiate(tutorialPref[0]);

        go.transform.SetParent(canvas.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(SecondPage);

        curPage = go;
    }
    
    private void SecondPage()
    {
        Debug.Log("2");
        Destroy(curPage);

        GameObject go = Instantiate(tutorialPref[1]);

        go.transform.SetParent(canvas.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(ThirdPage);

        curPage = go;
    }
    
    private void ThirdPage()
    {
        Debug.Log("3");
        Destroy(curPage);

        GameObject go = Instantiate(tutorialPref[2]);

        go.transform.SetParent(canvas.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        curPage = go;
        go.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => Destroy(curPage));
    }
}
