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

        public override string Name {
            get => base.Name;
            set {
                if (phrases.Count == 1) {
                    phrases[0].name = value;
                }
                name = value;
            }
        }

        public override double StartTime {
            get {
                if (phrases.Count > 0) {
                    return phrases[0].StartTime;
                }
                return startTime;
            }
            set {
                startTime = value;
                if (phrases.Count > 0) {
                    phrases[0].SetStartTimeKeepEndTime(startTime);
                }
            }
        }

        public override double DurationSeconds {
            get {
                return EndTime - StartTime;
                //return durationSeconds;
            }
            set {
                durationSeconds = value;
                if (phrases.Count == 1) {
                    phrases[0].DurationSeconds = value;
                } else if (phrases.Count > 0) {
                    Phrase lastPhrase = phrases[phrases.Count - 1];
                    lastPhrase.SetEndTimeKeepStartTime(EndTime);
                }
            }
        }

        public override double EndTime {
            get {
                if (phrases.Count > 0) {
                    Phrase lastPhrase = phrases[phrases.Count - 1];
                    return lastPhrase.EndTime;
                }
                return base.EndTime;
            }
        }

        public override void CalculateBPM() {
            base.CalculateBPM();
            if (phrases.Count == 1) {
                Phrase myPhrase = phrases[0];
                myPhrase.startBar = this.startBar;
                myPhrase.durationBars = this.durationBars;
                myPhrase.beatsPerBar = this.beatsPerBar;
                myPhrase.beatUnit = this.beatUnit;
                phrases[0].CalculateBPM();
            }
        }

        public void CalculateBarsFromBPMandTimes(int barInSong) {
            startBar = barInSong;
            foreach (Phrase phrase in phrases) {
                phrase.startBar = barInSong;
                phrase.CalculateBarsFromBPMandTimes(barInSong);
                barInSong += phrase.durationBars;
                durationBars += phrase.durationBars;
            }
        }

        public void CalculateStartBarsForPhrases() {
            int barInSong = startBar;
            durationBars = 0;
            foreach (Phrase phrase in phrases) {
                phrase.startBar = barInSong;
                phrase.CalculateBarsFromBPMandTimes(barInSong);
                barInSong += phrase.durationBars;
                durationBars += phrase.durationBars;
            }
        }

        public void Consume(Phrase phrase) {
            phrases.Add(phrase);
            UpdateTimesFromPhrases();
        }

        public void Consume(Section other) {
            other.phrases[0].Name = other.Name;
            phrases.AddRange(other.phrases);
            UpdateTimesFromPhrases();
        }

        public void UpdateTimesFromPhrases() {
            phrases.Sort();

            Phrase first = phrases[0];
            Phrase last = phrases[phrases.Count - 1];

            startTime = first.startTime;
            durationSeconds = last.EndTime - startTime;

            startBar = first.startBar;
            durationBars = last.startBar + last.durationBars - startBar;
        }

        public void FixDurationBars() {
            durationBars = 0;
            foreach (Phrase phrase in phrases) {
                durationBars += phrase.durationBars;
            }
        }

    }

}
