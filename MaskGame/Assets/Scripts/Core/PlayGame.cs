using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace Core
{
    public class PlayGame : MonoBehaviour
    {
        [SerializeField] private PlayableDirector startGameDirector;
        [SerializeField] private Key playKey;
        [SerializeField] private TextMeshProUGUI playKeyText;
        private bool _play;

        private void Start()
        {
            playKeyText.text = playKey.ToString().ToUpper();
        }

        private void Update()
        {
            if (_play) return;
            if (Keyboard.current == null || !Keyboard.current[playKey].wasPressedThisFrame) return;
            startGameDirector.Play();
            _play = true;
        }
    }
}
