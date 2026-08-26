using UnityEngine;

public class BathroomGhost : MonoBehaviour
{
    public void GhostEnterMirror()
    {
        BathroomManager.Instance.GhostEnterMirror();
    }

    public void GhostEnterDoor()
    {
        BathroomManager.Instance.GhostEnterDoor();
    }
}
