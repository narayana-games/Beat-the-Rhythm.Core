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
    ///     Represents a rhythmic event within a section.
    /// </summary>
    [Serializable]
    public class TimingEvent {

        /// <summary>
        ///     A unique id for this event within the sequence. The
        ///     other event types (gameplay, melodic, effects) rely
        ///     on these ids, so not ever change them once additional
        ///     tracks have been created.
        /// </summary>
        public int eventId = 0;

        /// <summary>
        ///     List of hands/feet that this event was originally generated with.
        /// </summary>
        public List<Appendage> pickupHint = new List<Appendage>();
        
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

        
        #region Beat Based
        /// <summary>Which bar in the section?</summary>
        public int startBarInPhrase = 0;

        /// <summary>Which beat in the bar?</summary>
        public int startBeatInBar = 0;

        /// <summary>Which eigthth in the beat (quarter)?</summary>
        public int start8thInBeat = 0;

        /// <summary>Which sixteenth in the eighth?</summary>
        public int start16thIn8th = 0;
        
        /// <summary>Which thirty second in the sixteenth?</summary>
        public int start32thIn16th = 0;
        
        /// <summary>How many sixteenths? Only for sustained notes.</summary>
        public int duration32ths = 0;
        
        // TODO: Handle triplets!!!

        public void ConvertToBeatBased(Phrase phrase) {
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
            
            // => if timeLeft "almost" another 32th => push it all up
            
            Debug.Log($"Event: {eventId:00} "
                      +$"[Bar {startBarInPhrase} : {startBeatInBar}/{phrase.beatUnit} : "
                      +$"{start8thInBeat}/8 : {start16thIn8th}/16 : {start32thIn16th}/32]"
                      +$" - Time left: {timeLeft}  ");
        }
        
        #endregion Beat Based

    }

}
