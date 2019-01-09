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
    ///     Represents a musical section, as explained in Wikipedia: 
    ///     A section is a complete, but not independent, musical idea. Types 
    ///     of sections include the introduction or intro, exposition, 
    ///     development, recapitulation, verse, chorus or refrain, conclusion, 
    ///     coda or outro, fadeout, bridge or interlude. See also:
    ///     https://en.wikipedia.org/wiki/Section_(music)
    ///     
    ///     Of course, drop and buildup are also important sections.
    /// </summary>
    [Serializable]
    public class Section {
        public enum Type : int {
            Intro,
            Verse,
            Chorus,
            Break,
            Bridge,
            BuildUp,
            Drop,
            FastDrop,
            Outro
        }

        /// <summary>The type of this section.</summary>
        public Type type;

        /// <summary>The name of this section, usually either the same as type, or type plus a number.</summary>
        public string name;

        /// <summary>The precise start time of this section.</summary>
        public double startTime = 0;
        
        /// <summary>The precise duration of this section.</summary>
        public double duration = 0;

        /// <summary>The number of the first bar of this section, in the song, starting at 1.</summary>
        public int firstBar = 0;
        
        /// <summary>The number of bars this section has, usually 4 or 8, but 16 or even 32 is also possible.</summary>
        public int barsPerSection = 0;

        /// <summary>The numerator of the meter signature (N in N/4).</summary>
        public int beatsPerBar = 4;

        /// <summary>The denominator of the meter signature (N in 4/N).</summary>
        public int beatUnit = 4;

        /// <summary>Tempo of this section in BPM.</summary>
        public float bpm = 120;

    }

}
