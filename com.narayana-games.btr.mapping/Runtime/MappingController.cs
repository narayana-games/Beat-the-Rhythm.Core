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
using NarayanaGames.Common.Audio;

namespace NarayanaGames.BeatTheRhythm.Mapping {

    /// <summary>
    ///     Provides all methods to build and modify maps, both in live and
    ///     offline editing modes.
    /// </summary>
    public class MappingController : MonoBehaviour {

        public UnityEvent onSectionChanged = new UnityEvent();
        public UnityEvent onPhraseChanged = new UnityEvent();
        public UnityEvent onCurrentBeatsPerBarChanged = new UnityEvent();
        public UnityEvent onChanged = new UnityEvent();

        public UnityEvent onPlayStateChanged = new UnityEvent();

        public MultiTrackAudioSource songAudio;

        public string currentMapPath = "C:/GameDev/TestMapA.json";

        private MapContainer currentMap;
        public MapContainer CurrentMap { get { return currentMap; } }

        private bool isPaused = false;
        public bool IsPaused {
            get { return isPaused; }
            set {
                if (isPaused != value) {
                    isPaused = value;
                    if (songAudio != null) {
                        if (isPaused) {
                            songAudio.Pause();
                        } else {
                            songAudio.UnPause();
                        }
                    }
                    onPlayStateChanged.Invoke();
                }
            }
        }

        private bool isLooping = false;
        public bool IsLooping {
            get { return isLooping; }
            set {
                if (isLooping != value) {
                    isLooping = value;
                    if (songAudio != null) {
                        if (isLooping) {
                            if (SelectedSection != null) {
                                songAudio.CurrentLoopedSegment = SelectedSection;
                            } else {
                                songAudio.CurrentLoopedSegment = CurrentPhrase;
                            }
                            songAudio.LoopCurrentSegment = true;
                        } else {
                            songAudio.LoopCurrentSegment = false;
                            songAudio.CurrentLoopedSegment = null;
                        }
                    }
                    onPlayStateChanged.Invoke();
                }
            }
        }

        private SongSegment selectedSection = null;
        public SongSegment SelectedSection {
            get { return selectedSection; }
            set {
                if (selectedSection != value) {
                    selectedSection = value;
                }
            }
        }

        private Section currentSection;
        public Section CurrentSection {
            set {
                if (currentSection != null) {
                    currentSection = value;
                    CurrentSectionChanged();
                }
            }
            get { return currentSection; }
        }

        private Phrase currentPhrase;
        public Phrase CurrentPhrase {
            set {
                if (currentPhrase != null) {
                    currentPhrase = value;
                    CurrentPhraseChanged();
                }
            }
            get { return currentPhrase; }
        }

        private Track currentTrack;
        private Sequence currentSequence;

        private int sectionInSong = 0; // starts at 0
        public int SectionInSong { get { return sectionInSong + 1; } }

        private int phraseInSection = 1; // starts at 1
        public int PhraseInSection { get { return phraseInSection; } }

        private int barInSection = 1; // starts at 1
        public int BarInSection { get { return barInSection; } }

        private int barInPhrase = 1; // starts at 1
        public int BarInPhrase { get { return barInPhrase; } }

        private int beatInBar = 1; // starts at 1
        public int BeatInBar { get { return beatInBar; } }

        private double secondsPerBar = 0;
        private double currentBPM = 0;

        private int currentBeatsPerBar = 4;

        public void Update() {
            if (songAudio != null && IsPlaying) {
                if (CurrentSection == null || songAudio.TimePrecise < CurrentSection.StartTime || songAudio.TimePrecise > CurrentSection.EndTime) {
                    currentPhrase = currentMap.songStructure.FindPhraseAt(songAudio.TimePrecise);
                    CurrentSection = currentMap.songStructure.FindSectionAt(songAudio.TimePrecise);
                } else if (CurrentPhrase == null || songAudio.TimePrecise < CurrentPhrase.StartTime || songAudio.TimePrecise > CurrentPhrase.EndTime) {
                    CurrentPhrase = currentMap.songStructure.FindPhraseAt(songAudio.TimePrecise);
                }
            }
        }

        public void CreateMap() {
            if (songAudio == null) {
                Debug.LogError("Called CreateMap while songAudio was not yet assigned!");
            }
            if (currentMap != null) {
                Debug.LogWarning("Called CreateMap while currentMap was not null!");
            }
            currentMap = new MapContainer();
            currentMap.songStructure.durationSeconds = songAudio.Length;
            currentMap.songStructure.AddSection(0);
            SetupNewMap();
        }

        public void LoadMap() {
            string json = null;
            using (System.IO.StreamReader reader = System.IO.File.OpenText(currentMapPath)) {
                json = reader.ReadToEnd();
            }
            currentMap = JsonUtility.FromJson<MapContainer>(json);
            SetupNewMap();
        }

        private void SetupNewMap() {
            double time = 0;
            if (songAudio != null) {
                time = songAudio.TimePrecise;
            }
            currentSection = currentMap.songStructure.FindSectionAt(time);
            currentPhrase = currentMap.songStructure.FindPhraseAt(time);
            CurrentSectionChanged();
        }

        public void SaveMap() {
            if (currentMap == null) { Debug.LogError("Cannot save map before map was created!"); return; }

            int firstBar = 1;
            for (int i = 0; i < currentMap.songStructure.sections.Count; i++) {
                currentMap.songStructure.sections[i].startBar = firstBar;
                firstBar += currentMap.songStructure.sections[i].durationBars;
            }
            string json = JsonUtility.ToJson(currentMap, true);
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(currentMapPath)) {
                writer.Write(json);
            }
        }

