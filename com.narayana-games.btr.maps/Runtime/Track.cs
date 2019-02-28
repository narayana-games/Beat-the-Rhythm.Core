﻿#region Copyright and License Information
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
    ///     Represents a gameplay track of a song / recording. Usually, a MapContainer
    ///     will only have a single track. However, for multiplayer, several different
    ///     tracks that are designed to be played together can be stored in a single
    ///     map container.
    /// </summary>
    [Serializable]
    public class Track {

        /// <summary>Game mechanic that this track has been designed for.</summary>
        public GameMechanic gameMechanic = GameMechanic.Catchers;

        /// <summary>Tracking / play style this track has been designed for.</summary>
        public TrackedAppendages trackedAppendages = TrackedAppendages.TwoHands;

        /// <summary>Dominant hand this was designed for; locations will be mirrored when different from player</summary>
        public PickupType dominantHand = PickupType.Right;

        /// <summary>The name of this track. This could be be the role in a multiplayer ensemble.</summary>
        public string name;

        /// <summary>A list of sequence definition references.</summary>
        public List<string> sequenceContainerIds = null;

        /// <summary>List of actual sequences; must match up with MapContainer.sections!</summary>
        public List<Sequence> sequences = new List<Sequence>();

    }

}