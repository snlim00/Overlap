using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectrum : MonoBehaviour
{
    [SerializeField] private GameObject barPref;

    private RectTransform[] barArr = new RectTransform[64];

    private AudioSource audioSource;

    private float[] samples = new float[64];

    private float sensivisity = 1;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        sensivisity = Screen.width / 1920;

        for(int i = 0; i < barArr.Length; ++i)
        {
            barArr[i] = Instantiate(barPref).GetComponent<RectTransform>();

            barArr[i].parent = transform;

            barArr[i].sizeDelta = new Vector2(5, 1);

            barArr[i].transform.position = Camera.main.ViewportToScreenPoint(new Vector2(i / (float)barArr.Length + 0.005f, 0));
        }

        audioSource.Play();
    }
    // Update is called once per frame
    void Update()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Rectangular);

        for(int i = 0; i < samples.Length; ++i)
        {
            barArr[i].sizeDelta = new Vector2(5 * sensivisity, (samples[i] * 1400 + 5) * sensivisity);
        }
    }
}
