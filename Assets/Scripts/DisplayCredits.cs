using TMPro;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using Unity.VisualScripting;

public class DisplayCredits : MonoBehaviour
{
    [SerializeField] Transform camTransform;
    private TextMeshPro tmpComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmpComponent = GetComponent<TextMeshPro>();
        Color color = new Color(tmpComponent.color.r, tmpComponent.color.g, tmpComponent.color.b, 0f);
        tmpComponent.color = color;
    }

    public IEnumerator ToggleDisplay(float startAlpha, float endAlpha, float duration)
    { 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);

            Color color = new Color(tmpComponent.color.r, tmpComponent.color.g, tmpComponent.color.b, currentAlpha);
            tmpComponent.color = color;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
