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
        public ObjectPooler enemyExplosionPool;
        public ObjectPooler forceFieldSparkPool;

        [Header("SPELL BOMBS")]
        public ObjectPooler spellBombSingleShotObjPool;
        public ObjectPooler spellBombMultiShotObjPool;

        [Header("ENEMIES")]
        public ObjectPooler attackerEnemyObjPool;
        public ObjectPooler dodgerEnemyObjPool;
        public ObjectPooler buffedEnemyObjPool;
    }
}
