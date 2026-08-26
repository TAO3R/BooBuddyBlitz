using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class Cuttable : MonoBehaviour
{
    public Animator cuttableAnim;
    public bool hasGhost;
    private bool _cut;
    private Knife _knifeScript;

    public HandGrabInteractable handGrabInteractable;

    private void Start()
    {
        _cut = false;
        _knifeScript = GetComponentInParent<Knife>();
    }

    public void StateTransition()
    {
        if (!_cut)
        {
            cuttableAnim.SetTrigger("Cut");
            _cut = true;

            handGrabInteractable.enabled = false;
        }
        
        if (!hasGhost) return;

        hasGhost = false;
        KitchenManager.Instance.GhostPotatoToPot();
    }
}
