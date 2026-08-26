using UnityEngine;
[ExecuteAlways]
public class LerpGhostPosition : MonoBehaviour
{
    public Transform realTrans1;
    public Transform realTrans2;
    public float lerp1;
    public float lerp2;
    public bool isSecond = false;
    [SerializeField] private Vector3 setPos1;
    [SerializeField] private Vector3 setPos2;
    private Vector3 offset1;
    private Vector3 offset2;
    private Vector3 startPos;
    void Start()
    {
        
    }
    void Update()
    {
        startPos = Vector3.zero;
        offset1 = realTrans1.position - setPos1;
        offset2 = realTrans2.position - setPos2;
        Vector3 lerpPos1 = Vector3.Slerp(startPos + offset1, startPos, lerp1);
        Vector3 lerpPos2 = Vector3.Slerp(startPos + offset2, startPos, lerp2);
        
        transform.position = isSecond? lerpPos2 : lerpPos1;
    }
}
