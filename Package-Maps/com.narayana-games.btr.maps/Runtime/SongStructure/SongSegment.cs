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

namespace NarayanaGames.BeatTheRhythm.Maps.Structure {

    /// <summary>
    ///     A song segment, either a Section or Phrase.
    /// </summary>
    [Serializable]
    public abstract class SongSegment : IComparable {
        /// <summary>The name of this segment.</summary>
        public string name = "SongSegment";
#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public virtual string Name {
            get { return name; }
            set { name = value; }
        }

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public abstract double StartTime { get; set; }

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public abstract double DurationSeconds { get; set; }

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public abstract double EndTime { get; }

        public abstract void SetStartTimeKeepEndTime(double newStartTime);

        public abstract void SetEndTimeKeepStartTime(double newEndTime);




#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public abstract int StartBar { get; set; }

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public abstract int DurationBars { get; set; }





#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public abstract int BeatsPerBar { get; set; }

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public abstract int BeatUnit { get; set; }


#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public abstract double BPM { get; set; }



        public double TimePerBar { get { return TimePerBeat * BeatsPerBar; } }

        public double TimePerBeat { get { return 60.0 / BPM * 4.0 / BeatUnit; } }


        public abstract void CalculateBPM();

        
        public int CompareTo(object obj) {
            SongSegment other = obj as SongSegment;
            if (other == null) {
                return 0;
            }
            return StartTime.CompareTo(other.StartTime);
        }

        public override string ToString() {
            return string.Format("{0} [{1}-{2}]", Name, StartTime, EndTime);
        }
    }

}
