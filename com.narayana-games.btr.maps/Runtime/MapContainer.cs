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

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     A container to store and transmit beatmap information. For storage
    ///     and transmission irrelevant fields must/should be set to null.
    ///     MapContainers are general purpose and can contain only the section
    ///     layout of a song, the actual gameplay of a full song, a snippet
    ///     for a general purpose section with a given tempo and applicable
    ///     tempo range, as well as meta-information that can be used e.g.
    ///     for effects and lightshows.
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

        #region Recording Meta Data
        /// <summary>
        ///     The unique ID of the recording this refers to. A "recording"
        ///     can be any digital form of a specific song, an audio file,
        ///     YouTube video, that can use the same beatmap with an optional
        ///     offset to handle different starting times. Recordings have the
        ///     same artist, same or similar title, and same duration +/- 5
        ///     seconds. This can be null, if it is a general purpose snippet.
        /// </summary>
        public string audioRecordingId = null;

        /// <summary>Name of the artist; stored redundantly with AudioRecording.</summary>
        public string artist = null;

        /// <summary>Title of the song; stored redundantly with AudioRecording.</summary>
        public string title = null;

        /// <summary>Title of the song; stored redundantly with AudioRecording.</summary>
        public float durationSeconds = 0;

        /// <summary>
        ///     If the song does not have tempo changes, this is simply the
        ///     tempo. Otherwise, this should be the "dominant" tempo, in other
        ///     words, how fast or slow the song as a whole feels, leaning towards
        ///     the fastest sections of the song, unless those are only very short.
        /// </summary>
        public float dominantBPM = 120;
        #endregion Recording Meta Data

        #region Ownership and Permissions
        /// <summary>
        ///     Owner of this specific instance. This is the person that has
        ///     created this version of the map. Ownership will only be
        ///     officially recognized upon map ranking when the author has
        ///     contributed at least 50% of the content of this map.
        /// </summary>
        public Author owner = null;

        /// <summary>Does the owner allow re-using the contents of this container?</summary>
        public bool ownerAllowsReuse = true;

        /// <summary>Does the owner allow modifying the contents of this container (only applies to new/copied instances)?</summary>
        public bool ownerAllowsModding = true;

        /// <summary>
        ///     A list of all authors that have contributed to any part of
        ///     this map. This is included here so we only need the player ids
        ///     in all nested levels.
        /// </summary>
        public List<Author> authors = new List<Author>();
        #endregion Ownership and Permissions

        /// <summary>Used for Phrase Libraries, together with sequence; containerId is the ID of the sequence in this case.</summary>
        public Phrase phrase;

        /// <summary>Used for Phrase Libraries, together with phrase; containerId is the ID of the sequence in this case.</summary>
        public Sequence sequence;


        /// <summary>Sections definition reference.</summary>
        public string sectionsContainerId = null;

        /// <summary>List of sections of this recording.</summary>
        public List<Section> sections = new List<Section>();


        /// <summary>A list of track definition references.</summary>
        public List<string> trackContainerIds = null;

        /// <summary>List of actual tracks.</summary>
        public List<Track> tracks = new List<Track>();


        public Section AddSection(double timeInSong) {
            // TODO: Do some sanity checks to make this safe
            Section newSection = new Section();
            newSection.startTime = timeInSong;
            sections.Add(newSection);

            newSection.AddPhrase(timeInSong);

            //sections.Sort();
            return newSection;
        }

        public int FindSectionIndex(Section section) {
            for (int i = 0; i < sections.Count; i++) {
                if (sections[i] == section) {
                    return i;
                }
            }
            return -1;
        }

        public Section FindSectionAt(double timeInSong) {
            Section section = null;
            for (int i = 0; i < sections.Count; i++) {
                section = sections[i];
                if (section.startTime <= timeInSong && timeInSong <= section.startTime + section.duration) {
                    return section;
                }
            }
            return null;
        }

        public Phrase FindPhraseAt(double timeInSong) {
            Section section = FindSectionAt(timeInSong);
            Phrase phrase = null;
            if (section != null) {
                for (int i = 0; i < section.phrases.Count; i++) {
                    phrase = section.phrases[i];
                    if (phrase.startTime <= timeInSong && timeInSong <= phrase.startTime + phrase.duration) {
                        return phrase;
                    }
                }
            }
            return null;
        }

        public Track AddTrack() {
            Track newTrack = new Track();
            tracks.Add(newTrack);
            return newTrack;
        }

        public Track FindTrack(int trackId) {
            return tracks[trackId];
        }

        public Sequence FindSequenceFor(Phrase phrase, Track track) {
            int phraseId = 0;
            for (int i = 0; i < sections.Count; i++) {
                for (int x = 0; x < sections[i].phrases.Count; x++) {
                    if (sections[i].phrases[x] == phrase) {
                        if (track.sequences.Count > i) {
                            return track.sequences[phraseId];
                        }
                    }
                    phraseId++;
                }
            }
            return null;
        }

    }
}
