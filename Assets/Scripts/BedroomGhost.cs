using UnityEngine;

public class Ghost : MonoBehaviour
{
    public void GhostEnterDresser()
    {
        BedroomManager.Instance.GhostEnterDresser();
    }
    
    public void UnlockBedroomDoor()
    {
        BedroomManager.Instance.GhostPassDoor();
    }
}
