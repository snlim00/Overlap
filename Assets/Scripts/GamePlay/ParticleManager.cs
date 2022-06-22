using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject[] paritclePref;

    [SerializeField] private GameObject CircleEffect;

    public Vector2 perfectTargetSize = new Vector2(0.35f, 0.35f); //����Ʈ �� ��Ÿ���� ����Ʈ�� ��ǥ ������
    public Vector2 goodTargetSize = new Vector2(0.3f, 0.3f); //����Ʈ �� ��Ÿ���� ����Ʈ�� ��ǥ ������
    public Vector2 missTargetSize = new Vector2(0.25f, 0.25f); //����Ʈ �� ��Ÿ���� ����Ʈ�� ��ǥ ������
    private Vector2 slideTargetSize = new Vector2(0.2f, 0.2f);

    [SerializeField] private Color sPerfectEffectColor;
    [SerializeField] private Color perfectEffectColor;
    [SerializeField] private Color goodEffectColor;
    [SerializeField] private Color missEffectColor;
    private Color slideEffectColor;

    public void Start()
    {
        slideEffectColor = perfectEffectColor;
    }

    public void ParticleGeneration(int judg)
    {
        //int judg = 0;

        ParticleSystem particle = Instantiate(paritclePref[judg]).GetComponent<ParticleSystem>();

        particle.Play();

        RemoveParticle(particle.gameObject);
    }

    private IEnumerator RemoveParticle(GameObject gameObject, float waitingTime = 2)
    {
        yield return new WaitForSeconds(waitingTime);

        Destroy(gameObject);
    }

    //public void ParticleGeneration(int judg)
    //{
    //    GameObject go = Instantiate(CircleEffect) as GameObject;
    //    CircleEffectController goCtrl = go.GetComponent<CircleEffectController>();

    //    switch (judg)
    //    {
    //        case JUDG.S_PERFECT:
    //            goCtrl.targetSize = perfectTargetSize;
    //            go.GetComponent<SpriteRenderer>().color = sPerfectEffectColor;
    //            break;

    //        case JUDG.PERFECT:
    //            goCtrl.targetSize = perfectTargetSize;
    //            go.GetComponent<SpriteRenderer>().color = perfectEffectColor;

    //            break;


    //        case JUDG.GOOD:
    //            goCtrl.targetSize = goodTargetSize;
    //            go.GetComponent<SpriteRenderer>().color = goodEffectColor;

    //            break;


    //        case JUDG.MISS:
    //            goCtrl.targetSize = missTargetSize;
    //            go.GetComponent<SpriteRenderer>().color = missEffectColor;

    //            break;

    //        case -1:
    //            goCtrl.targetSize = slideTargetSize;
    //            go.GetComponent<SpriteRenderer>().color = slideEffectColor;
    //            break;
    //    }
    //}
}
