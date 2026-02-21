using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour, IPlayerController, PlayerMovementInputs.IBasicActions
{
    [SerializeField] private ScriptableStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;
        
    [SerializeField] private LayerMask climbableLayer;
    private bool canClimb = false;


    [Header("Sounds")]
    public AudioClip deathSound;
    public AudioClip equipSound;
    public AudioClip grassSound;
    public AudioClip hurtSound;
    public AudioClip[] hurtSounds;
    public AudioClip holsterSound;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip punchSound;
    public AudioClip stepSound;
    public AudioClip balloonBreakSound;
    public AudioClip drinkingSound;
    [System.NonSerialized] public int whichHurtSound;

    private PlayerMovementInputs inputs;


    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    public bool dead = false;
    public float health = 1;
    public float maxHealth = 1;

    #endregion

    private float _time;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        inputs = new PlayerMovementInputs();
        inputs.Basic.SetCallbacks(this);
    }

    void OnEnable() => inputs.Basic.Enable();
    void OnDisable() => inputs.Basic.Disable();
    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    private void GatherInput()
    {
        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _frameInput.JumpDown = false;
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();
        HandleJump();
        HandleDirection();
        if(!canClimb)
            HandleGravity();
        ApplyMovement();
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.IgnorableLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.IgnorableLayer );

        // Hit a Ceiling
        if (ceilingHit) 
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;
        if (!_jumpToConsume && !HasBufferedJump) return;
        if (_grounded || CanUseCoyote) ExecuteJump();
        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion
    #region Horizontal

    private void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            if(canClimb) deceleration *= 10;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);

        if (!canClimb) return;
            
        if (_frameInput.Move.y == 0)
        {
            var deceleration = _stats.AirDeceleration * 10;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, 0, deceleration * Time.fixedDeltaTime);
        }
        else
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, _frameInput.Move.y * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);

    }

    #endregion
    #region Gravity
    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
            _frameVelocity.y = _stats.GroundingForce;
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion
    private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
    private void UseItem(uint placeId)
    {
        var (item, success) = GameManager.Instance.TryGetItemFromInventorySlot(0);
        if (success)
        {
            switch (item)
            {
                case ItemType.Beer: BeerAction(); GameManager.Instance.TryRemoveItemFromInventorySlot(0); break;
                //case ItemType.Balloon: BalloonAction(); GameManager.Instance.TryRemoveItemFromInventorySlot(0); break;
                case ItemType.Dynamite: DynamiteAction(); GameManager.Instance.TryRemoveItemFromInventorySlot(0); break;
                case ItemType.Knife: MeleeAction(); break;
                default: break;
            }
        }
    }
    public void MeleeAction()
    {
        //if (Time.time < nextAttack) return;

        //animator.SetTrigger("attack");
        //nextAttack = Time.time + attackCooldown;
    }
    //public void BalloonAction() => audioSource.PlayOneShot(balloonBreakSound);
    public void BeerAction()
    {
        //drinkedBeer++;
        //if (drinkedBeer == 2)
            //GameManager.Instance.EndGame("2Beers");

        //drunkEffectActive = true;
        //audioSource.PlayOneShot(drinkingSound);
    }
    public void DynamiteAction()
    {
        //if (Vector2.Distance(transform.position, explosives.transform.position) < 5)
            //GameManager.Instance.EndGame("BigBoom");
        //Instantiate(dynamitePrefab, null);
    }
    public void OnMovement(InputAction.CallbackContext context) => _frameInput.Move = context.ReadValue<Vector2>();
    public void OnAction1(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() != 0) UseItem(0);
    }
    public void OnAction2(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() != 0) UseItem(1);
    }
    public void OnAction3(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    public void OnJump(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            _frameInput.JumpDown = true;
            _frameInput.JumpHeld = true;
        }

        if(context.canceled)
            _frameInput.JumpHeld = false;
            
    }

    public void OnCancel(InputAction.CallbackContext context) => GameManager.Instance.TurnOnPauseMenu();

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((climbableLayer.value & (1 << other.gameObject.layer)) != 0)
            canClimb = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((climbableLayer.value & (1 << other.gameObject.layer)) != 0)
            canClimb = false;
    }
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    public Vector2 FrameInput { get; }
}
