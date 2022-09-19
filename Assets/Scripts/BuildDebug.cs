using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuildDebug : MonoBehaviour
{
    public static BuildDebug S = null;

    [SerializeField] private GameObject textPref;

    // Start is called before the first frame update
    void Start()
    {
        S = this;

        DontDestroyOnLoad(gameObject);

        Log("GameStart");
    }

    public void Log(string text)
    {
        //TMP_Text msg = Instantiate(textPref).GetComponent<TMP_Text>();

        //msg.transform.SetParent(transform);

        //msg.transform.localScale = Vector3.one;

        //msg.transform.position = Camera.main.ViewportToScreenPoint(new Vector3(0.8f, 0.2f, 0));

        //msg.text = text;

        //StartCoroutine(RemoveText(msg.gameObject));
    }

    private IEnumerator RemoveText(GameObject go)
    {
        yield return new WaitForSeconds(5);

        Destroy(go);
    }
}
