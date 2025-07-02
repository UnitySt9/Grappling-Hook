using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts
{
    public class RestartManager : MonoBehaviour
    {
        private readonly KeyCode _restartKey = KeyCode.R;

        private void Update()
        {
            if (Input.GetKeyDown(_restartKey))
            {
                RestartScene();
            }
        }

        private void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
