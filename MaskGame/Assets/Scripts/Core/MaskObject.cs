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
        [ShowIf("interactive")]
        [SerializeField] private Key key;
        [ShowIf("interactive")]
        [SerializeField] private TextMeshProUGUI keyText;
        [ShowIf("interactive")]
        [SerializeField] private MaskObject objectToSendInfo;

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

        public void SetSpriteToPart(MaskPart part, Sprite sprite)
        {
            var pair = partSpritePair.Find(pair => pair.One.Equals(part));
            if (pair != null)
            {
                pair.Two.sprite = sprite;
            }
        }

        public Sprite GetSpriteFromPart(MaskPart part)
        {
            var pair = partSpritePair.Find(pair => pair.One.Equals(part));
            return pair?.Two.sprite;
        }

        private void Update()
        {
            if (!interactive) return;
            if (Keyboard.current == null || !Keyboard.current[key].wasPressedThisFrame) return;
            // DebugUtils.DebugLogMsg($"Direct poll: {key} was pressed!", DebugUtils.DebugType.Regular);
            var pair = partSpritePair.Find(pair => pair.One.Equals(MaskPart.Eye));
            if (pair != null)
            {
                objectToSendInfo.SetSpriteToPart(MaskPart.Eye, pair.Two.sprite);    
            }
        }
    }
}
