using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;

namespace SpellBind
{
    public class UIController : MonoBehaviour
    {
        [Header("UI REFERENCES")]
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private Image playerHealthBar;

        [Header("SFX REFERENCES")]
        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private AudioClip scribbleSFX;

        private GameManager gameManager;
        private PlayerController player;

        // Start is called before the first frame update
        void Start()
        {
            gameManager= GameManager.Instance;
            player = gameManager.playerController;

            DisplayUIMessage("Lorem Ipsum is simply dummy text of the printing and typesetting industry.", 7);
        }

        // Update is called once per frame
        void Update()
        {
            //Display player's live health bar
            playerHealthBar.fillAmount = Mathf.Lerp(playerHealthBar.fillAmount,
                (float)player.currentPlayerHealth / player.maxPlayerHealth, Time.deltaTime);
        }

        /// <summary>
        /// Displays the given message over the specified amount of time
        /// </summary>
        /// <param name="_message"></param>
        /// <param name="_displayTime"></param>
        public void DisplayUIMessage(string _message, float _displayTime)
        {
            if(string.IsNullOrEmpty(_message))
            {
                StopCoroutine(nameof(DisplayUIMessageAsync));
                StopUISFX();
            }
            else
            {
                StartCoroutine(DisplayUIMessageAsync(_message, _displayTime));
            }
        }

        private IEnumerator DisplayUIMessageAsync(string _message, float _displayTime)
        {
            //Divide message into individual characters
            List<string> _messageSplit = Regex.Split(_message, string.Empty).ToList();
            float _delayForEachChar = _displayTime/ _messageSplit.Count;
            string _messageToDisplay = string.Empty;
            PlayUISFX(scribbleSFX);
            foreach(string _m in _messageSplit)
            {
                _messageToDisplay += _m;
                headerText.text = _messageToDisplay;
                yield return new WaitForSeconds(_delayForEachChar);
            }
            headerText.text = _message;
            StopUISFX();
        }

        private void PlayUISFX(AudioClip _clip, float _volume = 0.4f)
        {
            uiAudioSource.Stop();
            uiAudioSource.clip = _clip;
            uiAudioSource.volume = _volume;
            uiAudioSource.Play();
        }

        private void StopUISFX()
        {
            uiAudioSource.Stop();
        }
    }
}
