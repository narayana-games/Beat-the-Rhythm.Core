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

        public int uniqueRhythmTrackId = 0;
        
        /// <summary>The maximum difficulty that could be achieved with this full rhythm track.</summary>
        public DifficultyPreset difficulty = DifficultyPreset.Casual;

        /// <summary>The rhythmic style of the whole track.</summary>
        public RhythmStyle rhythmStyle = RhythmStyle.Mixed;

        /// <summary>The instrument type of this track.</summary>
        public InstrumentType instrumentType = InstrumentType.Mixed;

        /// <summary>Intended role of this track.</summary>
        public TrackRole trackRole = TrackRole.SinglePlayer;

        public string multiplayerGroup = "";
        
        /// <summary>The name of this track. This could be be the role in a multiplayer ensemble.</summary>
        public string name;

        /// <summary>List of actual sequences. Can have less entries than MapContainer.sections.phrases!</summary>
        public List<TimingSequence> sequences = new List<TimingSequence>();
    }
}
