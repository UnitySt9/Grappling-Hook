using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public KeyCode RestartKey = KeyCode.R;

        private void Update()
        {
            if (Input.GetKeyDown(RestartKey))
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
