using Prime31;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    public float gravity;
    public float speed;
    public float movementSmoothing;
    public float rayDist;

    private Transform groundCheckTransform;
    private CharacterController2D controller;
    private Vector3 velocity;
    private Vector2 currentVelocity = Vector2.zero;

    private bool movingRight = true;

    void Awake()
    {
        controller = GetComponent<CharacterController2D>();
        groundCheckTransform = transform.GetChild(0);
    }

    /*void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction1 = groundCheckTransform.TransformDirection(Vector3.down) * rayDist;
        Gizmos.DrawRay(groundCheckTransform.position, direction1);

        Vector3 direction2 = movingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(groundCheckTransform.position, direction2 * rayDist/2);
    }*/

    void Update()
    {
        float targetSpeed = movingRight ? speed : -speed;

        velocity.y += gravity * Time.deltaTime;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetSpeed, ref currentVelocity.x, movementSmoothing);

        controller.move(velocity * Time.deltaTime);
        velocity = controller.velocity;

        RaycastHit2D groundCheckRaycast = Physics2D.Raycast(groundCheckTransform.position, Vector2.down, rayDist);
        Vector2 obstacleCheckDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D obstacleCheckRaycast = Physics2D.Raycast(groundCheckTransform.position, obstacleCheckDirection, rayDist/2);

        if (obstacleCheckRaycast.collider != null || !groundCheckRaycast)
        {
            Flip();
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
