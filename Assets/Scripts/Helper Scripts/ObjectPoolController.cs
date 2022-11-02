using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    public class ObjectPoolController : MonoBehaviour
    {
        [Header("PARTICLE EFFECTS")]
        public ObjectPooler playerFireballPool;
        public ObjectPooler enemySparkPool;

        [Header("SPELL BOMBS")]
        public ObjectPooler spellBombSingleShotObjPool;
        public ObjectPooler spellBombMultiShotObjPool;
    }
}