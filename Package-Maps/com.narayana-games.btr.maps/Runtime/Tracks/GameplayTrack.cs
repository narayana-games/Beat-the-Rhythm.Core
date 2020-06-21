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

        /// <summary>Globally unique ID that leaderboards can refer to.</summary>
        public string gameplayTrackId = null;

        /// <summary>The name of this track. This could be be the role in a multiplayer ensemble.</summary>
        public string name;

        /// <summary>Owner, permissions and whether the item has been locked.</summary>
        public Permissions permissions = new Permissions();
        
        
        /// <summary>The timing track that this gameplay track was built for.</summary>
        public string timingTrackId = null;
        
        
        /// <summary>The difficulty of this full gameplay track.</summary>
        public DifficultyPreset difficulty = DifficultyPreset.Casual;

        /// <summary>Intended role of this track.</summary>
        public TrackRole trackRole = TrackRole.SinglePlayer;
        
        /// <summary>Multiple tracks can be grouped for multiplayer by giving them the same group name.</summary>
        public string multiplayerGroup = "";
        
        /// <summary>Tracking / play style this track has been designed for.</summary>
        public AppendageTracking trackedAppendages = AppendageTracking.TwoHands;

        /// <summary>
        ///     Dominant hand that this whole track was designed for; locations
        ///     will be mirrored when different from player
        /// </summary>
        public Appendage dominantHand = Appendage.Right;

        /// <summary>Weapon on dominant hand that this whole track was designed for.</summary>
        public WeaponType weaponDominant = WeaponType.MultiMechanic;
        
        /// <summary>Weapon on non-dominant hand that this whole track was designed for.</summary>
        public WeaponType weaponNonDominant = WeaponType.MultiMechanic;
        
        /// <summary>List of actual patterns. Can have less entries than TimingTrack.sequences!</summary>
        public List<GameplayPattern> patterns = new List<GameplayPattern>();
        
        /// <summary>Links phrases to patterns, index in list must match phraseId!</summary>
        public List<string> phrasesToPatternIds = new List<string>();
        
        private Dictionary<string, GameplayPattern> patternLookup = new Dictionary<string, GameplayPattern>();

        public GameplayPattern PatternForPhrase(int phraseId) {
            if (patternLookup.Count == 0) {
                UpdateLookup();
            }
            return patternLookup[phrasesToPatternIds[phraseId]];
        }
        
        public void UpdateLookup() {
            patternLookup.Clear();
            foreach (GameplayPattern pattern in patterns) {
                patternLookup[pattern.gameplayPatternId] = pattern;
            }
        }
    }
}
