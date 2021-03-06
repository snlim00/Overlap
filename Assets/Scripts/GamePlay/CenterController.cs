using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterController : MonoBehaviour
{
    private float angle;
    private Vector2 target, mouse;

    private void Start()
    {
        target = transform.position;
    }

    private void Update()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouse.y - target.y, mouse.x - target.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        //Debug.Log(transform.eulerAngles.z - 90);
    }
}
