using Facebook.WitAi.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace SpellBind
{
    public class SpellsController : MonoBehaviour
    {
        [SerializeField]
        private WandActionController wandActionController;

        [Header("DEBUG")]
        [SerializeField] private TextMeshProUGUI spellDebugText;

        // Start is called before the first frame update
        void Start()
        {
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
                Debug.LogFormat("Spell : {0}", _spell.ToString());
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
        /// Is called whenever the microphone is deactivated
        /// </summary>
        public void OnStoppedListening()
        {
            Debug.Log("STOPPED LISTENING");
            wandActionController.ValidateWitActivation();
        }

        private Spells ValidateSpell(string _command)
        {
            _command = _command.ToLower().Replace(".", "");
            Debug.LogFormat("Command Lower : {0}", _command);
            spellDebugText.text = _command;
            if (SpellDictionary.flySpellDictionary.Contains(_command))
                return Spells.Fly;
            else if (SpellDictionary.dropSpellDictionary.Contains(_command))
                return Spells.Drop;
            else if (SpellDictionary.throwSpellDictionary.Contains(_command))
                return Spells.Throw;
            else if (SpellDictionary.captureSpellDictionary.Contains(_command))
                return Spells.Capture;
            else if (SpellDictionary.smashSpellDictionary.Contains(_command))
                return Spells.Smash;
            else if (SpellDictionary.attackSpellDictionary.Contains(_command))
                return Spells.Attack;

            return Spells.None;
        }
    }
}


