#region Copyright and License Information
/*
 * Copyright (c) 2015-2020 narayana games UG.  All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 *
 * See LICENSE and NOTICE in the project root for license information.
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion Copyright and License Information

using System;
using System.Collections.Generic;
using NarayanaGames.BeatTheRhythm.Maps.Enums;

namespace NarayanaGames.BeatTheRhythm.Maps.Tracks {

    /// <summary>
    ///     Represents a rhythmic sequence, as explained in Wikipedia:
    ///     In music, a sequence is the restatement of a motif or longer melodic
    ///     (or harmonic) passage at a higher or lower pitch in the same voice.
    ///     See also: https://en.wikipedia.org/wiki/Sequence_(music)
    ///
    ///     Obviously, in our context, it's really a section of actual gameplay.
    /// </summary>
    [Serializable]
    public class TimingSequence { 

        /// <summary>Links this sequence to its phrase in the song structure.</summary>
        public int phraseId = 0;
        
        /// <summary>The name of this sequence, usually the name of the phrase.</summary>
        public string name;
        
        /// <summary>
        ///     Dominant hand that this sequence was designed for; locations
        ///     will be mirrored when different from player
        /// </summary>
        public PickupType dominantHand = PickupType.Right;

        /// <summary>
        ///     Mechanic on dominant hand that the rhythm for this sequence was recorded with.
        /// </summary>
        public Mechanic mechanicDominant = Mechanic.PunchKickFlying;
        /// <summary>
        ///     Mechanic on non-dominant hand that the rhythm for this sequence was recorded with.
        /// </summary>
        public Mechanic mechanicNonDominant = Mechanic.PunchKickFlying;
        
        /// <summary>The maximum difficulty that could be achieved with this rhythm sequence.</summary>
        public DifficultyPreset difficulty = DifficultyPreset.Casual;

        /// <summary>The rhythmic style of the whole track.</summary>
        public RhythmStyle rhythmStyle = RhythmStyle.Mixed;

        /// <summary>The instrument type of this track.</summary>
        public InstrumentType instrumentType = InstrumentType.Mixed;
        
        /// <summary>
        ///     Original tempo of this sequence in BPM. If different from the
        ///     sequence tempo, the timing of all events must be multiplied
        ///     accordingly.
        /// </summary>
        public float bpm = 120;

        /// <summary>The events comprising this sequence.</summary>
        public List<TimingEvent> events = new List<TimingEvent>();
    }

}
