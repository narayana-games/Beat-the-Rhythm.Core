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
using UnityEngine.Events;
using NarayanaGames.Common.Audio;
using NarayanaGames.BeatTheRhythm.Maps;
using NarayanaGames.BeatTheRhythm.Maps.Enums;
using NarayanaGames.BeatTheRhythm.Maps.Structure;
using  NarayanaGames.BeatTheRhythm.Maps.Tracks;

namespace NarayanaGames.BeatTheRhythm.Mapping {

    /// <summary>
    ///     Provides all methods to build and modify maps, both in live and
    ///     offline editing modes.
    /// </summary>
    public class MappingController : MonoBehaviour {

        private const double MARGIN = 0.0001F;

        [Header("Audio")]
        public MultiTrackAudioSource songAudio;
        public AudioSource previewPlayer;
        public AudioSource previewTick;
        public AudioSource loopTick;
        public AudioSource metronomeOne;
        public AudioSource metronomeTwoThreeFour;

        [Header("Testing")]
        public string currentMapPath = "C:/GameDev/TestMapA.json";

        [Header("Events")]
        public UnityEvent onMapChanged = new UnityEvent();
        
        public UnityEvent onSectionChanged = new UnityEvent();
        public UnityEvent onPhraseChanged = new UnityEvent();
        public UnityEvent onCurrentBeatsPerBarChanged = new UnityEvent();
        public UnityEvent onChanged = new UnityEvent();

        public UnityEvent onPlayStateChanged = new UnityEvent();

        public UnityEvent onTimingTrackChanged = new UnityEvent();
        public UnityEvent onGameplayTrackChanged = new UnityEvent();

        public UnityEvent onEventsChanged = new UnityEvent();
        
        public class UnityEventGE : UnityEvent<CondensedEvent> { }

        public UnityEventGE onGameplayEventAdded = new UnityEventGE();

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
                    CheckBPMChanged();
                    CurrentPhraseChanged();
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
        
        public void Update() {
            if (currentMap?.songStructure == null) {
                return;
            }
            CheckSegmentChanged();
            CheckLoopTick();
            CheckMetronome();
        }

        private void CheckSegmentChanged() {
            if (songAudio != null && IsPlaying) {
                double currentTime = songAudio.TimePrecise;
                double currentTimePlus = currentTime + MARGIN;
                if (CurrentSection == null || currentTimePlus < CurrentSection.StartTime || currentTime > CurrentSection.EndTime) {
                    currentPhrase = currentMap.songStructure.FindPhraseAt(currentTime);
                    CurrentSection = currentMap.songStructure.FindSectionAt(currentTime);
                    CheckBPMChanged();
                } else if (CurrentPhrase == null || currentTimePlus < CurrentPhrase.StartTime || currentTime > CurrentPhrase.EndTime) {
                    if (CurrentPhrase == null) {
                        Debug.LogFormat("Setting current phrase because it was previously null");
                    } else {
                        Debug.LogFormat("Current Time: {0:0.0000}/{1:0.0000}, Scheduled Time: {2:0.000}, current phrase: {3}", 
                            songAudio.TimePrecise, currentTimePlus, songAudio.TimePreciseScheduled, CurrentPhrase);
                    }
                    CurrentPhrase = currentMap.songStructure.FindPhraseAt(currentTime);
                    CheckBPMChanged();
                }
            }
        }

