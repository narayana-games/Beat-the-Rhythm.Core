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
using System.Runtime.Serialization;

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     A song segment, either a Section or Phrase.
    /// </summary>
    [Serializable]
    public abstract class SongSegment : IComparable {
        /// <summary>The name of this segment.</summary>
        public string name = "Phrase";

        /// <summary>The precise start time of this segment.</summary>
        public double startTime = 0;

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public virtual double StartTime {
            get { return startTime; }
            set { startTime = value; }
        }


        public void SetStartTimeKeepEndTime(double newStartTime) {
            double endTime = EndTime;
            StartTime = newStartTime;
            DurationSeconds = endTime - newStartTime;
        }

        public void SetEndTimeKeepStartTime(double newEndTime) {
            DurationSeconds = newEndTime - StartTime;
        }

        /// <summary>The precise duration of this segment.</summary>
        public double durationSeconds = 0;

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public virtual double DurationSeconds {
            get { return durationSeconds; }
            set { durationSeconds = value; }
        }

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public virtual double EndTime {
            get { return StartTime + DurationSeconds; }
        }


        /// <summary>The number of the first bar of this segment, in the song, starting at 1.</summary>
        public int startBar = 0;

        /// <summary>
        ///     The number of bars this segment has. 
        ///     For sections: Usually 4 or 8, but 12, 16 or even 32 is also possible.
        ///     For phrases: Usually 4 or 8, but 1, 12, 16 or more is also possible.
        /// </summary>
        public int durationBars = 0;

        /// <summary>The numerator of the meter signature (N in N/4).</summary>
        public int beatsPerBar = 4;

        /// <summary>The denominator of the meter signature (N in 4/N).</summary>
        public int beatUnit = 4;

        /// <summary>Tempo of this phrase in BPM.</summary>
        public double bpm = 120;

        public void CalculateBPM() {
            CalculateBPM(DurationSeconds, durationBars);
        }

        public void CalculateBPM(double seconds, double bars) {
            double timePerBeat = seconds / (bars * beatsPerBar * 4.0 / beatUnit);
            bpm = (60.0 / timePerBeat);
        }

        public double TimePerBar {
            get {
                return TimePerBeat * beatsPerBar;
            }
        }

        public double TimePerBeat {
            get {
                return 60.0 / bpm * 4.0 / beatUnit;
            }
        }

        //public int CalculateBarCountFromBPM() {

        //}

        public int CompareTo(object obj) {
            SongSegment other = obj as SongSegment;
            if (other == null) {
                return 0;
            }
            return StartTime.CompareTo(other.StartTime);
        }

        public override string ToString() {
            return string.Format("{0} [{1:0}-{2:0}]", name, StartTime, EndTime);
        }
    }

}
