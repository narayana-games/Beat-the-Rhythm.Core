namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum RhythmStyle : int {
        Precise = -1, // precisely follows on specific instrument
        Mixed = 0,    // parts follow precisely, some parts are creative
        Creative = 1, // adds additional rhythm to the song (like playing a bongo)
    }
    
    // NOTE: If "Precise" or "Mixed" is selected, an instrument type should also be set 
}
