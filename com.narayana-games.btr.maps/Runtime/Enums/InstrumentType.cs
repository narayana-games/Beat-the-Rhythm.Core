namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum InstrumentType : int {
        Irrelevant = -1, // default for creative!
        Mixed = 0,       // primarily for 
        Percussion = 1,  // this could also be used for Creative!
        Drums = 2,       // just kickdrum and snare; for hihats, cymbals and so forth, use percussion instead
        Bassline = 3,    // basslines often carry interesting rhythms, so that's also an option
        Vocals = 4       // sometimes, even the vocal lines are interesting
    }
}
