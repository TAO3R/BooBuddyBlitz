using UnityEngine;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine.InputSystem;

public class RoseWatering : MonoBehaviour
{
    [SerializeField] ParticleSystem waterParticles;
    [SerializeField] FlipBookMeshArrayManual roseFlipBookMeshArrayManual;
    [SerializeField] FlipBookMeshArrayManual rootFlipBookMeshArrayManual;
    [SerializeField] AudioTrigger magicHealingAudioTrigger;

    private float waterCounter;
    private int audioCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roseFlipBookMeshArrayManual.CurrentTime = 0f;
        rootFlipBookMeshArrayManual.CurrentTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        SetBloomValue();

        if (roseFlipBookMeshArrayManual.CurrentTime >= 6f && audioCounter == 0)
        {
            magicHealingAudioTrigger.PlayAudio();
            audioCounter++;
            SunroomManager.Instance.GhostExitFlower();
        }
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("WateringWater"))
        {
            waterCounter += .01f;
        }
    }

    private void SetBloomValue()
    {
        roseFlipBookMeshArrayManual.CurrentTime = Mathf.Clamp(waterCounter, 0f, 6.26f);
        rootFlipBookMeshArrayManual.CurrentTime = Mathf.Clamp(waterCounter, 0f, 6.26f);
    }
}
