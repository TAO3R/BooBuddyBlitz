using UnityEngine;

public class KnifeBlade : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (this.gameObject.CompareTag("Blade") && other.gameObject.CompareTag("Cuttable"))
        {
            if (GetComponentInParent<Knife>().isGrabbed && GetComponentInParent<Knife>().isCutting)
            {
                other.transform.GetComponent<Cuttable>().StateTransition();
            }
        }
    }
}