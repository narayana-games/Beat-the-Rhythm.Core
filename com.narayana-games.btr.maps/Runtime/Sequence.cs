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
using System.Collections.Generic;

using NarayanaGames.BeatTheRhythm.Maps.Enums;

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     Represents a musical and gameplay sequence, as explained in Wikipedia:
    ///     In music, a sequence is the restatement of a motif or longer melodic
    ///     (or harmonic) passage at a higher or lower pitch in the same voice.
    ///     See also: https://en.wikipedia.org/wiki/Sequence_(music)
    ///
    ///     Obviously, in our context, it's really a section of actual gameplay.
    /// </summary>
    [Serializable]
    public class Sequence {
        /// <summary>Game mechanic that this sequence has been designed for.</summary>
        public GameMechanic gameMechanic = GameMechanic.Catchers;

        /// <summary>Tracking / play style this sequence has been designed for.</summary>
        public TrackedAppendages trackedAppendages = TrackedAppendages.TwoHands;

        /// <summary>Dominant hand this sequence was designed for; locations will be mirrored when different from player</summary>
        public PickupType dominantHand = PickupType.Right;

        /// <summary>The name of this sequence, usually the name of the section.</summary>
        public string name;

        /// <summary>
        ///     Original tempo of this sequence in BPM. If different from the
        ///     sequence tempo, the timing of all events must be multiplied
        ///     accordingly.
        /// </summary>
        public float bpm = 120;

        /// <summary>
        ///     The events comprising this sequence.
        /// </summary>
        public List<NoteEvent> events = new List<NoteEvent>();
    }

}
