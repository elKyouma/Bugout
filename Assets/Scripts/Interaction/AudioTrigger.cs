using System.Collections;
using UnityEngine;

/*Use on any gameObject to fade in and out an audioClip. Used for things like
ambience and music.*/

public class AudioTrigger : MonoBehaviour
{

    private AudioSource audioSource;
    [SerializeField] private bool autoPlay; //Begins playing sound immediately without the player triggering the collider
    [SerializeField] private bool controlsTitle; //This allows the level title to fade in while also fading in the music
    [SerializeField] private float fadeSpeed;
    [SerializeField] private bool loop;
    [SerializeField] private AudioClip sound;
    public float maxVolume; //The volume we are going to fade to
    private bool triggered; //Is set to true once the player touches the collider trigger zone

    PlayerController player;
    void Start()
    {
        Reset(false, sound, 0);
        StartCoroutine(EnableCollider());
        player = GameManager.Instance.newPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.loop = loop;
        /*If the player isn't dead, and we either trigger or want to 
        AudioTrigger to automatically play, the audioSource will begin playing.
        */

        if (!player.dead)
        {
            if (triggered || autoPlay)
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();

                //Begin fading in the audioSource volume as long as it's smaller than the goToVolume
                if (audioSource.volume < maxVolume)
                    audioSource.volume += fadeSpeed * Time.deltaTime;
            }
            else
            {
                if (audioSource.volume > 0)
                    audioSource.volume -= fadeSpeed * Time.deltaTime;
                else
                    audioSource.Stop();
            }
        }
        else
            audioSource.Stop();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject == player.gameObject)
        {
            if (!triggered)
            {
                if (controlsTitle)
                    GameManager.Instance.hud.ShowTitle();

                triggered = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject == player.gameObject)
            triggered = false;
    }

    //Find the audioSource, set it's volume and clip, and determine if it should start or stop playing.
    public void Reset(bool play, AudioClip clip, float startVolume = 1)
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = startVolume;
        audioSource.clip = clip;
        if (play)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(4f);
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
