using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene changing


public class changeScene : MonoBehaviour
{
    // Make sure the function is 'public' so the button can see it
    public void LoadTargetScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
