/*using Prime31;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // movement config
    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;

    [HideInInspector]
    private float normalizedHorizontalSpeed = 0;

    //objects
    private CharacterController2D _controller;
    //private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    private Vector3 _velocity;


    void Awake()
    {
        //_animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController2D>();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
    }


    #region Event Listeners

    void onControllerCollider(RaycastHit2D hit)
    {
        // bail out on plain old ground hits cause they arent very interesting
        if (hit.normal.y == 1f)
            return;

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


    void onTriggerEnterEvent(Collider2D col)
    {
        Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
    }


    void onTriggerExitEvent(Collider2D col)
    {
        Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion


    // the Update loop contains a very simple example of moving the character around and controlling the animation
    void Update()
    {
        if (_controller.isGrounded)
            _velocity.y = 0;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            normalizedHorizontalSpeed = 1;
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded)
            {
                //_animator.Play(Animator.StringToHash("Run"));
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            normalizedHorizontalSpeed = -1;
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded)
            {
                //_animator.Play(Animator.StringToHash("Run"));
            }
        }
        else
        {
            normalizedHorizontalSpeed = 0;

            if (_controller.isGrounded)
            {
                //_animator.Play(Animator.StringToHash("Idle"));
            }
        }


        // we can only jump whilst grounded
        if (_controller.isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            //_animator.Play(Animator.StringToHash("Jump"));
        }


        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

        // apply gravity before moving
        _velocity.y += gravity * Time.deltaTime;

        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if (_controller.isGrounded && Input.GetKey(KeyCode.DownArrow))
        {
            _velocity.y *= 3f;
            _controller.ignoreOneWayPlatformsThisFrame = true;
        }

        _controller.move(_velocity * Time.deltaTime);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }
}*/

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

    void Awake()
    {
        _controller = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
    }

    #region Event Listeners

    void onControllerCollider(RaycastHit2D hit)
    {
        if (hit.normal.y == 1f)
            return;
    }

    void onTriggerEnterEvent(Collider2D col)
    {
        Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
    }

    void onTriggerExitEvent(Collider2D col)
    {
        Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion

    void Update()
    {
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
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            normalizedHorizontalSpeed = 1;
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded)
            {
                _animator.Play(Animator.StringToHash("Player_Run"));
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            normalizedHorizontalSpeed = -1;
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded)
            {
                _animator.Play(Animator.StringToHash("Player_Run"));
            }
        }
        else
        {
            normalizedHorizontalSpeed = 0;
            if (_controller.isGrounded)
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

        //handle downward dash
        if (!_controller.isGrounded && Input.GetKeyDown(KeyCode.Z)) {
            isDownDashing = true;
            startingVelocityY = _velocity.y;
        }
        else if (_controller.isGrounded)
        {
            //handle dash collision
            isDownDashing = false;
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

        _controller.move(_velocity * Time.deltaTime);
        _velocity = _controller.velocity;
    }
}
