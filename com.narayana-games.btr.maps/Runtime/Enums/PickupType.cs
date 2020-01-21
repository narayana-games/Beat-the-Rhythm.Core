namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum PickupType {
        Any = 0, // catch it however you like; also used when not caught
        Left = 1, // hand, obviously ;-)
        Right = 2, // hand, obviously ;-)
        Head = 3,
        LeftFoot = 4, // when you have Vive trackers ...
        RightFoot = 5 // when you have Vive trackers ...
    }
    
}
