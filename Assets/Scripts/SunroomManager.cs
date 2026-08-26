using System.Collections;
using Oculus.Interaction;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

public enum SunroomStates
{
    SceneFadeIn,
    GhostOnRose,
    GhostRoseToTelevision,
    GhostOnTelevision
}

public class SunroomManager : MonoBehaviour
{
    // Singleton
    public static SunroomManager Instance;

    [SerializeField] public SunroomStates currentState;

    // Rose
    [SerializeField] private GameObject roseGhostFogVFX;

    // Ghost
    [SerializeField] private Animator ghostAnim;

    private void Awake()
    {
        if (SunroomManager.Instance != null && Instance != this)
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
        currentState = SunroomStates.SceneFadeIn;

        // Fade in transition
        SceneTransitionManager.Instance.StartFadeInTransition();

        // Scene setup
        StartCoroutine(WaitForSceneSetup());

        // Rose fog
        roseGhostFogVFX.SetActive(true);

        // Ghost
        ghostAnim.GetComponent<Transform>().gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(currentState);
        switch (currentState)
        {
            case SunroomStates.SceneFadeIn:
                break;

            case SunroomStates.GhostOnRose:
                break;

            case SunroomStates.GhostRoseToTelevision:
                break;

            case SunroomStates.GhostOnTelevision:
                break;

            default:
                return;
        }
    }

    public void GhostExitFlower()
    {
        if (currentState != SunroomStates.GhostOnRose) return;

        roseGhostFogVFX.SetActive(false);

        currentState = SunroomStates.GhostRoseToTelevision;

        ghostAnim.GetComponent<Transform>().gameObject.SetActive(true);

        ghostAnim.SetTrigger("LeaveRose");
    }

    public void GhostEnterTelevision()
    {
        if (currentState != SunroomStates.GhostRoseToTelevision) return;

        currentState = SunroomStates.GhostOnTelevision;

        TVManager.Instance.PlayTVAnimation();

        ghostAnim.GetComponent<Transform>().gameObject.SetActive(false);
    }

    private IEnumerator WaitForSceneSetup()
    {
        yield return new WaitForSeconds(3f);

        // State change
        currentState = SunroomStates.GhostOnRose;
    }
}
