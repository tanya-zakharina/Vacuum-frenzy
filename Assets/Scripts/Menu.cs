using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void PlayEndless()
    {
        SceneManager.LoadSceneAsync("Endless");
    }
}
