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
using System.Runtime.Serialization;

namespace NarayanaGames.BeatTheRhythm.Maps.Structure {

    /// <summary>
    ///     Represents a musical section, as explained in Wikipedia:
    ///     A section is a complete, but not independent, musical idea. Types
    ///     of sections include the introduction or intro, exposition,
    ///     development, recapitulation, verse, chorus or refrain, conclusion,
    ///     coda or outro, fadeout, bridge or interlude. 
    ///     See also: https://en.wikipedia.org/wiki/Section_(music)
    ///     See also: https://en.wikipedia.org/wiki/Song_structure
    ///
    ///     Of course, drop and buildup are also important sections.
    /// </summary>
    [Serializable]
    public class Section : SongSegment {
        public enum Type : int {
            Silence = 0,
            Intro = 1,
            Breakdown = 2,
            BuildUp = 3, // synonyms: Ramp, Riser
            Drop = 4,
            FastDrop = 5,
            Verse = 6,
            Chorus = 7,
            Solo = 8,
            Break = 9, // synonyms: Tag, Middle 8
            Bridge = 10,
            Outro = 11,
            FadeOut = 12
        }

        public override string Name {
            get => base.Name;
            set {
                if (phrases.Count == 1) {
                    FirstPhrase.name = value;
                }
                name = value;
            }
        }

        public override double StartTime {
            get { return FirstPhrase.StartTime; }
            set { FirstPhrase.StartTime = value; }
        }

        public override double DurationSeconds {
            get { return EndTime - StartTime; }
            set {
                if (phrases.Count == 1) {
                    phrases[0].DurationSeconds = value;
                } else if (phrases.Count > 1) {
                    Phrase lastPhrase = phrases[phrases.Count - 1];
                    double newEndTime = StartTime + value;
                    double durationLastPhrase = newEndTime - lastPhrase.StartTime;
                    if (durationLastPhrase < 0) {
                        throw new ArgumentException(string.Format("[{0}] Cannot set duration to less than {1} (start time of last phrase in section: {2}), tried {3}",
                            this, lastPhrase.StartTime - StartTime, lastPhrase.StartTime, value));
                    }
                    lastPhrase.DurationSeconds = durationLastPhrase;
                }
            }
        }

        public override double EndTime { get { return LastPhrase.EndTime; } }

        public override void SetStartTimeKeepEndTime(double newStartTime) {
            FirstPhrase.SetStartTimeKeepEndTime(newStartTime);
        }

        public override void SetEndTimeKeepStartTime(double newEndTime) {
            LastPhrase.SetEndTimeKeepStartTime(newEndTime);
        }

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override int StartBar {
            get { return FirstPhrase.StartBar; }
            set { FirstPhrase.StartBar = value; }
        }

        public override int DurationBars {
            get {
                int durationBars = 0;
                for (int i = 0; i < phrases.Count; i++) {
                    durationBars += phrases[i].DurationBars;
                }
                return durationBars;
            }
            set {
                if (phrases.Count == 1) {
                    phrases[0].durationBars = value;
                } else {
                    throw new ArgumentException(string.Format("Cannot set DurationBars for sections with multiple Phrases, like: {0}", this));
                }
            }
        }


#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override int BeatsPerBar {
            get { return FirstPhrase.BeatsPerBar; }
            set { FirstPhrase.BeatsPerBar = value; }
        }


#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override int BeatUnit {
            get { return FirstPhrase.BeatUnit; }
            set { FirstPhrase.BeatUnit = value; }
        }

#if !UNITY_2017_4_OR_NEWER
        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
#endif
        [IgnoreDataMember]
        public override double BPM {
            get { return FirstPhrase.BPM; }
            set { FirstPhrase.BPM = value; }
        }


        /// <summary>
        ///     The type of this section. Common patterns:
        ///     EDM: Intro-Breakdown-Buildup-Drop-Breakdown-Buildup-Drop-Outro
        ///     Pop: Intro-Verse-Chorus-Verse-Chorus-Break-Chorus-Chorus-Outro
        ///     See also: http://dsmootz.blogspot.com/2015/05/song-structure-cage-match-verse-chorus.html
        /// </summary>
        public Type type = Type.Intro;

        /// <summary>One or more phrases that this section consists of.</summary>
        public List<Phrase> phrases = new List<Phrase>();

        public Section() {
            name = "Section";
            phrases.Add(new Phrase());
        }

        public Phrase FirstPhrase { get { return phrases[0]; } }
        public Phrase LastPhrase { get { return phrases[phrases.Count - 1]; } }


        public override void CalculateBPM() {
            for (int i=0; i < phrases.Count; i++) {
                phrases[i].CalculateBPM();
            }
        }

        public void CalculateBarsFromBPMandTimes(int barInSong) {
            foreach (Phrase phrase in phrases) {
                phrase.startBar = barInSong;
                phrase.CalculateBarsFromBPMandDuration(barInSong);
                barInSong += phrase.durationBars;
            }
        }

        public void CalculateStartBarsForPhrases() {
            int barInSong = StartBar;
            foreach (Phrase phrase in phrases) {
                phrase.startBar = barInSong;
                phrase.CalculateBarsFromBPMandDuration(barInSong);
                barInSong += phrase.durationBars;
            }
        }

        public void Consume(Phrase phrase) {
            phrases.Add(phrase);
            phrases.Sort();
        }

        public void Consume(Section other) {
            other.phrases[0].Name = other.Name;
            phrases.AddRange(other.phrases);
            phrases.Sort();
        }
    }

}
