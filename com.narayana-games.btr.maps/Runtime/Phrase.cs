#region Copyright and License Information
/*
 * Copyright (c) 2015-2019 narayana games UG.  All Rights Reserved.
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

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     Represents a musical phrase or period, often a full section and in 
    ///     some cases also just a one bar part with a specific tempo.
    ///     
    ///     As explained in Wikipedia:
    /// 
    ///     In music theory, a phrase (Greek: φράση) is a unit of musical meter 
    ///     that has a complete musical sense of its own,[5] built from figures, 
    ///     motifs, and cells, and combining to form melodies, periods and larger 
    ///     sections.
    /// 
    ///     See also: https://en.wikipedia.org/wiki/Phrase_(music)
    /// 
    ///     In Western art music or Classical music, a period is a group of phrases 
    ///     consisting usually of at least one antecedent phrase and one consequent 
    ///     phrase totaling about 8 bars in length (though this varies depending on 
    ///     meter and tempo).
    /// 
    ///     See also: https://en.wikipedia.org/wiki/Period_(music)
    /// </summary>
    [Serializable]
    public class Phrase {
        /// <summary>The name of this section, usually either the same as type, or type plus a number.</summary>
        public string name;

        /// <summary>The precise start time of this phrase.</summary>
        public double startTime = 0;

        /// <summary>The precise duration of this phrase.</summary>
        public double duration = 0;

        /// <summary>The number of the first bar of this phrase, in the song, starting at 1.</summary>
        public int firstBar = 0;

        /// <summary>The number of bars this phrase has, usually 4 or 8, but 1, 12, 16 or more is also possible.</summary>
        public int barsPerSection = 0;

        /// <summary>The numerator of the meter signature (N in N/4).</summary>
        public int beatsPerBar = 4;

        /// <summary>The denominator of the meter signature (N in 4/N).</summary>
        public int beatUnit = 4;

        /// <summary>Tempo of this phrase in BPM.</summary>
        public float bpm = 120;

        public void CalculateBPM() {
            double timePerBeat = duration / (barsPerSection * beatsPerBar);
            bpm = (float) (60.0 / timePerBeat);
        }
    }

}
