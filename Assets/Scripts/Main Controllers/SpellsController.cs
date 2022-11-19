using Facebook.WitAi.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Text.RegularExpressions;

namespace SpellBind
{
    public class SpellsController : MonoBehaviour
    {
        [SerializeField]
        private WandActionController wandActionController;

        private Spells lastSpell;
        private float lastSpellTime;
        private float spellDelayLimit = 1.5f;

        // Start is called before the first frame update
        void Start()
        {
            lastSpell = Spells.None;
            lastSpellTime = 0;
        }

        private void OnDestroy()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Is called whenever SDK detects a voice
        /// </summary>
        /// <param name="_command"></param>
        public void OnVoiceCommand(string _command)
        {
            Debug.LogFormat("Voice Command : {0}", _command);
            if (!string.IsNullOrEmpty(_command))
            {
                Spells _spell = ValidateSpell(_command);

                switch (_spell)
                {
                    case Spells.Fly:
                        //Make objects fly!
                        wandActionController.onFlySpell.Invoke();
                        break;
                    case Spells.Drop:
                        //Drop objects
                        wandActionController.onDropSpell.Invoke();
                        break;
                    case Spells.Throw:
                        //Throw objects
                        wandActionController.onThrowSpell.Invoke();
                        break;
                    case Spells.Capture:
                        //Capture objects
                        wandActionController.onCaptureSpell.Invoke();
                        break;
                    case Spells.Smash:
                        //Smash objects
                        wandActionController.onSmashSpell.Invoke();
                        break;
                    case Spells.Attack:
                        //Attack objects
                        wandActionController.onAttackSpell.Invoke();
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Is called whenever the microphone is activated
        /// </summary>
        public void OnStartedListening()
        {
            Debug.Log("<color=green>STARTED LISTENING</color>");
            GameManager.Instance.onMicStart?.Invoke();
            GameManager.Instance.uiController.ToggleMicIndicator(true);
        }

        /// <summary>
        /// Is called whenever the microphone is deactivated
        /// </summary>
        public void OnStoppedListening()
        {
            Debug.Log("<color=red>STOPPED LISTENING</color>");
            wandActionController.ValidateWitActivation();
            GameManager.Instance.onMicStop?.Invoke();
            GameManager.Instance.uiController.ToggleMicIndicator(false);
        }

        private Spells ValidateSpell(string _command)
        {
            //Remove special characters from the recognized command
            _command = Regex.Replace(_command, "[^a-zA-Z]+", "").ToLower();
            
            //Check if the spell dictionary contains the spoken command
            Spells _spell = Spells.None;
            if (SpellDictionary.flySpellDictionary.Contains(_command))
                _spell = Spells.Fly;
            else if (SpellDictionary.dropSpellDictionary.Contains(_command))
                _spell = Spells.Drop;
            else if (SpellDictionary.throwSpellDictionary.Contains(_command))
                _spell = Spells.Throw;
            else if (SpellDictionary.captureSpellDictionary.Contains(_command))
                _spell = Spells.Capture;
            else if (SpellDictionary.smashSpellDictionary.Contains(_command))
                _spell = Spells.Smash;
            else if (SpellDictionary.attackSpellDictionary.Contains(_command))
                _spell = Spells.Attack;

            //Avoid command spam problem
            float _timeSinceLastSpell = Time.time - lastSpellTime;
            Debug.LogFormat("<color={3}>Current Spell : {0} | Last Spell : {1} | Time Diff : {2}</color>",
                _spell.ToString(), lastSpell.ToString(), _timeSinceLastSpell.ToString("F2"),
                (_timeSinceLastSpell < spellDelayLimit) ? "orange" : "lime");

            if (_spell == lastSpell && _timeSinceLastSpell < spellDelayLimit)
            {
                return Spells.None;
            }

            //Set last spell and spell time
            lastSpell = _spell;
            lastSpellTime = Time.time;

            //Set the spoken command to the UI indicator
            GameManager.Instance.uiController.SetCommandText(_spell == Spells.None ?
                _command : _spell.ToString(), _spell != Spells.None);
            
            return _spell;
        }
    }
}


