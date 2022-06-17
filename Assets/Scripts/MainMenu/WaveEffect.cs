using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveEffect : MonoBehaviour
{
    [SerializeField] private GameObject wavePref;

    private Image lastWave = null;

    [SerializeField] private Color[] colors;

    private float waveDuration = 0.5f;

    private float maxSize = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator SpawnCircle(int dif)
    {
        float t = 0;

        StartCoroutine(SpawnWave(dif));

        Image wave = InstantiateWave();
        Image lastWave = this.lastWave;

        this.lastWave = wave;

        while(t <= 1)
        {
            t += Time.deltaTime / (waveDuration * 1.825f);

            if(lastWave != null)
            {
                lastWave.color = Utility.SetColorAlpha(lastWave.color, 1 - t);
            }

            wave.color = Utility.SetColorAlpha(colors[dif], t);
            wave.transform.localScale = Vector2.Lerp(Vector2.zero, new Vector2(maxSize, maxSize), Utility.LerpValue(t, 1));
            //Debug.Log(Utility.LerpValue(t, 1));
            yield return null;
        }

        t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime / 0.3f;

            wave.transform.localScale = Vector2.Lerp(new Vector2(10, 10), new Vector2(maxSize * 1.1f, maxSize * 1.1f), Utility.LerpValue(t, 2));
            yield return null;
        }

        if (lastWave != null)
            Destroy(lastWave.gameObject);
    }

    public IEnumerator SpawnWave(int dif)
    {
        yield return new WaitForSeconds(0.3f);

        for(int i = 0; i < 8; ++i)
        {
            yield return new WaitForSeconds(0.33f - (i / 50f));
            StartCoroutine(_SpawnWave(dif, maxSize * 0.7f - (i * 0.7f), 1 / (i + 1f)));
        }
    }

    private IEnumerator _SpawnWave(int dif, float size, float alpha)
    {
        float t = 0;
        //Debug.Log(alpha);
        Image wave = InstantiateWave();

        while(t <= 1)
        {
            t += Time.deltaTime / (waveDuration * 1.1f);

            wave.color = Utility.SetColorAlpha(colors[dif], Utility.LerpValue(t, 2) * alpha);
            wave.transform.localScale = Vector2.Lerp(Vector2.zero, new Vector2(size, size), Utility.LerpValue(t, 0));
            
            yield return null;
        }

        Destroy(wave.gameObject);
    }

    private Image InstantiateWave()
    {
        Image wave = Instantiate(wavePref).GetComponent<Image>();

        wave.transform.SetParent(transform);
        wave.transform.localPosition = Vector2.zero;
        wave.transform.localScale = Vector2.zero;

        return wave;
    }
}
