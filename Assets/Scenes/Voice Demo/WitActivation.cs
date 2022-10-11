using Oculus.Voice;
using UnityEngine;

public class WitActivation : MonoBehaviour
{
    [SerializeField]
    private AppVoiceExperience _voiceExperience;
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
        _voiceExperience.Activate();
    }

    /// <summary>
    /// Deactivates Wit i.e stops listening to the user.
    /// </summary>
    public void DeactivateWit()
    {
        _voiceExperience.Deactivate();
    }
}