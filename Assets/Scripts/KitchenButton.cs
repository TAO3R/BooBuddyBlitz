using UnityEngine;

public class KitchenButton : MonoBehaviour
{
    public Animator flameAnim;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect()
    {
        Debug.Log("[KitchenButton] button is selected");
        flameAnim.SetTrigger("TurnOn");
    }
    
}   // End of class
