using System;
using System.Collections;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public enum BathroomStates
{
    SceneFadeIn,
    GhostNotInToilet,
    GhostOnDuck,
    GhostToiletToMirror,
    GhostOnMirror,
    GhostMirrorToDoor,
    GhostOnDoor,
    SceneTransitioning
}

public class BathroomManager : MonoBehaviour
{
    // Singletons
    public static BathroomManager Instance;

    [SerializeField] public BathroomStates currentState;

    // Duck & toilet
    [SerializeField] HandleToiletHandle handleToiletHandle;
    [SerializeField] private Animator duckAnim;
    [SerializeField] private Animator toiletAnim;

    // Mirror
    [SerializeField] HandleMirrorInteraction handleMirrorInteraction;
    [SerializeField] private VisualEffect mirrorGhostFogVFX;

    // Mirror aura
    [SerializeField] private GameObject mirrorAura;
    [SerializeField] private Material mirrorAuraMat;
    [SerializeField] private float mirrorAuraChangeDuration;
    private static readonly int On = Shader.PropertyToID("_On");

    // Ghost
    [SerializeField] private Animator ghostAnim;

    // Door
    [SerializeField] Animator doorAnim;
    [SerializeField] private HandleDoorKnob handleDoorKnob;
    [SerializeField] private AudioTrigger ghostLeaveAudioTrigger, doorOpenAudioTrigger;

    // Transition
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform sceneTransitionMark;
    [SerializeField] private Material mat;
    public bool playerHasEnteredTransZone;

    private void Awake()
    {
        if (BathroomManager.Instance != null && Instance != this)
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
        currentState = BathroomStates.SceneFadeIn;

        // Fade in transition
        SceneTransitionManager.Instance.StartFadeInTransition();

        // Scene setup
        StartCoroutine(WaitForSceneSetup());

        // Mirror fog
        mirrorGhostFogVFX.Reinit();
        mirrorGhostFogVFX.Stop();

        // Aura
        mirrorAuraMat.SetFloat(On, 1f); // Aura on
        mirrorAura.SetActive(false);    // Mirror disabled

        // Ghost
        ghostAnim.GetComponent<Transform>().gameObject.SetActive(false);

        // Door
        handleDoorKnob.playCantOpen = true;
        handleDoorKnob.SetHandleRotation(0f, 15f);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(currentState);
        switch (currentState)
        {
            case BathroomStates.SceneFadeIn:
                handleToiletHandle.RotateToiletHandleOnly();
                break;

            case BathroomStates.GhostNotInToilet:
                handleToiletHandle.RotateToiletHandleOnly();
                break;

            case BathroomStates.GhostOnDuck:
                handleToiletHandle.SetHandleAngle(0f, -90f);
                handleToiletHandle.RotateToiletHandle();
                break;

            case BathroomStates.GhostToiletToMirror:
                handleToiletHandle.SetHandleAngle(0f, -15f);
                break;

            case BathroomStates.GhostOnMirror:
                break;

            case BathroomStates.GhostMirrorToDoor:
                break;

            case BathroomStates.GhostOnDoor:
                handleDoorKnob.playCantOpen = false;
                handleDoorKnob.SetHandleRotation(0f, 75f);
                handleDoorKnob.MoveDoorHandle();

                playerHasEnteredTransZone = playerTransform.position.x >= sceneTransitionMark.position.x ? true : false;
                if (playerHasEnteredTransZone) InitSceneTransition();

                break;

            case BathroomStates.SceneTransitioning:

                break;

            default:
                return;
        }
    }

    public void GhostExitToilet()
    {
        if (currentState != BathroomStates.GhostOnDuck)
            return;

        // Change state
        currentState = BathroomStates.GhostToiletToMirror;

        // Ghost
        ghostAnim.GetComponent<Transform>().gameObject.SetActive(true);

        // Trigger ghost move animation
        ghostAnim.SetTrigger("LeaveToilet");

        // Audio

    }

    public void GhostEnterMirror()
    {
        if (currentState != BathroomStates.GhostToiletToMirror)
            return;

        // Change state
        currentState = BathroomStates.GhostOnMirror;
        handleMirrorInteraction.punchCounter = 1;

        // Enable mirror fog
        mirrorGhostFogVFX.Play();

        // Enable mirror aura (_On 0 ~ 1)
        mirrorAura.SetActive(true);         // Mirror enabled
        StartCoroutine(MirrorAuraAppear());

        // Audio

    }

    public void GhostExitMirror()
    {
        if (currentState != BathroomStates.GhostOnMirror)
            return;

        // Change state
        currentState = BathroomStates.GhostMirrorToDoor;

        // Disable mirror fog
        mirrorGhostFogVFX.Stop();

        // Disable mirror aura (disable gameobject)
        StartCoroutine(MirrorAuraDisappear());

        // Audio

    }

    public void GhostEnterDoor()
    {
        if (currentState != BathroomStates.GhostMirrorToDoor)
            return;

        // Change state
        currentState = BathroomStates.GhostOnDoor;

        // Door anim
        doorAnim.SetTrigger("Open1");

        // Audio
        StartCoroutine(WaitForDoorAudio());
    }

    public void InitSceneTransition()
    {
        if (currentState != BathroomStates.GhostOnDoor)
            return;

        // Change state
        currentState = BathroomStates.SceneTransitioning;
        SceneTransitionManager.Instance.StartFadeOutTransition();
    }

    private IEnumerator MirrorAuraAppear()
    {
        // Enable aura gameobject
        mirrorAura.gameObject.SetActive(true);

        // Aura 0 ~ 1
        float timeElapsed = 0f;

        while (timeElapsed < mirrorAuraChangeDuration)
        {
            timeElapsed += Time.deltaTime;
            mirrorAuraMat.SetFloat(On, timeElapsed / mirrorAuraChangeDuration);

            yield return null;
        }

        mirrorAuraMat.SetFloat(On, 1f);
    }

    private IEnumerator MirrorAuraDisappear()
    {
        // Aura 1 ~ 0
        float timeElapsed = 0f;

        while (timeElapsed < mirrorAuraChangeDuration)
        {
            timeElapsed += Time.deltaTime;
            mirrorAuraMat.SetFloat(On, 1 - timeElapsed / mirrorAuraChangeDuration);

            yield return null;
        }

        mirrorAuraMat.SetFloat(On, 0f);

        // Disable aura gameobject
        mirrorAura.gameObject.SetActive(false);
    }

    private IEnumerator WaitForDoorAudio()
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

        // State change
        currentState = BathroomStates.GhostNotInToilet;
    }

}   // End of class
