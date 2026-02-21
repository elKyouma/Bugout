using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] float speed = 2f;
    [SerializeField] private float castDistance = 0.1f;

    private Rigidbody2D rb;
    private Collider2D col;
    private float direction = 1.0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.position, Vector2.down, 10f, groundLayer);

        if (hit.collider != null)
        {
            float halfHeight = col.bounds.extents.y;

            Vector2 newPosition = new Vector2(
                rb.position.x,
                hit.point.y + halfHeight
            );

            rb.position = newPosition;
        }
    }

    void FixedUpdate()
    {
        Vector2 castPosition = new Vector2(
            rb.position.x,
            rb.position.y + col.bounds.size.y * 0.1f
        );

        Vector2 castDirection = Vector2.right * direction;

        Vector2 castSize = new Vector2(
            col.bounds.size.x,
            col.bounds.size.y * 0.9f
        );

        RaycastHit2D hitWall = Physics2D.BoxCast(
            castPosition,
            castSize,
            0f,
            castDirection,
            castDistance,
            groundLayer
        );

        RaycastHit2D hitGround = Physics2D.Raycast(
            rb.position,
            Vector2.down + new Vector2(0.1f * direction, 0f),
            col.bounds.extents.y + 0.2f,
            groundLayer
        );

        Debug.DrawRay(
            rb.position,
            (Vector2.down + new Vector2(0.1f * direction, 0f)) * (col.bounds.extents.y + 0.2f),
            Color.red
        );

        if (hitWall.collider != null || hitGround.collider == null)
        {
            direction *= -1f;
            castDirection = Vector2.right * direction;
        }

        rb.MovePosition(rb.position + castDirection * speed * Time.fixedDeltaTime);
    }
}
