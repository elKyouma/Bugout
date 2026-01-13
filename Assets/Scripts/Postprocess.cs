using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class Postprocess : MonoBehaviour
{
    private VolumeProfile volumeProfile;
    [SerializeField] int bugsAmount = 5;
    FilmGrain filmGrain;
    float minGrain = 0f;
    ChromaticAberration chromaticAberration;
    float minDistortion = 0f;
    LensDistortion lensDistortion;
    float minAbberation = 0f;
    Bloom bloom;
    MotionBlur motionBlur;
    AudioSource source;

    bool gettingDrunk = false;

    private static Postprocess instance;
    public static Postprocess Instance
    {
        get
        {
            if (instance == null) instance = FindFirstObjectByType<Postprocess>();
            return instance;
        }
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
        volumeProfile = GetComponent<Volume>()?.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(VolumeProfile));
    }



    public void MultiplyBugEffect()
    {
        if (!volumeProfile.TryGet(out chromaticAberration)) throw new System.NullReferenceException(nameof(chromaticAberration));
        if (minAbberation < 2.0f)
        {
            minAbberation += 1.0f / bugsAmount;
            chromaticAberration.intensity.Override(minAbberation);
        }

        if (!volumeProfile.TryGet(out lensDistortion)) throw new System.NullReferenceException(nameof(lensDistortion));
        if (minDistortion < 0.5f)
        {
            minDistortion += 0.5f / bugsAmount;
            lensDistortion.intensity.Override(minDistortion);
        }
        if (!volumeProfile.TryGet(out filmGrain)) throw new System.NullReferenceException(nameof(filmGrain));
        if (minGrain < 1.0f)
        {
            minGrain += 1.0f / bugsAmount;
            filmGrain.intensity.Override(minGrain);
        }
    }

    public void ResetEffects()
    {
        if (!volumeProfile.TryGet(out bloom)) throw new System.NullReferenceException(nameof(bloom));
        bloom.intensity.Override(0);

        if (!volumeProfile.TryGet(out chromaticAberration)) throw new System.NullReferenceException(nameof(chromaticAberration));
        chromaticAberration.intensity.Override(0);


        if (!volumeProfile.TryGet(out motionBlur)) throw new System.NullReferenceException(nameof(motionBlur));
        motionBlur.intensity.Override(0);
    }
    public void TurnOnDrunkEffect()
    {
        gettingDrunk = true;
    }
    private void Update()
    {
        if (gettingDrunk)
            DrunkEffect();

    }

    public void DrunkEffect()
    {
        if (!volumeProfile.TryGet(out bloom)) throw new System.NullReferenceException(nameof(bloom));
        bloom.intensity.Override((Mathf.Sin(Time.time) + 1) * 15f);

        if (!volumeProfile.TryGet(out chromaticAberration)) throw new System.NullReferenceException(nameof(chromaticAberration));
        chromaticAberration.intensity.Override(((Mathf.Sin(Time.time) / 2) + 0.5f) * 2f + minAbberation);


        if (!volumeProfile.TryGet(out motionBlur)) throw new System.NullReferenceException(nameof(motionBlur));
        motionBlur.intensity.Override(0.2f);

        if (Player.Instance)
            Player.Instance.cameraEffects.Shake(5, 0.5f);

    }

    public void TrueEndingSequence()
    {
        Player.Instance.Freeze(true);
        StartCoroutine("TrueEnding");
    }


    IEnumerator TrueEnding()
    {
        source.Play();
        if (!volumeProfile.TryGet(out lensDistortion)) throw new System.NullReferenceException(nameof(lensDistortion));

        lensDistortion.scale.Override(lensDistortion.scale.value / 2);
        lensDistortion.intensity.Override(lensDistortion.intensity.value + 0.1f);
        yield return new WaitForSeconds(2);

        source.Play();
        if (!volumeProfile.TryGet(out lensDistortion)) throw new System.NullReferenceException(nameof(lensDistortion));

        lensDistortion.scale.Override(lensDistortion.scale.value / 2);
        lensDistortion.intensity.Override(lensDistortion.intensity.value + 0.1f);
        yield return new WaitForSeconds(2);
        source.Play();
        if (!volumeProfile.TryGet(out lensDistortion)) throw new System.NullReferenceException(nameof(lensDistortion));

        lensDistortion.scale.Override(lensDistortion.scale.value / 2);
        lensDistortion.intensity.Override(lensDistortion.intensity.value + 0.1f);
        yield return new WaitForSeconds(2);
        source.Play();
        if (!volumeProfile.TryGet(out lensDistortion)) throw new System.NullReferenceException(nameof(lensDistortion));

        lensDistortion.scale.Override(lensDistortion.scale.value / 2);
        lensDistortion.intensity.Override(lensDistortion.intensity.value + 0.1f);
        yield return new WaitForSeconds(2);
        source.Play();
        if (!volumeProfile.TryGet(out lensDistortion)) throw new System.NullReferenceException(nameof(lensDistortion));

        lensDistortion.scale.Override(lensDistortion.scale.value / 2);
        lensDistortion.intensity.Override(lensDistortion.intensity.value + 0.1f);
        yield return new WaitForSeconds(0.2f);
        GameManager.Instance.EndGame("TrueEnding");
    }
}
