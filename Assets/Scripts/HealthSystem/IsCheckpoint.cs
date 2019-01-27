using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCheckpoint : MonoBehaviour
{
    public Sprite sprite;

    public void updateCheckpoint()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}
