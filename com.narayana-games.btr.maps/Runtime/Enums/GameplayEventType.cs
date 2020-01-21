namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum GameplayEventType {
        Orb = 0,
        Arc = 1, // aka Slider, Tracer, Path, naming things is difficult ;-)
        Random = 2, // this mechanic is currently not supported
        Spinner = 3 // added to have full osu! support
    }
    
}
