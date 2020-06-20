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

using UnityEngine;

using System;
using System.Collections.Generic;

using NarayanaGames.BeatTheRhythm.Maps;
using UnityEngine.Events;

namespace NarayanaGames.BeatTheRhythm.Mapping {

    /// <summary>
    ///     A very simple keyboard based mapper. Simply set up the keys you
    ///     want to use for mapping, and map away.
    /// </summary>
    public class KeyboardMapper : MonoBehaviour {

        /// <summary>
        ///     The MappingController that does the actual work ;-)
        /// </summary>
        public MappingController controller = null;

        public string keyStartPlaying = "p";
        public string keyStopPlaying = "o";
        public string keyStartAtBeginning = "i";

        public float fastSeek = 2F;
        public float slowSeek = 0.5F;
        public string keyRewindFast = "h";
        public string keyRewindSlow = "j";
        public string keyForwardSlow = "k";
        public string keyForwardFast = "l";

        public string keyNewSection = "a";
        public string keyNewPhrase = "s";
        public string keyNewBar = "d";
        public string keyNewBeat = "f";
        public string keyNewTimingEvent = "y";

        public void Update() {
            if (controller.IsPaused) {
                // ignore any keyboard input when paused
                return;
            }
            if (Input.anyKeyDown) {
                if (Input.GetKeyDown(keyStartPlaying)) {
                    controller.StartPlaying();
                } else if (Input.GetKeyDown(keyStopPlaying)) {
                    controller.StopPlaying();
                } else if (Input.GetKeyDown(keyStartAtBeginning)) {
                    controller.StartSongFromBeginning();

                } else if (Input.GetKeyDown(keyRewindFast)) {
                    controller.Seek(-fastSeek);
                } else if (Input.GetKeyDown(keyRewindSlow)) {
                    controller.Seek(-slowSeek);
                } else if (Input.GetKeyDown(keyForwardSlow)) {
                    controller.Seek(slowSeek);
                } else if (Input.GetKeyDown(keyForwardFast)) {
                    controller.Seek(fastSeek);

                } else if (Input.GetKeyDown(keyNewSection)) {
                    controller.TappedNewSection();
                } else if (Input.GetKeyDown(keyNewPhrase)) {
                    controller.TappedNewPhrase();
                } else if (Input.GetKeyDown(keyNewBar)) {
                    controller.TappedNewBar();
                } else if (Input.GetKeyDown(keyNewBeat)) {
                    controller.TappedNewBeat();
                } else if (Input.GetKeyDown(keyNewTimingEvent)) {
                    controller.TappedImpact();
                }
            }
        }
    }
}
