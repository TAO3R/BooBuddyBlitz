using System;
using System.Collections;
using Oculus.Interaction;
using UnityEngine;

public class BedroomDrawer : MonoBehaviour
{
    private Transform _drawerTrans;
    private Rigidbody _rb;
    private OneGrabTranslateTransformer _oneGrabTranslateTransformer;
    private Coroutine _auraCoroutine;
    private bool _hasGhost;
    private static readonly int On = Shader.PropertyToID("_On");

    [SerializeField] private float drawerOpenDistanceZ;
    [SerializeField] private Renderer drawerAuraRend;
    private MaterialPropertyBlock _mpb;
    [SerializeField] private float auraDuration;
    [SerializeField] private BedroomDresser dresserScript;

    private void Start()
    {
        // Initialization

        _mpb = new MaterialPropertyBlock();
        _drawerTrans = transform;
        _oneGrabTranslateTransformer = GetComponent<OneGrabTranslateTransformer>();
        _auraCoroutine = null;
        _hasGhost = false;
    }

    /// <summary>
    /// Called when the ghost enters the drawer this script is attached to
    /// </summary>
    public void GhostEnter()
    {
        if (_auraCoroutine != null)
        {
            StopCoroutine(_auraCoroutine);
        }

        StartCoroutine(LerpDrawerAura(true));
        _hasGhost = true;
    }
    
    /// <summary>
    /// Called when the ghost leaves the drawer this script is attached to
    /// </summary>
    public void GhostExit()
    {
        if (_auraCoroutine != null)
        {
            StopCoroutine(_auraCoroutine);
        }

        StartCoroutine(LerpDrawerAura(false));
        
        // The ghost tries to possess another drawer
        dresserScript.PossessNextDrawer();
                
        // Update drawer state
        dresserScript.SetDrawerOpen(transform.GetSiblingIndex());
        _hasGhost = false;
    }

    private IEnumerator LerpDrawerAura(bool turnOn)
    {
        float initValue = turnOn ? 0 : 1;
        float targetValue = turnOn ? 1 : 0;
        float currentValue = drawerAuraRend.material.GetFloat(On);

        float timeElapsed = 1 - Mathf.Abs(currentValue - targetValue) * auraDuration;

        while (timeElapsed < auraDuration)
        {
            timeElapsed += Time.deltaTime;
            
            // Set intensity
            _mpb.SetFloat(On, Mathf.Lerp(initValue, targetValue, timeElapsed / auraDuration));
            drawerAuraRend.SetPropertyBlock(_mpb);
            
            yield return null;
        }
        
        _mpb.SetFloat(On, targetValue);
        drawerAuraRend.SetPropertyBlock(_mpb);

        _auraCoroutine = null;
    }

    private bool CheckOpened()
    {
        return _drawerTrans.localPosition.z >=
               _oneGrabTranslateTransformer.Constraints.MinZ.Value + drawerOpenDistanceZ;
    }

    private void OnOpened()
    {
        if (_hasGhost)
        {
            dresserScript.PossessNextDrawer();
        }
        
        dresserScript.SetDrawerOpen(transform.GetSiblingIndex());
    }

    private void OnClosed()
    {
        dresserScript.SetDrawerClosed(transform.GetSiblingIndex());
    }
    
    #region ListenerEvents

    public void OnSelect()
    {
        Debug.Log("[BedroomDrawer] drawer " + transform.GetSiblingIndex() + " is selected.");
    }

    public void OnMove()
    {
        if (CheckOpened())
        {
            // The drawer was closed, now opening it
            if (!dresserScript.DrawersOpen[transform.GetSiblingIndex()])
            {
                OnOpened();
            }
            
        }
        else
        {
            // The drawer was opened, now closing it
            if (dresserScript.DrawersOpen[transform.GetSiblingIndex()])
            {
                OnClosed();
            }
        }
    }

    public void OnRelease()
    {
        Debug.Log("[BedroomDrawer] drawer " + transform.GetSiblingIndex() + " is released.");
    }
    
    #endregion
    
}   // End of class
