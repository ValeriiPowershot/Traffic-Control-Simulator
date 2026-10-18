using UnityEngine;
using Button = UnityEngine.UI.Button;

public class TapToStartButton : MonoBehaviour
{
    
    
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(TapToStartOnClick);
    }

    private void TapToStartOnClick()
    {
        
    }
    
}
