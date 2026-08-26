using System;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.VFX;

public class KitchenManager : MonoBehaviour
{
    public static KitchenManager Instance;

    [SerializeField] private Animator ghostAnim;

    [SerializeField] private StoveManager stoveScript;

    [SerializeField] private HandGrabInteractable leftHandInteractable, rightHandInteractable;

    [SerializeField] private VisualEffect potSmokeVFX;
    
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
        ghostAnim.gameObject.SetActive(false);
        leftHandInteractable.enabled = false;
        rightHandInteractable.enabled = false;
        
        potSmokeVFX.Reinit();
        potSmokeVFX.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GhostPotatoToPot()
    {
        ghostAnim.gameObject.SetActive(true);
        ghostAnim.SetTrigger("ToPot");
    }

    public void GhostPotToDoor()
    {
        ghostAnim.SetTrigger("ToDoor");
        stoveScript.StopCycle();
    }
}
