
namespace NarayanaGames.Common.Audio {
    
    /// <summary>
    ///     Looped time is the time within a song (or also just a single
    ///     track or loop), relative to potential looped sections / phrases.
    ///     In the general case (no looping, no skipped sections), both
    ///     CurrentStartTime and CurrentEndTime as well as NextStartTime and
    ///     NextEndTime are zero and length of the track in seconds.
    ///     With regular looping, Current and Next have the same values.
    ///     With skipping sections (e.g. second 0-12, then 24-36), Current
    ///     has the current section/phrase, and next, obviously, the next.
    ///     This currently uses floats for performance reasons (this struct
    ///     is directly fed into DOTS) but keep in mind that for sample
    ///     precision we absolutely do need double. It might be that we
    ///     eventually have both
    /// </summary>
    [System.Serializable]
    public struct LoopedTime {
        public double Time;
        
        public double CurrentStartTime;
        public double CurrentEndTime;
        public double CurrentDuration;
        
        public double NextStartTime;
        public double NextEndTime;
        public double NextDuration;
    }

}
