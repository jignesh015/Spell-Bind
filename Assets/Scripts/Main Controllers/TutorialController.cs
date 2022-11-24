using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;

namespace SpellBind
{
    public class TutorialController : MonoBehaviour
    {
        private SpellTutorial currentSpellTutorial;

        private int enemyKilledCounter;
        private float messageDelay = 1.5f;

        //Attack Tutorial
        private bool isWaitingForWandPick;
        private bool isWaitingForFirstAttack;
        private bool completedFirstAttack;

        //Capture and Smash tutorial
        private bool isCaptured;

        //Fly and Throw tutorial
        private bool flyComplete;
        private bool isWaitingToThrow;
        private bool throwComplete;
        private bool spawnSpellBomb;

        //Defense tutorial
        private bool isWaitingToDefend;
        private bool defendComplete;

        private GameManager gameManager;
        private UIController uiController;
        private WandActionController wandController;

        private bool isTutorialEnabled;

        // Start is called before the first frame update
        void Start()
        {
            //Get script references
            gameManager = GameManager.Instance;
            uiController = gameManager.uiController;
            wandController = gameManager.wandActionController;

            //Assign delegates
            gameManager.onEnemyAttacked += OnFirstAttackComplete;
            gameManager.onEnemyCaptured += OnCaptured;
            gameManager.onEnemyKilled += OnEnemyKilled;
            gameManager.onSpellBombFly += OnSpellBombFly;
            gameManager.onSpellBombThrow += OnSpellBombThrow;
            gameManager.onPlayerDefend += OnPlayerDefend;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isTutorialEnabled) return;

            #region ATTACK TUTORIAL
            if (currentSpellTutorial == SpellTutorial.Attack)
            {
                //If waiting for wand pick and player picks the wand, proceed 
                if (isWaitingForWandPick && gameManager.playerController.isGrabbingWand)
                {
                    isWaitingForWandPick = false;
                    StopAllCoroutines();
                    StartCoroutine(AttackTutorial(1));
                }
                else if (isWaitingForFirstAttack && completedFirstAttack)
                {
                    isWaitingForFirstAttack = false;
                    StopAllCoroutines();
                    StartCoroutine(AttackTutorial(2));
                }
                else if (enemyKilledCounter == 1)
                {
                    //Start capture tutorial
                    StopAllCoroutines();
                    StartCoroutine(CaptureAndSmashTutorial(0));
                }
            }
            #endregion

            #region CAPTURE AND SMASH TUTORIAL
            if(currentSpellTutorial == SpellTutorial.Capture)
            {
                if(isCaptured)
                {
                    //Start smash tutorial
                    StopAllCoroutines();
                    StartCoroutine(CaptureAndSmashTutorial(1));
                }
            }
            if(currentSpellTutorial == SpellTutorial.Smash)
            {
                if(enemyKilledCounter == 2)
                {
                    //Start fly and throw tutorial
                    StopAllCoroutines();
                    StartCoroutine(FlyAndThrowTutorial(0));
                }
            }
            #endregion

            #region FLY AND THROW
            if(currentSpellTutorial == SpellTutorial.Fly)
            {
                if(spawnSpellBomb && gameManager.spawnedSpellBombs.Count == 0)
                {
                    gameManager.SpawnBomb(SpellBombType.SingleShot, true);
                    uiController.ToggleSpellBombArrowAlert(true);
                }

                if(flyComplete)
                {
                    //Start throw tutorial
                    StopAllCoroutines();
                    StartCoroutine(FlyAndThrowTutorial(1));
                }
            }
            if(currentSpellTutorial == SpellTutorial.Throw)
            {
                if (spawnSpellBomb && gameManager.spawnedSpellBombs.Count == 0)
                {
                    gameManager.SpawnBomb(SpellBombType.SingleShot, true);
                    uiController.ToggleSpellBombArrowAlert(true);
                    spawnSpellBomb = false;
                }

                if (isWaitingToThrow && throwComplete)
                {
                    isWaitingToThrow = false;
                    StopAllCoroutines();
                    StartCoroutine(FlyAndThrowTutorial(2));
                }
                if(enemyKilledCounter == 3)
                {
                    //Start Defense tutorial
                    StopAllCoroutines();
                    StartCoroutine(DefenseTutorial(0));
                }
            }
            #endregion

            #region DEFEND
            if(currentSpellTutorial == SpellTutorial.Defend)
            {
                if(isWaitingToDefend && defendComplete)
                {
                    isWaitingToDefend = false;
                    StopAllCoroutines();
                    StartCoroutine(DefenseTutorial(1));
                }

                if(enemyKilledCounter == 5)
                {
                    //Tutorial finish
                    StopAllCoroutines();
                    StartCoroutine(DefenseTutorial(2));
                }
            }
            #endregion
        }

        /// <summary>
        /// Starts the game tutorial
        /// </summary>
        public void InitializeTutorial()
        {
            StartCoroutine(InitializeTutorialAsync());
        }

        private IEnumerator InitializeTutorialAsync()
        {
            while(gameManager == null)
            {
                yield return null;
            }

            //Reset variables
            enemyKilledCounter = 0;
            isWaitingForWandPick = false;
            isWaitingForFirstAttack = false;
            completedFirstAttack = false;
            isCaptured = false;
            flyComplete = false;
            isWaitingToThrow = false;
            throwComplete = false;
            spawnSpellBomb = false;
            isWaitingToDefend = false;
            defendComplete = false;

            currentSpellTutorial = SpellTutorial.Attack;
            gameManager.playerController.disableDefense = true;

            //Enable tutorial
            isTutorialEnabled = true;

            //Start attack tutorial
            StartCoroutine(AttackTutorial(0));

        }

