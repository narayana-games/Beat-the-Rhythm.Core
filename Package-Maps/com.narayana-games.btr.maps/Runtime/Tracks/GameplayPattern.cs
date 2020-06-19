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
    ///     Represents a gameplay pattern.
    /// </summary>
    [Serializable]
    public class GameplayPattern { 

        /// <summary>Links this pattern to its phrase in the song structure.</summary>
        public int phraseId = 0;
        
        /// <summary>Game mechanic that this sequence has been designed for.</summary>
        public WeaponInteraction mechanic = WeaponInteraction.PunchKickFlying;

        /// <summary>Tracking / play style this sequence has been designed for.</summary>
        public AppendageTracking trackedAppendages = AppendageTracking.TwoHands;

        /// <summary>
        ///     Dominant hand that this sequence was designed for; locations
        ///     will be mirrored when different from player
        /// </summary>
        public Appendage dominantHand = Appendage.Right;

        /// <summary>The name of this pattern, usually the name of the phrase.</summary>
        public string name;

        /// <summary>
        ///     Original tempo of this sequence in BPM. If different from the
        ///     sequence tempo, the timing of all events must be multiplied
        ///     accordingly.
        /// </summary>
        public float bpm = 120;

        /// <summary>The events comprising this pattern.</summary>
        public List<GameplayEvent> events = new List<GameplayEvent>();
        
        /// <summary>Obstacles to make the gameplay more interesting.</summary>
        public List<GameplayObstacle> obstacles = new List<GameplayObstacle>();
    }

}
