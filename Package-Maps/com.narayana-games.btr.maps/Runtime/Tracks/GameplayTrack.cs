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
    ///     Represents a gameplay track of a song / recording. Usually, a MapContainer
    ///     will only have a single track. However, for multiplayer, several different
    ///     tracks that are designed to be played together can be stored in a single
    ///     map container.
    /// </summary>
    [Serializable]
    public class GameplayTrack {
        // => EffectTrack => to be designed ...
        // => MelodyTrack => might be added at some point, currently no use case

        /// <summary>
        ///     The rhythm track that this gameplay track was built for. 
        /// </summary>
        public int rhythmTrackId;
        
        /// <summary>The difficulty of this full rhythm track.</summary>
        public DifficultyPreset difficulty = DifficultyPreset.Casual;

        /// <summary>Intended role of this track.</summary>
        public TrackRole trackRole = TrackRole.SinglePlayer;
        
        /// <summary>Tracking / play style this track has been designed for.</summary>
        public TrackedAppendages trackedAppendages = TrackedAppendages.TwoHands;

        /// <summary>Game mechanic that this track has been designed for.</summary>
        public GameMechanic gameMechanic = GameMechanic.Catchers;

        /// <summary>Dominant hand this was designed for; locations will be mirrored when different from player</summary>
        public PickupType dominantHand = PickupType.Right;

        /// <summary>The name of this track. This could be be the role in a multiplayer ensemble.</summary>
        public string name;

        /// <summary>List of actual patterns. Can have less entries than RhythmTrack.sequences!</summary>
        public List<GameplayPattern> patterns = new List<GameplayPattern>();
    }
}
