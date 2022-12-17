using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace SpellBind
{
    public class MultiSceneManager : MonoBehaviour
    {
        [Header("SCENE NAMES")]
        [SerializeField] private string mainScene;
        [SerializeField] private string levelScene;

        [Header("MAIN SCENE REFERENCES")]
        [SerializeField] private GameObject titleCanvas;
        [SerializeField] private GameObject particleEffect;

        [Header("POST PROCESSING REFERENCES")]
        [SerializeField] private Volume ppv;
        [SerializeField] private float postExposureDefault;
        [SerializeField] private float postExposureFadeOut;

        [Header("PLAYER COLLIDER")]
        [SerializeField] private CapsuleCollider playerCollider;
        [SerializeField] private float playerColliderOffset;

        private ColorAdjustments colorAdjustments;

        private static MultiSceneManager _instance;
        public static MultiSceneManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
            DontDestroyOnLoad(gameObject);

            //Apply offset to player collider
            float _yOffset = playerColliderOffset;
            Vector3 _colliderPos = playerCollider.transform.position;
#if UNITY_EDITOR
            //_yOffset = playerColliderOffset * -1;
#endif
            playerCollider.transform.position = new Vector3(_colliderPos.x, _yOffset, _colliderPos.z);
        }

        // Start is called before the first frame update
        void Start()
        {
            //Populate PPV reference
            ppv.profile.TryGet(out colorAdjustments);

            //Load the level scene
            LoadScene(levelScene, 6f);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Loads the given scene in an additive manner
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="_activateTime"></param>
        private void LoadScene(string sceneName, float _activateTime = 0)
        {
            StartCoroutine(LoadSceneAsync(sceneName, _activateTime));
        }

        /// <summary>
        /// Loads the given scene in an additive as well as async manner
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="_activateTime"></param>
        /// <returns></returns>
        private IEnumerator LoadSceneAsync(string sceneName, float _activateTime)
        {
            //Load the scene, but don't activate it yet
            AsyncOperation _load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _load.allowSceneActivation = false;

            //Wait for the activation delay
            yield return new WaitForSeconds(_activateTime);

            //Fade out scene
            float _elapsedTime = 0;
            float _fadeOutTime = 1f;
            float _postExposureValue = postExposureDefault;
            while (_elapsedTime < _fadeOutTime)
            {
                _postExposureValue = Mathf.Lerp(_postExposureValue, postExposureFadeOut, (_elapsedTime / _fadeOutTime));
                colorAdjustments.postExposure.Override(_postExposureValue);
                //Debug.LogFormat("<color=cyan>Fading out {0}</color>", _postExposureValue);
                _elapsedTime += Time.deltaTime;
                yield return null;
            }
            colorAdjustments.postExposure.Override(postExposureFadeOut);

            //Activate scene
            _load.allowSceneActivation = true;

            yield return new WaitForSeconds(0.5f);

            //Disable the unnecessary items before scene activation
            titleCanvas.SetActive(false);
            particleEffect.SetActive(false);

            //Fade in scene
            float _elapsedTimeFI = 0;
            float _fadeInTime = 10f;
            _postExposureValue = postExposureFadeOut;
            while (_elapsedTimeFI < _fadeInTime)
            {
                _postExposureValue = Mathf.Lerp(_postExposureValue, postExposureDefault, (_elapsedTimeFI / _fadeInTime));
                colorAdjustments.postExposure.Override(_postExposureValue);
                //Debug.LogFormat("<color=orange>Fading in {0}</color>", _postExposureValue);
                _elapsedTimeFI += Time.deltaTime;
                yield return null;
            }
            colorAdjustments.postExposure.Override(postExposureDefault);
        }
    }
}
