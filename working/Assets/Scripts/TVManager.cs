using System.Collections.Generic;
using System.Collections;
using Oculus.Interaction;
using UnityEngine;
using TMPro;

public class TVManager : MonoBehaviour
{
    public static TVManager Instance;
    [SerializeField] private Animator tvAnim;
    [SerializeField] List<string> babbling = new List<string>();
    [SerializeField] DisplayCredits displayCredits;
    [SerializeField] AudioTrigger ghostPopAudioTrigger;

    [SerializeField] private float charDelay = 0.05f;
    [SerializeField] private float sentenceDelay = 2.0f;
    [SerializeField] private bool loop = true;
    [SerializeField] private TextMeshPro textComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TVManager.Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (textComponent == null) textComponent = GetComponentInChildren<TextMeshPro>();
    }

    public void PlayTVAnimation()
    {
        tvAnim.SetTrigger("EnterTV");

        ghostPopAudioTrigger.PlayAudio();

        StartCoroutine(LoopThroughBabbling());
        StartCoroutine(WaitForDisplayCredits());
    }

    public IEnumerator LoopThroughBabbling()
    {
        yield return new WaitForSeconds(4f); ;
        do
        {
            foreach (string sentence in babbling)
            {
                yield return StartCoroutine(TypewriterEffect(sentence));

                yield return new WaitForSeconds(sentenceDelay);
            }

        } while (loop && babbling.Count > 0);
    }

    public IEnumerator TypewriterEffect(string textToType)
    {
        if (textComponent == null) yield break;

        textComponent.text = "";

        for (int i = 0; i < textToType.Length; i++)
        {
            textComponent.text += textToType[i];
            yield return new WaitForSeconds(charDelay);
        }
    }

    private IEnumerator WaitForDisplayCredits()
    {
        yield return new WaitForSeconds(10f);

        StartCoroutine(displayCredits.ToggleDisplay(0f, 1f, 2f));

        // yield return new WaitForSeconds(6f);

        // StartCoroutine(displayCredits.ToggleDisplay(1f, 0f, 2f));
    }
}
