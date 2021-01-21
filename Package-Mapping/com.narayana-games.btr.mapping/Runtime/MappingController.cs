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
using UnityEngine;
using UnityEngine.Events;
using NarayanaGames.Common.Audio;
using NarayanaGames.BeatTheRhythm.Maps;
using NarayanaGames.BeatTheRhythm.Maps.Enums;
using NarayanaGames.BeatTheRhythm.Maps.Structure;
using NarayanaGames.BeatTheRhythm.Maps.Tracks;
using Debug = UnityEngine.Debug;

namespace NarayanaGames.BeatTheRhythm.Mapping {

    /// <summary>
    ///     Provides all methods to build and modify maps, both in live and
    ///     offline editing modes.
    /// </summary>
    public class MappingController : MonoBehaviour {

        private const double MARGIN = 0.0001F;

        public enum MappingMode {
            NoMapLoaded,
            Sections,
            Timing,
            Gameplay,
            Playtest
        }

        public MappingMode Mode { get; set; }
        
        [Header("Audio")]
        public MultiTrackAudioSource songAudio;
        public AudioSource previewPlayer;
        public AudioSource previewTick;
        public AudioSource loopTick;
        public AudioSource metronomeOne;
        public AudioSource metronomeTwoThreeFour;

        [Header("Note Template")]
        public GameplayEvent genericTap = new GameplayEvent();
        
        [Header("Testing")]
        public string currentMapPath = "C:/GameDev/TestMapA.json";

        [Header("Events")]
        public UnityEvent onMapChanged = new UnityEvent();

        public class UnityEventCountIn : UnityEvent<int, int, float> { }

        public UnityEventCountIn onCountIn = new UnityEventCountIn();

        public class UnityEventTime : UnityEvent<float> { }
        public UnityEventTime onJumpToTime = new UnityEventTime();
        
        public UnityEvent onSectionChanged = new UnityEvent();
        public UnityEvent onPhraseChanged = new UnityEvent();
        public UnityEvent onCurrentBeatsPerBarChanged = new UnityEvent();
        public UnityEvent onChanged = new UnityEvent();

        public UnityEvent onPlayStateChanged = new UnityEvent();

        public UnityEvent onTimingTrackChanged = new UnityEvent();
        public UnityEvent onGameplayTrackChanged = new UnityEvent();

        public UnityEvent onEventsChanged = new UnityEvent();
        
        public class UnityEventGE : UnityEvent<CondensedEvent, bool> { }

        public UnityEventGE onGameplayEventAdded = new UnityEventGE();

        private Song currentSong = new Song();
        public Song CurrentSong {
            get => currentSong;
            set => currentSong = value;
        }

