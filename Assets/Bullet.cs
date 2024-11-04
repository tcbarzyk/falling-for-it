using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10.0f;
    public float timeToDestroy = 10.0f;
    public float damage = 1f;
    public bool isPlayerBullet = false;

    public ParticleSystem hitEffect;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine("DestroyCountdown");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = (transform.up * bulletSpeed);
        //print(rb.velocity);
    }

    IEnumerator DestroyCountdown()
    {
        yield return new WaitForSeconds(timeToDestroy);
        DestroyBullet();
    }

    private void DestroyBullet()
    {
        Instantiate(hitEffect, transform.position, transform.rotation);
        GameObject.Find("BulletAudioSource").GetComponent<AudioSource>().Play();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Platform") || other.CompareTag("Spikes"))
        {
            DestroyBullet();
        }
    }
}