        public void StartSongFromBeginning() {
            songAudio.TimePrecise = 0;
            if (!IsPlaying) {
                if (currentMap == null) {
                    CreateMap();
                }
                StartPlaying();
            }
        }

        public void Seek(double offset) {
            songAudio.TimePrecise = Mathf.Clamp((float) (songAudio.TimePrecise + offset), 0, songAudio.Length - 1F);
        }

        public void StartPlaying(SongSegment segment) {
            StartPlaying(segment.StartTime);
            if (segment is Section) {
                currentSection = (Section)segment;
                currentPhrase = CurrentSection.phrases[0];
                CurrentSectionChanged();
            } else if (segment is Phrase) {
                currentSection = currentMap.songStructure.FindSectionAt(segment.StartTime);
                CurrentPhrase = (Phrase)segment;
            }
            if (IsLooping) {
                IsLooping = true;
            }
        }

        public void StartPlaying(double startTime) {
            songAudio.TimePrecise = startTime;
            StartPlaying();
        }

        public void StartPlaying() {
            if (!IsPlaying && !IsPaused) {
                songAudio.Play();
                onPlayStateChanged.Invoke();
            }
        }

        public void StopPlaying() {
            IsPaused = false;
            songAudio.Stop();
            onPlayStateChanged.Invoke();
        }

        public bool IsPlaying {
            get { return songAudio != null && songAudio.IsPlaying; }
        }

        public void DeleteSegment(SongSegment songSegment) {
            if (currentMap.songStructure.DeleteSegment(songSegment)) {
                selectedSection = null;
                currentSection = null;
                currentPhrase = null;
                Update();
                CurrentSectionChanged();
            }
        }

        public void TappedNewSection() {
            if (currentMap == null) { Debug.LogError("Cannot start section before map was created!"); return; }

            CloseCurrentSegment(currentSection, barInSection);
            CloseCurrentSegment(currentPhrase, barInPhrase);

            currentSection = currentMap.songStructure.AddSection(songAudio.TimePrecise);
            currentPhrase = currentMap.songStructure.FindPhraseAt(songAudio.TimePrecise);

            if (currentBPM > 1) {
                currentSection.bpm = currentBPM;
                currentPhrase.bpm = currentBPM;
            }

            CurrentSectionChanged();
        }

        private void CurrentSectionChanged() {
            sectionInSong = currentMap.songStructure.FindSectionIndex(currentSection);

            phraseInSection = 1;
            barInSection = 1;
            barInPhrase = 1;
            beatInBar = 1;

            onSectionChanged.Invoke();
        }

        public void TappedNewPhrase() {
            if (currentMap == null) { Debug.LogError("Cannot start phrase before map was created!"); return; }

            CloseCurrentSegment(currentPhrase, barInPhrase);

            currentPhrase = currentMap.songStructure.AddPhrase(currentSection, songAudio.TimePrecise);

            if (currentBPM > 1) {
                currentPhrase.bpm = currentBPM;
            }

            CurrentPhraseChanged();
        }

        private void CloseCurrentSegment(SongSegment segment, int barCount) {
            string whatsTapped = "TappendNewSection";
            if (segment is Phrase) {
                whatsTapped = "TappendNewPhrase";
            }
            if (segment != null) {
                segment.DurationSeconds = songAudio.TimePrecise - segment.StartTime;
                if (barCount > 1) {
                    segment.durationBars = barCount;
                    secondsPerBar = segment.DurationSeconds / barCount;
                    //Debug.LogFormat("{2} (barCount > 1) => durationBars = {0}, secondsPerBar = {1}", barCount, secondsPerBar, whatsTapped);
                } else {
                    segment.durationBars = Mathf.RoundToInt((float)(segment.DurationSeconds / secondsPerBar));
                    if (segment.durationBars > 1) {
                        //Debug.LogFormat("{2} (barCount == 1, secondsPerBar = {1}) => durationBars = {0}", barCount, secondsPerBar, whatsTapped);
                    } else {
                        segment.durationBars = 1;
                        secondsPerBar = segment.DurationSeconds;
                        Debug.LogFormat("{2} (barCount > 1 but segment too short for one bar) => durationBars = {0}, secondsPerBar = {1}", barCount, secondsPerBar, whatsTapped);
                    }
                }
                segment.beatsPerBar = currentBeatsPerBar;
                segment.CalculateBPM();
                currentBPM = segment.bpm;
            }
        }

        private void CurrentPhraseChanged() {
            barInPhrase = 1;
            beatInBar = 1;

            onPhraseChanged.Invoke();
        }

        public void TappedNewBar() {
            if (beatInBar > 1) {
                currentBeatsPerBar = beatInBar;
                onCurrentBeatsPerBarChanged.Invoke();
            }
            barInSection++;
            if (CurrentSection != null) {
                CurrentSection.durationBars = barInSection;
                CurrentSection.CalculateBPM(songAudio.TimePrecise - CurrentSection.StartTime, barInSection - 1);
                currentBPM = CurrentSection.bpm;
            }

            barInPhrase++;
            if (CurrentPhrase != null) {
                CurrentPhrase.durationBars = barInPhrase;
                CurrentPhrase.CalculateBPM(songAudio.TimePrecise - CurrentPhrase.StartTime, barInPhrase - 1);
            }
            beatInBar = 1;
            onChanged.Invoke();
        }

        public void TappedNewBeat() {
            beatInBar++;
            onChanged.Invoke();
        }


        // for looping, see: https://docs.unity3d.com/ScriptReference/AudioSource.PlayScheduled.html

    }
}
