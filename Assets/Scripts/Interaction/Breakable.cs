using UnityEngine;

//Allows object to break after depleting its "health".

[RequireComponent(typeof(RecoveryCounter))]

public class Breakable : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite brokenSprite; //If destroyAfterDeath is false, a broken sprite will appear instead
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private bool destroyAfterDeath = true; //If false, a broken sprite will appear instead of complete destruction
    public int health;
    [SerializeField] private Instantiator instantiator;
    [SerializeField] private AudioClip hitSound;
    private bool recovered;
    [SerializeField] private RecoveryCounter recoveryCounter;
    [SerializeField] private bool requireDownAttack;
    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        recoveryCounter = GetComponent<RecoveryCounter>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void GetHurt(int hitPower)
    {
        if (health > 0 && !recoveryCounter.recovering)
        {
            if (!requireDownAttack || (requireDownAttack))
            {

                if (hitSound != null)
                    GameManager.Instance.audioSource.PlayOneShot(hitSound);

                recoveryCounter.counter = 0;

                health -= 1;
                animator.SetTrigger("hit");

                if (health <= 0)
                    Die();
            }
        }
    }

    public void Die()
    {
        Time.timeScale = 1;
        deathParticles.SetActive(true);
        deathParticles.transform.parent = null;

        if (instantiator != null)
            instantiator.InstantiateObjects();

        if (destroyAfterDeath)
            Destroy(gameObject);
        else
            spriteRenderer.sprite = brokenSprite;
    }
}