        /// <summary>
        /// Shows tutorial for Attack Spell
        /// </summary>
        /// <returns></returns>
        private IEnumerator AttackTutorial(int _index)
        {
            currentSpellTutorial = SpellTutorial.Attack;
            switch(_index)
            {
                case 0:
                    //Tutorial Intro
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextIntro, 0));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextIntro, 1));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextIntro, 2));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextIntro, 3, 0));
                    isWaitingForWandPick = true;
                    //Show and Highlight wand
                    wandController.ToggleWandVisibility(true);
                    wandController.ToggleWandHighlight(true);
                    break;
                case 1:
                    //On picking up wand
                    wandController.ToggleWandHighlight(false);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnWandPick, 0));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnWandPick, 1));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnWandPick, 2, 0));
                    //Spawn an attacker for tutorial
                    gameManager.SpawnEnemy(EnemyType.Attacker, true);
                    yield return new WaitForSeconds(messageDelay * 1.2f);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnWandPick, 3, 0));
                    isWaitingForFirstAttack = true;
                    break;
                case 2:
                    //On first attack
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnFirstAttack, 0));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnFirstAttack, 1, 0));
                    break;
            }
        }

        /// <summary>
        /// Shows tutorial for Capture and Smash Spells
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        private IEnumerator CaptureAndSmashTutorial(int _index)
        {
            
            switch(_index)
            {
                case 0:
                    //Capture intro
                    isCaptured = false;
                    currentSpellTutorial = SpellTutorial.Capture;
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialCapture, 0));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialCapture, 1, 0));
                    //Spawn a dodger for tutorial
                    gameManager.SpawnEnemy(EnemyType.Dodger, true);
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialCapture, 2, 0));
                    break;
                case 1:
                    //On Captured
                    currentSpellTutorial = SpellTutorial.Smash;
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialSmash, 0, 0));
                    break;
            }
        }

        /// <summary>
        /// Starts the tutorial for Fly and Throw Spells
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        private IEnumerator FlyAndThrowTutorial(int _index)
        {
            switch(_index)
            {
                case 0:
                    //Fly spell intro
                    currentSpellTutorial = SpellTutorial.Fly;
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialFly, 0));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialFly, 1, 0));
                    //Spawn a buffed enemy for tutorial
                    gameManager.SpawnEnemy(EnemyType.Buffed, true);
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialFly, 2));
                    //Spawn and highlight a spell bomb
                    spawnSpellBomb = true;
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialFly, 3, 0));
                    break;
                case 1:
                    //Throw Spell
                    currentSpellTutorial = SpellTutorial.Throw;
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialThrow, 0, 0));
                    isWaitingToThrow = true;
                    break;
                case 2:
                    //On Spell bomb throw
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialThrow, 1, 0));
                    break;
            }
        }

        /// <summary>
        /// Starts the tutorial for Defense action
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        private IEnumerator DefenseTutorial(int _index)
        {
            currentSpellTutorial = SpellTutorial.Defend;
            switch(_index)
            {
                case 0:
                    //Defense intro
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialDefense, 0));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialDefense, 1));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialDefense, 2, 0));
                    isWaitingToDefend = true;
                    gameManager.playerController.disableDefense = false;
                    break;
                case 1:
                    //On successful defend
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextOutro, 0));
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextOutro, 1, 0));
                    //Spawn an attacker and dodger
                    gameManager.SpawnEnemy(EnemyType.Attacker);
                    yield return new WaitForSeconds(messageDelay);
                    gameManager.SpawnEnemy(EnemyType.Dodger);
                    break;
                case 2:
                    //Tutorial Complete
                    isTutorialEnabled = false;
                    currentSpellTutorial = SpellTutorial.Done;
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextOutro, 2, 0));
                    break;
            }
        }

        /// <summary>
        /// Reads the message from the given dictionary and prints it on the UI
        /// </summary>
        /// <param name="_textDict"></param>
        /// <param name="_index"></param>
        private IEnumerator DisplayUIMessage(Dictionary<string, float> _textDict, int _index, float _delayFactor = 1)
        {
            uiController.DisplayUIMessage(_textDict.ElementAt(_index).Key,
                _textDict.ElementAt(_index).Value);

            yield return new WaitForSeconds(_textDict.ElementAt(_index).Value + 0.25f);

            yield return new WaitForSeconds(messageDelay * _delayFactor);
        }

        private void OnFirstAttackComplete() { completedFirstAttack = true; }

        private void OnEnemyKilled() { enemyKilledCounter++; }

        private void OnCaptured() { isCaptured = true; } 

        private void OnSpellBombFly() 
        { 
            flyComplete = true;
            uiController.ToggleSpellBombArrowAlert(false);
        }

        private void OnSpellBombThrow() { throwComplete = true; }

        private void OnPlayerDefend() { defendComplete = true; }
    }

    public enum SpellTutorial
    {
        Attack,
        Capture,
        Smash,
        Fly,
        Throw,
        Drop,
        Defend,
        Done
    }
}
