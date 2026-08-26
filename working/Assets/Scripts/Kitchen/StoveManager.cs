using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveManager : MonoBehaviour
{
    [Header("Stove Settings")]
    public List<BoxCollider> stoves = new List<BoxCollider>();
    public List<Animator> stoveAnimators = new List<Animator>();

    [Header("Timing Settings")]
    [Tooltip("Min delay before a stove switches.")]
    public float minDelay = 2f;
    [Tooltip("Max delay before a stove switches.")]
    public float maxDelay = 5f;
    [Tooltip("Optional delay before disabling stove after TurnOff trigger (to let animation play).")]
    public float offAnimationDelay = 0.5f;

    [Header("Global Control")]
    [Tooltip("If true, all stoves are turned off.")]
    public bool allOff = false;

    private List<bool> stoveStates = new List<bool>(5);
    private Coroutine stoveCycleCoroutine;
    private int currentActiveIndex = -1;

    void Start()
    {
        // Initialize all stoves as off
        for (int i = 0; i < stoves.Count; i++)
        {
            stoveStates.Add(false);
            SetStoveState(i, false, instant: true);
        }
    }

    IEnumerator StoveCycle()
    {
        while (true)
        {
            if (allOff)
            {
                // Turn off everything
                for (int i = 0; i < stoves.Count; i++)
                    SetStoveState(i, false);
                
                currentActiveIndex = -1;
                yield return null;
                continue;
            }

            // Pick a stove that's not the same as the current one
            int newIndex;
            do
            {
                newIndex = Random.Range(0, stoves.Count);
            } while (newIndex == currentActiveIndex && stoves.Count > 1);

            // Turn off old stove
            if (currentActiveIndex >= 0)
                SetStoveState(currentActiveIndex, false);

            // Turn on new stove
            currentActiveIndex = newIndex;
            SetStoveState(currentActiveIndex, true);

            // Wait for random duration before switching
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SetStoveState(int index, bool isOn, bool instant = false)
    {
        if (index < 0 || index >= stoves.Count) return;

        stoveStates[index] = isOn;

        if (isOn)
        {
            stoves[index].enabled = true;

            if (stoveAnimators[index] != null)
                stoveAnimators[index].SetTrigger("TurnOn");
        }
        else
        {
            if (stoveAnimators[index] != null)
                stoveAnimators[index].SetTrigger("TurnOff");

            // Delay disabling the stove so the animation can play
            if (!instant)
                StartCoroutine(DisableAfterDelay(stoves[index].gameObject, offAnimationDelay));
            else
                stoves[index].enabled = false;
        }
    }

    IEnumerator DisableAfterDelay(GameObject stove, float delay)
    {
        yield return new WaitForSeconds(delay);
        stove.SetActive(false);
    }

    void OnValidate()
    {
        while (stoves.Count < 5) stoves.Add(null);
        while (stoveAnimators.Count < 5) stoveAnimators.Add(null);
    }
    
    /// <summary>
    /// Starts the stove cycling routine (if not already running).
    /// </summary>
    public void StartCycle()
    {
        if (stoveCycleCoroutine == null)
        {
            stoveCycleCoroutine = StartCoroutine(StoveCycle());
        }
    }

    /// <summary>
    /// Stops the stove cycling routine and turns off all stoves.
    /// </summary>
    public void StopCycle()
    {
        if (stoveCycleCoroutine != null)
        {
            StopCoroutine(stoveCycleCoroutine);
            stoveCycleCoroutine = null;
        }

        // Optionally turn off all stoves
        for (int i = 0; i < stoves.Count; i++)
        {
            SetStoveState(i, false);
        }

        currentActiveIndex = -1;
    }
    
}   // End of class
