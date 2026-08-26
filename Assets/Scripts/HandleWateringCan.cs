using UnityEngine;

public class HandleWateringCan : MonoBehaviour
{
    [SerializeField] ParticleSystem waterParticles;
    [SerializeField] AudioSource waterAudio;

    private bool isWatering;
    private bool isPlayingWaterAudio;
    private float initialRotationZ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialRotationZ = transform.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        // DetectWatering();

        if (isWatering && !isPlayingWaterAudio)
        {
            waterAudio.Play();
            isPlayingWaterAudio = true;
        }
        else if (!isWatering)
        {
            waterAudio.Stop();
            isPlayingWaterAudio = false;
        }
    }

    public void DetectWatering()
    {
        float angleDiff = Mathf.DeltaAngle(initialRotationZ, transform.eulerAngles.z);
        bool hasAngle = angleDiff <= -45f ? true : false;

        if ((isWatering && hasAngle) || (!isWatering && !hasAngle)) return;

        PlayWatering(hasAngle);
    }

    private void PlayWatering(bool hasAngle)
    {
        if (hasAngle)
        {
            waterParticles.Play();
            isWatering = true;
        }
        else
        {
            waterParticles.Stop();
            isWatering = false;
        }
    }
}
