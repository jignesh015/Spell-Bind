using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    public class AnimatorEvents : MonoBehaviour
    {
        public void PlayUISFX(AudioClip _clip)
        {
            FindObjectOfType<UIController>().PlayUISFX(_clip);
        }
    }
}
