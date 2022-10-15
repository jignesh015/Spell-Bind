using Facebook.WitAi.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpellBind
{
    public class SpellsController : MonoBehaviour
    {
        [SerializeField]
        private WandActionController wandActionController;

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
                        wandActionController.LevitateThings();
                        break;
                    case Spells.Drop:
                        //Drop objects
                        wandActionController.DropThings();
                        break;
                    case Spells.Throw:
                        //Throw objects
                        wandActionController.ThrowThings();
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
            if (SpellDictionary.flySpellDictionary.Contains(_command))
                return Spells.Fly;
            else if (SpellDictionary.dropSpellDictionary.Contains(_command))
                return Spells.Drop;
            else if (SpellDictionary.throwSpellDictionary.Contains(_command))
                return Spells.Throw;
            else if (SpellDictionary.attackSpellDictionary.Contains(_command))
                return Spells.Attack;

            return Spells.None;
        }
    }
}


