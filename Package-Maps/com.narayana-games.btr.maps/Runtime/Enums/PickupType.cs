namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum PickupType {
        None = -1,    // AVOID, bombs or the like that players must not catch
        Any = 0,      // catch it however you like; also used when not caught
        Left = 1,     // hand or foot
        Right = 2,    // hand or foot
        Head = 3,
        LeftFoot = 4, // when you have Vive trackers ...
        RightFoot = 5 // when you have Vive trackers ...
    }
    
}
