using System.Collections;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class HandleMirrorInteraction : MonoBehaviour
{
    [SerializeField] Animator mirrorAnim;
    [SerializeField] Animator ghostAnim;
    [SerializeField] GameObject glassTransform0, glassOriginal, glassTransform1, glassTransform2;
    [SerializeField] AudioTrigger break0AudioTrigger, break1AudioTrigger, break2AudioTrigger, break3AudioTrigger, ghostBooAudioTrigger;

    private InteractableUnityEventWrapper interactableUnityEventWrapper;

    // private bool isRightHandFist;
    // private bool isLeftHandFist;
    public int punchCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // interactableUnityEventWrapper = GetComponent<InteractableUnityEventWrapper>();
        // interactableUnityEventWrapper.WhenSelect.AddListener(OnSelect);
    }

    // TODO: needs testing
    public void OnSelect()
    {
        if (punchCounter >= 5 || punchCounter == 0) return;

        switch (punchCounter)
        {
            case 1:
                glassTransform0.SetActive(true);
                break0AudioTrigger.PlayAudio();

                break;

            case 2:
                glassTransform0.SetActive(false);
                glassTransform1.SetActive(true);
                break1AudioTrigger.PlayAudio();
                break;

            case 3:
                glassTransform1.SetActive(false);
                glassTransform2.SetActive(true);

                break2AudioTrigger.PlayAudio();
                break;

            case 4:
                glassOriginal.SetActive(false);
                glassTransform1.SetActive(false);

                break3AudioTrigger.PlayAudio();

                mirrorAnim.SetTrigger("Break");
                ghostAnim.SetTrigger("LeaveMirror");

                StartCoroutine(WaitForGhostBoo());

                BathroomManager.Instance.GhostExitMirror();
                break;
        }

        punchCounter++;
    }

    private IEnumerator WaitForGhostBoo()
    {
        yield return new WaitForSeconds(1f);

        ghostBooAudioTrigger.PlayAudio();
    }

    // public void OnRightGestureDetected()
    // {
    //     isRightHandFist = true;
    // }

    // public void OnRightGestureLost()
    // {
    //     isRightHandFist = false;
    // }

    // public void OnLeftGestureDetected()
    // {
    //     isLeftHandFist = true;
    // }

    // public void OnLeftGestureLost()
    // {
    //     isLeftHandFist = false;
    // }
}
