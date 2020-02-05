namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum GameModeExt {
        Ignore = -1, // for maps that don't make sense, like BSMG Lawless or Lightshow
        
        // from osu!
        osu = 0,
        Taiko = 1,
        CatchTheBeat = 2,
        Mania = 3,
        
        // from BSMG
        Standard = 10,
        NoArrows = 11,
        OneSaber = 12,
        Degrees90 = 13,
        Degrees360 = 14
    }

}
