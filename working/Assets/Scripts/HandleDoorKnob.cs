using System.Data.Common;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class HandleDoorKnob : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] AudioTrigger cantOpenAudioTrigger;
    [SerializeField] OneGrabRotateTransformer oneGrabRotateTransformer;

    public bool playCantOpen;
    private Quaternion initialRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialRotation = transform.parent.localRotation;
    }

    void Update()
    {

    }

    public void SetHandleRotation(float minAngle, float maxAngle)
    {
        oneGrabRotateTransformer.Constraints.MaxAngle = new FloatConstraint() { Constrain = true, Value = maxAngle };
        oneGrabRotateTransformer.Constraints.MinAngle = new FloatConstraint() { Constrain = true, Value = minAngle };
    }

    public void MoveDoorHandle()
    {
        float angleDiff = Quaternion.Angle(initialRotation, transform.parent.localRotation);
        if (angleDiff >= 72f)
        {
            Debug.Log("[HandleDoorKnob] opening the door");

            anim.SetTrigger("Open2");

            // AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

            // if (stateInfo.IsName("OpenDoor") && stateInfo.normalizedTime >= 0.9f)
            // {
            //     // TODO: play transition animation
            //     BedroomManager.Instance.InitSceneTransition();
            // }
        }
    }

    public void OnMove()
    {
        int counter = 0;
        if (playCantOpen)
        {
            float angleDiff = Quaternion.Angle(initialRotation, transform.parent.localRotation);
            if (angleDiff >= 14f && counter == 0)
            {
                cantOpenAudioTrigger.PlayAudio();
                counter++;
            }
        }
    }
}