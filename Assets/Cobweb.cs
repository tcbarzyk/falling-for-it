using ChristinaCreatesGames.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cobweb : MonoBehaviour
{
    private SquashAndStretch reactEffect;

    // Start is called before the first frame update
    void Start()
    {
        reactEffect = GetComponent<SquashAndStretch>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            reactEffect.PlaySquashAndStretch();
        }
    }
}
