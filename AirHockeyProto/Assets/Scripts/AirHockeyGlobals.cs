using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AirHockeyGlobals
{
    public class TableSettings 
    {
        public static float wallDistanceThreshold = 0.5f;
        public static float wallPushStrength = 1f;
    }

    public class StrikerSettings
    {
        public static float interpolationSpeed = 0.3f;
        public static float goalieModeLiftAmt = 0.1f;
    }
    public class PlayerStrikerSettings : StrikerSettings
    {
        public static float inputModifierSpeed = 3f;       
    }
    public class AIStrikerSettings : StrikerSettings 
    {
        public static new float interpolationSpeed = 0.1f;
        public static float baseAttackDistance = 0.1f;
    }

    public class PuckSettings 
    {
        public static float maxSpeed = 4f;
    }

    public class Hacks 
    {
        public static float CharacterIKBlendDelay = 0.5f;
    }
}
