using UnityEngine;
using UnityEngine.VFX;

public enum PotStates
{
    Idle,
    Heating,
    DriedUp
}

public class Pot : MonoBehaviour
{
    [SerializeField] private Animator potAnim;
    [SerializeField] private AnimationClip idleClip, boilClip;
    [SerializeField] private GameObject boilingParent;
    
    [Header("Water level mechanism")]
    public PotStates currentPotState;
    
    [Tooltip("Exposed for debugging, should be from 0 to clip length")] [SerializeField]
    private float boilProgress;

    public bool isHeating;
    
    [Tooltip("Time it takes to have the water level decreased after the pot is placed on fire")]
    public float heatBufferTime;
    
    public float currentBufferTime;
    
    [Tooltip("Time it takes to transit water from idle to boiling")] [SerializeField]
    private float idleToHeatTime;
    
    [SerializeField]
    private float remainingIdleTime;
    
    
    [Header("Position mechanism")]
    public bool isGrabbed;

    public Vector3 snapPos;
    
    [SerializeField] private VisualEffect potSmokeVFX;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Water Level
        currentPotState = PotStates.Idle;
        boilProgress = 0;
        currentBufferTime = heatBufferTime;
        remainingIdleTime = idleToHeatTime;
        idleClip.SampleAnimation(boilingParent, 0f);
        
        // Snap pos
        snapPos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPotState == PotStates.Idle)
        {
            if (isHeating)
            {
                remainingIdleTime -= Time.deltaTime;

                if (remainingIdleTime <= 0)
                {
                    // Water start to boil
                    currentPotState = PotStates.Heating;
                    boilClip.SampleAnimation(boilingParent, boilProgress);
                    potSmokeVFX.Play();
                }
            }
        }
        else if (currentPotState == PotStates.Heating)
        {
            if (isHeating)
            {
                if (currentBufferTime <= 0)
                {
                    // Advance the heating process
                    boilProgress += Time.deltaTime;
                    boilClip.SampleAnimation(boilingParent, boilProgress);

                    if (boilProgress >= boilClip.length)
                    {
                        // Ghost state transition
                        potSmokeVFX.Stop();
                        KitchenManager.Instance.GhostPotToDoor();
                        currentPotState = PotStates.DriedUp;
                    }
                }
                else
                {
                    // Wait to clear the buffer before actual heating
                    currentBufferTime -= Time.deltaTime;
                }
            }
        }
        else
        {
            
        }

    }

    private void SnapPosAndRot()
    {
        Transform potTrans = GetComponentInParent<Transform>().GetComponentInParent<Transform>();
        
        potTrans.localPosition = snapPos;
        potTrans.localEulerAngles = new Vector3(0, potTrans.localEulerAngles.y, 0);
    }

    private float WaterLevelToClipTime(float level)
    {
        return Mathf.Lerp(0, boilClip.length,  Mathf.Clamp(1 - level, 0, 1));
    }
    
    #region Event Listeners

    public void OnSelect()
    {
        isGrabbed = true;
    }
        
    public void OnMove()
    {
        
    }

    public void OnRelease()
    {
        isGrabbed = false;
        
        // Actual position and rotation snap
        SnapPosAndRot();
    }
        
    #endregion
    
}   // End of class
