using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    [SerializeField] private OVRManager OVRManagerScript;
    [SerializeField] private Transform cameraZoomTarget;
    [SerializeField] private AnimationCurve zoomLerpCurve;
    [SerializeField] private float zoomDuration;

    [SerializeField] private Animator fadeCanvasAnim;

    [SerializeField] private Material mat;

    private void Awake()
    {
        if (SceneTransitionManager.Instance != null && SceneTransitionManager.Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void EnterCurrentScene()
    {
        // Head up and open the eye
        StartCoroutine(SceneEnterViaEyeOpen());
    }

    private IEnumerator SceneEnterViaEyeOpen()
    {
        yield return null;
    }

    public void StartCameraZoom()
    {
        Debug.Log("Start scene transition");
        StartCoroutine(SceneTransitionViaCamera());
    }

    private IEnumerator SceneTransitionViaCamera()
    {
        Transform camRigTrans = OVRManagerScript.GetComponent<Transform>();

        Vector3 initPos = camRigTrans.localPosition;
        Quaternion initRot = camRigTrans.localRotation;
        OVRManagerScript.usePositionTracking = false;

        Vector3 targetPos = new Vector3(cameraZoomTarget.localPosition.x, camRigTrans.localPosition.y, cameraZoomTarget.localPosition.z);

        float timeElapsed = 0f;

        while (timeElapsed < zoomDuration)
        {
            timeElapsed += Time.deltaTime;

            // Position
            camRigTrans.localPosition =
                Vector3.Lerp(
                    initPos,
                    targetPos,
                    zoomLerpCurve.Evaluate(timeElapsed / zoomDuration));

            Debug.Log("Current camera rig local position: " + camRigTrans.localPosition);

            // Rotation
            camRigTrans.localRotation =
                Quaternion.Slerp(
                    initRot,
                    cameraZoomTarget.localRotation,
                    zoomLerpCurve.Evaluate(timeElapsed / zoomDuration));

            yield return null;
        }

        // camRigTrans.localPosition = targetPos;
        // camRigTrans.localRotation = cameraZoomTarget.localRotation;

        LoadNextScene();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void CanvasFadeIn()
    {
        fadeCanvasAnim.SetTrigger("Fade");
    }

    public void StartFadeInTransition()
    {
        StartCoroutine(SceneTransitionViaBlackBox(1f, 0f, 2f));
    }

    public void StartFadeOutTransition()
    {
        StartCoroutine(SceneTransitionViaBlackBox(0f, 1f, 1f));
    }

    private IEnumerator SceneTransitionViaBlackBox(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);

            Color color = new Color(mat.color.r, mat.color.g, mat.color.b, currentAlpha);
            mat.color = color;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (startAlpha == 0f) LoadNextScene();
    }
}
