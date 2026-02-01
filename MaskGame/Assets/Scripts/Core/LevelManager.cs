using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DG.Tweening;
using Mono.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UUtils;
using Random = System.Random;

namespace Core
{
    public class LevelManager : WeakSingleton<LevelManager>
    {
        private static readonly int Stand = Animator.StringToHash("Stand");
        private static readonly int Randomize = Animator.StringToHash("Randomize");

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
        [SerializeField] private SpriteRenderer symbolSpriteRenderer;
        [SerializeField] private Sprite eyesSymbol;
        [SerializeField] private Sprite noseSymbol;
        [SerializeField] private Sprite acceSymbol;
        [SerializeField] private Animator plaqueAnimator;
        [SerializeField] private PlayableDirector correctDirector;
        [SerializeField] private ParticleSystem smokeParticles;
        [SerializeField] private ParticleSystem floorSmokeParticles;
        [SerializeField] private ParticleSystem randomOrderParticles;
        [SerializeField] private PlayableDirector gameOverDirector;
        [SerializeField] private CrescendoManager crescendoManager;
        [SerializeField] private AudioSource sfxInteractSource;
        [SerializeField] private List<AudioClip> interactClips;
        [SerializeField] private AudioClip incorrectClip;

        [Header("Level Settings")]
        [SerializeField]
        private int startDifficulty = 0;
        [SerializeField]
        private float startTimer;
        [SerializeField]
        private float timerReducer;
        [SerializeField] 
        private AnimationCurve difficultyCurve;

        private int _currentDifficultyLevel;
        private bool _allowInteraction;
        private float _currentTime;
        private readonly MaskPart[] _levelOrder = { MaskPart.Eye, MaskPart.Nose, MaskPart.Access };
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
            _score = 0;
            UpdateCurrentDifficultyLevel();
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
                    currentPartText.text = "Game Over!";
                    gameIsOn = false;
                    _allowInteraction = false;
                    gameOverDirector.Play();
                }
                else
                {
                    _allowInteraction = false;
                    _score++;
                    //RandomizeOrder();
                    
                    crescendoManager.UpdateCrescendo(_score);
                    UpdateScoreText();
                    smokeParticles.Play();
                    correctDirector.Play();
                    //Give it one frame for the director
                    yield return null;
                    //Wait until the director is done
                    yield return new WaitUntil(() => correctDirector.state != PlayState.Playing);
                    
                    UpdateCurrentDifficultyLevel();
                    _currentTime *= timerReducer;
                    UpdateTimerReducer(30, 0.8f);
                    UpdateTimerReducer(20, 0.9f);
                    UpdateTimerReducer(10, 0.95f);
                    UpdateTimerReducer(30, 0.8f);
              
                    //Minimal time is 3 seconds, for now. Hardcoded!
                    _currentTime = Mathf.Max(3, _currentTime);
                }
            }

            yield break;

            void UpdateTimerReducer(int threshold, float newValue)
            {
                if (_score != threshold) return;
                floorSmokeParticles.Play();
                timerReducer = newValue;
            }

            void RandomizeOrder()
            {
                if (_score % 5 != 0) return;
                if (RandomChanceUtils.GetChance(50.0f)) return;
                randomOrderParticles.Play();
                plaqueAnimator.SetTrigger(Randomize);
                var asList = _levelOrder.ToList();
                var one = RandomHelper<MaskPart>.GetRandomFromListWithIndex(asList, out var indexOne);
                var two = RandomHelper<MaskPart>.GetRandomFromListWithIndex(asList, out var indexTwo);
                    
                _levelOrder[indexOne] = two;
                _levelOrder[indexTwo] = one;
                UpdateOrderPlaqueVisuals();
            }
        }

        public void ReceiveInteraction(MaskObject interactObject)
        {
            var currentPart = _levelOrder[_orderIndex];
            var spriteRenderer = interactObject.GetSpriteFromPart(currentPart);
            makingObject.SetSpriteToPart(currentPart, spriteRenderer);
            var check = CheckMatchingPart(currentPart, spriteRenderer);
            sfxInteractSource.Stop();
            sfxInteractSource.PlayOneShot(check
                ? RandomHelper<AudioClip>.GetRandomFromList(interactClips)
                : incorrectClip);
            switch (currentPart)
            {
                case MaskPart.Eye:
                    _correctEye = check;
                    break;
                case MaskPart.Nose:
                    _correctNose = check;
                    break;
                case MaskPart.Access:
                    _correctAccess = check;
                    break;
                case MaskPart.Base:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
            plaqueAnimator.SetTrigger(Stand);
            UpdateOrderPlaqueVisuals();
        }

        private void UpdateOrderPlaqueVisuals()
        {
            switch (_levelOrder[_orderIndex])
            {
                case MaskPart.Eye:
                    symbolSpriteRenderer.sprite = eyesSymbol;
                    break;
                case MaskPart.Nose:
                    symbolSpriteRenderer.sprite = noseSymbol;
                    break;
                case MaskPart.Access:
                    symbolSpriteRenderer.sprite = acceSymbol;
                    break;
                case MaskPart.Base:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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

        private void UpdateCurrentDifficultyLevel()
        {
            _currentDifficultyLevel = (int) difficultyCurve.Evaluate(_score);
            maskObjects.ForEach(maskObject => maskObject.UpdateBlockedStatus(_currentDifficultyLevel));
        }
        
        public bool AllowInteraction() => _allowInteraction;
        
        public int GetScore() => _score;
    }
}