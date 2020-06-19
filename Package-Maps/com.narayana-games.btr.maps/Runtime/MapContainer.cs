﻿#region Copyright and License Information
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
using NarayanaGames.BeatTheRhythm.Maps.Structure;
using NarayanaGames.BeatTheRhythm.Maps.Tracks;

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     A container to store and transmit beatmap information. For storage
    ///     and transmission, irrelevant fields must be set to null.
    ///     MapContainers are general purpose and can contain only the section
    ///     layout of a song, the actual gameplay of a full song, the whole
    ///     arrangement for a multiplayer choreography, a snippet for a 
    ///     general purpose section with a given tempo and applicable tempo 
    ///     range, as well as meta-information that can be used e.g. for 
    ///     effects and lightshows.
    /// </summary>
    [Serializable]
    public class MapContainer {

#if !UNITY_2017_4_OR_NEWER // used to see if we are OUTSIDE Unity
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public MongoDB.Bson.ObjectId Id { get; set; }
#endif

        /// <summary>
        ///     Current version of MapContainer.
        /// </summary>
        public static int VERSION = 0;
        /// <summary>
        ///     The version of MapContainer that this has been stored with.
        /// </summary>
        public int version = 0;

        /// <summary>
        ///     The unique id of this container. This can be used to create
        ///     references and composite maps from building blocks.
        /// </summary>
        public string containerId = null;

        /// <summary>
        ///     Owner, authors, permissions and whether the item has been locked.
        /// </summary>
        public Permissions permissions = new Permissions();

        /// <summary>
        ///     External Song Structure; used to save without redundancy.
        /// </summary>
        public string songStructureContainerId = null;

        /// <summary>
        ///     Internal Song Structure; used to transmit data conveniently.
        ///     This should be set to null when storing in a database.
        /// </summary>
        public SongStructure songStructure = new SongStructure();

        /// <summary>
        ///     A list of track definition references.
        /// </summary>
        public List<string> trackContainerIds = null;

        /// <summary>List of actual tracks.</summary>
        public List<RhythmTrack> tracks = new List<RhythmTrack>();


        public RhythmTrack AddTrack() {
            RhythmTrack newRhythmTrack = new RhythmTrack();
            tracks.Add(newRhythmTrack);
            return newRhythmTrack;
        }

        public RhythmTrack FindTrack(int trackId) {
            return tracks[trackId];
        }

        public RhythmSequence FindSequenceFor(Phrase phrase, RhythmTrack rhythmTrack) {
            int phraseId = 0;
            for (int i = 0; i < songStructure.sections.Count; i++) {
                for (int x = 0; x < songStructure.sections[i].phrases.Count; x++) {
                    if (songStructure.sections[i].phrases[x] == phrase) {
                        if (rhythmTrack.sequences.Count > i) {
                            return rhythmTrack.sequences[phraseId];
                        }
                    }
                    phraseId++;
                }
            }
            return null;
        }

    }
}