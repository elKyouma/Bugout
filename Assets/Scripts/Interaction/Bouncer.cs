using UnityEngine;

/*Causes a bouncy platform to appear via animation if player is within range, allowing the player to bounce!*/

public class Trampoline : MonoBehaviour
{

    private Animator animator;
    [SerializeField] float appearRange = 1f;
    [SerializeField] string requiredItem;
    [SerializeField] private float playerDifferenceX = 1f;
    [SerializeField] float jumpPower = 1.4f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, appearRange);
    }

    void Update()
    {
        playerDifferenceX = Mathf.Abs(Player.Instance.gameObject.transform.position.x - transform.position.x);
        if (playerDifferenceX < appearRange)
        {
            if (GameManager.Instance.inventory.ContainsKey(requiredItem))
            {
                Appear(true);
            }
            else
            {
                Appear(false);
            }
        }
        else
        {
            Appear(false);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == Player.Instance.gameObject && !Player.Instance.frozen)
        {
            Debug.Log(Player.Instance.velocity.y);
            if (Player.Instance.velocity.y < -.5)
            {
                Bounce();
            }
        }
    }

    void Bounce()
    {
        Player.Instance.Jump(jumpPower);
        animator.SetTrigger("bounce");
    }

    void Appear(bool show)
    {
        animator.SetBool("appear", show);
    }
}
