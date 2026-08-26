using UnityEngine;

public class FadeCanvasPanel : MonoBehaviour
{
    public void CallSceneTransition()
    {
        SceneTransitionManager.Instance.LoadNextScene();
    }
}
