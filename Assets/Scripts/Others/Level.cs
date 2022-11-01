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
        public int enemyCount;
        public float enemySpawnRate;
        public List<EnemyTypeSpawnRate> enemyTypes;
        public List<SpellBombType> spellBombTypes;
        public float spellBombSpawnRate;
    }

    [Serializable]
    public class EnemyTypeSpawnRate
    {
        public EnemyType enemyType;
        public float spawnRate;
    }

    [Serializable]
    public class SpellBombTypeSpawnRate
    {
        public SpellBombType spellBombType;
        public float spawnRate;
    }
}
