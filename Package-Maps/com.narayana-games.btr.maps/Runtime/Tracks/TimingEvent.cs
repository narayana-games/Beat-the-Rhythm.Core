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
        #endregion Beat Based

    }

}
