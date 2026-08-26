using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.VFX;

public class HandleVanityInteraction : MonoBehaviour
{
    [SerializeField] GameObject evilDuck;
    [SerializeField] private Animator vanityAnim;
    [SerializeField] Transform leftDoor;
    [SerializeField] Transform rightDoor;
    [SerializeField] Transform leftFacuet;
    [SerializeField] Transform rightFacuet;
    [SerializeField] List<GameObject> leftIdleDucks, rightIdleDucks, leftActiveDucks, rightActiveDucks;
    [SerializeField] private float thrustForce;
    [SerializeField] private float idleForce;
    [SerializeField] AudioTrigger duckExplodeAudioTrigger, duckJumpAudioTrigger, toToiletAudioTrigger;
    [SerializeField] private VisualEffect vanityGhostFogVFX;

    private Animator duckAnim;
    private bool faucetHasRotation;
    private Quaternion initialFaucetRotation, initialDoorRotation;
    private int leftThrowCounter, rightThrowCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        duckAnim = evilDuck.GetComponent<Animator>();

        initialFaucetRotation = leftFacuet.localRotation;
        initialDoorRotation = leftDoor.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        DetectFaucetFlow();
        DetectDoorRotation();
    }

    public void OnSelectDuck()
    {
        AnimatorStateInfo stateInfo = duckAnim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1f)
        {
            if (stateInfo.IsName("DuckFlyOut"))
            {
                duckAnim.SetTrigger("Jump2");
                duckJumpAudioTrigger.PlayAudio();
            }
            if (stateInfo.IsName("DuckJumpTwice"))
            {
                duckAnim.SetTrigger("Jump3");
                duckJumpAudioTrigger.PlayAudio();

                StartCoroutine(WaitForToToiletAudio());

                BathroomManager.Instance.currentState = BathroomStates.GhostOnDuck;
            }
        }
    }

    private void DetectFaucetFlow()
    {
        float leftAngleDiff = Quaternion.Angle(leftFacuet.transform.localRotation, initialFaucetRotation);
        float rightAngleDiff = Quaternion.Angle(rightFacuet.transform.localRotation, initialFaucetRotation);

        faucetHasRotation = leftAngleDiff >= 15f || rightAngleDiff >= 15f;
        vanityAnim.SetBool("faucetIsOpen", faucetHasRotation);
    }

    private void DetectDoorRotation()
    {
        float leftRotationDiff = Quaternion.Angle(initialDoorRotation, leftDoor.localRotation);
        float rightRotationDiff = Quaternion.Angle(initialDoorRotation, rightDoor.localRotation);

        if (leftThrowCounter == 0 && leftRotationDiff >= 30f)
        {
            duckAnim.SetTrigger("Jump1");
            ThrowDucks(leftIdleDucks, leftActiveDucks);

            duckExplodeAudioTrigger.PlayAudio();

            leftThrowCounter++;
        }

        if (rightThrowCounter == 0 && rightRotationDiff >= 30f)
        {
            ThrowDucks(rightIdleDucks, rightActiveDucks);

            duckExplodeAudioTrigger.PlayAudio();

            rightThrowCounter++;
        }
    }

    private void ThrowDucks(List<GameObject> idleDucks, List<GameObject> activeDucks)
    {
        for (int i = 0; i < idleDucks.Count; i++)
        {
            Rigidbody rb = idleDucks[i].GetComponent<Rigidbody>();
            rb.AddForce(idleDucks[i].transform.forward * idleForce);
        }

        for (int i = 0; i < activeDucks.Count; i++)
        {
            Vector3 throwDir = new Vector3(Random.Range(-0.2f, 1.5f), Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.5f));
            Vector3 throwForce = throwDir * thrustForce;

            Rigidbody rb = activeDucks[i].GetComponent<Rigidbody>();
            rb.AddForce(throwForce);
        }

        vanityGhostFogVFX.Stop();
    }

    private IEnumerator WaitForToToiletAudio()
    {
        yield return new WaitForSeconds(0.35f);
        toToiletAudioTrigger.PlayAudio();
    }
}
