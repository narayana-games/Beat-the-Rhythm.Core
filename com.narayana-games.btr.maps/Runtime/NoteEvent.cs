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
using NarayanaGames.BeatTheRhythm.Maps.Enums;

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     Represents a gameplay event.
    /// </summary>
    [Serializable]
    public class NoteEvent {

        /// <summary>
        ///     Start time of this note event, relative to the beginning time
        ///     of the phrase/sequence.
        /// </summary>
        public double startTime = 0;

        /// <summary>Duration; only used for sustained notes.</summary>
        public double duration = 0;

        /// <summary>Alternative timing: Which bar in the section?</summary>
        public int startBarInPhrase = 1;

        /// <summary>Alternative timing: Which beat in the bar?</summary>
        public int startBeatInBar = 1;

        /// <summary>Alternative timing: Which sixteenth in the beat?</summary>
        public int startSixteenthInBeat = 1;

        /// <summary>Alternative timing: How many sixteenths?</summary>
        public int durationSixteenths = 0;

        /// <summary>
        ///     Location; zero is center. This is based on dominantHand of sequence and
        ///     may be mirrored on X depending on dominantHand of current player.
        /// </summary>
        public UnityEngine.Vector2 location = UnityEngine.Vector2.zero;

        /// <summary>With which appendage.</summary>
        public PickupType pickupWith = PickupType.Any;

        /// <summary>What is the mechanic of this event?</summary>
        public TouchType touchType = TouchType.Orb;


    }

}
