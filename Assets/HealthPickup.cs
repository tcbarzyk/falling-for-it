using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public float healAmount;
    public void despawn()
    {
        GameObject.Find("PickupAudioSource").GetComponent<AudioSource>().Play();
        Destroy(gameObject);
    }
}