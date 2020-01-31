namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum EnvironmentDifficulty : int {
        RadialSource = -1,            // override for SingleSourceStatic
        SingleSourceStatic = 0,       // One Orb Source that does not change location
        SingleSourceMoving = 1,       // single source but it's moving (e.g. dragons)
        MultipleSources180Static = 2, // several static sources but only 180° (in front of player)
        MultipleSources180Moving = 3, // several moving sources but only 180° (in front of player)
        MultipleSources360Static = 4, // several static sources, full 360°
        MultipleSources360Moving = 5, // several moving sources, full 360°
    }

}
