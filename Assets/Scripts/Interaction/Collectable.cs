using UnityEngine;

/*Used for coins, health, inventory items, and even ammo if you want to create a gun shooting mechanic!*/

public abstract class Collectable : MonoBehaviour
{
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip bounceSound;
    [SerializeField] protected AudioClip[] collectSounds;
    [SerializeField] protected int itemAmount;
    void Start() => audioSource = GetComponent<AudioSource>();

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == GameManager.Instance.newPlayer.gameObject)
            Collect();

        //Collect me if I trigger with an object tagged "Death Zone", aka an area the player can fall to certain death
        if (col.gameObject.layer == 14)
            Collect();
    }

    public void ObjectDestroy()
    {
        if (collectSounds.Length > 0)
            GameManager.Instance.audioSource.PlayOneShot(collectSounds[Random.Range(0, collectSounds.Length)], Random.Range(.6f, 1f));
        //GameManager.Instance.newPlayer.FlashEffect();

        // If my parent has an Ejector script, it means that my parent is actually what needs to be destroyed, along with me, once collected
        if (transform.parent.GetComponent<Ejector>() != null)
            Destroy(transform.parent.gameObject);
        else
            Destroy(gameObject);
    }

    protected abstract void Collect();
}
