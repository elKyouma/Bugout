using UnityEngine;
using UnityEngine.SceneManagement;

/*This script can be used on pretty much any gameObject. It provides several functions that can be called with 
animation events in the animation window.*/

public class AnimatorFunctions : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private Animator setBoolInAnimator;

    public void PunchEffect()
    {
        //gsf
    }

    public void PlayStepSound()
    {
        //heh
    }

    void Start()
    {
        if (Player.Instance)
            if (!audioSource)
                audioSource = Player.Instance.audioSource;
    }
    public void HidePlayer(bool hide) => Player.Instance.Hide(hide);

    //Sometimes we want an animated object to force the player to jump, like a jump pad.
    public void JumpPlayer(float power = 1f) => Player.Instance.Jump(power);
    //Freeze and unfreeze the player movement
    void FreezePlayer(bool freeze) => Player.Instance.Freeze(freeze);
    void PlaySound(AudioClip whichSound) => audioSource.PlayOneShot(whichSound);
    public void LandEffect() {/*lol*/}
    public void PoundEffect() {/*lol2*/}
    public void EmitParticles(int amount) => particleSystem.Emit(amount);
    public void ScreenShake(float power) => Player.Instance.cameraEffects.Shake(power, 1f);
    public void SetTimeScale(float time) => Time.timeScale = time;
    public void SetAnimBoolToFalse(string boolName) => setBoolInAnimator.SetBool(boolName, false);
    public void SetAnimBoolToTrue(string boolName) => setBoolInAnimator.SetBool(boolName, true);
    public void FadeOutMusic() => GameManager.Instance.gameMusic.GetComponent<AudioTrigger>().maxVolume = 0f;
    public void LoadScene(string whichLevel) => SceneManager.LoadScene(whichLevel);
    public void SetTimeScaleTo(float timeScale) => Time.timeScale = timeScale;
}
