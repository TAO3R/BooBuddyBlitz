using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class BedroomDresser : MonoBehaviour
{
    public List<bool> DrawersOpen { get; private set; }
    private int _ghostedDrawerIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initializes drawer states
        DrawersOpen = new List<bool> {false, false, false};
        
        // Initialize ghost position
            _ghostedDrawerIndex = -1;
    }

    private void InitializeDresser()
    {

    }

    private void WrapUpDresser()
    {

    }
    
    /// <summary>
    /// Called when the ghost is trying to transfer to another drawer
    /// </summary>
    public void PossessNextDrawer()
    {
        int closedDrawerIndex = ReturnAClosedDrawer();
        Debug.Log("[Bedroom Dresser] get closed drawer index: " + closedDrawerIndex);

        // No more closed drawers
        if (closedDrawerIndex == -1)
        {
            // Trigger ghost anim
            BedroomManager.Instance.GhostExitDresser();

            // Audio

        }
        else
        {
            // Transfer the ghost to the next drawer

            // Check index
            if (closedDrawerIndex < 0 || closedDrawerIndex >= DrawersOpen.Count)
            {
                Debug.Log("[BedroomDresser] The ghost is trying to possess a drawer whose index is out of range: " + closedDrawerIndex);
                return;
            }

            // // Leave current drawer
            // if (_ghostedDrawerIndex != -1)
            // {
            //     transform.GetChild(_ghostedDrawerIndex).GetComponent<BedroomDrawer>().GhostExit();
            // }

            // Enter the next drawer
            transform.GetChild(closedDrawerIndex).GetComponent<BedroomDrawer>().GhostEnter();
            _ghostedDrawerIndex = closedDrawerIndex;
        }
    }
    
    /// <summary>
    /// Helper function to return the index of a closed drawer, or -1 meaning all drawers are open
    /// </summary>
    /// <returns></returns>
    private int ReturnAClosedDrawer()
    {
        List<int> closedDrawers = new List<int>();

        // foreach (var i in DrawersOpen)
        // {
        //     Debug.Log("[BedroomBresser]: " + i);
        // }

        for (int i = 0; i < DrawersOpen.Count; i++)
        {
            if (!DrawersOpen[i])
            {
                Debug.Log("[BedroomDresser] Adding closed drawer: " + i);
                closedDrawers.Add(i);
            }
        }

        if (closedDrawers.Count != 0)
        {
            int index = Random.Range(0, closedDrawers.Count);
            return closedDrawers[index];
        }
        else
        {
            return -1;
        }
    }
    
    /// <summary>
    /// Helper function for a drawer to set itself as closed in dresser script
    /// </summary>
    /// <param name="index"></param>
    public void SetDrawerClosed(int index)
    {
        if (index < 0 || index >= DrawersOpen.Count) return;

        DrawersOpen[index] = false;
    }
    
    /// <summary>
    /// Helper function for a drawer to set itself as open
    /// </summary>
    /// <param name="index"></param>
    public void SetDrawerOpen(int index)
    {
        if (index < 0 || index >= DrawersOpen.Count) return;

        DrawersOpen[index] = true;
    }
    
}   // End of class
