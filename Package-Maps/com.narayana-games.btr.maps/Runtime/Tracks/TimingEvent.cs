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
using NarayanaGames.BeatTheRhythm.Maps.Structure;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Maps.Tracks {

    /// <summary>
    ///     Represents a rhythmic event within a phrase.
    /// </summary>
    [Serializable]
    public class TimingEvent {

        // startBeat is first entry for better JSON viewing in Rider
        /// <summary>
        ///     The start time expressed in bars, beats, eighths and so forth.
        ///     1 is the 64ths (currently not used), 10 the 32ths, 100 the 16ths,
        ///     1000 the 8ths (unless beats are expressed in 8ths), 10000 the beats,
        ///     100000 the bars. So, e.g. 1742000 would be the last eighth of the
        ///     17th bar (assuming 4/4). Bars, beats, eighths and so forth are
        ///     stored beginning with 1 (not 0), so having no 1 means that the
        ///     level has been quantized away.
        ///     See also region Beat Based.
        /// </summary>
        public int startNote = 0;

        /// <summary>
        ///     Same as startNote - but triplet based. Use this only if
        ///     TimeSequence.dividerCount is set to 3 or 6.
        /// </summary>
        public int startTriplet = 0;
        
        #region Time Based
        /// <summary>
        ///     Start time of this note event, relative to the beginning time
        ///     of the phrase/sequence. Like all times, this is in seconds.
        ///     This is the original time of the event during recording time,
        ///     without quantization applied. In general, this should not be
        ///     used for gameplay.
        /// </summary>
        public double startTime = 0;

        /// <summary>Duration; only used for sustained notes.</summary>
        public double duration = 0;
        #endregion Time Based

        /// <summary>How many sixteenths? Only for sustained notes.</summary>
        public int duration32ths = 0;
        
        /// <summary>
        ///     A unique id for this event within the sequence. The
        ///     other event types (gameplay, melodic, effects) rely
        ///     on these ids, so do not ever change them once
        ///     additional tracks have been created.
        /// </summary>
        public int eventId = 0;
        
        /// <summary>
        ///     List of hands/feet that this event was originally generated with.
        /// </summary>
        public List<Appendage> pickupHint = new List<Appendage>();
        
        #region Beat Based
        // TODO: Handle triplets!!!

        /// <summary>Find out where exactly this event is rhythmically.</summary>
        /// <param name="phrase">Phrase that this event was recorded in</param>
        /// <returns>false, if this actually needs to go to the next phrase</returns>
        public bool ConvertToBeatBased(Phrase phrase) {
            
            // the easy part:
            duration32ths = (int) (duration / phrase.TimePer32th);
            
            /*
             * Step 1: Get minimum bar/beat/8th/16th and so forth
             * Step 2: Check if we're actually closer to the next
             * Step 3: Write the whole thing into a compact format in startBeat
             */

            // if -1 is kept, this will result in 0, meaning: "quantized away"
            int startBarInPhrase = -1; // Which bar in the section?
            int startBeatInBar = -1; // Which beat in the bar?
            int start8thInBeat = -1; // Which eighth in the beat (quarter)?
            int start16thIn8th = -1; // Which sixteenth in the eighth?
            int start32thIn16th = -1; // Which thirty second in the sixteenth?
            
            // Step 1
            double timeLeft = startTime;
            
            startBarInPhrase = (int) (timeLeft / phrase.TimePerBar);
            timeLeft -= ((double)startBarInPhrase) * phrase.TimePerBar;

            startBeatInBar = (int)(timeLeft / phrase.TimePerBeat);
            timeLeft -= ((double)startBeatInBar) * phrase.TimePerBeat;

            if (phrase.beatUnit != 8) {
                start8thInBeat = (int)(timeLeft / phrase.TimePer8th);
                timeLeft -= ((double)start8thInBeat) * phrase.TimePer8th;
            } else {
                Debug.LogWarning("phrase.beatUnit == 8 => might be trouble!!!");
            }
            
            start16thIn8th = (int)(timeLeft / phrase.TimePer16th);
            timeLeft -= ((double)start16thIn8th) * phrase.TimePer16th;

            start32thIn16th = (int)(timeLeft / phrase.TimePer32th);
            timeLeft -= ((double)start32thIn16th) * phrase.TimePer32th;
            
            // Step 2: If timeLeft more than half another 32th => push it all up
            //         This could be more precise by adding the time
            

            // this is kind of rare, but I have seen it: starting almost exactly
            // at the beginning of the first bar of the next phrase
            // this is most likely due to Update() being called a tiny bit
            // late, so the next phrase isn't active, yet, when the collision occurred
            if (startBarInPhrase >= phrase.DurationBars) {
                eventId = 0;
                startTime = 0;
                startNote = 111110;
                return false;
            }
            
            // the more usual case: we're just a little early on the next phrase
            if (start32thIn16th == 1 && timeLeft > phrase.TimePer32th * 0.5F) {
                start32thIn16th = 0; // drop that 32th
                timeLeft += phrase.TimePer32th;
                if (start16thIn8th == 1 && timeLeft > phrase.TimePer16th * 0.5F) {
                    start16thIn8th = 0;
                    timeLeft += phrase.TimePer16th;
                    if ((start8thInBeat == 1 && timeLeft > phrase.TimePer8th * 0.5F)
                        || phrase.beatUnit == 8) { // in that case: skip 8ths!
                        
                        start8thInBeat = 0;
                        // only add time if we have to!
                        if (phrase.beatUnit != 8) { timeLeft += phrase.TimePer8th; }
                        
                        if (startBeatInBar == phrase.beatsPerBar - 1 && timeLeft > phrase.TimePerBeat * 0.5F) {
                            startBeatInBar = 0;
                            startBarInPhrase++;
                            if (startBarInPhrase >= phrase.DurationBars) {
                                eventId = 0;
                                startTime = 0;
                                startNote = 111110;
                                return false;
                            }
                        } else {
                            startBeatInBar++;
                        }
                    } else {
                        start8thInBeat = 1;
                    }
                } else {
                    start16thIn8th = 1;
                }
            }
            
            // Step 3 (note: 000010 looks like binary - but this is really 10)
            startNote =   (startBarInPhrase + 1) * 100000
                        + (startBeatInBar   + 1) * 010000
                        + (start8thInBeat   + 1) * 001000
                        + (start16thIn8th   + 1) * 000100
                        + (start32thIn16th  + 1) * 000010;
            
            return true;
        }
        
        /// <summary>Find out where exactly this event is rhythmically.</summary>
        /// <param name="phrase">Phrase that this event was recorded in</param>
        /// <returns>false, if this actually needs to go to the next phrase</returns>
        public bool ConvertToTripletBased(Phrase phrase) {
            /*
             * Step 1: Get minimum bar/beat/4th-tripets/8th-triplets
             * Step 2: Check if we're actually closer to the next
             * Step 3: Write the whole thing into a compact format in startBeat
             */

            // if -1 is kept, this will result in 0, meaning: "quantized away"
            int startBarInPhrase = -1; // Which bar in the section?
            int startBeatInBar = -1; // Which beat in the bar?
            int start4thTripletInBeat = -1; // Which 4th-triplet in the beat?
            int start8thTripletIn8th = -1; // Which 8th-tiplet in the eighth?
            
            // Step 1
            double timeLeft = startTime;
            
            startBarInPhrase = (int) (timeLeft / phrase.TimePerBar);
            timeLeft -= ((double)startBarInPhrase) * phrase.TimePerBar;

            startBeatInBar = (int)(timeLeft / phrase.TimePerBeat);
            timeLeft -= ((double)startBeatInBar) * phrase.TimePerBeat;

            start4thTripletInBeat = (int)(timeLeft / phrase.TimePer4thTriplet);
            timeLeft -= ((double)start4thTripletInBeat) * phrase.TimePer4thTriplet;
            
            start8thTripletIn8th = (int)(timeLeft / phrase.TimePer8thTriplet);
            timeLeft -= ((double)start8thTripletIn8th) * phrase.TimePer8thTriplet;
            
            // Step 2: If timeLeft more than half another 32th => push it all up
            //         This could be more precise by adding the time

            // this is kind of rare, but I have seen it: starting almost exactly
            // at the beginning of the first bar of the next phrase
            // this is most likely due to Update() being called a tiny bit
            // late, so the next phrase isn't active, yet, when the collision occurred
            if (startBarInPhrase >= phrase.DurationBars) {
                eventId = 0;
                startTime = 0;
                startTriplet = 111100;
                return false;
            }
            
            // the more usual case: we're just a little early on the next phrase
            if (start8thTripletIn8th == 1 && timeLeft > phrase.TimePer8thTriplet * 0.7F) {
                start8thTripletIn8th = 0;
                timeLeft += phrase.TimePer8thTriplet;
                if (start4thTripletInBeat == 1 && timeLeft > phrase.TimePer4thTriplet * 0.7F) {
                    
                    start4thTripletInBeat = 0;
                    
                    if (startBeatInBar == phrase.beatsPerBar - 1 && timeLeft > phrase.TimePerBeat * 0.5F) {
                        startBeatInBar = 0;
                        startBarInPhrase++;
                        if (startBarInPhrase >= phrase.DurationBars) {
                            eventId = 0;
                            startTime = 0;
                            startTriplet = 111100;
                            return false;
                        }
                    } else {
                        startBeatInBar++;
                    }
                } else {
                    start4thTripletInBeat = 1;
                }
            } else {
                start8thTripletIn8th = 1;
            }

            // Step 3 (note: 000010 looks like binary - but this is really 10)
            startTriplet =   (startBarInPhrase      + 1) * 100000
                           + (startBeatInBar        + 1) * 010000
                           + (start4thTripletInBeat + 1) * 001000
                           + (start8thTripletIn8th  + 1) * 000100;
            
            return true;
        }

        public int QuantizedStartNote(Phrase phrase, int dividerCount) {
            int startBarInPhrase = 0; // Which bar in the section?
            int startBeatInBar = 0; // Which beat in the bar?
            int start8thInBeat = 0; // Which eighth in the beat (quarter)?
            int start16thIn8th = 0; // Which sixteenth in the eighth?
            int start32thIn16th = 0; // Which thirty second in the sixteenth?
            
            // dividerCount = 1 => 4ths
            // dividerCount = 2 => 8ths
            // dividerCount = 4 => 16ths
            // dividerCount = 8 => 32ths
            switch (dividerCount) {
                case 1: // 4ths
                case 2: // 8ths
                case 4: // 16ths
                case 8: // 32ths
                    QuantizedStartTime2(phrase, dividerCount,
                        ref startBarInPhrase,
                        ref startBeatInBar,
                        ref start8thInBeat,
                        ref start16thIn8th,
                        ref start32thIn16th
                    );
                    break;
                // case 3: // 4th-triplets
                // case 6: // 8th-triplets
                //     return QuantizedStartTime3(phrase, dividerCount);
            }

            if (dividerCount != 0) {
                if (dividerCount < 8) {
                    start32thIn16th = -1;
                    if (dividerCount < 4) {
                        start16thIn8th = -1;
                        if (dividerCount < 2) {
                            start8thInBeat = -1;
                        }
                    }
                }
            }

            return (startBarInPhrase + 1) * 100000
                   + (startBeatInBar   + 1) * 010000
                   + (start8thInBeat   + 1) * 001000
                   + (start16thIn8th   + 1) * 000100
                   + (start32thIn16th  + 1) * 000010;
        }        
        
        public double QuantizedStartTime(Phrase phrase, int dividerCount) {
            int startBarInPhrase = 0; // Which bar in the section?
            int startBeatInBar = 0; // Which beat in the bar?
            int start8thInBeat = 0; // Which eighth in the beat (quarter)?
            int start16thIn8th = 0; // Which sixteenth in the eighth?
            int start32thIn16th = 0; // Which thirty second in the sixteenth?
            
            // dividerCount = 1 => 4ths
            // dividerCount = 2 => 8ths
            // dividerCount = 4 => 16ths
            // dividerCount = 8 => 32ths
            switch (dividerCount) {
                case 1: // 4ths
                case 2: // 8ths
                case 4: // 16ths
                case 8: // 32ths
                    QuantizedStartTime2(phrase, dividerCount,
                        ref startBarInPhrase,
                        ref startBeatInBar,
                        ref start8thInBeat,
                        ref start16thIn8th,
                        ref start32thIn16th
                        );
                    break;
                case 3: // 4th-triplets
                case 6: // 8th-triplets
                    return QuantizedStartTime3(phrase, dividerCount);
            }

            return startBarInPhrase * phrase.TimePerBar
                   + startBeatInBar * phrase.TimePerBeat
                   + start8thInBeat * phrase.TimePer8th
                   + start16thIn8th * phrase.TimePer16th
                   + start32thIn16th * phrase.TimePer32th;
        }

        private void QuantizedStartTime2(Phrase phrase, int dividerCount, 
                    ref int startBarInPhrase,
                    ref int startBeatInBar,
                    ref int start8thInBeat,
                    ref int start16thIn8th,
                    ref int start32thIn16th
            ) {
            
            int timeLeft = startNote;

            startBarInPhrase = Mathf.Max(0, (timeLeft / 100000) - 1);
            timeLeft -= (startBarInPhrase + 1) * 100000;

            startBeatInBar = Mathf.Max(0, (timeLeft / 010000) - 1);
            timeLeft -= (startBeatInBar + 1) * 010000;

            if (dividerCount > 1) {
                if (phrase.beatUnit != 8) {
                    start8thInBeat = Mathf.Max(0, (timeLeft / 001000) - 1);
                    timeLeft -= (start8thInBeat + 1) * 001000;
                } else {
                    Debug.LogWarning("phrase.beatUnit == 8 => might be trouble!!!");
                }

                if (dividerCount > 2) {
                    start16thIn8th = Mathf.Max(0, (timeLeft / 000100) - 1);
                    timeLeft -= (start16thIn8th + 1) * 000100;
                    if (dividerCount > 4) {
                        start32thIn16th = Mathf.Max(0, (timeLeft / 000010) - 1);
                    }
                }
            }
        }

        private double QuantizedStartTime3(Phrase phrase, int dividerCount) {
            return startTime;
        }
        
        public double QuantizedDuration(Phrase phrase, int dividerCount) {
            
            return duration;
        }
        
        #endregion Beat Based

        
    }

}
