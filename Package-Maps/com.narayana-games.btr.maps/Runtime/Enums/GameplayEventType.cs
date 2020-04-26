namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable]
    public enum GameplayEventType {
        /// <summary>
        ///     *Beat*, Note, also Tatum => notes that can be caught, punched, sliced, shot => one "instant".
        ///     These have a location but no duration!
        /// </summary>
        Orb = 0, 
        
        /// <summary>
        ///     A combined series of beats. Similar to Follow (Arc) in that it
        ///     defines a path but instead of defining a continous movement,
        ///     BeatSequence defines a stream of beats. osu! streams should
        ///     probably become BeatSequences. Audica has a really well-done
        ///     implementation of these. Has a location, duration and path,
        ///     but path does not use continuous time but instead, number
        ///     of 32ths "distance" to previous element.
        ///     TODO: Not supported, yet
        /// </summary>
        BeatSequence = 4, // must keep old numbers to not mess up stored files

        /// <summary>
        ///     Several beats that can be caught, punched, sliced, shot in a
        ///     random order. This could be a bar, or two bars, and have
        ///     locations based on melody for a bonus if the order fits the
        ///     melody. This could also be used for a Pistol Whip style
        ///     mechanic where each gameplay event could be assigned to
        ///     multiple rhythmic events with different scores for each
        ///     (so that there is an ideal sequence but other sequences
        ///     also still work)
        ///     TODO: Not supported, yet
        /// </summary>
        Random = 2,
        
        /// <summary>
        ///     A generic held note. It has a duration but no additional coordinates.
        /// </summary>
        Hold = 5, 
        
        /// <summary>
        ///     *Follow*. A 2D or 3D path that has to be followed continuously.
        ///     It has a location (where the path begins), duration, and path
        ///     (number of times between 0 and 1 and corresponding locations).
        ///     Aka Slider, Tracer, Path, naming things is difficult ;-)
        /// </summary>
        Arc = 1,      
        
        /// <summary>
        ///     *Spin*. A special kind of held note where players need to "spin the wheel".
        ///     Added to have full osu! support.
        /// </summary>
        Spinner = 3,
        
        /// <summary>
        ///     Mines, Bombs - beats to be avoided. These could be placed for offbeats, or
        ///     to force the player into a specific position. Rhythmically, these could be
        ///     placed as "off-beats", in other words, where the beat is not or should not
        ///     be, but in most cases, that's probably too much education and too little
        ///     entertainment (read: it probably isn't fun)!
        ///     Avoid notes trigger with hand weapons and head collider.
        /// </summary>
        Avoid = -1,

        /// <summary>
        ///     An obstacle that must be avoided, either by crouching, or by stepping to the side.
        ///     Theoretically, we could even have "jump obstacles" ... but for now ... not.
        ///     Obstacles do not trigger with hand weapons, only with the head collider.
        ///     Obstacles usually have a duration.
        /// </summary>
        Obstacle = -2,
        
        /// <summary>
        ///     Like obstacle, but triggers with all colliders (head, hands, feet, if available).
        ///     These can be used to force players into specific positions.
        /// </summary>
        ObstacleStrict = -3
        
    }
    
}
