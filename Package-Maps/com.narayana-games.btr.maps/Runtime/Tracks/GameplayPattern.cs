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

namespace NarayanaGames.BeatTheRhythm.Maps.Tracks {

    /// <summary>
    ///     Represents a gameplay pattern.
    /// </summary>
    [Serializable]
    public class GameplayPattern { 

        /// <summary>Globally unique ID that gameplay and other tracks can refer to.</summary>
        public string gameplayPatternId = null;
        
        /// <summary>The name of this pattern, usually the name of the phrase.</summary>
        public string name;

        
        /// <summary>The timing sequence that this gameplay pattern was built for.</summary>
        public string timingSequenceId = null;
        
        /// <summary>The difficulty of this gameplay pattern.</summary>
        public DifficultyPreset difficulty = DifficultyPreset.Casual;
        
        /// <summary>Tracking / play style this pattern has been designed for.</summary>
        public AppendageTracking trackedAppendages = AppendageTracking.TwoHands;

        /// <summary>
        ///     Dominant hand that this pattern was designed for; locations
        ///     will be mirrored when different from player.
        /// </summary>
        public Appendage dominantHand = Appendage.Right;

        /// <summary>Weapon interaction on dominant hand that this pattern was designed for.</summary>
        public WeaponInteraction weaponInteractionDominant = WeaponInteraction.PunchKickFlying;
        
        /// <summary>Weapon interaction on non-dominant hand that this pattern was designed for.</summary>
        public WeaponInteraction weaponInteractionNonDominant = WeaponInteraction.PunchKickFlying;

        /// <summary>
        ///     The default target for this pattern. Can be overridden by targetChanges.
        ///     A unique string that defines which prefab should be used.
        ///     The lookup order is: In the folder where the beatmap resides
        ///     (so a beatmap can override the default target prefabs),
        ///     in the active target mod folder (so mods can override the
        ///     targets) and finally, in the default game folder. 
        /// </summary>
        public string targetPrefab = string.Empty;

        /// <summary>
        ///     Direction changes are allowed at any time.
        /// </summary>
        public List<GameplayDirection> directionChanges = new List<GameplayDirection>();
        
        /// <summary>
        ///     Target changes are allowed at any time and will last
        ///     either until the next pattern that defines a targetPrefab,
        ///     or until the next change target event occurs (whichever
        ///     comes first).
        /// </summary>
        public List<GameplayChangeTarget> targetChanges = new List<GameplayChangeTarget>();
        
        /// <summary>
        ///     Weapon changes are only allowed before, and after the events
        ///     in a pattern. If a weapon change event is given after the
        ///     events, the actual weapon may be overriden by the following
        ///     pattern.
        /// </summary>
        public List<GameplayChangeWeapon> weaponChanges = new List<GameplayChangeWeapon>();
        
        /// <summary>The events comprising this pattern.</summary>
        public List<GameplayEvent> events = new List<GameplayEvent>();
        
        /// <summary>Obstacles to make the gameplay more interesting.</summary>
        public List<GameplayObstacle> obstacles = new List<GameplayObstacle>();
        
        /// <summary>
        ///     Links to additional patterns. Usually, it's best to design
        ///     gameplay for left and right hand (and feet/head, if applicable)
        ///     together in one stream. But sometimes, you may want to have
        ///     the dominant hand have a different rhythm from the right hand,
        ///     and in those cases, it's best to have one sequence and
        ///     pattern for each hand that only has the events of that
        ///     rhythmic pattern.
        /// </summary>
        public List<string> multiHandPatternIds = new List<string>();

        public void Delete(CondensedEvent evt) {
            if (evt.Direction != null) { directionChanges.Remove(evt.Direction); }
            if (evt.ChangeTarget != null) { targetChanges.Remove(evt.ChangeTarget); }
            if (evt.ChangeWeapon != null) { weaponChanges.Remove(evt.ChangeWeapon); }
            if (evt.Event != null) { events.Remove(evt.Event); }
            if (evt.Obstacle != null) { obstacles.Remove(evt.Obstacle); }
        }
        
        public void Delete(GameplayEvent evt) {
            events.Remove(evt);
        }

        public void Delete(TimingEvent evt) {
            directionChanges.RemoveAll(x => x.timingEventId == evt.eventId);
            targetChanges.RemoveAll(x => x.timingEventId == evt.eventId);
            weaponChanges.RemoveAll(x => x.timingEventId == evt.eventId);
            events.RemoveAll(x => x.timingEventId == evt.eventId);
            obstacles.RemoveAll(x => x.timingEventId == evt.eventId);
        }

        public void Sort(TimingSequence sequence) {
            directionChanges.Sort((a, b) => {
                TimingEvent ta = sequence.FindTimingEvent(a.timingEventId);
                TimingEvent tb = sequence.FindTimingEvent(b.timingEventId);
                return ta.startTime.CompareTo(tb.startTime);
            });
            targetChanges.Sort((a, b) => {
                TimingEvent ta = sequence.FindTimingEvent(a.timingEventId);
                TimingEvent tb = sequence.FindTimingEvent(b.timingEventId);
                return ta.startTime.CompareTo(tb.startTime);
            });
            weaponChanges.Sort((a, b) => {
                TimingEvent ta = sequence.FindTimingEvent(a.timingEventId);
                TimingEvent tb = sequence.FindTimingEvent(b.timingEventId);
                return ta.startTime.CompareTo(tb.startTime);
            });
            events.Sort((a, b) => {
                TimingEvent ta = sequence.FindTimingEvent(a.timingEventId);
                TimingEvent tb = sequence.FindTimingEvent(b.timingEventId);
                return ta.startTime.CompareTo(tb.startTime);
            });
            obstacles.Sort((a, b) => {
                TimingEvent ta = sequence.FindTimingEvent(a.timingEventId);
                TimingEvent tb = sequence.FindTimingEvent(b.timingEventId);
                return ta.startTime.CompareTo(tb.startTime);
            });
        }
    }

}
