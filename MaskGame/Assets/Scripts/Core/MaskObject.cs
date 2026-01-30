using System.Collections.Generic;
using UnityEngine;
using UUtils;

namespace Core
{
    public class MaskObject : MonoBehaviour
    {
        [SerializeField] private List<MaskPartSpritePair> partSpritePair;

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

        public void SetSpriteToPart(MaskPart part, Sprite sprite)
        {
            var pair = partSpritePair.Find(pair => pair.One.Equals(part));
            if (pair != null)
            {
                pair.Two.sprite = sprite;
            }
        }
    }
}
