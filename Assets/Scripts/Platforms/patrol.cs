using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrol : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public bool shouldFlip;
    public float speed;

    float value;
    SpriteRenderer render;

    void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        value += Time.fixedDeltaTime * speed;
        float lerpValue = Mathf.Abs(Mathf.Abs(value % 2) - 1);
        transform.position = Vector3.Lerp(point1.position, point2.position, lerpValue);
        if(shouldFlip)
        {
            render.flipX = Mathf.FloorToInt(value) % 2 == 0;
        }
    }
}
