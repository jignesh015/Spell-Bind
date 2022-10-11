using Oculus.Voice;
using UnityEngine;

public class WitActivation : MonoBehaviour
{
    [SerializeField]
    private AppVoiceExperience _voiceExperience;

    private bool isActivated;
    private void OnValidate()
    {
        if (!_voiceExperience) _voiceExperience = GetComponent<AppVoiceExperience>();
    }

    private void Start()
    {
        _voiceExperience = GetComponent<AppVoiceExperience>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Debug.Log("*** Pressed Space bar ***");
        //    ActivateWit();
        //}
    }

    /// <summary>
    /// Activates Wit i.e. start listening to the user.
    /// </summary>
    public void ActivateWit()
    {
        isActivated = true;
        _voiceExperience.Activate();
        Debug.Log("Activate Wit");
    }

    /// <summary>
    /// Deactivates Wit i.e stops listening to the user.
    /// </summary>
    public void DeactivateWit()
    {
        isActivated = false;
        _voiceExperience.Deactivate();
    }

    public bool IsActive()
    {
        return isActivated || _voiceExperience.MicActive;
    }
}