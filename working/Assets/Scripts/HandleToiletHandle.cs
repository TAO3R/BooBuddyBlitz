using System.Collections;
using Oculus.Interaction;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class HandleToiletHandle : MonoBehaviour
{
    [SerializeField] FlipBookMeshArrayManual flipBookMeshArrayManual;
    [SerializeField] Animator flushAnim, duckAnim;
    [SerializeField] AudioTrigger flushAudioTrigger;

    private Grabbable grabbable;
    private OneGrabRotateTransformer oneGrabRotateTransformer;
    private Quaternion initialRotation;
    private float lerpDuration = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabbable = GetComponent<Grabbable>();
        oneGrabRotateTransformer = GetComponent<OneGrabRotateTransformer>();

        initialRotation = transform.localRotation;
    }

    public void SetHandleAngle(float maxAngle, float minAngle)
    {
        oneGrabRotateTransformer.Constraints.MaxAngle = new FloatConstraint() { Constrain = true, Value = maxAngle};
        oneGrabRotateTransformer.Constraints.MinAngle = new FloatConstraint() { Constrain = true, Value = minAngle};
    }

    public void RotateToiletHandleOnly()
    {
        float angleDiff = Quaternion.Angle(initialRotation, transform.localRotation);
        if (Mathf.Abs(angleDiff) >= 85f)
        {
            flushAudioTrigger.PlayAudio();
            flushAnim.SetTrigger("TriggerFlush");
        }
    }

    public void RotateToiletHandle()
    {
        float angleDiff = Quaternion.Angle(initialRotation, transform.localRotation);
        if (Mathf.Abs(angleDiff) >= 85f)
        {
            BathroomManager.Instance.GhostExitToilet();

            flushAnim.SetTrigger("TriggerFlush");
            duckAnim.SetTrigger("TriggerFlush");

            flushAudioTrigger.PlayAudio();

            grabbable.enabled = false;
            oneGrabRotateTransformer.enabled = false;

            StartCoroutine(LerpToInitialRotation());
        }
    }

    private IEnumerator LerpToInitialRotation()
    {
        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;
        Quaternion startRotation = transform.localRotation;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / lerpDuration;
            transform.localRotation = Quaternion.Lerp(startRotation, initialRotation, t);
            yield return null;
        }

        transform.localRotation = initialRotation;
    }
}
