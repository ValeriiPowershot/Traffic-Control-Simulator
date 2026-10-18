using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneRestarter : MonoBehaviour
{
    [SerializeField] private KeyCode _restartKey = KeyCode.R;

    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(_restartKey))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
