using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public enum BedroomStates
{
    SceneFadeIn,
    NoGhost,
    GhostOnRadio,
    GhostRadioToDresser,
    GhostOnDresser,
    GhostDresserToDoor,
    GhostOnDoor,
    SceneTransitioning
}

public class BedroomManager : MonoBehaviour
{
    // Singleton
    public static BedroomManager Instance { get; private set; }

    // State machine
    public BedroomStates CurrentState { get; private set; }

    // Bedroom light
    [SerializeField] private Material bedroomLightOn, bedroomLightOff;
    [SerializeField] private Renderer bedroomLightRend;
    [SerializeField] private Vector2 exposureRange;
    [SerializeField] private List<float> bedroomLightFlickerDuration;
    [SerializeField] private Volume volume;
    private ColorAdjustments _colorAdjustments;

    // Radio fog
    [SerializeField] private VisualEffect radioGhostFogVFX;

    // Radio aura
    [SerializeField] private Material radioAuraMat;
    [SerializeField] private Renderer radioAuraRend;
    [SerializeField] private float radioAuraChangeDuration;
    private static readonly int On = Shader.PropertyToID("_On");

    // Ghost
    [SerializeField] private Animator ghostAnim;
    private Coroutine hummingCoroutine;

    // Audio
    [SerializeField] private AudioTrigger radioAudioTrigger, doorOpenAudioTrigger;
    [SerializeField] private AudioTrigger ghostLeaveAudioTrigger, ghostHummingAudioTrigger, ghostPulledAudioTrigger;
    [SerializeField] private AudioSource ghostHummingAudioSource;

    // Dresser
    // [SerializeField] private DresserManager dresserScript;
    [SerializeField] private GameObject drawer0Aura, drawer1Aura, drawer2Aura;
    [SerializeField] private RealDresser dresserScript;

    // Door
    [SerializeField] private HandleDoorKnob handleDoorKnob;
    [SerializeField] private Animator doorAnim;

    // Transition
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform sceneTransitionMark;
    [SerializeField] private Material mat;
    public bool playerHasEnteredTransZone;

    private void Awake()
    {
        if (BedroomManager.Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // State
        CurrentState = BedroomStates.SceneFadeIn;

        // Fade in transition
        SceneTransitionManager.Instance.StartFadeInTransition();

        // Scene setup
        StartCoroutine(WaitForSceneSetup());
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case BedroomStates.SceneFadeIn:
                break;

            case BedroomStates.GhostOnRadio:
                // On radio
                handleDoorKnob.playCantOpen = true;
                handleDoorKnob.SetHandleRotation(0f, 15f);
                break;

            case BedroomStates.GhostRadioToDresser:
                {
                    // From radio to Drawer
                    if (hummingCoroutine != null) StopCoroutine(hummingCoroutine);
                    hummingCoroutine = null;
                    ghostHummingAudioSource.Stop();
                    break;
                }
            case BedroomStates.GhostOnDresser:
                break;
            case BedroomStates.GhostDresserToDoor:
                break;
            case BedroomStates.GhostOnDoor:
                // On door
                handleDoorKnob.playCantOpen = false;
                handleDoorKnob.SetHandleRotation(0f, 75f);
                handleDoorKnob.MoveDoorHandle();

                playerHasEnteredTransZone = playerTransform.position.x >= sceneTransitionMark.position.x ? true : false;
                if (playerHasEnteredTransZone) InitSceneTransition();
                break;
                
            default:
                // No ghost
                break;
        }
    }

    public void SceneSetup()
    {
        // Bedroom light
        volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);
        _colorAdjustments.postExposure.value = exposureRange.y;

        // Radio fog
        radioGhostFogVFX.Reinit();
        radioGhostFogVFX.Stop();

        // Radio aura
        // radioAuraRend.material.SetFloat(On, 0f);
        radioAuraMat.SetFloat(On, 0f);

        // Play audio 
        radioAudioTrigger.PlayAudio();

        // Dresser
        drawer0Aura.SetActive(false);
        drawer1Aura.SetActive(false);
        drawer2Aura.SetActive(false);

        // Ghost
        ghostAnim.GetComponent<Transform>().gameObject.SetActive(false);

        // Countdown towards entering the radio
        GhostEnterRadio();

