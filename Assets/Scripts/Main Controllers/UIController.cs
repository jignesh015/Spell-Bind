using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;

namespace SpellBind
{
    public class UIController : MonoBehaviour
    {
        [Header("MAIN CANVAS REFERENCES")]
        [SerializeField] private Animator mainCanvasAnim;

        [Header("TABLE CANVAS REFERENCES")]
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private Image playerHealthBar;
        [SerializeField] private GameObject spellBombArrowAlert;
        [SerializeField] private GameObject spellBombMarker;
        [SerializeField] private GameObject micIndicator;

        [Header("HUD CANVAS REFERENCES")]
        [SerializeField] private Canvas hudCanvas;

        [Header("COMMAND TEXT REFERENCES")]
        [SerializeField] private TextMeshProUGUI commandText;
        [SerializeField] private float activeFontSize;
        [SerializeField] private float inactiveFontSize;
        [SerializeField] private Color validSpellTextColor;
        [SerializeField] private Color invalidSpellTextColor;
        [SerializeField] private Color inactiveSpellTextColor;

        [Header("SFX REFERENCES")]
        [SerializeField] private AudioSource uiAudioSource;
        [SerializeField] private AudioClip scribbleSFX;
        [SerializeField] private AudioClip uiSelectSFX;

        [Header("HAND TRACKING REFERENCES")]
        [SerializeField] private OVRHand hand;
        [SerializeField] private OVRInputModule inputModule;
        [SerializeField] private LineRenderer uiPointerLineRenderer; 
        [SerializeField] private MeshRenderer uiPointerCursor; 

        private GameManager gameManager;
        private PlayerController player;

        private void Awake()
        {
            inputModule.rayTransform = FindObjectsOfType<OVRHand>().ToList().Find(x => x.name.Contains("Right")).PointerPose;
        }

        // Start is called before the first frame update
        void Start()
        {
            gameManager= GameManager.Instance;
            player = gameManager.playerController;

            HandleMainCanvas(1);

            //Set HUD Canvas camera
            hudCanvas.worldCamera = GameObject.Find("CenterEyeAnchor").GetComponent<Camera>();
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
            //Stop the previous message coroutine and SFX
            StopCoroutine(nameof(DisplayUIMessageAsync));
            StopUISFX();
            if (!string.IsNullOrEmpty(_message))
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

        /// <summary>
        /// Handles the Main Canvas screens
        /// | 0 = Idle | 1 = Start Screen | 2 = Continue Screen | 3 = End Screen | 4 = Game Over Screen
        /// </summary>
        /// <param name="_index"></param>
        public void HandleMainCanvas(int _index)
        {
            string _animTrigger = "Idle";
            switch(_index)
            {
                case 0:
                    break;
                case 1:
                    _animTrigger = "StartScreen";
                    break;
                case 2:
                    _animTrigger = "ContinueScreen";
                    break;
                case 3:
                    _animTrigger = "EndScreen";
                    break;
                case 4:
                    _animTrigger = "GameOverScreen";
                    break;
            }
            mainCanvasAnim.SetTrigger(_animTrigger);
            ToggleHandUISelection(_index != 0);
        }

        /// <summary>
        /// Toggles the Arrow UI to alert player of the spawned spell bomb
        /// </summary>
        public void ToggleSpellBombArrowAlert(bool _toggleOn)
        {
            spellBombArrowAlert.SetActive(_toggleOn);
            spellBombMarker.SetActive(_toggleOn);
        }

        /// <summary>
        /// Plays the given audio clip on the UI audio source
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_volume"></param>
        public void PlayUISFX(AudioClip _clip, float _volume = 0.4f)
        {
            uiAudioSource.Stop();
            uiAudioSource.clip = _clip;
            uiAudioSource.volume = _volume;
            uiAudioSource.Play();
        }

        /// <summary>
        /// Stops the UI audio
        /// </summary>
        private void StopUISFX()
        {
            uiAudioSource.Stop();
        }

        /// <summary>
        /// Is called when "Start" button is clicked on UI
        /// </summary>
        public void OnStartButtonClick()
        {
            PlayUISFX(uiSelectSFX, 0.75f);
            gameManager.StartTutorial();
            //gameManager.StartLevel(1);
            HandleMainCanvas(0);
        }

        /// <summary>
        /// Is called when "Continue" button is clicked on UI
        /// </summary>
        public void OnContinueButtonClick()
        {
            PlayUISFX(uiSelectSFX, 0.75f);
            gameManager.StartLevel(1);
            HandleMainCanvas(0);
        }

        /// <summary>
        /// Is called when "Replay" Button is clicked on UI
        /// </summary>
        public void OnReplayButtonClicked()
        {
            PlayUISFX(uiSelectSFX, 0.75f);
            gameManager.StartLevel(1);
            HandleMainCanvas(0);
        }

        /// <summary>
        /// Is called when "Relearn" Button is clicked on UI
        /// </summary>
        public void OnRelearnButtonClicked()
        {
            PlayUISFX(uiSelectSFX, 0.75f);
            gameManager.StartTutorial();
            HandleMainCanvas(0);
        }

        /// <summary>
        /// Toggles Hands UI selection on/off
        /// </summary>
        /// <param name="_enable"></param>
        public void ToggleHandUISelection(bool _enable)
        {
            uiPointerLineRenderer.enabled = _enable;
            uiPointerCursor.enabled= _enable;
        }

        /// <summary>
        /// Toggles the mic indicator on/off
        /// </summary>
        public void ToggleMicIndicator(bool _enable)
        {
            micIndicator.SetActive(_enable);
            commandText.color = inactiveSpellTextColor;
            commandText.fontSize = inactiveFontSize;
        }

        /// <summary>
        /// Sets the command text to the command given by the player
        /// </summary>
        public void SetCommandText(string _command, bool _isValidSpell)
        {
            commandText.text = _command;
            commandText.color = _isValidSpell ? validSpellTextColor : invalidSpellTextColor;
            commandText.fontSize = activeFontSize;
        }
    }
}
