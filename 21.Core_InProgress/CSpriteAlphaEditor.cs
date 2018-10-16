using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CSpriteAlphaEditor : MonoBehaviour
{
    [Range(0, 1f)]
    public float fAlpha;

    private void Update()
    {
        SpriteRenderer[] arrSpriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < arrSpriteRenderer.Length; i++)
        {
            Color pColorOrigin = arrSpriteRenderer[i].color;
            pColorOrigin.a = fAlpha;
            arrSpriteRenderer[i].color = pColorOrigin;
        }
    }
}
