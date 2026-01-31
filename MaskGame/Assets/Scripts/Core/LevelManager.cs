using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
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
        [SerializeField] private TextMeshProUGUI currentPartText;
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Level Settings")]
        [SerializeField]
        private int startDifficulty = 0;
        [SerializeField]
        private float startTimer;
        [SerializeField]
        private float timerReducer;

        private int _currentDifficultyLevel;
        private bool _allowInteraction;
        private float _currentTime;
        private MaskPart[] _levelOrder = { MaskPart.Eye, MaskPart.Nose, MaskPart.Access };
        private int _orderIndex = 0;
        private bool _correctEye;
        private bool _correctNose;
        private bool _correctAccess;
        private int _score = 0;

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
            _currentDifficultyLevel = startDifficulty;
            _currentTime = startTimer;
            UpdateScoreText();
            yield return null;

            var gameIsOn = true;
            while (gameIsOn)
            {
                currentPartText.text = "";
                exampleObject.ResetMask();
                //Generate Mask
                GenerateRandomMask();
            
                //Unblock Mask Making
                makingObject.ResetMask();
                _allowInteraction = true;

                _orderIndex = 0;
                _correctEye = false;
                _correctNose = false;
                _correctAccess = false;
                currentPartText.text = _levelOrder[_orderIndex].ToString();
            
                var timesUp = false;
                timerSprite.transform.localScale = new Vector3(0, 1, 1);
                var timerTween = timerSprite.transform.DOScaleX(1, _currentTime).OnComplete(() =>
                {
                    timesUp = true;
                });
                yield return new WaitUntil(() => timesUp || (_correctEye && _correctNose && _correctAccess));
                timerTween.Kill();
                timerSprite.transform.localScale = new Vector3(0, 1, 1);

                if (timesUp)
                {
                    //TODO Game Over!
                    currentPartText.text = "Game Over!";
                    gameIsOn = false;
                }
                else
                {
                    _score++;
                    UpdateScoreText();
                    _currentTime *= timerReducer;
                    //Minimal time is 4 seconds, for now. Hardcoded!
                    _currentTime = Mathf.Max(4, _currentTime);
                }
            }
            
        }

        public void ReceiveInteraction(MaskObject interactObject)
        {
            var currentPart = _levelOrder[_orderIndex];
            var spriteRenderer = interactObject.GetSpriteFromPart(currentPart);
            makingObject.SetSpriteToPart(currentPart, spriteRenderer);
            if (CheckMatchingPart(currentPart, spriteRenderer))
            {
                switch (currentPart)
                {
                    case MaskPart.Eye:
                        _correctEye = true;
                        break;
                    case MaskPart.Nose:
                        _correctNose = true;
                        break;
                    case MaskPart.Access:
                        _correctAccess = true;
                        break;
                    case MaskPart.Base:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            UpdateOrderText();
        }

        private bool CheckMatchingPart(MaskPart part, SpriteRenderer spriteRendererToCheck)
        {
            var correctSpriteRenderer = exampleObject.GetSpriteFromPart(part);
            return spriteRendererToCheck.sprite == correctSpriteRenderer.sprite;
        }

        private void UpdateOrderText()
        {
            _orderIndex = ++_orderIndex % _levelOrder.Length;
            currentPartText.text = _levelOrder[_orderIndex].ToString();
        }

        [Button("Generate Random Mask")]
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
                var spriteRenderer = random.GetSpriteFromPart(part);
                exampleObject.SetSpriteToPart(part, spriteRenderer);
            }
        }

        private void UpdateScoreText()
        {
            scoreText.text = _score.ToString();
        }

        public bool AllowInteraction() => _allowInteraction;
    }
}