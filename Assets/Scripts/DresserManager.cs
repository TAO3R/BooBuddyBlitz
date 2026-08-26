using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;

public class DresserManager : MonoBehaviour
{
    [Header("Mechanism")] 
    [SerializeField] private float drawerOpenZ;
    [SerializeField] private float drawerCloseZ;
    [SerializeField] private float drawerMinZ;
    [SerializeField] private float drawerMaxZ;
    private Transform _drawer0Trans, _drawer1Trans, _drawer2Trans;
    [Tooltip("Exposed for debugging")]
    [SerializeField] private int ghostPos;
    [Tooltip("Exposed for debugging")]
    [SerializeField] private bool drawer0Closed, drawer1Closed, drawer2Closed;
    [SerializeField] private bool drawer0CanMakeSound, drawer1CanMakeSound, drawer2CanMakeSound;
    [SerializeField] private AudioTrigger drawer0AudioTrigger, drawer1AudioTrigger, drawer2AudioTrigger;


    [Header("Visual")]
    [SerializeField] private Renderer drawer0AuraRend;
    [SerializeField] private Renderer drawer1AuraRend;
    [SerializeField] private Renderer drawer2AuraRend;
    private Coroutine _drawer0Coroutine, _drawer1Coroutine, _drawer2Coroutine;
    private static readonly int On = Shader.PropertyToID("_On");
    private MaterialPropertyBlock _mpb0, _mpb1, _mpb2;
    [SerializeField] private float auraDuration;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Seed RNG
        Random.InitState((int)System.DateTime.Now.Ticks);
        
        // Get transforms
        _drawer0Trans = transform.GetChild(0);
        _drawer1Trans = transform.GetChild(1);
        _drawer2Trans = transform.GetChild(2);
        
        // No ghost in dresser
        ghostPos = -1;
        
        // All drawers are closed at the start
        drawer0Closed = true;
        drawer1Closed = true;
        drawer2Closed = true;
        
        // Sound
        drawer0CanMakeSound = true;
        drawer1CanMakeSound = true;
        drawer2CanMakeSound = true;
        
        // Initialize drawer constraints
        InitializeDrawerConstraints(_drawer0Trans);
        InitializeDrawerConstraints(_drawer1Trans);
        InitializeDrawerConstraints(_drawer2Trans);
        
