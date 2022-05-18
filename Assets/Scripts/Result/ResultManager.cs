using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private Image cover;
    private Color black = new Color(0.2f, 0.2f, 0.2f, 0.7f);
    private Color none = new Color(0.2f, 0.2f, 0.2f, 0);

    private AudioSource audioSource;

    [SerializeField] private TMP_Text[] textArr;
    [SerializeField] private TMP_Text score;
    [SerializeField] private TMP_Text perfect;
    [SerializeField] private TMP_Text good;
    [SerializeField] private TMP_Text miss;
    [SerializeField] private TMP_Text combo;
    [SerializeField] private TMP_Text accuracy;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BGFadeOut());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator BGFadeOut()
    {
        float t = 0;

        while(t <= 1)
        {
            t += Time.deltaTime / 1;

            cover.color = Color.Lerp(none, black, t);

            yield return null;
        }

        audioSource.Play();

        int missCount = Level.S.noteCount - GameInfo.S.perfect - GameInfo.S.good;
        float acc = ((100 * GameInfo.S.perfect) + (50 * GameInfo.S.good)) / Level.S.noteCount;

        yield return new WaitForSeconds(1f);

        t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime / 1;

            cover.color = Color.Lerp(black, none, t);

            Color color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);

            score.color = color;
            perfect.color = color;
            good.color = color;
            miss.color = color;
            combo.color = color;
            accuracy.color = color;

            for(int i = 0; i < textArr.Length; ++i)
            {
                textArr[i].color = color;
            }

            score.text = Mathf.Lerp(0, GameInfo.S.score, t).ToString();
            perfect.text = Mathf.Lerp(0, GameInfo.S.perfect, t).ToString();
            good.text = Mathf.Lerp(0, GameInfo.S.good, t).ToString();
            miss.text = Mathf.Lerp(0, missCount, t).ToString();
            combo.text = Mathf.Lerp(0, GameInfo.S.maxCombo, t).ToString();
            accuracy.text = Mathf.Lerp(0, acc, t).ToString("f2");

            yield return null;
        }
    }
}
