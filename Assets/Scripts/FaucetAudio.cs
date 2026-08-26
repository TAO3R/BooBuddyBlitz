using Oculus.Interaction;
using UnityEngine;

public class FaucetAudio : MonoBehaviour
{
    [SerializeField] AudioSource faucetWaterAudio;

    private AudioTrigger faucetAudio;

    void PlayAudio()
    {
        if (faucetWaterAudio != null && !faucetWaterAudio.isPlaying) faucetWaterAudio.Play();
    }

    void StopAudio()
    {
         if (faucetWaterAudio != null && faucetWaterAudio.isPlaying) faucetWaterAudio.Stop();
    }
}
