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
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     Represents a musical phrase or period, often a full section and in 
    ///     some cases also just a one bar part with a specific tempo or meter
    ///     signature. We have phrases as an optional way to subdivide sections,
    ///     so usually, sections and phrases are the same. Only when necessary,
    ///     e.g. because there are tempo changes within a section (e.g. a 
    ///     build-up that speeds up), it is recommended to subdivide a section 
    ///     into multiple phrases. Another reason to create phrases is when 
    ///     there is one particularly difficult phrase within a longer section).
    ///     
    ///     As explained in Wikipedia:
    /// 
    ///     In music theory, a phrase (Greek: φράση) is a unit of musical meter 
    ///     that has a complete musical sense of its own, built from figures, 
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
    public class Phrase : SongSegment {
        public Phrase() {
            name = "Phrase";
        }

        public void CalculateBarsFromBPMandTimes(int barInSong) {
            startBar = barInSong;
            durationBars = Mathf.RoundToInt((float) (durationSeconds / TimePerBar));
        }
    }

}
