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

        /// <summary>Globally unique ID that gameplay and other tracks can refer to.</summary>
        public string timingSequenceId = null;
        
        /// <summary>The name of this sequence, usually the name of the phrase.</summary>
        public string name;
        
        /// <summary>The rhythmic difficulty of this rhythm sequence.</summary>
        public DifficultyPreset difficulty = DifficultyPreset.Casual;

        /// <summary>The rhythmic style of this sequence.</summary>
        public RhythmStyle rhythmStyle = RhythmStyle.Mixed;

        /// <summary>The instrument type of this sequence.</summary>
        public InstrumentType instrumentType = InstrumentType.Mixed;
        
        /// <summary>The numerator of the meter signature (N in N/4).</summary>
        public int beatsPerBar = 4;

        /// <summary>The denominator of the meter signature (N in 4/N).</summary>
        public int beatUnit = 4;
        
        /// <summary>The number of bars that this sequence has.</summary>
        public int durationBars = 0;
        
        /// <summary>Original tempo of this sequence in BPM.</summary>
        public double bpm = 120;

        /// <summary>Original duration of this sequence.</summary>
        public double durationSeconds = 0;
        
        /// <summary>The timing events comprising this sequence.</summary>
        public List<TimingEvent> events = new List<TimingEvent>();
        
        /// <summary>
        ///     Links to additional patterns. Usually, it's best to design
        ///     gameplay for left and right hand (and feet/head, if applicable)
        ///     together in one stream. But sometimes, you may want to have
        ///     the dominant hand have a different rhythm from the right hand,
        ///     and in those cases, it's best to have one sequence for each
        ///     hand that only has the events of that rhythmic pattern.
        /// </summary>
        public List<string> multiTrackSequenceIds = new List<string>();
        
    }

}
