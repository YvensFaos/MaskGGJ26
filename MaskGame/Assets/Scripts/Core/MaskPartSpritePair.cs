using System;
using UnityEngine;
using UUtils;

namespace Core
{
    [Serializable]
    public class MaskPartSpritePair : Pair<MaskPart, SpriteRenderer>
    {
        public MaskPartSpritePair(MaskPart one, SpriteRenderer two) : base(one, two)
        {
            
        }
    }
}