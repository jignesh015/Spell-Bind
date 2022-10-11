using Facebook.WitAi.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpellsController : MonoBehaviour
{
    [SerializeField]
    private WandActionController wandActionController;

    private List<string> flySpellDictionary;

    //public UnityEvent<string> voiceCommandEvent;

    // Start is called before the first frame update
    void Start()
    {
        flySpellDictionary = new List<string>()
        { 
            "fly", "die", "play", "shy", "my", "fair", "flower", "sky", "ply", "why",
            "high", "sigh", "five", "fine", "flight", "plight", "sight", "right",
            "might", "height", "light", "night", "flag", "friend", "diet", "hi", "bye",
            "spy", "tie", "fry", "pry", "plan", "plane", "snap", "buy", "pie", "try", "defy",
            "like", "bike", "mike", "spike", "reply", "goodbye", "polite", "police", "follow",
            "follower"
        };

        //voiceCommandEvent.AddListener(OnVoiceCommand);
    }

    private void OnDestroy()
    {
        //voiceCommandEvent.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
                    wandActionController.MakeThingsFly();
                    break;
                default:
                    break;
            }
        }
    }

    private Spells ValidateSpell(string _command)
    {
        _command = _command.ToLower().Replace(".","");
        Debug.LogFormat("Command Lower : {0}", _command);
        if (flySpellDictionary.Contains(_command))
            return Spells.Fly;

        return Spells.None;
    }
}

public enum Spells
{ 
    None,
    Fly,
    Drop,
    Light,
    Dark
}
