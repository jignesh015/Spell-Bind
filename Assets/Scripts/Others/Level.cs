using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    [Serializable]
    public class Level
    {
        public int levelNo;
        public float minEnemySpawnGap;
        public float maxEnemySpawnGap;
        public int maxSpawnedEnemiesAtTime;
        public List<EnemyTypeSpawnRate> enemyTypes;
        public List<SpellBombTypeSpawnRate> spellBombTypes;
    }

    [Serializable]
    public class EnemyTypeSpawnRate
    {
        public EnemyType enemyType;
        public int spawnCount;
    }

    [Serializable]
    public class SpellBombTypeSpawnRate
    {
        public SpellBombType spellBombType;
        public float spawnRate;
    }
}
