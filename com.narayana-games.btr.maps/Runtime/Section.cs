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
            Silence,
            Intro,
            Verse,
            Chorus,
            Solo,
            Break,
            Bridge,
            BuildUp,
            Drop,
            FastDrop,
            Outro,
            FadeOut
        }

        /// <summary>The type of this section.</summary>
        public Type type = Type.Intro;

        /// <summary>One or more phrases that this section consists of.</summary>
        public List<Phrase> phrases = new List<Phrase>();

        public Section() {
            name = "Section";
            phrases.Add(new Phrase());
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

    }

}