        // Mpb
        _mpb0 = new MaterialPropertyBlock();
        _mpb1 = new MaterialPropertyBlock();
        _mpb2 = new MaterialPropertyBlock();
    }

    private bool HasClosedDrawer()
    {
        return drawer0Closed || drawer1Closed || drawer2Closed;
    }

    private int GetClosedDrawer()
    {
        List<int> closedDrawerIndices = new List<int>();
        if (drawer0Closed) closedDrawerIndices.Add(0);
        if (drawer1Closed) closedDrawerIndices.Add(1);
        if (drawer2Closed) closedDrawerIndices.Add(2);

        if (closedDrawerIndices.Count > 0)
        {
            int randomIndex = Random.Range(0, closedDrawerIndices.Count);
            return closedDrawerIndices[randomIndex];
        }
        
        return -1;
    }

    private void GhostEnterDrawer(Transform drawerTrans)
    {
        Debug.Log("[DresserManager] Ghost entering " + drawerTrans.name);
        
        // Visual
        if (drawerTrans == _drawer0Trans)
        {
            if (_drawer0Coroutine != null)
            {
                StopCoroutine(_drawer0Coroutine);
            }
            
            _drawer0Coroutine = StartCoroutine(LerpDrawerAura(drawerTrans, true));
        }
        else if (drawerTrans == _drawer1Trans)
        {
            if (_drawer1Coroutine != null)
            {
                StopCoroutine(_drawer1Coroutine);
            }
            
            _drawer1Coroutine = StartCoroutine(LerpDrawerAura(drawerTrans, true));
        }
        else
        {
            if (_drawer2Coroutine != null)
            {
                StopCoroutine(_drawer2Coroutine);
            }
            
            _drawer2Coroutine = StartCoroutine(LerpDrawerAura(drawerTrans, true));
        }
    }

    private void GhostExitDrawer(Transform drawerTrans)
    {
        Debug.Log("[DresserManager] Ghost exiting " + drawerTrans.name);
        
        // Visual
        if (drawerTrans == _drawer0Trans)
        {
            if (_drawer0Coroutine != null)
            {
                StopCoroutine(_drawer0Coroutine);
            }
            
            _drawer0Coroutine = StartCoroutine(LerpDrawerAura(drawerTrans, false));
        }
        else if (drawerTrans == _drawer1Trans)
        {
            if (_drawer1Coroutine != null)
            {
                StopCoroutine(_drawer1Coroutine);
            }
            
            _drawer1Coroutine = StartCoroutine(LerpDrawerAura(drawerTrans, false));
        }
        else
        {
            if (_drawer2Coroutine != null)
            {
                StopCoroutine(_drawer2Coroutine);
            }
            
            _drawer2Coroutine = StartCoroutine(LerpDrawerAura(drawerTrans, false));
        }
        
        // Try find the next drawer
        PossessADrawer();
    }

    public void PossessADrawer()
    {
        Debug.Log("[DresserManager] Possessing a drawer");
        
        if (HasClosedDrawer())
        {
            // Get an empty drawer index, and let the ghost enter
            ghostPos = GetClosedDrawer();
            Debug.Log("[DresserManager] Possessing drawer " + ghostPos);
            GhostEnterDrawer(DrawerIndexToTrans(ghostPos));
        }
        else
        {
            Debug.Log("[DresserManager] ghost leaving dresser");
            
            // State transition in bedroom manager
            StartCoroutine(LerpDrawerAura(_drawer0Trans, false));
            StartCoroutine(LerpDrawerAura(_drawer1Trans, false));
            StartCoroutine(LerpDrawerAura(_drawer2Trans, false));
            
            BedroomManager.Instance.GhostExitDresser();
        }
    }
    
    #region Event Listeners
    
    private void DrawerOnMove(Transform drawerTrans, ref bool drawerClosed, ref bool drawerCanMakeSound)
    {
        // Open drawer sound
        if (!CheckClosed(drawerTrans) && drawerClosed && drawerCanMakeSound)
        {
            drawerCanMakeSound = false;
            DrawerTransToAudioTrigger(drawerTrans).PlayAudio();
        }
        
        // Opening drawer mechanism
        if (CheckOpened(drawerTrans) && drawerClosed)
        {
            // Change state
            drawerClosed = false;

            // Let go ghost if has one
            if (DrawerHasGhost(drawerTrans))
            {
                GhostExitDrawer(drawerTrans);
            }
        }
        
        // Closing drawer
        if (CheckClosed(drawerTrans) && !drawerClosed)
        {
            // Change state
            drawerClosed = true;
            drawerCanMakeSound = true;
        }
    }

    public void Drawer0OnMove()
    {
        DrawerOnMove(_drawer0Trans, ref drawer0Closed, ref drawer0CanMakeSound);
    }
    
    public void Drawer1OnMove()
    {
        DrawerOnMove(_drawer1Trans, ref drawer1Closed, ref drawer1CanMakeSound);
    }
    
    public void Drawer2OnMove()
    {
        DrawerOnMove(_drawer2Trans, ref drawer2Closed, ref drawer2CanMakeSound);
    }
    
    #endregion

    private void InitializeDrawerConstraints(Transform drawerTrans)
    {
        OneGrabTranslateTransformer script = drawerTrans.GetComponent<OneGrabTranslateTransformer>();
        
        script.Constraints.ConstraintsAreRelative = false;
        
        // Enable constraints
        script.Constraints.MinX.Constrain = true;
        script.Constraints.MaxX.Constrain = true;
        script.Constraints.MinY.Constrain = true;
        script.Constraints.MaxY.Constrain = true;
        script.Constraints.MinZ.Constrain = true;
        script.Constraints.MaxZ.Constrain = true;

        // Only allow translation along z-axis
        Vector3 initLocalPos = drawerTrans.localPosition;
        script.Constraints.MinX.Value = initLocalPos.x;
        script.Constraints.MaxX.Value = initLocalPos.x;
        script.Constraints.MinY.Value = initLocalPos.y;
        script.Constraints.MaxY.Value = initLocalPos.y;
        script.Constraints.MinZ.Value = initLocalPos.z;
        script.Constraints.MaxZ.Value = drawerMaxZ;
    }

    private bool CheckOpened(Transform drawerTrans)
    {
        return drawerTrans.localPosition.z >= drawerMinZ + drawerOpenZ;
    }

    private bool CheckClosed(Transform drawerTrans)
    {
        return drawerTrans.localPosition.z <= drawerMinZ + drawerCloseZ;
    }

    private Transform DrawerIndexToTrans(int index)
    {
        switch (index)
        {
            case 0:
                return _drawer0Trans;
            case 1:
                return _drawer1Trans;
            case 2:
                return _drawer2Trans;
            default:
                return null;
        }
    }

    private bool DrawerHasGhost(Transform drawerTrans)
    {
        if (drawerTrans == _drawer0Trans && ghostPos == 0 ||
            drawerTrans == _drawer1Trans && ghostPos == 1 ||
            drawerTrans == _drawer2Trans && ghostPos == 2)
        {
            return true;
        }

        return false;
    }

    private MaterialPropertyBlock DrawerTransToMpb(Transform drawerTrans)
    {
        if (drawerTrans == _drawer0Trans)
        {
            Debug.Log("[DresserManager] Modifying " + _mpb0);
            return _mpb0;
        }
        
        if (drawerTrans == _drawer1Trans)
        {
            Debug.Log("[DresserManager] Modifying " + _mpb1);
            return _mpb1;
        }
        
        Debug.Log("[DresserManager] Modifying " + _mpb2);
        return _mpb2;
    }

    private Renderer DrawerTransToRend(Transform drawerTrans)
    {
        if (drawerTrans == _drawer0Trans)
        {
            return drawer0AuraRend;
        }

        if (drawerTrans == _drawer1Trans)
        {
            return drawer1AuraRend;

        }
        
        return drawer2AuraRend;
    }
    
    private AudioTrigger DrawerTransToAudioTrigger(Transform drawerTrans)
    {
        if (drawerTrans == _drawer0Trans)
        {
            return drawer0AudioTrigger;
        }

        if (drawerTrans == _drawer1Trans)
        {
            return drawer1AudioTrigger;

        }
        
        return drawer2AudioTrigger;
    }

    private IEnumerator LerpDrawerAura(Transform drawerTrans, bool hasGhost)
    {
        Renderer drawerRend = DrawerTransToRend(drawerTrans);
        float initValue = drawerRend.material.GetFloat(On);
        float targetValue = hasGhost ? 1 : 0;
        MaterialPropertyBlock mpb = DrawerTransToMpb(drawerTrans);

        float timeElapsed = 0f;
        while (timeElapsed < Time.deltaTime)
        {
            timeElapsed += Time.deltaTime;
            
            // Visual
            // mpb.SetFloat(On, Mathf.Lerp(initValue, targetValue, timeElapsed / auraDuration));
            // drawerRend.SetPropertyBlock(mpb);
            drawerRend.material.SetFloat(On, Mathf.Lerp(initValue, targetValue, timeElapsed / auraDuration));
            
            yield return null;
        }
        
        // Visual
        // _mpb0.SetFloat(On, targetValue);
        // drawerRend.SetPropertyBlock(mpb);
        drawerRend.material.SetFloat(On, targetValue);
    
        // Coroutine reference
        if (drawerTrans == _drawer0Trans)
        {
            _drawer0Coroutine = null;
        }
        else if (drawerTrans == _drawer1Trans)
        {
            _drawer1Coroutine = null;
        }
        else
        {
            _drawer2Coroutine = null;
        }
    }
    
    
}   // End of class
