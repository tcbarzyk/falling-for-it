using Prime31;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyMovement : MonoBehaviour
{
    [Header("Movement")]
    public float gravity;
    public float speed;
    public float movementSmoothing;
    public float rayDist;
    public Animator animator;

    [Header("Combat")]
    public float damage;
    public bool canAttack = true;
    public float attackCooldown;
    public float timeToNextAttack = 0f;
    public bool attackingPlayer = false;

    [Header("Death")]
    public GameObject healthPickup;
    public float healthPickupAmount;
    public float healthPickupDropChance;

    private Transform groundCheckTransform;
    private CharacterController2D controller;
    private Vector3 velocity;
    private Vector2 currentVelocity = Vector2.zero;

    private bool movingRight = true;

    void Awake()
    {
        controller = GetComponent<CharacterController2D>();
        groundCheckTransform = transform.GetChild(0);

        controller.onControllerCollidedEvent += onControllerCollider;
        controller.onTriggerEnterEvent += onTriggerEnterEvent;
        controller.onTriggerExitEvent += onTriggerExitEvent;

        animator.Play(Animator.StringToHash("Enemy_Move"));
    }

    /*void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 direction1 = groundCheckTransform.TransformDirection(Vector3.down) * rayDist;
        Gizmos.DrawRay(groundCheckTransform.position, direction1);

        Vector3 direction2 = movingRight ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(groundCheckTransform.position, direction2 * rayDist/2);
    }*/

    void onControllerCollider(RaycastHit2D hit)
    {
        if (hit.normal.y == 1f)
            return;
    }

    void onTriggerEnterEvent(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            attackingPlayer = true;
        }
        else if (col.CompareTag("Bullet"))
        {
            if (col.GetComponent<Bullet>().isPlayerBullet) die();
        }
        //Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
    }

    void onTriggerExitEvent(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            attackingPlayer = false;
        }
        //Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    public void die()
    {
        if (Random.value <= healthPickupDropChance)
        {
            GameObject pickupObj = Instantiate(healthPickup, gameObject.transform.position, Quaternion.identity);
            pickupObj.GetComponent<HealthPickup>().healAmount = Random.Range(healthPickupAmount * 0.8f, healthPickupAmount * 1.2f);
        }
        Destroy(gameObject);
    }


    void Update()
    {
        if (timeToNextAttack < 0) canAttack = true;
        timeToNextAttack -= Time.deltaTime;

        if (!attackingPlayer)
        {
            float targetSpeed = movingRight ? speed : -speed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetSpeed, ref currentVelocity.x, movementSmoothing);
        }
        else
        {
            velocity.x = 0;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.move(velocity * Time.deltaTime);
        velocity = controller.velocity;

        RaycastHit2D groundCheckRaycast = Physics2D.Raycast(groundCheckTransform.position, Vector2.down, rayDist);
        Vector2 obstacleCheckDirection = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D obstacleCheckRaycast = Physics2D.Raycast(groundCheckTransform.position, obstacleCheckDirection, rayDist/2);

        if ((obstacleCheckRaycast.collider != null && obstacleCheckRaycast.collider.tag != "Player") || !groundCheckRaycast)
        {
            Flip();
        }
    }

    private void Flip()
    {
        animator.Play(Animator.StringToHash("Enemy_Move"));
        movingRight = !movingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
