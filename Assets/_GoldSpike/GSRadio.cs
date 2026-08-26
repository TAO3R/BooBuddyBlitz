using System;
using Oculus.Interaction;
using UnityEngine;

public class GSRadio : MonoBehaviour
{
    private InteractableUnityEventWrapper _interactableUnityEventWrapper;
    private Animator _animator;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip activationSound;

    private void Awake()
    {
        _interactableUnityEventWrapper = GetComponent<InteractableUnityEventWrapper>();
        _interactableUnityEventWrapper.WhenSelect.AddListener(Select);

        _animator = GetComponentInParent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Select()
    {
        Debug.Log("[GSRadio] : Select() being called.");

        bool toggleAnim = _animator.GetBool("Spinning");
        _animator?.SetBool("Spinning", !toggleAnim);

        bool toggleAudio = _audioSource.isPlaying;
        if (toggleAudio)
        {
            _audioSource.Stop();
        }
        else
        {
            _audioSource.Play();
        }
    }
}
