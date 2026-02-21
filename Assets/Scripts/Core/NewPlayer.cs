using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RecoveryCounter))]
public class NewPlayer : MonoBehaviour, PlayerMovementInputs.IBasicActions
{
    [Header("Reference")]
    public GameObject attackHit;
    public CameraEffects cameraEffects;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private GameObject graphic;
    [SerializeField] private Component[] graphicSprites;
    [SerializeField] private ParticleSystem jumpParticles;

    private float jumpHoldTime;
    [SerializeField] private float maxJumpHoldTime = 0.8f;
    [SerializeField] private float minJumpMultiplier = 0.2f;
    [SerializeField] private float maxJumpMultiplier = 1f;

    [Header("Asymmetrical Jump")]
    [SerializeField] private float baseGravity = 100f;
    [SerializeField] private float riseGravityMultiplier = 1f;
    [SerializeField] private float fallGravityMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    private bool jumpHeld;

    private PlayerMovementInputs inputs;
    private int drinkedBeer = 0;
    public GameObject explosives;

    private bool isClimbing = false;
    private bool canClimb = false;

    [Header("Properties")]
    public bool dead = false;
    public bool frozen = false;
    private float fallForgivenessCounter; //Counts how long the player has fallen off a ledge
    [SerializeField] private float fallForgiveness = .2f; //How long the player can fall from a ledge and still jump
    [System.NonSerialized] public string groundType = "grass";
    [System.NonSerialized] public RaycastHit2D ground;
    [SerializeField] Vector2 hurtLaunchPower; //How much force should be applied to the player when getting hurt?
    private float launch; //The float added to x and y moveSpeed. This is set with hurtLaunchPower, and is always brought back to zero
    [SerializeField] private float launchRecovery; //How slow should recovering from the launch be? (Higher the number, the longer the launch will last)
    public float maxSpeed = 7; //Max move speed
    public float jumpPower = 17;
    private bool jumping;
    private Vector3 origLocalScale;
    [System.NonSerialized] public bool shooting = false;

    [SerializeField] float attackCooldown = 0.5f;
    private float nextAttack = 0f;

    public bool drunkEffectActive = false;

    [Header("Inventory")]
    private int mBugs;
    public int bugs { get { return mBugs; } set { mBugs = value; PlayerPrefs.SetInt("Bugs", value); } }
    public int health;
    [Range(1, 10)]
    public int maxHealth;

    public GameObject dynamitePrefab;

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
    Vector2 move;

    [SerializeField] private LayerMask climbableLayer;

    private float gravityModifier = 1;
    private Vector2 velocity;
    private Vector2 targetVelocity;
    private Rigidbody2D rb2d;
    private AudioSource audioSource;
    private Animator animator;

    public bool IsGrounded { get { return ground.transform != null; } }


    void Awake()
    {
        inputs = new PlayerMovementInputs();
        inputs.Basic.SetCallbacks(this);
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }
    void OnEnable() => inputs.Basic.Enable();
    void OnDisable() => inputs.Basic.Disable();

    void Start()
    {
        bugs = PlayerPrefs.GetInt("Bugs", 0);
        Cursor.visible = false;
        health = maxHealth;
        origLocalScale = transform.localScale;
        //graphicSprites = GetComponentsInChildren<SpriteRenderer>();
        SetGroundType();
    }

    void Update()
    {
        applyItemEffects();

        jumpHoldTime += Time.deltaTime;
        
        if(!canClimb) computeVelocity();
        if (drunkEffectActive == true) GameManager.Instance.postProcess.DrunkEffect();

        if (!IsGrounded)
        {
            float gravity = baseGravity * gravityModifier;
            if (velocity.y < 0)
                targetVelocity.y -= gravity * fallGravityMultiplier * Time.deltaTime;
            else if (velocity.y > 0)
            {
                if (!jumpHeld)
                    targetVelocity.y -= gravity * lowJumpMultiplier * Time.deltaTime;
                else
                    targetVelocity.y -= gravity * riseGravityMultiplier * Time.deltaTime;
            }
        }
        velocity = Vector2.Lerp(velocity, targetVelocity, Time.deltaTime * (IsGrounded ? 10 : 5));
        rb2d.linearVelocity = velocity;
    }

