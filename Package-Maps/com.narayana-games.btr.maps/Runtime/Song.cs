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

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     Simplest Meta Data for a Song to be able to quickly check if two
    ///     audio files or beatmap files refer to the same song. More specifically
    ///     this refers to recordings of songs.
    /// </summary>
    [Serializable]
    public class Song {

#if !UNITY_2017_4_OR_NEWER // used to see if we are OUTSIDE Unity
        [MongoDB.Bson.Serialization.Attributes.BsonId]
        public MongoDB.Bson.ObjectId Id { get; set; }
#endif

        /// <summary>
        ///     The unique ID of the recording this refers to. A "recording"
        ///     can be any digital form of a specific song, an audio file,
        ///     YouTube video, that can use the same beatmap with an optional
        ///     offset to handle different starting times. Recordings have the
        ///     same artist, same or similar title, and same duration +/- 3
        ///     seconds. This can be null, if it is a general purpose snippet.
        /// </summary>
        public string audioRecordingId = null;

        /// <summary>Full audio file URL, e.g. on Spotify or YouTube.</summary>
        public string audioFileUrl = null;
        
        /// <summary>Just the name of the audio file.</summary>
        public string audioFileName = null;

        /// <summary>Full local audio file path.</summary>
        public string audioFilePath = null;

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

        /// <summary>The numerator of the meter signature (N in N/4).</summary>
        public int dominantBeatsPerBar = 4;

        /// <summary>The denominator of the meter signature (N in 4/N).</summary>
        public int dominantBeatUnit = 4;

        public Song Copy() {
            Song result = new Song() {
                audioRecordingId = audioRecordingId,
                audioFileUrl = audioFileUrl,
                audioFileName = audioFileName,
                audioFilePath = audioFilePath,
                artist = artist,
                title = title,
                durationSeconds = durationSeconds,
                dominantBPM = dominantBPM,
                dominantBeatsPerBar = dominantBeatsPerBar,
                dominantBeatUnit = dominantBeatUnit
            };
            return result;
        }
        
        public override bool Equals(object other) {
            if (other is Song otherSong) {
                // definitely the same
                if (!string.IsNullOrEmpty(audioRecordingId)
                    && !string.IsNullOrEmpty(otherSong.audioRecordingId)) {
                    return audioRecordingId.Equals(otherSong.audioRecordingId);
                }

                if (!string.IsNullOrEmpty(audioFileUrl)
                    && !string.IsNullOrEmpty(otherSong.audioFileUrl)) {
                    return audioFileUrl.Equals(otherSong.audioFileUrl);
                }

                if (!string.IsNullOrEmpty(artist)
                    && !string.IsNullOrEmpty(otherSong.artist)
                    && !string.IsNullOrEmpty(title)
                    && !string.IsNullOrEmpty(otherSong.title)
                    && durationSeconds > 0 && otherSong.durationSeconds > 0) {
                    return artist.Equals(otherSong.artist)
                           && title.Equals(otherSong.title)
                           && System.Math.Abs(durationSeconds - otherSong.durationSeconds) < 3;
                }
            }

            return false;
        }
    }
}
