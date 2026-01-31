using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UUtils;

namespace Core
{
    public class LevelManager : WeakSingleton<LevelManager>
    {
        [SerializeField]
        private List<MaskObject> maskObjects;
        [SerializeField]
        private MaskObject exampleObject;
        [SerializeField]
        private MaskObject makingObject;
        [SerializeField]
        private bool initOnStart;
        [SerializeField] private SpriteRenderer timerSprite;

        [Header("Level Settings")] 
        [SerializeField]
        private float startTimer;
        [SerializeField]
        private float timerReducer;

        private int _currentDifficultyLevel;
        private bool _allowInteraction;
        private float _currentTime;

        private void Start()
        {
            _allowInteraction = false;
            if (initOnStart)
            {
                StartLevel();
            }
        }

        public void StartLevel()
        {
            StartCoroutine(StartLevelCoroutine());
        }

        private IEnumerator StartLevelCoroutine()
        {
            _allowInteraction = false;
            _currentDifficultyLevel = 1;
            yield return null;
            
            //Generate Mask
            GenerateRandomMask();
            
            //Unblock Mask Making
            makingObject.ResetMask();
            _allowInteraction = true;
            
            
        }

        private void GenerateRandomMask()
        {
            var difficultyList = maskObjects.FindAll(maskObject => maskObject.DifficultyLevel() <= _currentDifficultyLevel);

            SetRandomPart(difficultyList, MaskPart.Eye);
            SetRandomPart(difficultyList, MaskPart.Nose);
            SetRandomPart(difficultyList, MaskPart.Access);
            return;

            void SetRandomPart(List<MaskObject> randomList, MaskPart part)
            {
                var random = RandomHelper<MaskObject>.GetRandomFromList(randomList);
                random.SetSpriteToPart(part, random.GetSpriteFromPart(part));
            }
        }

        public bool AllowInteraction() => _allowInteraction;
    }
}