    void applyItemEffects()
    {
        if (GameManager.Instance.IsItemInInventory(ItemType.Balloon))
            gravityModifier = 2.0f;
        else
            gravityModifier = 3.2f;

        if (GameManager.Instance.DoesInventoryHaveTheSameItems(ItemType.Balloon))
            gravityModifier = -1.5f;
    }

    void flipSpriteInRightDir()
    {
        if (move.x > 0.01f)
            transform.localScale = new Vector3(origLocalScale.x, origLocalScale.y, origLocalScale.z);
        else if (move.x < -0.01f)
            transform.localScale = new Vector3(-origLocalScale.x, origLocalScale.y, origLocalScale.z);
    }

    void computeVelocity()
    {
        ground = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -Vector2.up, 0.1f);

        if (IsGrounded && velocity.y < 0f)
        {
            velocity.y = 0f;
            targetVelocity.y = 0f;
        }
        //Lerp launch back to zero at all times
        launch += (0 - launch) * Time.deltaTime * launchRecovery;

        if (!frozen)
        {
            move.x += launch;
            flipSpriteInRightDir();

            //Allow the player to jump even if they have just fallen off an edge ("fall forgiveness")
            if (!IsGrounded)
            {
                if (fallForgivenessCounter < fallForgiveness && !jumping)
                    fallForgivenessCounter += Time.deltaTime;
                else
                    animator.SetBool("grounded", false);
            }
            else
            {
                fallForgivenessCounter = 0;
                animator.SetBool("grounded", true);
            }

            //Set each animator float, bool, and trigger to it knows which animation to fire
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
            animator.SetFloat("velocityY", velocity.y);
            animator.SetInteger("attackDirectionY", (int)move.y);
            animator.SetInteger("moveDirection", (int)move.x);
            targetVelocity = move * maxSpeed;
        }
        else //If the player is set to frozen, his launch should be zeroed out!
            launch = 0;
    }

    public void MeleeAction()
    {
        if (Time.time < nextAttack) return;
        
        animator.SetTrigger("attack");
        nextAttack = Time.time + attackCooldown;
    }
    public void BalloonAction() => audioSource.PlayOneShot(balloonBreakSound);
    public void BeerAction()
    {
        drinkedBeer++;
        if (drinkedBeer == 2)
            GameManager.Instance.EndGame("2Beers");

        drunkEffectActive = true;
        audioSource.PlayOneShot(drinkingSound);
    }

    public void DynamiteAction()
    {
        if (Vector2.Distance(transform.position, explosives.transform.position) < 5)
            GameManager.Instance.EndGame("BigBoom");
        Instantiate(dynamitePrefab, null);
    }

    public void SetGroundType()
    {
        switch (groundType)
        {
            case "Grass": stepSound = grassSound; break;
        }
    }

    public void Freeze(bool freeze)
    {
        if (freeze)
        {
            animator.SetInteger("moveDirection", 0);
            animator.SetBool("grounded", true);
            animator.SetFloat("velocityX", 0f);
            animator.SetFloat("velocityY", 0f);
            GetComponent<PhysicsObject>().targetVelocity = Vector2.zero;
        }

        frozen = freeze;
        shooting = false;
        launch = 0;
    }


    public void GetHurt(int hurtDirection, int hitPower)
    {
        //If the player is not frozen (ie talking, spawning, etc), recovering, and pounding, get hurt!
        if (!frozen)
        {
            HurtEffect();
            cameraEffects.Shake(100, 1);
            animator.SetTrigger("hurt");
            velocity.y = hurtLaunchPower.y;
            launch = hurtDirection * (hurtLaunchPower.x);

            if (health <= 0)
                StartCoroutine(Die());
            else
                health -= hitPower;

            GameManager.Instance.hud.HealthBarHurt();
        }
    }

    private void HurtEffect()
    {
        GameManager.Instance.audioSource.PlayOneShot(hurtSound);
        StartCoroutine(FreezeFrameEffect());
        GameManager.Instance.audioSource.PlayOneShot(hurtSounds[whichHurtSound]);

        if (whichHurtSound >= hurtSounds.Length - 1)
            whichHurtSound = 0;
        else
            whichHurtSound++;
        
        cameraEffects.Shake(100, 1f);
    }

    public IEnumerator FreezeFrameEffect(float length = .007f)
    {
        Time.timeScale = .1f;
        yield return new WaitForSeconds(length);
        Time.timeScale = 1f;
    }
    public IEnumerator Die()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.EndGame("Death");
    }
    public void ResetLevel()
    {
        Freeze(true);
        dead = false;
        health = maxHealth;
    }
    public void Jump(float jumpMultiplier)
    {
        velocity.y = jumpPower * jumpMultiplier; //The jumpMultiplier allows us to use the Jump function to also launch the player from bounce platforms
        targetVelocity.y = jumpPower * jumpMultiplier; //The jumpMultiplier allows us to use the Jump function to also launch the player from bounce platforms
        PlayJumpSound();
        PlayStepSound();
        JumpEffect();
        jumping = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((climbableLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            canClimb = true;
            gravityModifier = 0;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if ((climbableLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            canClimb = false;
            gravityModifier = 1;
        }
    }
    public void PlayStepSound()
    {
        audioSource.pitch = (Random.Range(0.9f, 1.1f));
        audioSource.PlayOneShot(stepSound, Mathf.Abs(Input.GetAxis("Horizontal") / 10));
    }
    public void PlayJumpSound()
    {
        audioSource.pitch = (Random.Range(1f, 1f));
        GameManager.Instance.audioSource.PlayOneShot(jumpSound, .1f);
    }
    public void JumpEffect()
    {
        jumpParticles.Emit(1);
        audioSource.pitch = (Random.Range(0.6f, 1f));
        audioSource.PlayOneShot(landSound);
    }

    public void LandEffect()
    {
        if (jumping)
        {
            jumpParticles.Emit(1);
            audioSource.pitch = (Random.Range(0.6f, 1f));
            audioSource.PlayOneShot(landSound);
            jumping = false;
        }
    }
    public void FlashEffect() => animator.SetTrigger("flash");
    public void Hide(bool hide)
    {
        Freeze(hide);
        foreach (SpriteRenderer sprite in graphicSprites)
            sprite.gameObject.SetActive(!hide);
    }
    private void OnCollisionExit2D(Collision2D collision) => rb2d.linearVelocity = Vector2.zero;

    public void OnMovement(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        if (canClimb) velocity = input * maxSpeed / 2;
        else move = new Vector2(input.x, move.y);
    }

    private void UseItem(uint placeId)
    {
        var (item, success) = GameManager.Instance.TryGetItemFromInventorySlot(0);
        if (success)
        {
            switch (item)
            {
                case ItemType.Beer: BeerAction(); GameManager.Instance.TryRemoveItemFromInventorySlot(0); break;
                case ItemType.Balloon: BalloonAction(); GameManager.Instance.TryRemoveItemFromInventorySlot(0); break;
                case ItemType.Dynamite: DynamiteAction(); GameManager.Instance.TryRemoveItemFromInventorySlot(0); break;
                case ItemType.Knife: MeleeAction(); break;
                default: break;
            }
        }
    }

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
            jumpHoldTime = 0f;
            jumpHeld = true;
        }

        if (context.canceled)
        {
            jumpHeld = false;

            if (!animator.GetBool("grounded") || jumping)
                return;

            float normalizedHold = jumpHoldTime / maxJumpHoldTime;
            float jumpMultiplier = Mathf.Max(
                minJumpMultiplier,
                Mathf.Lerp(0, maxJumpMultiplier, normalizedHold)
            );

            Jump(jumpMultiplier);
        }
    }

    public void OnCancel(InputAction.CallbackContext context) => GameManager.Instance.TurnOnPauseMenu();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector2 origin = new(transform.position.x, transform.position.y);
        Vector2 direction = -Vector2.up;
        float distance = 0.1f;

        Gizmos.DrawLine(origin, origin + direction * distance);
    }
}