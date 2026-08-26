using System;
using Oculus.Interaction;
using UnityEngine;

/// <summary>
/// This script should be attached to the gameobject whose transform guides the ik of the cable of the radio.
/// </summary>
public class Plug : MonoBehaviour
{
    private PointableUnityEventWrapper _pointableUnityEventWrapper;
    private Transform _plugTrans;
    private Rigidbody _rb;
    private OneGrabTranslateTransformer _oneGrabTranslateTransformer;
    
    [SerializeField] private float movableRangeMaxZ, unplugDistanceZ;
    [SerializeField] private AudioTrigger plugAudioTrigger, unplugAudioTrigger;

    #region Mono
    
    private void Awake()
    {
        _pointableUnityEventWrapper = GetComponent<PointableUnityEventWrapper>();
        _plugTrans = GetComponent<Transform>();
        _rb = GetComponent<Rigidbody>();
        _oneGrabTranslateTransformer = GetComponent<OneGrabTranslateTransformer>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUpPlugConstraints();
        OnPlugged();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    private void SetUpPlugConstraints()
    {
        _oneGrabTranslateTransformer.Constraints.ConstraintsAreRelative = false;
        
        // Enable constraints
        _oneGrabTranslateTransformer.Constraints.MinX.Constrain = true;
        _oneGrabTranslateTransformer.Constraints.MaxX.Constrain = true;
        _oneGrabTranslateTransformer.Constraints.MinY.Constrain = true;
        _oneGrabTranslateTransformer.Constraints.MaxY.Constrain = true;
        _oneGrabTranslateTransformer.Constraints.MinZ.Constrain = true;
        _oneGrabTranslateTransformer.Constraints.MaxZ.Constrain = true;
        
        // Only allow translation along Z-axis
        Vector3 initLocalPos = _plugTrans.localPosition;
        _oneGrabTranslateTransformer.Constraints.MinX.Value = initLocalPos.x;
        _oneGrabTranslateTransformer.Constraints.MaxX.Value = initLocalPos.x;
        _oneGrabTranslateTransformer.Constraints.MinY.Value = initLocalPos.y;
        _oneGrabTranslateTransformer.Constraints.MaxY.Value = initLocalPos.y;
        _oneGrabTranslateTransformer.Constraints.MinZ.Value = initLocalPos.z;
        _oneGrabTranslateTransformer.Constraints.MaxZ.Value = movableRangeMaxZ;
    }
    
    private bool CheckUnPlugged()
    {
        return _plugTrans.localPosition.z >= _oneGrabTranslateTransformer.Constraints.MinZ.Value + unplugDistanceZ;
    }

    private void OnPlugged()
    {
        _rb.useGravity = false;
        _rb.isKinematic = true;
        plugAudioTrigger.PlayAudio();
    }
    
    private void OnUnplugged()
    {
        _rb.useGravity = true;
        _rb.isKinematic = false;
        unplugAudioTrigger.PlayAudio();
        
        // If first time unplugged, change bedroom state
        if (BedroomManager.Instance.CurrentState == BedroomStates.GhostOnRadio)
        {
            BedroomManager.Instance.GhostExitRadio();
        }
    }
    
    
    #region Listener Events
    
    /// <summary>
    /// Enter plug being grabbed
    /// </summary>
    public void OnSelect()
    {
        if (!CheckUnPlugged())
        {
            OnPlugged();
        }
    }
    
    /// <summary>
    /// Plug being grabbed
    /// </summary>
    public void OnMove()
    {
        if (CheckUnPlugged())
        {
            Debug.Log("[Plug] unplugged!");
            OnUnplugged();
        }
    }
    
    /// <summary>
    /// Exit plug being grabbed
    /// </summary>
    public void OnRelease()
    {
        
    }
    
    #endregion
    
}   // End of class