        // State change
        CurrentState = BedroomStates.GhostOnRadio;
    }

    public void GhostEnterRadio()
    {
        StartCoroutine(EnterRadio());
    }

    private IEnumerator EnterRadio()
    {
        yield return new WaitForSecondsRealtime(19f);

        if (CurrentState != BedroomStates.GhostOnRadio)
        {
            yield break;
        }

        // Change state
        CurrentState = BedroomStates.GhostOnRadio;

        // Fog
        radioGhostFogVFX.Play();

        // Aura
        StartCoroutine(RadioAuraAppear());

        // Lights
        StartCoroutine(LightsFlicker());

        // Set ghost humming coroutine
        hummingCoroutine = StartCoroutine(PlayHummingAudio());
    }

    public void GhostExitRadio()
    {
        if (CurrentState != BedroomStates.GhostOnRadio)
        {
            return;
        }

        ghostAnim.GetComponent<Transform>().gameObject.SetActive(true);

        // Change state
        CurrentState = BedroomStates.GhostRadioToDresser;

        // Stop radio fog
        radioGhostFogVFX.Stop();

        // Stop radio aura
        StartCoroutine(RadioAuraDisappear());

        // Trigger ghost animation
        ghostAnim.SetTrigger("LeaveRadio");

        // Play pulled audio
        ghostPulledAudioTrigger.PlayAudio();
    }

    public void GhostEnterDresser()
    {
        if (CurrentState != BedroomStates.GhostRadioToDresser)
        {
            return;
        }

        // Change bedroom state
        CurrentState = BedroomStates.GhostOnDresser;

        // Activate dresser aura
        drawer0Aura.SetActive(true);
        // drawer1Aura.SetActive(true);
        // drawer2Aura.SetActive(true);

        // Get the ghost into a random drawer
        // dresserScript.PossessADrawer();
        
        // Get the ghost into the first drawer
        dresserScript.InitializeGhostDrawer();
    }

    public void GhostExitDresser()
    {
        if (CurrentState != BedroomStates.GhostOnDresser)
        {
            return;
        }

        // Change state
        CurrentState = BedroomStates.GhostDresserToDoor;

        // Trigger ghost animation
        ghostAnim.SetTrigger("LeaveDresser");
        
        // Wrap up last round
        dresserScript.WrapUpGhostDrawer();
    }

    public void GhostPassDoor()
    {
        if (CurrentState != BedroomStates.GhostDresserToDoor) { return; }

        // Change state
        CurrentState = BedroomStates.GhostOnDoor;

        // Unlock the door
        doorAnim.SetTrigger("Open1");

        // Play audio
        StartCoroutine(WaitForAudio());
    }

    public void InitSceneTransition()
    {
        Debug.Log("Initiating scene transition");

        if (CurrentState != BedroomStates.GhostOnDoor)
            return;

        CurrentState = BedroomStates.SceneTransitioning;
        SceneTransitionManager.Instance.StartFadeOutTransition();
    }

    private IEnumerator RadioAuraAppear()
    {
        // Aura 0 ~ 1
        float timeElapsed = 0f;

        while (timeElapsed < radioAuraChangeDuration)
        {
            timeElapsed += Time.deltaTime;
            radioAuraMat.SetFloat(On, timeElapsed / radioAuraChangeDuration);

            yield return null;
        }

        radioAuraMat.SetFloat(On, 1f);
    }

    private IEnumerator RadioAuraDisappear()
    {
        // Aura 1 ~ 0
        float timeElapsed = 0f;
        while (timeElapsed < radioAuraChangeDuration)
        {
            timeElapsed += Time.deltaTime;
            radioAuraMat.SetFloat(On, 1 - timeElapsed / radioAuraChangeDuration);

            yield return null;
        }

        radioAuraMat.SetFloat(On, 0f);
    }

    // private void ToggleBedroomLight(bool isOn)
    // {
    //     if (isOn)
    //     {
    //         // Light off
    //         bedroomLightRend.material = bedroomLightOff;
    //         _colorAdjustments.postExposure.value = exposureRange.x;
    //     }
    //     else
    //     {
    //         // Light on
    //         bedroomLightRend.material = bedroomLightOn;
    //         _colorAdjustments.postExposure.value = exposureRange.y;
    //     }
    // }

    private IEnumerator LightsFlicker()
    {
        bool lightIsOn = true;
        int flickerCounter = 0;

        while (flickerCounter < 4)
        {
            float targetAlpha = lightIsOn ? 0.9f : 0f;

            Color color = new Color(mat.color.r, mat.color.g, mat.color.b, targetAlpha);
            mat.color = color;

            lightIsOn = !lightIsOn;
            flickerCounter++;

            yield return new WaitForSeconds(0.14f);
        }
    }

    private IEnumerator PlayHummingAudio()
    {
        yield return new WaitForSeconds(3f);

        while (CurrentState == BedroomStates.GhostOnRadio)
        {
            float waitInterval = UnityEngine.Random.Range(7f, 10f);

            yield return new WaitForSeconds(waitInterval);

            ghostHummingAudioTrigger.PlayAudio();

            yield return null;
        }
    }

    private IEnumerator WaitForAudio()
    {
        // Play ghost leave audio
        ghostLeaveAudioTrigger.PlayAudio();

        yield return new WaitForSeconds(1f);

        // Play door open audio
        doorOpenAudioTrigger.PlayAudio();
    }

    private IEnumerator WaitForSceneSetup()
    {
        yield return new WaitForSeconds(3f);
        SceneSetup();
    }

}   // End of class
