using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class RealDresser : MonoBehaviour
{
    [SerializeField] private float drawerOpenZ;
    [SerializeField] private float drawerCloseZ;
    [SerializeField] private List<HandGrabInteractable>  drawerGrab;
    [SerializeField] private Transform drawerTrans;

    public bool drawerClosed, drawerCanMakeSound;

    public float drawerCloseDuration, auraDuration;

    public AudioTrigger drawerAudioTrigger;

    public bool hasGhost;

    public Renderer drawerAuraRend;
    
    private static readonly int On = Shader.PropertyToID("_On");

    public GameObject letter;

    private void Start()
    {
        drawerClosed = true;
        drawerCanMakeSound = true;
    }

    public void InitializeGhostDrawer()
    {
        StartCoroutine(LerpDrawerClose());
    }

    public void WrapUpGhostDrawer()
    {
        hasGhost = false;
        StartCoroutine(LerpDrawerAura(hasGhost));
    }
    
    // Helper function to make drawers able to interact or not at once
    private void ToggleDrawerGrab(bool canGrab)
    {
        for (int i = 0; i < drawerGrab.Count; i++)
        {
            drawerGrab[i].enabled = canGrab;
        }
    }
    
    // Lerp drawer to close when the ghost enters the dresser
    private IEnumerator LerpDrawerClose()
    {
        ToggleDrawerGrab(false);
        
        float timeElapsed = 0f;
        float initZ = drawerTrans.localPosition.z;
        float targetZ = 0;
        
        while (timeElapsed < drawerCloseDuration)
        {
            timeElapsed += Time.deltaTime;

            drawerTrans.localPosition = new Vector3(0, 0, Mathf.Lerp(initZ, targetZ, timeElapsed / drawerCloseDuration));

            yield return null;
        }
        
        ToggleDrawerGrab(true);
        drawerClosed = true;
        drawerCanMakeSound = true;
        hasGhost = true;
        letter.SetActive(true);

        StartCoroutine(LerpDrawerAura(hasGhost));
    }
    
    public void DrawerOnMove()
    {
        if (BedroomManager.Instance.CurrentState != BedroomStates.GhostOnDresser) return;
        
        // Open drawer sound
        if (!CheckClosed() && drawerClosed && drawerCanMakeSound)
        {
            drawerCanMakeSound = false;
            drawerAudioTrigger.PlayAudio();
        }
        
        // Opening drawer mechanism
        if (CheckOpened() && drawerClosed)
        {
            // Change state
            drawerClosed = false;

            // Let go ghost if has one
            if (hasGhost)
            {
                BedroomManager.Instance.GhostExitDresser();
            }
        }
        
        // Closing drawer
        if (CheckClosed() && !drawerClosed)
        {
            // Change state
            drawerClosed = true;
            drawerCanMakeSound = true;
        }
    }
    
    private bool CheckOpened()
    {
        return drawerTrans.localPosition.z >= drawerOpenZ;
    }

    private bool CheckClosed()
    {
        return drawerTrans.localPosition.z <= drawerCloseZ;
    }
    
    private IEnumerator LerpDrawerAura( bool _hasGhost)
    {
        float initValue = drawerAuraRend.material.GetFloat(On);
        float targetValue = _hasGhost ? 1 : 0;

        float timeElapsed = 0f;
        while (timeElapsed < Time.deltaTime)
        {
            timeElapsed += Time.deltaTime;
            
            // Visual
            // mpb.SetFloat(On, Mathf.Lerp(initValue, targetValue, timeElapsed / auraDuration));
            // drawerRend.SetPropertyBlock(mpb);
            drawerAuraRend.material.SetFloat(On, Mathf.Lerp(initValue, targetValue, timeElapsed / auraDuration));
            
            yield return null;
        }
        
        // Visual
        drawerAuraRend.material.SetFloat(On, targetValue);
    }
    
}   // End of class
