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
using System.Runtime.Serialization;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Maps.Structure {

    /// <summary>
    ///     Represents a musical phrase or period, often a full section and in 
    ///     some cases also just a one bar part with a specific tempo or meter
    ///     signature. We have phrases as an optional way to subdivide sections,
    ///     so usually, sections and phrases are the same. Only when necessary,
    ///     e.g. because there are tempo changes within a section (e.g. a 
    ///     build-up that speeds up), it is recommended to subdivide a section 
    ///     into multiple phrases. Another reason to create phrases is when 
    ///     there is one particularly difficult phrase within a longer section,
    ///     or when the weapon should be changed within a section.
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

        /// <summary>
        ///     A local id for this phrase within the audio recording.
        ///     This is used by TimingSequence, GameplayPattern and
        ///     others to refer to a specific phrase in the song. We
        ///     use int instead of string/Guid for simplicity here,
        ///     and because phrases can not be reused across different
        ///     songs. Also, this id is the index e.g. for
        ///     TimingTrack.phraseToSequenceId.
        /// </summary>
        public int phraseId = 0;
        
        /// <summary>The precise start time of this phrase.</summary>
        public double startTime = 0;

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override double StartTime {
            get { return startTime; }
            set { startTime = value; }
        }

        public override void SetStartTimeKeepEndTime(double newStartTime) {
            double endTime = EndTime;
            startTime = newStartTime;
            durationSeconds = endTime - newStartTime;
        }

        public override void SetEndTimeKeepStartTime(double newEndTime) {
            durationSeconds = newEndTime - StartTime;
        }

        /// <summary>The precise duration of this segment.</summary>
        public double durationSeconds = 0;

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override double DurationSeconds {
            get { return durationSeconds; }
            set { durationSeconds = value; }
        }

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override double EndTime {
            get { return StartTime + DurationSeconds; }
        }

        /// <summary>
        ///     The number of the first bar of this phrase, in the song,
        ///     starting at 0 (-1 means unassigned).
        /// </summary>
        public int startBar = -1;

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override int StartBar {
            get { return startBar; }
            set { startBar = value; }
        }


        /// <summary>
        ///     The number of bars this segment has. 
        ///     For sections: Usually 4 or 8, but 12, 16 or even 32 is also possible.
        ///     For phrases: Usually 4 or 8, but 1, 12, 16 or more is also possible.
        /// </summary>
        public int durationBars = 0;

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override int DurationBars {
            get { return durationBars; }
            set { durationBars = value; }
        }


        /// <summary>The numerator of the meter signature (N in N/4).</summary>
        public int beatsPerBar = 4;

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override int BeatsPerBar {
            get { return beatsPerBar; }
            set { beatsPerBar = value; }
        }

        /// <summary>The denominator of the meter signature (N in 4/N).</summary>
        public int beatUnit = 4;

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override int BeatUnit {
            get { return beatUnit; }
            set { beatUnit = value; }
        }

        /// <summary>Tempo of this phrase in BPM.</summary>
        public double bpm = 120;

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override double BPM {
            get { return bpm; }
            set { bpm = value; }
        }


        public override void CalculateBPM() {
            CalculateBPM(DurationSeconds, DurationBars);
        }

        public void CalculateBPM(double seconds, double bars) {
            double timePerBeat = seconds / (bars * beatsPerBar * 4.0 / beatUnit);
            bpm = (60.0 / timePerBeat);
        }

        public void CalculateBarsFromBPMandDuration(int barInSong) {
            startBar = barInSong;
            DurationBars = Mathf.RoundToInt((float) (durationSeconds / TimePerBar));
        }

        public void CalculateSecondsFromBarsAndBPM() {
            durationSeconds = durationBars * TimePerBar;
        }

    }

}
