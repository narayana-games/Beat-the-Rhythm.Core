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
using System.Runtime.Serialization;
using NarayanaGames.BeatTheRhythm.Maps.Enums;
using NarayanaGames.BeatTheRhythm.Maps.Structure;
using UnityEngine;

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

        /// <summary>
        ///     Number of times 4ths are divided for quantization.
        ///     0 means no quantization; in that case, use TimingEvent.startTime/duration as is.
        ///     1 means quantization on 4ths, 2 on 8ths, 4 on 16ths, 8 on 32ths.
        ///     3 means quantization on 4th-triplets (three notes during one fourth),
        ///     6 is on 8th-triplets (three per 8th, or 6 per fourth).
        ///     In this case, use TimingEvent.startNote/duration32ths.
        /// </summary>
        public int dividerCount = 2;


#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember] 
        public bool alwaysMaxRes = false;
        
        /// <summary>When rendering a grid / step sequencer view, we want max res when quantizing</summary>
#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public int DividerCountView => DontQuantize || alwaysMaxRes ? DividerCountMax : dividerCount;

        public const int DividerCountMax = 8;
        
        /// <summary>Convenience Check for when no quantization shall be applied</summary>
#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public bool DontQuantize => dividerCount == 0;
        
        /// <summary>The number of bars that this sequence has.</summary>
        public int durationBars = 0;

        /// <summary>Original tempo of this sequence in BPM.</summary>
        public double bpm = 120;

        /// <summary>Original duration of this sequence.</summary>
        public double durationSeconds = 0;

        /// <summary>The timing events comprising this sequence.</summary>
        public List<TimingEvent> events = new List<TimingEvent>();

        public void Sort() {
            events.Sort((a, b) => a.startTime.CompareTo(b.startTime));
        }
        
        public void Delete(TimingEvent evt) {
            events.Remove(evt);
        }
        
        public int MaxEventID {
            get {
                int maxEventId = 0;
                for (int i = 0; i < events.Count; i++) {
                    maxEventId = Mathf.Max(maxEventId, events[i].eventId);
                }

                return maxEventId;
            }
        }
        
        public TimingEvent FindTimingEvent(int timingEventId) {
            for (int i = 0; i < events.Count; i++) {
                if (events[i].eventId == timingEventId) {
                    return events[i];
                }
            }
            throw new ArgumentException($"Could not find timingEventId {timingEventId} in {timingSequenceId}");
        }

        public TimingEvent FindTimingEvent(Phrase phrase, double time, int duration32ths) {
            TimingEvent helper = new TimingEvent();
            helper.startTime = time;
            helper.ConvertToBeatBased(phrase);
            helper.ConvertToTripletBased(phrase);

            //Debug.Log($"New Event: {helper.startTime} | {helper.startNote} | {helper.startTriplet} - {events.Count} timing events in sequence");

            // If not quantizing => combine if it's less than 1/128th
            double timePer128th = phrase.TimePer32th * 0.25;
            int nextDividerCount = GetNextDividerCount(dividerCount);
            
            for (int i = 0; i < events.Count; i++) {
                // if the duration doesn't match => forget it right away!
                if (events[i].duration32ths == duration32ths) {
                    if (DontQuantize || nextDividerCount == 0) {
                        if (Math.Abs(events[i].startTime - time) < timePer128th) {
                            Debug.Log($"Matched: {events[i].startTime} == {time}, with 128th-tolerance: {timePer128th}");
                            return events[i];
                        }
                    } else {
                        int quantizedA = events[i].QuantizedStartNote(phrase, nextDividerCount);
                        int quantizedB = helper.QuantizedStartNote(phrase, nextDividerCount); 
                        if (quantizedA == quantizedB) {
                            Debug.Log($"Matched: ({events[i].startTime}) {quantizedA} == {quantizedA} ({time}) | ({events[i].startNote} == {helper.startNote} | {events[i].startTriplet} == {helper.startTriplet})");
                            return events[i];
                        }
                    }
                }
            }
            return null;
        }

        private int GetNextDividerCount(int dc) {
            if (dc == 0 || dc == 8 || dc == 6) {
                // if no quantization, or max quantization => no quantization
                return 0;
            }

            if (dc == 1 || dc == 2 || dc == 4 || dc == 3) {
                return dc * 2;
            }
            Debug.LogError($"Unknown divider count: {dc}, using no quantization");
            return 0;
        }

        /// <summary>
        ///     Links to additional patterns. Usually, it's best to design
        ///     gameplay for left and right hand (and feet/head, if applicable)
        ///     together in one stream. But sometimes, you may want to have
        ///     the dominant hand have a different rhythm from the other hand,
        ///     and in those cases, it's best to have one sequence for each
        ///     hand that only has the events of that rhythmic pattern (same
        ///     applies, of course, to feet and head).
        /// </summary>
        public List<string> multiTrackSequenceIds = new List<string>();

    }

}
