using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellBind
{
    public static class UIMessageDictionary
    {
        #region TUTORIAL MESSAGES
        public static Dictionary<string, float> tutorialTextIntro = new Dictionary<string, float>()
        {
            { "Welcome!", 1f },
            { "This is a Defence Against the Dark Arts class", 3f },
            { "Let's begin our practice", 2f },
            { "Start with picking up your wand", 2f },
        };

        public static Dictionary<string, float> tutorialOnWandPick = new Dictionary<string, float>()
        {
            { "Marvelous!", 1f },
            { "Let's practice some spells!", 1.5f },
            { "Point your wand at the incoming dementor", 2f },
            { "While the wand is pointed, shout \n\"Attack\"", 1.5f },
        };

        public static Dictionary<string, float> tutorialOnFirstAttack = new Dictionary<string, float>()
        {
            { "Amazing!" , 1f },
            { "Let's do that again and destroy the dementor", 2f },
        };

        public static Dictionary<string, float> tutorialCapture = new Dictionary<string, float>()
        {
            { "Well Done!" , 1f },
            { "Now you'll encounter a new type of dementor", 2f },
            { "While the wand is pointed, shout \n\"Capture\"", 1.5f },
        };

        public static Dictionary<string, float> tutorialSmash = new Dictionary<string, float>()
        {
            { "Good Work!" , 1f },
            { "Now shout \"Smash\" to smash the dementor against the ground", 2f },
        };

        public static Dictionary<string, float> tutorialFly = new Dictionary<string, float>()
        {
            { "You're doing a great job!" , 1.5f },
            { "Now you'll encounter a final type of dementor!" , 2f },
            { "Here, make use of a spell bomb to destroy it" , 2f },
            { "Point your wand at the spell bomb and shout \n\"Fly\"", 2f },
        };

        public static Dictionary<string, float> tutorialThrow = new Dictionary<string, float>()
        {
            { "Awesome!" , 1f },
            { "Now point towards the dementor and shout \n\"Throw\"", 2f },
            { "Let's do that again and destroy the dementor", 2f },
        };
        #endregion
    }
}
