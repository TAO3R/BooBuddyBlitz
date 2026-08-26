using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pot"))
        {
            // Snap pos
            other.GetComponent<Pot>().snapPos = GetComponentInParent<Transform>().localPosition;
            
            // Water level
            other.GetComponent<Pot>().isHeating = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Pot"))
        {
            // Snap pos
            if (other.GetComponent<Pot>().snapPos == Vector3.zero)
            {
                other.GetComponent<Pot>().snapPos = GetComponentInParent<Transform>().localPosition;
            }
            
            // Water level
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pot"))
        {
            // Snap pos
            
            // Water level
            other.GetComponent<Pot>().isHeating = false;
            other.GetComponent<Pot>().currentBufferTime = other.GetComponent<Pot>().heatBufferTime;
        }
    }
}
