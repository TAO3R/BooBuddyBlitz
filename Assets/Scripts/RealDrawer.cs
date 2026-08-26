using Oculus.Interaction;
using UnityEngine;

public class RealDrawer : MonoBehaviour
{
    [SerializeField] private float drawerOpenZ = 0.15f;
    [SerializeField] private float drawerCloseZ = 0.05f;
    
    public bool drawerClosed, drawerCanMakeSound;

    public AudioTrigger drawerAudioTrigger;

    public void OnMove()
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
        return transform.localPosition.z >= drawerOpenZ;
    }

    private bool CheckClosed()
    {
        return transform.localPosition.z <= drawerCloseZ;
    }
}
