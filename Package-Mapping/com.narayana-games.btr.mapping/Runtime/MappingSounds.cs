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

using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Mapping {

    /// <summary>
    ///     A collection of AudioClips that can be used as feedback for
    ///     mapping
    /// </summary>
    [CreateAssetMenu(fileName = "MappingSounds", menuName = "Beatographer/MappingSounds", order = 1)]
    public class MappingSounds : ScriptableObject {

        [Header("Metronome Sounds")]

        [Tooltip("Sound played by the system on the first beat of a bar")]
        public AudioClip metronomeTickOne = null;

        [Tooltip("Sound played by the system on other beats of the bar")]
        public AudioClip metronomeTickOthers = null;

        [Header("Input Sounds")]

        [Tooltip("Sound played when a section start is triggered")]
        public AudioClip sectionStart = null;

        [Tooltip("Sound played when a phrase start is triggered")]
        public AudioClip phraseStart = null;

        [Tooltip("Sound played when the first beat of a bar is triggered")]
        public AudioClip downbeat = null;

        [Tooltip("Sound played when the second and fourth beat of a bar is triggered")]
        public AudioClip backbeat = null;

        [Tooltip("Sound played when the third beat of a bar is triggered")]
        public AudioClip thirdbeat = null;
    }
}