        private void CheckBPMChanged() {
            if (CurrentMap.songStructure.keepTempo) {
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
                    nextLoopTickStartTime = time + (CurrentPhrase.EndTime - songAudio.TimePrecise);
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
        private double lastTimeInPhase = 0;

        private void CheckMetronome() {
            if (PlayMetronome && IsPlaying) {
                double time = AudioSettings.dspTime;
                double timeInPhrase = songAudio.TimePrecise - CurrentPhrase.StartTime;

                if (lastTimeInPhase > timeInPhrase) {
                    metronomeBar = 0;
                    metronomeBeat = 0;
                    isNextMetronomeTickScheduled = false;
                    metronomeOne.Stop();
                    metronomeTwoThreeFour.Stop();
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
                        nextMetronomeTickStartTime = time + scheduledTime;
                        (isNextMetronomeTickDominant ? metronomeOne : metronomeTwoThreeFour).PlayScheduled(nextMetronomeTickStartTime);
                        //Debug.LogFormat("Will play {4} at: {0}, in {1} (time per bar: {2}, time per beat: {3})",
                        //    nextMetronomeTickStartTime, scheduledTime, CurrentPhrase.TimePerBar, CurrentPhrase.TimePerBeat,
                        //    isNextMetronomeTickDominant ? "BAR" : "Beat");
                    }
                }
                if (isNextMetronomeTickScheduled && time >= nextMetronomeTickStartTime) {
                    //Debug.LogFormat("isNextLoopTickScheduled was reset at {0} >= {1}", time, nextMetronomeTickStartTime);
                    isNextMetronomeTickScheduled = false;
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
            int firstBar = 1;
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
            currentPhrase.StartBar = 0;
            CurrentSectionChanged();
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
            if (segment is Section) {
                currentSection = (Section)segment;
                currentPhrase = CurrentSection.phrases[0];
                CurrentSectionChanged();
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
            StartPlaying();
            songAudio.TimePrecise = startTime;
        }

        public void StartPlaying() {
            if (!IsPlaying && !IsPaused) {
                songAudio.Play();
                onPlayStateChanged.Invoke();
            }
            metronomeBar = 0;
            metronomeBeat = 0;
        }

        public void StopPlaying() {
            IsPaused = false;
            songAudio.Stop();
            onPlayStateChanged.Invoke();
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
                Update();
                CurrentSectionChanged();
            }
        }

        public void TappedNewSection() {
            if (currentMap == null) { Debug.LogError("Cannot start section before map was created!"); return; }

            CloseCurrentSegment(currentSection, barInSection);
            double time = CloseCurrentSegment(currentPhrase, barInPhrase);

            currentSection = currentMap.songStructure.AddSection(time);
            currentPhrase = currentMap.songStructure.FindPhraseAt(time);

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
                currentTimingSequence.bpm = currentPhrase.bpm;
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

        public GameplayEvent genericTap = new GameplayEvent() {
            
        };
        
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
            
            // check if we're in the current phrase after quantization
            if (timingEvent.ConvertToBeatBased(CurrentPhrase)) {
                AddEventToSequence(timingEvent, condensedEvent.TimingSequence, pickedUpWith, weapon);
                AddEventToPattern(gameplayEvent, condensedEvent.GameplayPattern);
            } else { // false => overflow to the next sequence/phase - at least if we're not looping!
                if (!IsLooping) {
                    Phrase nextPhrase = currentMap.songStructure.FindPhraseAfter(CurrentPhrase);
                    
                    condensedEvent.TimingSequence = currentMap.FindSequenceFor(nextPhrase, CurrentTimingTrack);
                    
                    timingEvent.eventId = condensedEvent.TimingSequence.MaxEventID + 1;
                    gameplayEvent.timingEventId = timingEvent.eventId; 
                    
                    AddEventToSequence(timingEvent, condensedEvent.TimingSequence, pickedUpWith, weapon);

                    condensedEvent.GameplayPattern = currentMap.FindPatternFor(nextPhrase, CurrentGameplayTrack);
                    AddEventToPattern(gameplayEvent, condensedEvent.GameplayPattern);
                    
                } else {
                    Debug.LogError("Dropping timing event because it would have looped!");
                }
            }

            condensedEvent.WeaponInteraction = interaction;
            
            onGameplayEventAdded.Invoke(condensedEvent);
        }

        private void AddEventToSequence(TimingEvent timingEvent, TimingSequence sequence, Appendage pickedUpWith, WeaponType weapon) {
            sequence.events.Add(timingEvent);

            // sequence.dominantHand = currentDominantHand;
            //
            // if (sequence.dominantHand == pickedUpWith) {
            //     sequence.weaponDominant = weapon;
            // } else {
            //     sequence.weaponNonDominant = weapon;
            // }
        }

        private void AddEventToPattern(GameplayEvent gameplayEvent, GameplayPattern pattern) {
            pattern.events.Add(gameplayEvent);
        }
        
        // for looping, see: https://docs.unity3d.com/ScriptReference/AudioSource.PlayScheduled.html
    }
}
