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
using NarayanaGames.BeatTheRhythm.Maps.Enums;
using NarayanaGames.BeatTheRhythm.Maps.Structure;
using NarayanaGames.BeatTheRhythm.Maps.Tracks;
using NPOI.XWPF.UserModel;
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     A container to store and transmit beatmap information. For storage
    ///     and transmission, irrelevant fields must be set to null.
    ///     MapContainers are general purpose and can contain only the section
    ///     layout of a song, the actual gameplay of a full song, the whole
    ///     arrangement for a multiplayer choreography, a snippet for a 
    ///     general purpose section with a given tempo and applicable tempo 
    ///     range, as well as meta-information that can be used e.g. for 
    ///     effects and lightshows.
    /// </summary>
    [Serializable]
    public class MapContainer {

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
        ///     The unique id of this container. This can be used to create
        ///     references and composite maps from building blocks. If the
        ///     container has a single element (e.g. a TimingTrack, or even
        ///     just a GameplaySequence, the containerId is the id of that
        ///     object).
        /// </summary>
        public string containerId = null;

        /// <summary>Owner, permissions and whether the item has been locked.</summary>
        public Permissions permissions = new Permissions();

        
        /// <summary>
        ///     External Song Structure; used to save without redundancy.
        /// </summary>
        public string songStructureId = null;

        /// <summary>
        ///     Internal Song Structure; used to transmit data conveniently.
        ///     This should be set to null when storing in a database.
        /// </summary>
        public SongStructure songStructure = new SongStructure();

        
        /// <summary>
        ///     A list of timing track definition references (external).
        /// </summary>
        public List<string> timingTrackIds = null;

        /// <summary>List of actual timing tracks (internal).</summary>
        public List<TimingTrack> timingTracks = new List<TimingTrack>();

        
        /// <summary>
        ///     A list of gameplay track definition references (external).
        /// </summary>
        public List<string> gameplayTrackIds = null;

        /// <summary>List of actual gameplay tracks (internal).</summary>
        public List<GameplayTrack> gameplayTracks = new List<GameplayTrack>();
        

        public TimingTrack AddTimingTrack(Appendage dominantHand) {
            TimingTrack newTimingTrack = new TimingTrack();
            newTimingTrack.timingTrackId = Guid.NewGuid().ToString();
            newTimingTrack.name = $"Track {timingTracks.Count}";
            
            GameplayTrack newGameplayTrack = new GameplayTrack();
            newGameplayTrack.gameplayTrackId = Guid.NewGuid().ToString();
            newGameplayTrack.name = $"Default Gameplay for {newTimingTrack.name}";
            newGameplayTrack.timingTrackId = newTimingTrack.timingTrackId;
            newGameplayTrack.dominantHand = dominantHand;
            
            foreach (Section section in songStructure.sections){
                foreach (Phrase phrase in section.phrases){
                    TimingSequence ts = new TimingSequence();
                    ts.timingSequenceId = Guid.NewGuid().ToString();
                    ts.name = phrase.name;
                    ts.beatsPerBar = phrase.beatsPerBar;
                    ts.beatUnit = phrase.beatUnit;
                    ts.durationBars = phrase.durationBars;
                    ts.bpm = phrase.bpm;
                    ts.durationSeconds = phrase.durationSeconds;
                    newTimingTrack.sequences.Add(ts);
                    newTimingTrack.phrasesToSequenceIds.Add(ts.timingSequenceId);
                    
                    GameplayPattern pt = new GameplayPattern();
                    pt.gameplayPatternId = Guid.NewGuid().ToString();
                    pt.name = phrase.name;
                    pt.timingSequenceId = ts.timingSequenceId;
                    pt.dominantHand = newGameplayTrack.dominantHand;
                    
                    newGameplayTrack.patterns.Add(pt);
                    newGameplayTrack.phrasesToPatternIds.Add(pt.gameplayPatternId);
                }
            }
            
            newTimingTrack.UpdateLookup();
            newGameplayTrack.UpdateLookup();
            
            timingTracks.Add(newTimingTrack);
            gameplayTracks.Add(newGameplayTrack);
            
            return newTimingTrack;
        }

        public GameplayTrack AddGameplayTrack(TimingTrack newTimingTrack, Appendage dominantHand) {
            GameplayTrack newGameplayTrack = new GameplayTrack();
            newGameplayTrack.gameplayTrackId = Guid.NewGuid().ToString();
            newGameplayTrack.name = $"Gameplay for {newTimingTrack.name}";
            newGameplayTrack.timingTrackId = newTimingTrack.timingTrackId;
            newGameplayTrack.dominantHand = dominantHand;
            
            foreach (Section section in songStructure.sections){
                foreach (Phrase phrase in section.phrases) {
                    TimingSequence ts = FindSequenceFor(phrase, newTimingTrack);
                    
                    GameplayPattern pt = new GameplayPattern();
                    pt.gameplayPatternId = Guid.NewGuid().ToString();
                    pt.name = phrase.name;
                    pt.timingSequenceId = ts.timingSequenceId;
                    pt.dominantHand = newGameplayTrack.dominantHand;
                    
                    newGameplayTrack.patterns.Add(pt);
                    newGameplayTrack.phrasesToPatternIds.Add(pt.gameplayPatternId);
                }
            }

            return newGameplayTrack;
        }
        
        public TimingTrack FindTimingTrack(int trackId) {
            return timingTracks[trackId];
        }

        public TimingTrack FindTimingTrack(string trackId) {
            for (int i = 0; i < timingTracks.Count; i++) {
                if (timingTracks[i].timingTrackId.Equals(trackId)) {
                    return timingTracks[i];
                }
            }

            return null;
        }
        
        public TimingSequence FindSequenceFor(Phrase phrase, TimingTrack timingTrack) {
            int phraseId = 0;
            for (int i = 0; i < songStructure.sections.Count; i++) {
                for (int x = 0; x < songStructure.sections[i].phrases.Count; x++) {
                    if (songStructure.sections[i].phrases[x] == phrase) {
                        if (timingTrack.sequences.Count > i) {
                            return timingTrack.sequences[phraseId];
                        }
                    }
                    phraseId++;
                }
            }
            return null;
        }

        public GameplayTrack FindGameplayTrack(int trackId) {
            if (trackId < 0 || trackId >= gameplayTracks.Count) {
                return null;
            }
            
            return gameplayTracks[trackId];
        }
        
        public GameplayPattern FindPatternFor(Phrase phrase, GameplayTrack gameplayTrack) {
            int phraseId = 0;
            for (int i = 0; i < songStructure.sections.Count; i++) {
                for (int x = 0; x < songStructure.sections[i].phrases.Count; x++) {
                    if (songStructure.sections[i].phrases[x] == phrase) {
                        // gameplayTrack.PatternForPhrase() ???
                        if (gameplayTrack.patterns.Count > i) {
                            return gameplayTrack.patterns[phraseId];
                        }
                    }
                    phraseId++;
                }
            }
            return null;
        }

        public List<CondensedEvent> BuildGameplay(int gameplayTrackId = 0) {
            List<CondensedEvent> events = new List<CondensedEvent>();
            GameplayTrack gameplayTrack = FindGameplayTrack(gameplayTrackId);
            if (gameplayTrack == null) {
                Debug.LogError($"gameplayTrackId {gameplayTrackId} not found!");
                return events;
            }
            TimingTrack timingTrack = FindTimingTrack(gameplayTrack.timingTrackId);
            if (timingTrack == null) {
                Debug.LogError($"Could not resolve timing track {gameplayTrack.timingTrackId} in ");
                return null;
            }

            int eventIndex = 0;

            bool loggedException = false;
            
            foreach (Section section in songStructure.sections){
                foreach (Phrase phrase in section.phrases) {
                    TimingSequence sequence = timingTrack.SequenceForPhrase(phrase.phraseId);
                    GameplayPattern pattern = gameplayTrack.PatternForPhrase(phrase.phraseId);
                    if (!sequence.timingSequenceId.Equals(pattern.timingSequenceId)) {
                        Debug.LogError($"Phrase {phrase.phraseId} has mismatch between"
                                       + $" sequence ({sequence.timingSequenceId}) and"
                                       + $" pattern ({pattern.gameplayPatternId}) which links to"
                                       + $" sequence ({pattern.timingSequenceId})!");
                    }

                    if (pattern.multiHandPatternIds.Count > 0) {
                        #warning multiHandPatternIds is not supported, yet!
                        Debug.LogWarning("multiHandPatternIds is not supported, yet!");
                        // merge the patterns
                    }
                    
                    // rotations might best be in a separate list
                    // change target must be before events
                    // change weapon must be before events
                    // obstacles and events => order doesn't really matter
                    for (var i = 0; i < pattern.events.Count; i++) {
                        GameplayEvent evt = pattern.events[i];
                        try {
                            TimingEvent timingEvent = sequence.FindTimingEvent(evt.timingEventId);
                            
                            CondensedEvent condensedEvent = new CondensedEvent() {
                                Index = eventIndex++,
                                Song = songStructure,
                                Phrase = phrase,
                                
                                TimingTrack = timingTrack,
                                TimingSequence = sequence,
                                TimingEvent = timingEvent,
                                
                                GameplayTrack = gameplayTrack,
                                GameplayPattern = pattern,
                                
                                Event = evt
                            };
                            events.Add(condensedEvent);
                        } catch (Exception exc) {
                            //if (!loggedException) {
                                Debug.LogError($"Could not get timingEvent {evt.timingEventId}"
                                               + $" (Sequence Events: {sequence.events.Count})"
                                               + $" (Pattern Events: {pattern.events.Count})"
                                               + $" for phrase ({phrase.phraseId}),"
                                               + $" sequence: {sequence.timingSequenceId}"
                                               + $" pattern: {pattern.gameplayPatternId}"
                                               );
                                loggedException = true;
                            //}
                        }
                    }
                }
            }

            return events;
        }
        
        // TODO: Create a method that turns a list of CondensedEvents into a GameplayPattern
    }
}