        private MapContainer currentMap;
        public MapContainer CurrentMap => currentMap;
        
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
                        UpdateLoopedSegment();
                    }
                    onPlayStateChanged.Invoke();
                }
            }
        }

        private void UpdateLoopedSegment() {
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

        public bool PlayLoopTick { get; set; }
        public bool PlayMetronome { get; set; }

        public bool CountIn { get; set; }
        
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
                    songAudio.CurrentSegment = currentPhrase;
                    CheckBPMChanged();
                    CurrentPhraseChanged();
                } else {
                    Debug.LogError("Tried to set CurrentPhrase = null => ignored!");
                }
            }
            get { return currentPhrase; }
        }

        private TimingTrack currentTimingTrack;

        public TimingTrack CurrentTimingTrack {
            get => currentTimingTrack;
            set {
                currentTimingTrack = value;
                for (int i = 0; i < currentMap.timingTracks.Count; i++) {
                    if (currentMap.timingTracks[i] == value) {
                        CurrentTimingTrackId = i;
                        return;
                    }
                }
                Debug.LogError("Had to add a new timing track while assigning timing track. Investigate!");
                currentMap.timingTracks.Add(currentTimingTrack);
                CurrentTimingTrackId = currentMap.timingTracks.Count - 1;
            }
        }

        private int currentTimingTrackId = 0;
        public int CurrentTimingTrackId {
            get => currentTimingTrackId;
            set {
                currentTimingTrackId = value;
                currentTimingTrack = currentMap.timingTracks[value];
                onTimingTrackChanged.Invoke();
                onEventsChanged.Invoke();
            }
        }

        public int TimingTrackCount {
            get => currentMap != null ? currentMap.timingTracks.Count : 0;
        }


        private GameplayTrack currentGameplayTrack;

        public GameplayTrack CurrentGameplayTrack {
            get => currentGameplayTrack;
            set {
                currentGameplayTrack = value;
                for (int i = 0; i < currentMap.timingTracks.Count; i++) {
                    if (currentMap.gameplayTracks[i] == value) {
                        CurrentGameplayTrackId = i;
                        return;
                    }
                }
                Debug.LogError("Had to add a new timing track while assigning timing track. Investigate!");
                currentMap.gameplayTracks.Add(currentGameplayTrack);
                CurrentGameplayTrackId = currentMap.gameplayTracks.Count - 1;
            }
        }

        private int currentGameplayTrackId = 0;
        public int CurrentGameplayTrackId {
            get => currentGameplayTrackId;
            set {
                currentGameplayTrackId = value;
                currentGameplayTrack = currentMap.gameplayTracks[value];
                onGameplayTrackChanged.Invoke();
                onEventsChanged.Invoke();
            }
        }

        public int GameplayTrackCount {
            get => currentMap != null ? currentMap.gameplayTracks.Count : 0;
        }        
        
        
        private TimingSequence currentTimingSequence;
        public TimingSequence CurrentTimingSequence { get => currentTimingSequence; }

        private GameplayPattern currentGameplayPattern;
        public GameplayPattern CurrentGameplayPattern { get => currentGameplayPattern; }
        
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

        private static int condensedEventIndex = 0;

        public void IsCurrentSong(Song song) {
            Debug.Log($"[MappingController] IsCurrentSong() => {currentSong.Equals(song)}");
            if (!currentSong.Equals(song)) {
                currentSong = song.Copy();
                Mode = MappingMode.NoMapLoaded;
                currentMap = null;
            }
        }
        
        public void OnDisable() {
            GameplayTime.TimingSource = null;
        }

        public void Update() {
            if (currentMap == null || currentMap.songStructure == null) {
                return;
            }
            CheckSegmentChanged();
            CheckLoopTick();
            CheckMetronome();
        }

        private void CheckSegmentChanged() {
            if (songAudio != null /*&& IsPlaying*/) {
                if (!songAudio.IsPreRolling) {
                    double currentTime = songAudio.TimePrecise;
                    double currentTimePlus = currentTime + MARGIN;
                    if (CurrentSection == null 
                        || currentTimePlus < CurrentSection.StartTime 
                        || currentTime > CurrentSection.EndTime) {

                        currentPhrase = currentMap.songStructure.FindPhraseAt(currentTime);
                        songAudio.CurrentSegment = currentPhrase;
                        if (currentPhrase == null && IsPlaying) {
                            Debug.LogError($"Current Phrase was set to NULL! - based on time: {currentTime:0.00}");
                        }

                        CurrentSection = currentMap.songStructure.FindSectionAt(currentTime);
                        CheckBPMChanged();
                    } else if (CurrentPhrase == null 
                               || currentTimePlus < CurrentPhrase.StartTime
                               || currentTime > CurrentPhrase.EndTime) {
                        
                        if (CurrentPhrase == null) {
                            Debug.LogFormat("Setting current phrase because it was previously null");
                        } else {
                            Debug.LogFormat(
                                "Current Time: {0:0.0000}/{1:0.0000}, Scheduled Time: {2:0.000}, current phrase: {3}",
                                songAudio.TimePrecise, currentTimePlus, songAudio.TimePreciseScheduled, CurrentPhrase);
                        }

                        CurrentPhrase = currentMap.songStructure.FindPhraseAt(currentTime);
                        CheckBPMChanged();
                    }
                }
            }
        }

        private void CheckBPMChanged() {
            if (CurrentMap.songStructure.keepTempo && CurrentPhrase != null) {
                currentBPM = CurrentPhrase.BPM;
                secondsPerBar = CurrentPhrase.TimePerBar;
            }
        }

        public void TestPlay(double startTime, double testTime, double endTime) {
            // We need very high precision here - seems like while 16ms does not
            // feel "laggy", 5ms feels tighter and more precise, see also the
            // answer starting with "The accepted answer here mainly discusses 
            // perception of audio synchronization in passively watching video" at:
            // https://gamedev.stackexchange.com/questions/74973/maximum-audio-delay-before-the-player-notices
            // https://gdcvault.com/play/1017877/The-Audio-Callback-for-Audio

            if (IsPlaying) {
                IsPaused = true;
                loopTick.Stop();
                metronomeOne.Stop();
                metronomeTwoThreeFour.Stop();
            }
            previewPlayer.clip = songAudio.individualTracks[0].clip;
            double time = AudioSettings.dspTime;
            time += 0.05F; // give it a brief moment

            previewPlayer.PlayScheduled(time);
            previewPlayer.timeSamples = MultiTrackAudioSource.TimeToSamples(previewPlayer.clip, startTime);
            previewPlayer.SetScheduledEndTime(time + endTime - startTime);

            if (testTime > 0) {
                previewTick.PlayScheduled(time + testTime - startTime);
            }
        }


        private bool isNextLoopTickScheduled = false;
        private double nextLoopTickStartTime = 0;

        private void CheckLoopTick() {
            if (PlayLoopTick && IsPlaying) {
                double time = AudioSettings.dspTime;
                if (!isNextLoopTickScheduled && (CurrentPhrase.EndTime - songAudio.TimePrecise) < 0.1f) {
                    isNextLoopTickScheduled = true;
                    nextLoopTickStartTime = time + (CurrentPhrase.EndTime - songAudio.TimePrecise) / songAudio.PitchNonZero;
                    loopTick.PlayScheduled(nextLoopTickStartTime);
                }
                if (isNextLoopTickScheduled && time > nextLoopTickStartTime) {
                    isNextLoopTickScheduled = false;
                }
            } else {
                isNextLoopTickScheduled = false;
                loopTick.Stop();
                nextLoopTickStartTime = double.MaxValue;
            }
        }

        private int metronomeBar = 0;
        private int metronomeBeat = 0;

        private bool isNextMetronomeTickScheduled = false;
        private bool isNextMetronomeTickDominant = false;
        private double nextMetronomeTickStartTime = 0;
        private double lastTimeInPhase = 100;

        private float lastPitch = 0;
        
        private void CheckMetronome() {
            bool isCountIn = (CountIn && songAudio.IsPreRolling);
            if (IsPlaying && (PlayMetronome || isCountIn)) {
                if (Mathf.Abs(songAudio.Pitch - lastPitch) > 0.01F) {
                    //RescheduleLoop();
                    lastPitch = songAudio.Pitch;
                }
                
                double time = AudioSettings.dspTime;
                double timeInPhrase = songAudio.TimePrecise - CurrentPhrase.StartTime;

                if (isCountIn) {
                    // skip first One of second count-in bar!
                    if (songAudio.TimePrecise > -0.1F) {
                        lastTimeInPhase = 100;
                        return;
                    }
                    timeInPhrase = songAudio.TimePrecise + 2F * CurrentPhrase.TimePerBar;
                }

                if (lastTimeInPhase > timeInPhrase) {
                    metronomeBar = 0;
                    metronomeBeat = 0;
                    isNextMetronomeTickScheduled = false;
                    metronomeOne.Stop();
                    metronomeTwoThreeFour.Stop();
                    metronomeOne.Play();
                    lastTimeInPhase = timeInPhrase;
                    if (isCountIn) {
                        onCountIn.Invoke(metronomeBar, metronomeBeat, 1F);
                    }
                    return;
                }
                lastTimeInPhase = timeInPhrase;

                double nextBar = (metronomeBar + 1) * CurrentPhrase.TimePerBar;
                double nextBeat = metronomeBar * CurrentPhrase.TimePerBar + (metronomeBeat + 1) * CurrentPhrase.TimePerBeat;

                double scheduledTime = 0;

                if (!isNextMetronomeTickScheduled) {
                    if (timeInPhrase + 0.1F >= nextBeat) {
                        metronomeBeat++;
                        scheduledTime = nextBeat - timeInPhrase;
                        if (scheduledTime > 0) {
                            isNextMetronomeTickDominant = false;
                            //if (metronomeBeat < CurrentPhrase.beatsPerBar) {
                            //    isNextMetronomeTickScheduled = true;
                            //    nextMetronomeTickStartTime = time + scheduledTime;
                            //    metronomeTwoThreeFour.PlayScheduled(nextMetronomeTickStartTime);
                            //    Debug.LogFormat("Will play Beat at: {0}, in {1} (time per bar: {2}, time per beat: {3})",
                            //        nextMetronomeTickStartTime, scheduledTime, CurrentPhrase.TimePerBar, CurrentPhrase.TimePerBeat);
                            //}
                        }
                    }
                    if (timeInPhrase + 0.1F >= nextBar) {
                        metronomeBar++;
                        metronomeBeat = 0;
                        scheduledTime = nextBar - timeInPhrase;
                        if (scheduledTime > 0) {
                            isNextMetronomeTickDominant = true;
                        }
                    }
                    if (scheduledTime > 0) {
                        isNextMetronomeTickScheduled = true;
                        nextMetronomeTickStartTime = time + scheduledTime / songAudio.PitchNonZero;
                        (isNextMetronomeTickDominant ? metronomeOne : metronomeTwoThreeFour).PlayScheduled(nextMetronomeTickStartTime);
                        //Debug.LogFormat("Will play {4} at: {0}, in {1} (time per bar: {2}, time per beat: {3})",
                        //    nextMetronomeTickStartTime, scheduledTime, CurrentPhrase.TimePerBar, CurrentPhrase.TimePerBeat,
                        //    isNextMetronomeTickDominant ? "BAR" : "Beat");
                    }
                }
                if (isNextMetronomeTickScheduled && time >= nextMetronomeTickStartTime) {
                    //Debug.LogFormat("isNextLoopTickScheduled was reset at {0} >= {1}", time, nextMetronomeTickStartTime);
                    isNextMetronomeTickScheduled = false;
                    if (isCountIn) {
                        // + but TimePrecise is negative => [1..0]
                        double remaingTimeNormalized = 1 - (timeInPhrase / (2F * CurrentPhrase.TimePerBar));
                        onCountIn.Invoke(metronomeBar, metronomeBeat, (float)remaingTimeNormalized);
                    }
                }

            } else {
                metronomeBar = 0;
                metronomeBeat = 0;

                isNextMetronomeTickScheduled = false;
                metronomeOne.Stop();
                metronomeTwoThreeFour.Stop();
                nextMetronomeTickStartTime = double.MaxValue;
            }
        }

        public void CreateMap() {
            if (songAudio == null) {
                Debug.LogError("Called CreateMap while songAudio was not yet assigned!");
            }
            if (currentMap != null) {
                Debug.LogWarning("Called CreateMap while currentMap was not null!");
            }
            Debug.Log($"Creating an empty MapContainer");
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
            Debug.Log($"Loading MapContainer");
            currentMap = JsonUtility.FromJson<MapContainer>(json);
            if (currentMap.timingTracks.Count > 0) {
                currentTimingTrack = currentMap.timingTracks[0];
            }

            if (currentMap.gameplayTracks.Count > 0) {
                currentGameplayTrack = currentMap.gameplayTracks[0];
            }

            if (string.IsNullOrEmpty(currentMap.songStructure.song.artist)) {
                currentMap.songStructure.song.artist = currentMap.songStructure.artist;
                currentMap.songStructure.song.title = currentMap.songStructure.title;
                currentMap.songStructure.song.dominantBPM = currentMap.songStructure.dominantBPM;
            }

            currentSong = currentMap.songStructure.song.Copy();
            
            FixBars();
            SetupNewMap();
        }

        public void SaveMap() {
            if (currentMap == null) { Debug.LogError("Cannot save map before map was created!"); return; }

            FixBars();

            string json = JsonUtility.ToJson(currentMap, true);
            using (System.IO.StreamWriter writer = System.IO.File.CreateText(currentMapPath)) {
                writer.Write(json);
            }
        }

        private void FixBars() {
            int firstBar = 0;
            for (int i = 0; i < currentMap.songStructure.sections.Count; i++) {
                currentMap.songStructure.sections[i].StartBar = firstBar;
                currentMap.songStructure.sections[i].CalculateStartBarsForPhrases();
                firstBar += currentMap.songStructure.sections[i].DurationBars;
            }
        }

        private void SetupNewMap() {
            double time = 0;
            if (songAudio != null) {
                time = songAudio.TimePrecise;
            }
            currentSection = currentMap.songStructure.FindSectionAt(time);
            currentPhrase = currentMap.songStructure.FindPhraseAt(time);
            songAudio.CurrentSegment = currentPhrase;
            CurrentSectionChanged();
            if (Mode == MappingMode.NoMapLoaded) {
                Mode = MappingMode.Sections;
            }

            onMapChanged.Invoke();
        }

        private Appendage currentDominantHand;
        
        public void StartTimingRecording(Appendage dominantHand, string playerId) {
            if (currentMap != null) {
                if (currentMap.timingTracks.Count > 0) {
                    CurrentTimingTrack = currentMap.timingTracks[0];
                } else {
                    CurrentTimingTrack = currentMap.AddTimingTrack(dominantHand);
                }

                if (currentMap.gameplayTracks.Count > 0) {
                    currentGameplayTrack = currentMap.gameplayTracks[0];
                } else {
                    currentGameplayTrack = currentMap.AddGameplayTrack(CurrentTimingTrack, dominantHand);
                }

                CurrentTimingTrack.permissions.AddAuthor(playerId);
                currentGameplayTrack.permissions.AddAuthor(playerId);
                
                // we must not make changes in the song structure after recording timings!
                currentMap.songStructure.permissions.isLocked = true;
                
                currentDominantHand = dominantHand;
                CheckSequenceChanged();
            } else {
                Debug.LogError("Can't Start Timing Recording when no Map is loaded!!!");
            }
        }

        public void AddTimingTrack(string playerId) {
            CurrentTimingTrack = currentMap.AddTimingTrack(currentDominantHand);
            CurrentTimingTrack.permissions.AddAuthor(playerId);
            
            CheckSequenceChanged();
            
            onEventsChanged.Invoke();
        }
        
        public void ClearCurrentTimingTrack() {
            foreach (GameplayPattern pattern in currentGameplayTrack.patterns) {
                pattern.events.Clear();
            }
            foreach (TimingSequence sequence in currentTimingTrack.sequences) {
                sequence.events.Clear();
            }
            
            onEventsChanged.Invoke();
        }

        public void DeleteCurrentTimingTrack() {
            if (currentMap.timingTracks.Count == 1) {
                ClearCurrentTimingTrack();
                return;
            }
            currentMap.timingTracks.RemoveAt(CurrentTimingTrackId);
            if (CurrentTimingTrackId >= TimingTrackCount) {
                CurrentTimingTrackId--;
            } else {
                CurrentTimingTrackId = CurrentTimingTrackId;
            }
            //onEventsChanged.Invoke(); => done in CurrentTimingTrackId
        }
        
        public void StartSongFromBeginning() {
            JumpToTime(0);
            if (!IsPlaying) {
                if (currentMap == null) {
                    CreateMap();
                }
                StartPlaying();
            }
        }

        public void Seek(double offset) {
            if (GameplayTime.TimingSource == null) {
                GameplayTime.TimingSource = songAudio;
            } 
            
            float time = Mathf.Clamp((float) (songAudio.TimePrecise + offset), 0, songAudio.Length - 1F);
            JumpToTime(time);
        }

        public void StartPlaying(SongSegment segment) {
            if (segment is Section) {
                currentSection = (Section)segment;
                currentPhrase = CurrentSection.phrases[0];
                songAudio.CurrentSegment = currentSection;
                CurrentSectionChanged();
                UpdateLoopedSegment();
            } else if (segment is Phrase) {
                currentSection = currentMap.songStructure.FindSectionAt(segment.StartTime);
                CurrentPhrase = (Phrase)segment;
            }
            CheckBPMChanged();
            StartPlaying(segment.StartTime);
            //if (IsLooping) {
            //    IsLooping = true;
            //}
        }

        public void StartPlaying(double startTime) {
            bool needsCountIn = !IsPlaying && !IsPaused;
            StartPlaying();
            JumpToTime(startTime);
            if (needsCountIn) {
                CheckCountIn();
            }
        }

        private void JumpToTime(double time) {
            songAudio.TimePrecise = time;
            onJumpToTime.Invoke((float)time);
        }
        
        public void StartPlaying() {
            if (!IsPlaying && !IsPaused) {
                songAudio.Play();
                onPlayStateChanged.Invoke();
                CheckCountIn();
            }
            GameplayTime.TimingSource = songAudio;
            metronomeBar = 0;
            metronomeBeat = 0;
        }

        private void CheckCountIn() {
            if (CountIn && currentPhrase != null) {
                songAudio.Stop(); // let's keep the start time
                songAudio.PlayAfterPreciseDelay(2F * currentPhrase.TimePerBar);
                lastTimeInPhase = 100;
            }
        }
        
        public void StopPlaying() {
            IsPaused = false;
            songAudio.Stop();
            onPlayStateChanged.Invoke();
            onJumpToTime.Invoke(0);
        }

        public bool IsPlaying {
            get { return songAudio != null && songAudio.IsPlaying; }
        }

        public void MoveToPrevSection(Phrase phrase) {
            currentMap.songStructure.MoveToPrevSection(phrase);
            Update();
            CurrentSectionChanged();
        }

        public void MoveToNextSection(Phrase phrase) {
            currentMap.songStructure.MoveToNextSection(phrase);
            Update();
            CurrentSectionChanged();
        }

        public void ConvertPhraseIntoSection(Phrase phrase) {
            currentMap.songStructure.ConvertPhraseIntoSection(phrase);
            Update();
            CurrentSectionChanged();
            //if (currentMap.songStructure.ConvertPhraseIntoSection(phrase)) {
            //    selectedSection = null;
            //    currentSection = null;
            //    currentPhrase = null;
            //    Update();
            //    CurrentSectionChanged();
            //}
        }

        public void DeleteSegment(SongSegment songSegment) {
            if (currentMap.songStructure.DeleteSegment(songSegment)) {
                selectedSection = null;
                currentSection = null;
                currentPhrase = null;
                songAudio.CurrentSegment = currentPhrase;
                Update();
                CurrentSectionChanged();
            }
        }

        public void DeleteEventsIn(Phrase phrase, bool includeTimingEvents) {
            List<GameplayTrack> dependentTracks = new List<GameplayTrack>();
            foreach (GameplayTrack track in currentMap.gameplayTracks) {
                if (track.timingTrackId == currentTimingTrack.timingTrackId) {
                    dependentTracks.Add(track);
                }
            }

            TimingSequence sequence = currentMap.FindSequenceFor(phrase, currentTimingTrack);
            GameplayPattern pattern = currentMap.FindPatternFor(phrase, currentGameplayTrack);
            Debug.Log($"Deleting everything in {phrase.Name}, {sequence.name}, {pattern.name} - {includeTimingEvents}, {sequence.events.Count} timing events, {pattern.events.Count} gameplay events");
            foreach (TimingEvent evt in sequence.events) {
                pattern.Delete(evt);
                if (includeTimingEvents) {
                    foreach (GameplayTrack track in dependentTracks) {
                        GameplayPattern dependentPattern = track.PatternForPhrase(phrase.phraseId);
                        dependentPattern.Delete(evt);
                    }
                }
            }

            if (includeTimingEvents) {
                sequence.events.Clear();
            }
        }
        
        public void DeleteEvents(List<CondensedEvent> events, bool includeTimingEvents) {
            List<GameplayTrack> dependentTracks = new List<GameplayTrack>();
            foreach (GameplayTrack track in currentMap.gameplayTracks) {
                if (track.timingTrackId == currentTimingTrack.timingTrackId) {
                    dependentTracks.Add(track);
                }
            }
            Debug.Log($"Deleting {events.Count} specific events from {events[0].Phrase.Name}");
            foreach (CondensedEvent evt in events) {
                if (evt.Event != null) {
                    evt.GameplayPattern.Delete(evt.Event);
                }

                if (includeTimingEvents) {
                    evt.TimingSequence.Delete(evt.TimingEvent);
                    foreach (GameplayTrack track in dependentTracks) {
                        GameplayPattern pattern = track.PatternForPhrase(evt.Phrase.phraseId);
                        pattern.Delete(evt.TimingEvent);
                    }
                }
            }
        }
        
        public bool PasteEvents(Phrase phrase, TimingSequence sequence, GameplayPattern pattern, 
                                double startTime, double startTimeCopyBuffer, double endTime, 
                                List<CondensedEvent> copyEvents) {

            bool changedDividerCount = false;
            if (copyEvents.Count == 0) {
                Debug.LogError("Tried to copy empty selection");
                return changedDividerCount;
            }

            Phrase fromPhrase = copyEvents[0].Phrase;
            TimingSequence fromSequence = copyEvents[0].TimingSequence;
            
            Debug.Log($"Copying from {fromPhrase.Name} ({fromSequence.dividerCount}) to {phrase.Name} ({sequence.dividerCount})");
            
            if (sequence.dividerCount < fromSequence.dividerCount) {
                sequence.dividerCount = fromSequence.dividerCount;
                changedDividerCount = true;
            }

            double bpmFactor = 1;
            if (Math.Abs(fromPhrase.bpm - phrase.bpm) > double.Epsilon) {
                bpmFactor *= fromPhrase.bpm / phrase.bpm;
            }
            
            foreach (CondensedEvent evt in copyEvents) {
                double impactTimeRelative = evt.ImpactTimeRelative - startTimeCopyBuffer;
                if (Math.Abs(bpmFactor - 1) > double.Epsilon) {
                    impactTimeRelative *= bpmFactor;
                    // Debug.Log($"From BPM: {fromPhrase.bpm}, To BPM: {phrase.bpm}, BPM-Factor: {bpmFactor}"
                    //           +$" | Original Time: {evt.ImpactTimeRelative} | Original Relative Time: {evt.ImpactTimeRelative - startTimeCopyBuffer}"
                    //           +$" | Final relative time: {impactTimeRelative} | Final Time: {startTime + impactTimeRelative}"
                    //           +$" (Start Time: {startTime})");
                }
                double impactTime = startTime + impactTimeRelative;
                AddGameplayEvent(phrase, sequence, pattern, evt.Event.pickupWith, impactTime, evt);
            }

            return changedDividerCount;
        }
        
        public void TappedNewSection() {
            if (currentMap == null) { Debug.LogError("Cannot start section before map was created!"); return; }

            CloseCurrentSegment(currentSection, barInSection);
            double time = CloseCurrentSegment(currentPhrase, barInPhrase);

            currentSection = currentMap.songStructure.AddSection(time);
            currentPhrase = currentMap.songStructure.FindPhraseAt(time);
            songAudio.CurrentSegment = currentPhrase;

            if (currentBPM > 1) {
                currentSection.BPM = currentBPM;
                currentPhrase.BPM = currentBPM;
            }

            FixBars();

            CurrentSectionChanged();
        }

        public void CurrentSectionChanged() {
            sectionInSong = currentMap.songStructure.FindSectionIndex(currentSection);

            phraseInSection = 1;
            barInSection = 1;
            barInPhrase = 1;
            beatInBar = 1;

            CheckSequenceChanged();
            
            UpdateLoopedSegment();
            onSectionChanged.Invoke();
        }

        public void TappedNewPhrase() {
            if (currentMap == null) { Debug.LogError("Cannot start phrase before map was created!"); return; }

            double time = CloseCurrentSegment(currentPhrase, barInPhrase);

            currentPhrase = currentMap.songStructure.AddPhrase(currentSection, time);
            songAudio.CurrentSegment = currentPhrase;

            if (currentBPM > 1) {
                currentPhrase.BPM = currentBPM;
            }

            currentSection.CalculateStartBarsForPhrases();

            CurrentPhraseChanged();
        }

        private double CloseCurrentSegment(SongSegment segment, int barCount) {
            double time = songAudio.TimePrecise;
            bool canChangeDuration = segment is Phrase || ((Section)segment).phrases.Count == 1;

            if (CurrentMap.songStructure.keepTempo) {
                if (canChangeDuration) {
                    double duration = time - segment.StartTime;
                    barCount = Mathf.RoundToInt((float) (duration / segment.TimePerBar));
                    duration = barCount * segment.TimePerBar;
                    time = segment.StartTime + duration;
                    Debug.LogFormat("Adjusted time: {0}; original time: {1}", time, songAudio.TimePrecise);
                }
            }

            if (segment != null) {
                if (canChangeDuration) {

                    string whatsTapped = "TappendNewSection";
                    if (segment is Phrase) {
                        whatsTapped = "TappendNewPhrase";
                    }

                    segment.DurationSeconds = time - segment.StartTime;
                    if (barCount > 1) {
                        segment.DurationBars = barCount;
                        secondsPerBar = segment.DurationSeconds / barCount;
                        //Debug.LogFormat("{2} (barCount > 1) => durationBars = {0}, secondsPerBar = {1}", barCount, secondsPerBar, whatsTapped);
                    } else {
                        segment.DurationBars = Mathf.RoundToInt((float)(segment.DurationSeconds / secondsPerBar));
                        if (segment.DurationBars > 1) {
                            //Debug.LogFormat("{2} (barCount == 1, secondsPerBar = {1}) => durationBars = {0}", barCount, secondsPerBar, whatsTapped);
                        } else {
                            segment.DurationBars = 1;
                            secondsPerBar = segment.DurationSeconds;
                            Debug.LogFormat("{2} (barCount > 1 but segment too short for one bar) => durationBars = {0}, secondsPerBar = {1}", barCount, secondsPerBar, whatsTapped);
                        }
                    }
                    segment.BeatsPerBar = currentBeatsPerBar;
                    segment.CalculateBPM();
                    currentBPM = segment.BPM;
                }
            }
            return time;
        }

        private void CurrentPhraseChanged() {
            barInPhrase = 1;
            beatInBar = 1;

            CheckSequenceChanged();

            UpdateLoopedSegment();
            onPhraseChanged.Invoke();
        }

        private void CheckSequenceChanged() {
            if (currentTimingTrack != null) {
                currentTimingSequence = currentMap.FindSequenceFor(currentPhrase, currentTimingTrack);
                if (currentTimingSequence != null) {
                    currentTimingSequence.bpm = currentPhrase.bpm;
                }
            }
            if (currentGameplayTrack != null) {
                currentGameplayPattern = currentMap.FindPatternFor(currentPhrase, currentGameplayTrack);
            }
        }
        
        public void TappedNewBar() {
            if (beatInBar > 1) {
                currentBeatsPerBar = beatInBar;
                onCurrentBeatsPerBarChanged.Invoke();
            }
            barInSection++;
            if (CurrentSection != null) {
                CurrentSection.DurationBars = barInSection;
                CurrentSection.FirstPhrase.CalculateBPM(songAudio.TimePrecise - CurrentSection.StartTime, barInSection - 1);
                currentBPM = CurrentSection.BPM;
            }

            barInPhrase++;
            if (CurrentPhrase != null) {
                CurrentPhrase.DurationBars = barInPhrase;
                CurrentPhrase.CalculateBPM(songAudio.TimePrecise - CurrentPhrase.StartTime, barInPhrase - 1);
            }
            beatInBar = 1;
            onChanged.Invoke();
        }

        public void TappedNewBeat() {
            beatInBar++;
            onChanged.Invoke();
        }

        private int impactCounter = 0;
        private TimingEvent lastTimingEvent = null;
        private double lastImpactTime = 0;
        private Appendage lastAppendage = Appendage.Any;
        private WeaponType lastWeaponLeft = WeaponType.CatcherCasual;
        private WeaponType lastWeaponRight = WeaponType.CatcherCasual;
        private double minDistanceLeft = 1F;
        private double minDistanceRight = 1F;

        // Minimum Times:
        //
        // [Laserblade] Current time: 0.223990929705217, minimum time left: 0.0213378684807246, right: 0.0746712018140592
        // [Laserblade] Current time: 0.202675736961446, minimum time left: 0.0213378684807246, right: 0
        // [Laserblade] Current time: 0.149319727891154, minimum time left: 0.0106575963718711, right: 0.0960090702947838
        // Laserblade: 0.02 => 0.01
        //
        // [Catcher] Current time: 0.44800453514739, minimum time left: 0.117346938775512, right: 0.117346938775512
        // [Catcher] Current time: 0.224013605442181, minimum time left: 0.117346938775512, right: 0.128004535147383
        // Catcher: 0.11
        //
        // [Gun] Current time: 0.202675736961453, minimum time left: 0.0533333333333275, right: 0.0106802721088428
        // [Gun] Current time: 0.170657596371882, minimum time left: 0.159999999999997,  right: 0.149342403628118
        // Gun: 0.01 => 0.15
        //
        // [BowAndArrow] Current time: 0.554648526077099, minimum time left: 0.52267573696145, right: 1
        // BowAndArrow: 0.52 => probably 0.5

        public void AddGameplayEvent(Phrase phrase, TimingSequence sequence, GameplayPattern pattern,
            Appendage pickUpWith, double impactTime, CondensedEvent eventToCopy = null) {
            WeaponType weapon = WeaponType.Catcher;
            WeaponInteraction interaction = pattern.weaponInteractionDominant;
            TimingEvent timingEvent = sequence.FindTimingEvent(phrase, impactTime, 0);
            if (timingEvent == null) {
                timingEvent = new TimingEvent();
                timingEvent.eventId = sequence.MaxEventID + 1;
                timingEvent.startTime = impactTime;
                timingEvent.pickupHint.Add(pickUpWith);
                Debug.Log($"Creating new event for time {impactTime:0.000} | eventId = {timingEvent.eventId}");
            } else {
                Debug.Log(
                    $"Using existing event {timingEvent.eventId} at {timingEvent.startTime:0.000} for time {impactTime:0.000}");
            }

            GameplayEvent gameplayEvent = null;
            if (eventToCopy != null) {
                // we already have a gameplay event from somewhere else (e.g. copy/paste)
                gameplayEvent = eventToCopy.Event.Copy();
                gameplayEvent.timingEventId = timingEvent.eventId;
                interaction = eventToCopy.WeaponInteraction;
            } else {
                gameplayEvent = genericTap.Copy();
                gameplayEvent.timingEventId = timingEvent.eventId;
                gameplayEvent.pickupWith = pickUpWith;
                switch (pickUpWith) {
                    case Appendage.Any:
                        gameplayEvent.rasterPos.x = 0;
                        gameplayEvent.rasterPos.y = 0;
                        gameplayEvent.hasDirection = false;
                        gameplayEvent.direction = 180;
                        break;
                    case Appendage.Head:
                        gameplayEvent.rasterPos.x = 0;
                        gameplayEvent.rasterPos.y = +1;
                        gameplayEvent.hasDirection = false;
                        break;
                    case Appendage.Left:
                        gameplayEvent.rasterPos.x = -1;
                        gameplayEvent.rasterPos.y = -1;
                        gameplayEvent.hasDirection = true;
                        gameplayEvent.direction = 0;
                        break;
                    case Appendage.Right:
                        gameplayEvent.rasterPos.x = +1;
                        gameplayEvent.rasterPos.y = -1;
                        gameplayEvent.hasDirection = true;
                        gameplayEvent.direction = 0;
                        break;
                    case Appendage.LeftFoot:
                        gameplayEvent.rasterPos.x = -1;
                        gameplayEvent.rasterPos.y = -3;
                        gameplayEvent.hasDirection = true;
                        gameplayEvent.direction = 90;
                        break;
                    case Appendage.RightFoot:
                        gameplayEvent.rasterPos.x = +1;
                        gameplayEvent.rasterPos.y = -3;
                        gameplayEvent.hasDirection = true;
                        gameplayEvent.direction = -90;
                        break;
                }
                gameplayEvent.pos = gameplayEvent.rasterPos.ToFloat();
            }

            CondensedEvent condensedEvent = new CondensedEvent() {
                Index = condensedEventIndex++,
                Song = currentMap.songStructure,
                Phrase = phrase,
                
                TimingTrack = currentTimingTrack,
                TimingSequence = sequence,
                TimingEvent = timingEvent,
                
                GameplayTrack = currentGameplayTrack,
                GameplayPattern = pattern,
                
                Event = gameplayEvent
            };

            // do the triplet conversion, even though it usually not used!
            timingEvent.ConvertToTripletBased(phrase);
            
            timingEvent.ConvertToBeatBased(phrase);
            
            AddEventToSequence(timingEvent, sequence, pickUpWith, weapon);
            AddEventToPattern(sequence, gameplayEvent, pattern);

            condensedEvent.WeaponInteraction = interaction;
            
            onGameplayEventAdded.Invoke(condensedEvent, false);
            
            //Debug.Log($"Created new event at: {timingEvent.startTime:0.000} | {timingEvent.startNote} | {timingEvent.startTriplet}");
        }
        
        public void TappedImpact() {
            TappedImpact(genericTap, WeaponInteraction.PunchKickFlying, WeaponType.Catcher);
        }
        
        public void TappedImpact(GameplayEvent tappedEvent, WeaponInteraction interaction, WeaponType weapon) {
            Appendage pickedUpWith = tappedEvent.pickupWith;

            if (!IsPlaying) {
                Debug.LogError("Cannot tap timings when not playing!");
                return;
            }

            impactCounter++;
            double impactTime = songAudio.TimePrecise;
            if (songAudio.IsPreRolling) { // counting in
                impactTime = CurrentPhrase.StartTime;
            }
            double distance = impactTime - lastImpactTime;

            if (pickedUpWith == Appendage.Left && weapon != lastWeaponLeft
                || pickedUpWith == Appendage.Right && weapon != lastWeaponRight) {
                
                switch (pickedUpWith) {
                    case Appendage.Left:
                        Debug.Log($"{impactCounter} Changed LEFT weapon from {lastWeaponLeft} to {weapon}, resetting min times!");
                        minDistanceLeft = 1F;
                        lastWeaponLeft = weapon;
                        break;
                    case Appendage.Right:
                        Debug.Log($"{impactCounter} Changed RIGHT weapon from {lastWeaponRight} to {weapon}, resetting min times!");
                        minDistanceRight = 1F;
                        lastWeaponRight = weapon;
                        break;
                }
                
            }

            if (distance > 0) {
                switch (pickedUpWith) {
                    case Appendage.Left:
                        minDistanceLeft = System.Math.Min(minDistanceLeft, distance);
                        break;
                    case Appendage.Right:
                        minDistanceRight = System.Math.Min(minDistanceRight, distance);
                        break;
                }
                Debug.Log($"[{pickedUpWith}/{weapon}/{impactCounter}-{condensedEventIndex}] Current time: {distance}, minimum time left: {minDistanceLeft}, right: {minDistanceRight}");
            } else if (lastAppendage == pickedUpWith) {
                Debug.LogError($"[{pickedUpWith}/{weapon}/{impactCounter}] Current time: {distance} - ZERO!?!?, minimum time left: {minDistanceLeft}, right: {minDistanceRight}");
            }

            lastAppendage = pickedUpWith;
            
            TimingEvent timingEvent = new TimingEvent();
            if (lastTimingEvent != null && distance < 0.01F && lastAppendage != pickedUpWith) {
                lastTimingEvent.pickupHint.Add(pickedUpWith);
                timingEvent = lastTimingEvent;
                Debug.Log($"[{pickedUpWith}/{weapon}/{impactCounter}-{condensedEventIndex}] Added {pickedUpWith} "
                          +$"to event {lastTimingEvent.eventId}, "
                          +$"{lastTimingEvent.pickupHint.Count} appendages registered");
            } else {
                timingEvent.eventId = currentTimingSequence.MaxEventID + 1;
                timingEvent.startTime = impactTime - CurrentPhrase.StartTime;
                timingEvent.pickupHint.Add(pickedUpWith);
                lastImpactTime = impactTime;
            }

            

            lastTimingEvent = timingEvent;

            GameplayEvent gameplayEvent = tappedEvent.Copy();
            gameplayEvent.timingEventId = timingEvent.eventId;

            CondensedEvent condensedEvent = new CondensedEvent() {
                Index = condensedEventIndex++,
                Song = currentMap.songStructure,
                Phrase = currentPhrase,
                
                TimingTrack = currentTimingTrack,
                TimingSequence = currentTimingSequence,
                TimingEvent = timingEvent,
                
                GameplayTrack = currentGameplayTrack,
                GameplayPattern = currentGameplayPattern,
                
                Event = gameplayEvent
            };

            // do the triplet conversion, even though it usually not used!
            bool tripletInThis = timingEvent.ConvertToTripletBased(CurrentPhrase);
            
            // check if we're in the current phrase after quantization
            if (timingEvent.ConvertToBeatBased(CurrentPhrase)) {
                AddEventToSequence(timingEvent, condensedEvent.TimingSequence, pickedUpWith, weapon);
                AddEventToPattern(currentTimingSequence, gameplayEvent, condensedEvent.GameplayPattern);
            } else { // false => overflow to the next sequence/phase - at least if we're not looping!
                if (!IsLooping) {
                    Phrase nextPhrase = currentMap.songStructure.FindPhraseAfter(CurrentPhrase);
                    
                    condensedEvent.TimingSequence = currentMap.FindSequenceFor(nextPhrase, CurrentTimingTrack);
                    
                    timingEvent.eventId = condensedEvent.TimingSequence.MaxEventID + 1;
                    gameplayEvent.timingEventId = timingEvent.eventId; 
                    
                    AddEventToSequence(timingEvent, condensedEvent.TimingSequence, pickedUpWith, weapon);

                    condensedEvent.GameplayPattern = currentMap.FindPatternFor(nextPhrase, CurrentGameplayTrack);
                    AddEventToPattern(currentTimingSequence, gameplayEvent, condensedEvent.GameplayPattern);
                    
                } else {
                    /*
                     * TODO: Also, make sure this all works with loops by adding TimePreciseNextLoop and
                     * always comparing to both numbers.
                     */
                    Debug.LogError("Dropping timing event because it would have looped!");
                }
            }

            condensedEvent.WeaponInteraction = interaction;
            
            onGameplayEventAdded.Invoke(condensedEvent, true);
        }

        private void AddEventToSequence(TimingEvent timingEvent, TimingSequence sequence, Appendage pickedUpWith, WeaponType weapon) {
            int insertAt = 0;
            for (int i = 0; i < sequence.events.Count; i++) {
                insertAt = i;
                if (sequence.events[i].startTime > timingEvent.startTime) {
                    break;
                }
                insertAt++;
            }

            if (insertAt < sequence.events.Count) {
                //Debug.Log($"Inserting: {insertAt} < {sequence.events.Count}");
                sequence.events.Insert(insertAt, timingEvent);
            } else {
                //Debug.Log($"Adding: {insertAt} < {sequence.events.Count}");
                sequence.events.Add(timingEvent);
            }
            //sequence.events.Sort((a, b) => a.startTime.CompareTo(b.startTime));

            // sequence.dominantHand = currentDominantHand;
            //
            // if (sequence.dominantHand == pickedUpWith) {
            //     sequence.weaponDominant = weapon;
            // } else {
            //     sequence.weaponNonDominant = weapon;
            // }
        }

        private void AddEventToPattern(TimingSequence sequence, GameplayEvent gameplayEvent, GameplayPattern pattern) {
            TimingEvent myTimingEvent = sequence.FindTimingEvent(gameplayEvent.timingEventId);
            int insertAt = 0;
            for (int i = 0; i < pattern.events.Count; i++) {
                insertAt = i;
                TimingEvent otherTimingEvent = sequence.FindTimingEvent(pattern.events[i].timingEventId);
                if (otherTimingEvent.startTime > myTimingEvent.startTime) {
                    break;
                }
                insertAt++;
            }

            if (insertAt < pattern.events.Count) {
                pattern.events.Insert(insertAt, gameplayEvent);
            } else {
                pattern.events.Add(gameplayEvent);
            }
            
            // pattern.events.Sort((a, b) => {
            //     TimingEvent ta = sequence.FindTimingEvent(a.timingEventId);
            //     TimingEvent tb = sequence.FindTimingEvent(b.timingEventId);
            //     return ta.startTime.CompareTo(tb.startTime);
            // });
        }
        
        // for looping, see: https://docs.unity3d.com/ScriptReference/AudioSource.PlayScheduled.html
    }
}
