using Prime31;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // movement config
    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 0.1f;
    public float inAirDamping = 0.5f;
    public float jumpHeight = 3f;
    public float jumpReleaseMultiplier = 3f; // multiplier to control jump height on release
    public float jumpBufferTime = 0.2f;

    [Header("Down Dash")]
    public float downDashMaxSpeed = 5f;
    //public float downDashInitialSpeed = 10f;
    public float downDashSmoothing = 5f;
    //public float downDashTimeToMaxSpeed = 2f;
    public AnimationCurve downDashCurve;
    private float downDashTimePosition;
    private float startingVelocityY;
    public float downDashCooldown;
    public float timeToNextDownDash = 0f;
    public AudioSource diveSound;

    [Header("Fall Damage")]
    public bool justHitGround;
    public float fallDamageVelocityThreshold;
    public float fallDamageFactor;
    public float fallDamageDashResistanceFactor;

    [Header("Interactables")]
    public bool inCobweb = false;
    public float cobwebDamping;
    public float cobwebSpeed;
    public float jumpPadSpeed;
    public bool hitJumpPad = false;
    public float spikeDamage;
    public float spikeDamageCooldown;
    private float timeToNextSpikeDamage;

    [Header("Effects")]
    public ParticleSystem walkEffect;
    public GameObject fallEffect;
    public Transform fallEffectSpawnPoint;
    public AudioSource jumpSound;
    public GameObject winArea;
    public AudioSource winSound;
    public AudioSource enemyAttackSound;
    public AudioSource gameMusic;
    public ParticleSystem dashEffect;
    public GameObject diveLight;

    [HideInInspector]
    private float normalizedHorizontalSpeed = 0;

    // objects
    private CharacterController2D _controller;
    private RaycastHit2D _lastControllerColliderHit;
    private Vector3 _velocity;
    private bool isJumping; // track if player is jumping
    private bool isDownDashing;
    private float jumpBufferCounter = 0f;
    private Vector2 currentVelocity = Vector2.zero;
    private Animator _animator;
    private PlayerCombat combat;

    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
        combat = GetComponent<PlayerCombat>();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
        _controller.onTriggerStayEvent += onTriggerStayEvent;
    }

    #region Event Listeners

    void onControllerCollider(RaycastHit2D hit)
    {
        if (hit.normal.y == 1f)
            return;
    }

    void onTriggerEnterEvent(Collider2D col)
    {
        if (col.CompareTag("HealthPickup"))
        {
            combat.heal(col.GetComponent<HealthPickup>().healAmount);
            col.GetComponent<HealthPickup>().despawn();
        }
        if (col.CompareTag("Cobweb"))
        {
            inCobweb = true;
        }
        if (col.CompareTag("JumpPad"))
        {
            hitJumpPad = true;
        }
        if (col.CompareTag("WinArea"))
        {
            winArea.SetActive(true);
            gameMusic.Stop();
            winSound.Play();
            Time.timeScale = 0f;
        }
        Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
    }

    void onTriggerStayEvent(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            BasicEnemyMovement enemy = col.GetComponent<BasicEnemyMovement>();
            if (enemy.canAttack && !isDownDashing)
            {
                combat.takeHit(enemy.damage);
                enemyAttackSound.Play();
                enemy.timeToNextAttack = enemy.attackCooldown;
                enemy.canAttack = false;
            }
            print("taking damage!");
        }
        if (col.CompareTag("Spikes"))
        {
            if (timeToNextSpikeDamage < 0)
            {
                combat.takeHit(spikeDamage);
                timeToNextSpikeDamage = spikeDamageCooldown;
            }
        }
        Debug.Log("onTriggerStayEvent: " + col.gameObject.name);
    }

    void onTriggerExitEvent(Collider2D col)
    {
        if (col.CompareTag("Cobweb"))
        {
            inCobweb = false;
        }
        Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion

    void Update()
    {
        timeToNextSpikeDamage -= Time.deltaTime;
        timeToNextDownDash -= Time.deltaTime;

        // Reduce buffer timer over time
        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;

        // Reset velocity and buffer on ground contact
        if (_controller.isGrounded)
        {
            _velocity.y = 0;
            isJumping = false;

            // Check for buffered jump
            if (jumpBufferCounter > 0)
            {
                _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
                isJumping = true;
                jumpBufferCounter = 0f; // Reset buffer after jump
                jumpSound.Play();
            }
        }
        else if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack"))
        {
            _animator.Play(Animator.StringToHash("Player_Jump"));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            normalizedHorizontalSpeed = 1;
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack"))
            {
                _animator.Play(Animator.StringToHash("Player_Run"));
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            normalizedHorizontalSpeed = -1;
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack"))
            {
                _animator.Play(Animator.StringToHash("Player_Run"));
            }
        }
        else
        {
            normalizedHorizontalSpeed = 0;
            if (_controller.isGrounded && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack"))
            {
                _animator.Play(Animator.StringToHash("Player_Idle"));
            }
        }

        // Jump input buffering logic
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_controller.isGrounded)
            {
                _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
                isJumping = true;
                if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack"))
                {
                    _animator.Play(Animator.StringToHash("Player_Jump"));
                }
                jumpSound.Play();
            }
            else
            {
                // Start jump buffer if in the air
                jumpBufferCounter = jumpBufferTime;
            }
        }
        else if (isJumping && Input.GetKeyUp(KeyCode.UpArrow))
        {
            // Shorten jump if jump button is released early
            if (_velocity.y > 0f)
            {
                _velocity.y /= jumpReleaseMultiplier;
                isJumping = false;
            }
        }


        if (hitJumpPad)
        {
            if (isDownDashing)
            {
                timeToNextDownDash = downDashCooldown;
                isDownDashing = false;
                dashEffect.Stop();
                diveLight.SetActive(false);
            }
            _velocity.y = jumpPadSpeed;
            isJumping = true;
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Attack"))
            {
                _animator.Play(Animator.StringToHash("Player_Jump"));
            }
            jumpSound.Play();
            hitJumpPad = false;
        }

        //handle downward dash
        if (!_controller.isGrounded && Input.GetKeyDown(KeyCode.X) && timeToNextDownDash < 0 && !isDownDashing) {
            isDownDashing = true;
            startingVelocityY = _velocity.y;
            diveSound.Play();
            dashEffect.Play();
            diveLight.SetActive(true);
        }
        else if (_controller.isGrounded)
        {
            if (justHitGround)
            {
                print("Just hit ground! Velocity: " + _controller.airVelocityY);
                if (_controller.airVelocityY < -fallDamageVelocityThreshold && !hitJumpPad)
                {
                    Instantiate(fallEffect, fallEffectSpawnPoint.position, Quaternion.identity);
                    if (isDownDashing)
                    {
                        combat.takeHit((Mathf.Abs(_controller.airVelocityY * _controller.airVelocityY)) * fallDamageFactor * fallDamageDashResistanceFactor);
                    }
                    else
                    {
                        print(Mathf.Abs(_controller.airVelocityY * _controller.airVelocityY));
                        combat.takeHit((Mathf.Abs(_controller.airVelocityY * _controller.airVelocityY)) * fallDamageFactor);
                    }
                }
                if (isDownDashing)
                {
                    combat.diveAttack();
                    timeToNextDownDash = downDashCooldown;
                }
                justHitGround = false;
            }
            isDownDashing = false;
            dashEffect.Stop();
            diveLight.SetActive(false);
        }

        if (!_controller.isGrounded)
        {
            justHitGround = true;
        }

        // Apply gravity if not dashing down
        if (!isDownDashing)
        {
            downDashTimePosition = 0f;
            var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, normalizedHorizontalSpeed * runSpeed, ref currentVelocity.x, smoothedMovementFactor);
            _velocity.y += gravity * Time.deltaTime;
        }
        //if dashing down, apply down dash velocity
        else
        {   
            //if player is moving up, smooth y velocity to 0
            if (_velocity.y > 0.1f)
            {
                _velocity.y = Mathf.SmoothDamp(_velocity.y, 0, ref currentVelocity.y, downDashSmoothing/2);
                startingVelocityY = _velocity.y;
            }
            //otherwise, accelerate player down
            else
            {
                downDashTimePosition += Time.deltaTime;
                _velocity.y = Mathf.Lerp(startingVelocityY, -downDashMaxSpeed, downDashCurve.Evaluate(downDashTimePosition));
            }
            print(_velocity.y);

            _velocity.x = Mathf.SmoothDamp(_velocity.x, 0, ref currentVelocity.x, downDashSmoothing);
        }

        // Jump down through one-way platforms
        if (_controller.isGrounded && Input.GetKey(KeyCode.DownArrow))
        {
            _velocity.y *= 3f;
            _controller.ignoreOneWayPlatformsThisFrame = true;
        }

        if (inCobweb)
        {
            _velocity.x = Mathf.SmoothDamp(_velocity.x, cobwebSpeed * normalizedHorizontalSpeed, ref currentVelocity.x, cobwebDamping);
            //_velocity.y = Mathf.SmoothDamp(_velocity.y, cobwebSpeed * Mathf.Clamp(_velocity.y, -1, 1), ref currentVelocity.y, cobwebDamping);
            _velocity.y = Mathf.SmoothDamp(_velocity.y, Mathf.Clamp(_velocity.y, -cobwebSpeed, cobwebSpeed), ref currentVelocity.y, cobwebDamping);
        }

        if (Mathf.Abs(_velocity.x) > 1 && _controller.isGrounded && !inCobweb)
        {
            if (walkEffect.isPlaying == false)
            {
                walkEffect.Play();
            }
        }
        else {
            walkEffect.Stop();
        }

        _controller.move(_velocity * Time.deltaTime);
        _velocity = _controller.velocity;
    }
}
