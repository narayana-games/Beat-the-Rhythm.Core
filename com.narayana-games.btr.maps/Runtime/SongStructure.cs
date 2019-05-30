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
    ///     The structure of a song in terms of sections and (optional) phrases.
    /// </summary>
    [Serializable]
    public class SongStructure {

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
        ///     The unique id of this song structure.
        /// </summary>
        public string songStructureId = null;

        /// <summary>
        ///     Owner, permissions and whether the item has been locked.
        /// </summary>
        public Access access = new Access();

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

        /// <summary>The numerator of the meter signature (N in N/4).</summary>
        public int dominantBeatsPerBar = 4;

        /// <summary>The denominator of the meter signature (N in 4/N).</summary>
        public int dominantBeatUnit = 4;

        #endregion Recording Meta Data

        /// <summary>List of sections of this recording.</summary>
        public List<Section> sections = new List<Section>();

        public Section ConvertPhraseIntoSection(Phrase phrase) {
            Section oldParent = FindSectionAt(phrase.StartTime);

            Section newSection = new Section();
            sections.Add(newSection);
            newSection.type = Section.Type.Break;
            newSection.CopyFrom(phrase);
            newSection.phrases.Clear();
            newSection.phrases.Add(phrase);

            int phraseIndex = -1;
            for (int i=0; i < oldParent.phrases.Count; i++) {
                if (oldParent.phrases[i] == phrase) {
                    phraseIndex = i;
                    break;
                }
            }

            if (phraseIndex < 0) {
                UnityEngine.Debug.LogErrorFormat("Fatal error: cannot find '{0}' in '{1}'", phrase.name, oldParent.name);
                return null;
            }

            oldParent.phrases.Remove(phrase);

            if (phraseIndex == 0) {
                oldParent.SetStartTimeKeepEndTime(newSection.EndTime);
            } else if (phraseIndex == oldParent.phrases.Count) { // already removed!
                oldParent.SetEndTimeKeepStartTime(newSection.StartTime);
            } else {
                // a section in the middle => more trouble, now we'll have three sections
                Section thirdSection = new Section();
                sections.Add(thirdSection);
                thirdSection.CopyFrom(oldParent);
                thirdSection.phrases.Clear();
                thirdSection.phrases.AddRange(oldParent.phrases);
                thirdSection.type = oldParent.type;

                oldParent.phrases.RemoveRange(phraseIndex, oldParent.phrases.Count - phraseIndex);
                oldParent.SetEndTimeKeepStartTime(newSection.StartTime);

                thirdSection.phrases.RemoveRange(0, phraseIndex);
                thirdSection.name = thirdSection.phrases[0].name;
                thirdSection.SetStartTimeKeepEndTime(newSection.EndTime);
                thirdSection.FixDurationBars();
            }
            oldParent.FixDurationBars();

            sections.Sort();

            return newSection;
        }

        public void CalculateBarsFromBPMandTimes() {
            int barInSong = 1;
            foreach (Section section in sections) {
                section.startBar = barInSong;
                section.CalculateBarsFromBPMandTimes(barInSong);
                barInSong += section.durationBars;
            }
        }

        public bool DeleteSegment(SongSegment segment) {
            if (segment is Phrase) {
                Section parentSection = FindSectionAt(segment.StartTime);
                if (parentSection.phrases.Count > 1) {
                    DeleteSegment(parentSection.phrases, (Phrase) segment, parentSection.EndTime);
                    return true;
                } else {
                    // you should not be able to even select phrases the equal sections; but
                    // if you do => handle the section instead
                    segment = parentSection;
                }
            }
            if (sections.Count > 1) {
                DeleteSegment(sections, (Section)segment, durationSeconds);
                return true;
            }
            return false;
        }

        private void DeleteSegment<T>(List<T> segments, T segmentToDelete, double maxTime) where T : SongSegment {
            int index = segments.IndexOf(segmentToDelete);
            if (index < segments.Count - 1) {
                SongSegment next = segments[index + 1];
                double nextEndTime = next.EndTime;
                next.StartTime = segmentToDelete.StartTime;
                next.DurationSeconds = nextEndTime - next.StartTime;
            } else {
                SongSegment previous = segments[index - 1];
                previous.DurationSeconds = maxTime - previous.StartTime;
            }
            segments.RemoveAt(index);
        }


        public Section AddSection(double timeInSong) {
            Section newSection = new Section();
            AddSongSegment(sections, newSection, timeInSong);
            return newSection;
        }

        public Phrase AddPhrase(Section section, double timeInSong) {
            Phrase newPhrase = new Phrase();
            AddSongSegment(section.phrases, newPhrase, timeInSong);
            return newPhrase;
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
            return FindSegmentAt(sections, timeInSong);
        }

        public Phrase FindPhraseAt(double timeInSong) {
            Section section = FindSectionAt(timeInSong);
            if (section == null) {
                return null;
            }
            return FindSegmentAt(section.phrases, timeInSong);
        }

        public Section FindSectionAfter(double timeInSong) {
            return FindSegmentAfter(sections, timeInSong);
        }

        private void AddSongSegment<T>(List<T> segments, T segmentToAdd, double timeInSong) where T : SongSegment {
            if (sections.Count == 0) { // first section? => section == song
                segmentToAdd.StartTime = 0;
                segmentToAdd.DurationSeconds = durationSeconds;
            } else { // otherwise, until the next section, or end of song
                segmentToAdd.StartTime = timeInSong;

                T nextSegment = FindSegmentAfter(segments, timeInSong);
                if (nextSegment != null) {
                    segmentToAdd.DurationSeconds = nextSegment.StartTime - segmentToAdd.StartTime;
                } else {
                    Section nextSection = FindSegmentAfter(sections, timeInSong);
                    if (nextSection != null) {
                        segmentToAdd.DurationSeconds = nextSection.StartTime - segmentToAdd.StartTime;
                    } else {
                        segmentToAdd.DurationSeconds = durationSeconds - segmentToAdd.StartTime;
                    }
                }

                T currentSegment = FindSegmentAt(segments, timeInSong);
                if (currentSegment != null) {
                    currentSegment.DurationSeconds = segmentToAdd.StartTime - currentSegment.StartTime;
                }
            }

            segments.Add(segmentToAdd);

            if (segments.Count > 1) {
                segments.Sort();
            }
        }

        public T FindSegmentAt<T>(List<T> segments, double timeInSong) where T : SongSegment {
            T segment = null;
            for (int i = 0; i < segments.Count; i++) {
                segment = segments[i];
                double minEndTime = durationSeconds;
                if (segments.Count > i + 1) {
                    minEndTime = segments[i + 1].StartTime;
                }
                if (segment.StartTime <= timeInSong && (timeInSong < Math.Max(segment.EndTime, minEndTime))) {
                    return segment;
                }
            }
            //UnityEngine.Debug.LogErrorFormat("Could not find segment at {0}, {1} segments, latest time: {2}", timeInSong, segments.Count, segments[segments.Count - 1].EndTime);
            return null;
        }

        public T FindSegmentAfter<T>(List<T> segments, double timeInSong) where T : SongSegment {
            for (int i = 0; i < segments.Count; i++) {
                double minEndTime = durationSeconds;
                if (segments.Count > i + 1) {
                    minEndTime = segments[i + 1].StartTime;
                }
                if (segments[i].StartTime <= timeInSong && timeInSong < Math.Max(segments[i].EndTime, minEndTime)) {
                    if (segments.Count > i + 1) {
                        return segments[i + 1];
                    }
                }
            }
            return null;
        }

    }
}
