using System.Collections;
using System.ComponentModel;
using UnityEngine;

public class RealPot : MonoBehaviour
{
    public Animator potAnim;
    
    public void StartHeating()
    {
        StartCoroutine(Heat());
    }

    private IEnumerator Heat()
    {
        yield return new WaitForSecondsRealtime(2f);
        potAnim.SetTrigger("Boil");
    }
}
