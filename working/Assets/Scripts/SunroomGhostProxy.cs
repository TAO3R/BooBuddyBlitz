using UnityEngine;

public class SunroomGhostProxy : MonoBehaviour
{
    public void SetGhostState()
    {
        SunroomManager.Instance.GhostEnterTelevision();
    }

}
