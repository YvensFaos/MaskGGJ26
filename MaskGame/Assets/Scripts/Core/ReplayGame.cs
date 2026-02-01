using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Core
{
    public class ReplayGame : MonoBehaviour
    {
        [SerializeField] private Key replayKey;
        [SerializeField] private TextMeshProUGUI replayKeyText;

        private void Start()
        {
            replayKeyText.text = replayKey.ToString().ToUpper();
        }

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current[replayKey].wasPressedThisFrame) return;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}