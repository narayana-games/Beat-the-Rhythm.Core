namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum TrackRole : int {
        Head = -4,        // separate, optional channel, for head orbs (probably has no use case)
        Feet = -3,   // separate channel for feet (splitting left/right has no use case)
        NonDominantHand = -2, // Only non-dominant hand, when rhythm for left / right hand are independent
        DominantHand = -1,    // Only dominant hand, when rhythm for left / right hand are independent
        // NOTE: As we already have
        SinglePlayer = 0, // for single player or competitive multiplayer (all play the same)
        Player1 = 1,      // First player in multiplayer
        Player2 = 2,      // Second player in multiplayer
        Player3 = 3,      // Third player in multiplayer
        Player4 = 4       // Fourth player in multiplayer
        // currently we have a max of 4 players
    }
}
