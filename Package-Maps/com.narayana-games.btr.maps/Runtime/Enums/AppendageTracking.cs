﻿namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum AppendageTracking : int {
        Unknown = -1,         // We don't have the data, yet
        SingleHand = 0,       // SH
        TwoHands = 1,         // TH
        HeadAndHands = 2,     // HAH
        HeadHandsAndFeet = 3, // HHAF
        HandsAndFeet = 4,     // HAF
        TwoFeet = 5,          // TF
        SingleFoot = 6,       // SF
        HeadOnly = 7          // HO
    }
    
}
