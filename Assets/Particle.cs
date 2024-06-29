using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Sprites;

[RequireComponent(typeof(Sprite))]
[RequireComponent(typeof(SpriteRenderer))]

public class Particle : MonoBehaviour
{

    public SpriteRenderer circle;

    private void Awake()
    {
        circle = GetComponent<SpriteRenderer>();
    }
}
