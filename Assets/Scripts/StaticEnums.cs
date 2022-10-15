using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    public enum InteractableType
    {
        Enemy,
        SpellBomb
    }
    public enum EnemyType
    {
        Attacker,
        Dodger,
        Buffed
    }

    public enum EnemyState
    {
        Attacking,
        Dodging,
        Captured,
        Spellbombed,
        Dead
    }

    public enum SpellBombType
    {
        SingleShot,
        MultiShot
    }

    public enum SpellBombState
    {
        Idle,
        Highlighted,
        Levitated,
        Thrown,
        Dropped,
        Exploded
    }

    public enum Spells
    {
        None, 
        Fly,
        Drop,
        Throw,
        Capture,
        Smash,
        Attack
    }
}


