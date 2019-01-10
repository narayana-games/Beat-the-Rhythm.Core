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

using NarayanaGames.BeatTheRhythm.Maps;
using UnityEngine.Events;

namespace NarayanaGames.BeatTheRhythm.Mapping {

    /// <summary>
    ///     Provides all methods to build and modify maps, both in live and
    ///     offline editing modes.
    /// </summary>
    public class MappingController : MonoBehaviour {

        public UnityEvent onSectionChanged = new UnityEvent();
        public UnityEvent onCurrentBeatsPerBarChanged = new UnityEvent();

        public AudioSource songAudio;

        public string currentMapPath = @"C:/GameDev/TestMap.json";

        private MapContainer currentMap;

        private Section currentSection;
        private Track currentTrack;
        private Sequence currentSequence;

        private int sectionInSong = 0; // starts at 0
        private int barInSection = 1; // starts at 1
        private int beatInBar = 1; // starts at 1

        private double secondsPerBar = 0F;

        private int currentBeatsPerBar = 0;

        public void CreateMap() {
            if (currentMap != null) {
                Debug.LogWarning("Called CreateMap while currentMap was not null!");
            }
            currentMap = new MapContainer();
        }

        public void LoadMap(MapContainer loadedMap) {
            if (currentMap != null) {
                Debug.LogWarning("Called CreateMap while currentMap was not null!");
            }
            currentMap = loadedMap;
        }

        public void SaveMap() {
            int firstBar = 1;
            for (int i = 0; i < currentMap.sections.Count; i++) {
                currentMap.sections[i].firstBar = firstBar;
                firstBar += currentMap.sections[i].barsPerSection;
            }
            string json = JsonUtility.ToJson(currentMap, true);
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(currentMapPath)) {
                writer.Write(json);
            }
        }

        public void OnApplicationQuit() {
            SaveMap();
        }

        public void StartSongFromBeginnig() {
            TimePrecise = 0;
            if (!songAudio.isPlaying) {
                StartPlaying();
                if (currentMap == null) {
                    CreateMap();
                }
                TappedNewSection();
            }
        }

        public void Seek(double offset) {
            TimePrecise = Mathf.Clamp((float) (TimePrecise + offset), 0, songAudio.clip.length - 1F);
            // TODO: Check where we are, set things up appropriately
            // Usually, this will be used for fixing section beginning / end, so that should be enough
        }

        public void StartPlaying() {
            songAudio.Play();
        }

        public void StartPlaying(double startTime) {
            TimePrecise = startTime;
            songAudio.Play();
        }

        public void StartPlaying(Section section) {
            StartPlaying(section.startTime);
        }

        public void StopPlaying() {
            songAudio.Stop();
        }

        public void TappedNewSection() {
            if (currentSection != null) {
                currentSection.duration = TimePrecise - currentSection.startTime;
                if (barInSection > 1) {
                    currentSection.barsPerSection = barInSection;
                    secondsPerBar = currentSection.duration / barInSection;
                } else {
                    currentSection.barsPerSection = Mathf.RoundToInt((float) (currentSection.duration / secondsPerBar));
                }
                currentSection.beatsPerBar = currentBeatsPerBar;
                currentSection.CalculateBPM();
            }
            currentSection = currentMap.FindSectionAt(TimePrecise + 0.5F);
            if (currentSection == null) {
                currentSection = currentMap.AddSection(TimePrecise);
            }
            CurrentSectionChanged();
            barInSection = 1;
            beatInBar = 1;
        }

        private void CurrentSectionChanged() {
            sectionInSong = currentMap.FindSectionIndex(currentSection);
            onSectionChanged.Invoke();
        }

        public void TappedNewBar() {
            if (beatInBar > 1) {
                currentBeatsPerBar = beatInBar;
                onCurrentBeatsPerBarChanged.Invoke();
            }
            barInSection++;
            beatInBar = 1;
        }

        public void TappedNewBeat() {
            beatInBar++;
        }


        #region Taken from Holodance.MultiTrackAudioSource

        // for looping, see: https://docs.unity3d.com/ScriptReference/AudioSource.PlayScheduled.html

        /// <summary>
        ///     Get or set the precise time as double value representing seconds.
        /// </summary>
        public double TimePrecise {
            get {
                if (CheckStartTime()) {
                    return (float)PreRollTime;
                }
                //if (individualTracks == null || individualTracks.Count == 0 || individualTracks[0].clip == null) {
                //    return 0.0;
                //}
                //return IsValid ? ((double)individualTracks[0].timeSamples) / ((double)individualTracks[0].clip.frequency) : 0.0;
                return ((double)songAudio.timeSamples) / ((double)songAudio.clip.frequency); 
            }
            set {
                //foreach (AudioSource audioSource in individualTracks) {
                //    audioSource.timeSamples = (int)(value * ((double)individualTracks[0].clip.frequency));
                //}
                songAudio.timeSamples = (int)(value * ((double)songAudio.clip.frequency));
            }
        }

        private bool CheckStartTime() {
            if (startTime > 0) {
                if (AudioSettings.dspTime > startTime) {
                    startTime = -1;
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        }

        private double PreRollTime {
            get { return AudioSettings.dspTime - startTime; }
        }

        /// <summary>
        ///     Similar to PlayDelayed but uses PlayScheduled to make sure all
        ///     clips are started exactly at the same sample.
        /// </summary>
        /// <param name="delaySeconds"></param>
        public void PlayAfterPreciseDelay(double delaySeconds) {
            PlayScheduled(AudioSettings.dspTime + delaySeconds);
        }

        private double startTime = -1;

        /// <summary>
        ///     Plays the clips of all AudioSources of this multitrack audio 
        ///     source scheduled. See 
        ///     Unity Scripting API, AudioSource.PlayScheduled
        ///     for more info.
        /// </summary>
        public void PlayScheduled(double time) {
            startTime = time;
            songAudio.PlayScheduled(time);
        }
        #endregion Taken from Holodance.MultiTrackAudioSource
    }
}
