using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UUtils;

namespace Core
{
    public class MaskObject : MonoBehaviour
    {
        [SerializeField] private List<MaskPartSpritePair> partSpritePair;
        [SerializeField] private bool interactive;
        [SerializeField] private SpriteRenderer blockedSprite;

        [ShowIf("interactive")] [SerializeField]
        private Key key;

        [ShowIf("interactive")] [SerializeField]
        private TextMeshProUGUI keyText;

        [ShowIf("interactive")] [SerializeField]
        private MaskObject objectToSendInfo;

        [ShowIf("interactive")] [SerializeField]
        private int difficultyLevel;

        private void Awake()
        {
            foreach (var maskPartSpritePair in partSpritePair)
            {
                if (maskPartSpritePair.Two == null)
                {
                    DebugUtils.DebugLogErrorMsg($"Sprite pair {maskPartSpritePair.One} has no valid sprite renderer.");
                }
            }
        }

        private void Start()
        {
            if (interactive)
            {
                keyText.text = key.ToString().ToLower();
            }
            else
            {
                keyText.gameObject.SetActive(false);
            }
        }

        public void SetSpriteToPart(MaskPart part, SpriteRenderer spriteRenderer)
        {
            var pair = partSpritePair.Find(pair => pair.One.Equals(part));
            if (pair != null)
            {
                pair.Two.transform.localPosition = spriteRenderer.gameObject.transform.localPosition;
                pair.Two.sprite = spriteRenderer.sprite;
            }
        }

        public SpriteRenderer GetSpriteFromPart(MaskPart part)
        {
            var pair = partSpritePair.Find(pair => pair.One.Equals(part));
            return pair?.Two;
        }

        private void Update()
        {
            if (!interactive) return;
            if (!LevelManager.GetSingleton().AllowInteraction()) return;
            if (blockedSprite.enabled) return;
            if (Keyboard.current == null || !Keyboard.current[key].wasPressedThisFrame) return;
            // DebugUtils.DebugLogMsg($"Direct poll: {key} was pressed!", DebugUtils.DebugType.Regular);
            LevelManager.GetSingleton().ReceiveInteraction(this);
        }

        [Button("Reset Mask")]
        public void ResetMask()
        {
            partSpritePair.ForEach(part => part.Two.sprite = null);
        }

        public void UpdateBlockedStatus(int currentDifficultyLevel)
        {
            blockedSprite.enabled = currentDifficultyLevel < difficultyLevel;
        }

        public int DifficultyLevel() => difficultyLevel;
    }
}