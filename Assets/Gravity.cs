using Prime31;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public float gravity;

    private CharacterController2D controller;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity.y += gravity * Time.deltaTime;

        controller.move(velocity * Time.deltaTime);
        velocity = controller.velocity;
    }
}
