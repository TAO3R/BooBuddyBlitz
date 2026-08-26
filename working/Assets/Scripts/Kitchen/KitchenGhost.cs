using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.VFX;

public class KitchenGhost : MonoBehaviour
{
    public StoveManager stoveScript;
    [SerializeField] private HandGrabInteractable leftHandInteractable, rightHandInteractable;

    public void GhostEnterPot()
    {
        stoveScript.StartCycle();
        leftHandInteractable.enabled = true;
        rightHandInteractable.enabled = true;
    }

    public void GhostEnterDoor()
    {
        
    }
}
