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
    ///     Represents one rhythmic track of a song / recording. Usually, a MapContainer
    ///     will only have a single track. However, for multiplayer, several different
    ///     tracks that are designed to be played together can be stored in a single
    ///     map container.
    /// </summary>
    [Serializable]
    public class TimingTrack {

        /// <summary>Globally unique ID that gameplay and other tracks can refer to.</summary>
        public string timingTrackId = null;

        /// <summary>The name of this track. This could be be the role in a multiplayer ensemble.</summary>
        public string name;
        
        /// <summary>Owner, permissions and whether the item has been locked.</summary>
        public Permissions permissions = new Permissions();
        
        /// <summary>The maximum difficulty that could be achieved with this full rhythm track.</summary>
        public DifficultyPreset difficulty = DifficultyPreset.Casual;

        /// <summary>The rhythmic style of the whole track.</summary>
        public RhythmStyle rhythmStyle = RhythmStyle.Mixed;

        /// <summary>The instrument type of this track.</summary>
        public InstrumentType instrumentType = InstrumentType.Mixed;

        /// <summary>Intended role of this track.</summary>
        public TrackRole trackRole = TrackRole.SinglePlayer;

        /// <summary>Multiple tracks can be grouped for multiplayer by giving them the same group name.</summary>
        public string multiplayerGroup = "";
        
        /// <summary>
        ///     List of actual sequences, order is irrelevant. Can have more or less
        ///     entries than phrasesToSequenceIds because sequences can be re-used,
        ///     and it's sequences can also link to additional sequences.
        /// </summary>
        public List<TimingSequence> sequences = new List<TimingSequence>();
        
        /// <summary>Links phrases to sequences, index in list must match phraseId!</summary>
        public List<string> phrasesToSequenceIds = new List<string>();
        
        private Dictionary<string, TimingSequence> sequenceLookup = new Dictionary<string, TimingSequence>();
        

        public TimingSequence SequenceForPhrase(int phraseId) {
            if (sequenceLookup.Count == 0) {
                UpdateLookup();
            }
            return sequenceLookup[phrasesToSequenceIds[phraseId]];
        }
        
        public void UpdateLookup() {
            sequenceLookup.Clear();
            foreach (TimingSequence sequence in sequences) {
                sequenceLookup[sequence.timingSequenceId] = sequence;
            }
        }
        
    }
}
