using UnityEngine;

public class Knife : MonoBehaviour
{
    private Rigidbody _rb;

    public bool isGrabbed, isCutting;

    [SerializeField] private float angleThreshold;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        isGrabbed = false;
        isCutting = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = _rb.linearVelocity;

        // If the object is not moving, skip
        if (velocity.sqrMagnitude < 0.0001f)
        {
            isCutting = false;
            return;
        }

        // Compute angle between velocity and the downward Y-axis
        float angle = Vector3.Angle(velocity, Vector3.down);

        // Set bool true if angle is within threshold
        isCutting = angle < angleThreshold;
    }

    public void OnSelect()
    {
        isGrabbed = true;
    }

    public void OnRelease()
    {
        isGrabbed = false;
    }
    
}   // End of class
