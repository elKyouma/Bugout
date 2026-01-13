using Unity.Cinemachine;
using UnityEngine;

/*Allows the camera to shake when the player punches, gets hurt, etc. Put any other custom camera effects in this script!*/

public class CameraEffects : MonoBehaviour
{
    public Vector3 cameraWorldSize;
    public CinemachinePositionComposer cinemachineFramingTransposer;
    [SerializeField] private CinemachineBasicMultiChannelPerlin multiChannelPerlin;
    public float defaultShake = 1;
    public float defaultShakeLength = 0.2f;
    [Range(0, 10)]
    [System.NonSerialized] public float shakeLength = 10;
    [SerializeField] private CinemachineCamera virtualCamera;

    void Start()
    {
        //Ensures we can shake the camera using Cinemachine. Don't really worry too much about this weird stuff. It's just Cinemachine's variables.
        //cinemachineFramingTransposer = virtualCamera.GetComponent<CinemachinePositionComposer>();

        //Inform the player what CameraEffect it should be controlling, no matter what scene we are on.
        if (Player.Instance)
            Player.Instance.cameraEffects = this;
        //virtualCamera = GetComponent<CinemachineCamera>();
        //multiChannelPerlin = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        virtualCamera.Follow = Player.Instance.transform;
    }

    void Update() => multiChannelPerlin.FrequencyGain += (0 - multiChannelPerlin.FrequencyGain) * Time.deltaTime * (10 - shakeLength);

    public void Shake(float shake, float length)
    {
        shakeLength = length;
        multiChannelPerlin.FrequencyGain = shake;
    }

    public void ShakeS(float length)
    {
        shakeLength = length;
        multiChannelPerlin.FrequencyGain = defaultShake;
    }
}
