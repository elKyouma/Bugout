using UnityEngine;

/*Attach this to any collectable. When it is instantiated from a broken box or dead enemy, 
it will launch. This script also ensures the collectable's trigger is disabled for
a brief period so the player doesn't immediately collect it after instantiation, not knowing what he collected.
*/

public class Ejector : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private BoxCollider2D collectableTrigger;
    private float counter; //Counts to a value, and then allows the collectable can be collected
    public bool launchOnStart;
    private Vector2 launchPower = new Vector2(300, 300);
    private Rigidbody2D rb;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        if (launchOnStart)
        {
            Launch(launchPower);
            collectableTrigger.enabled = false;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Collider2D>().enabled = false;
            collectableTrigger.enabled = true;
        }
    }

    void Update()
    {
        if (collectableTrigger != null && counter > .5f)
            collectableTrigger.enabled = true;
        else if (collectableTrigger != null)
            counter += Time.deltaTime;
    }

    //Called when the cube hits the floor
    void OnCollisionEnter2D(Collision2D col)
    {
        if (launchOnStart && collectableTrigger.enabled)
            audioSource.PlayOneShot(bounceSound, rb.linearVelocity.magnitude / 10 * audioSource.volume);
    }

    //Launch collectable after box explosion at the specificied launch power, multiplied by a random range.
    public void Launch(Vector2 launchPower) => rb.AddForce(new Vector2(launchPower.x * Random.Range(-1f, 1f), launchPower.y * Random.Range(1f, 1.5f)));

}
