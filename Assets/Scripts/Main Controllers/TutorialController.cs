using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        private bool attackTutorialComplete;

        //Capture and Smash tutorial
        private bool isCaptured;

        //Fly and Throw tutorial
        private bool flyComplete;


        private GameManager gameManager;
        private UIController uiController;

        // Start is called before the first frame update
        void Start()
        {
            //Get script references
            gameManager = GameManager.Instance;
            uiController = gameManager.uiController;

            //Assign delegates
            gameManager.onEnemyAttacked += OnFirstAttackComplete;
            gameManager.onEnemyCaptured += OnCaptured;
            gameManager.onEnemyKilled += OnEnemyKilled;
        }

        // Update is called once per frame
        void Update()
        {
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
        }

        /// <summary>
        /// Starts the game tutorial
        /// </summary>
        public void InitializeTutorial()
        {
            //Reset variables
            enemyKilledCounter = 0;
            isWaitingForWandPick = false;
            isWaitingForFirstAttack = false;
            completedFirstAttack = false;
            isCaptured = false;
            currentSpellTutorial = SpellTutorial.Attack;

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
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextIntro, 1));
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextIntro, 2));
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialTextIntro, 3));
                    isWaitingForWandPick = true;
                    //TODO : Highlight wand
                    break;
                case 1:
                    //On picking up wand
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnWandPick, 0));
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnWandPick, 1));
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnWandPick, 2));
                    //Spawn an attacker for tutorial
                    gameManager.SpawnEnemy(EnemyType.Attacker, true);
                    yield return new WaitForSeconds(messageDelay + 3);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnWandPick, 3));
                    isWaitingForFirstAttack = true;
                    break;
                case 2:
                    //On first attack
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnFirstAttack, 0));
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialOnFirstAttack, 1));
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
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialCapture, 1));
                    //Spawn a dodger for tutorial
                    gameManager.SpawnEnemy(EnemyType.Dodger, true);
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialCapture, 2));
                    break;
                case 1:
                    //On Captured
                    currentSpellTutorial = SpellTutorial.Smash;
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialSmash, 0));
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialSmash, 1));
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
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialFly, 1));
                    //Spawn a buffed enemy for tutorial
                    gameManager.SpawnEnemy(EnemyType.Buffed, true);
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialFly, 2));
                    //Spawn and highlight a spell bomb
                    yield return new WaitForSeconds(messageDelay);
                    yield return StartCoroutine(DisplayUIMessage(UIMessageDictionary.tutorialFly, 3));
                    break;
                case 1:
                    break;
            }
        }

        /// <summary>
        /// Reads the message from the given dictionary and prints it on the UI
        /// </summary>
        /// <param name="_textDict"></param>
        /// <param name="_index"></param>
        private IEnumerator DisplayUIMessage(Dictionary<string, float> _textDict, int _index)
        {
            uiController.DisplayUIMessage(_textDict.ElementAt(_index).Key,
                _textDict.ElementAt(_index).Value);

            yield return new WaitForSeconds(_textDict.ElementAt(_index).Value);
        }

        private void OnFirstAttackComplete() { completedFirstAttack = true; }

        private void OnEnemyKilled() { enemyKilledCounter++; }

        private void OnCaptured() { isCaptured = true; } 
    }

    public enum SpellTutorial
    {
        Attack,
        Capture,
        Smash,
        Fly,
        Throw,
        Drop
    }
}
