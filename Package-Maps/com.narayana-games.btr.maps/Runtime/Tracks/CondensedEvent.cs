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

using System.Collections.Generic;
using NarayanaGames.BeatTheRhythm.Maps.Enums;
using NarayanaGames.BeatTheRhythm.Maps.Structure;

namespace NarayanaGames.BeatTheRhythm.Maps.Tracks {

    [System.Serializable]
    /// <summary>
    ///     Has all the data relevant for a given gameplay event,
    ///     or timing event, when using the GamePlayevent and GameplayObstacle
    ///     lists.
    /// </summary>
    public class CondensedEvent {

        public int Index = 0;

        // the following properties must not be null!
        public SongStructure Song = null;
        public Phrase Phrase = null;
        
        public TimingTrack TimingTrack = null;
        public TimingSequence TimingSequence = null;
        public TimingEvent TimingEvent = null;

        public GameplayTrack GameplayTrack = null;
        public GameplayPattern GameplayPattern = null;
        
        // all except one of the following is usually null (in gameplay event lists)
        // TODO: Change direction!!!
        public GameplayChangeTarget ChangeTarget = null;
        public GameplayChangeWeapon ChangeWeapon = null;

        // use for lists serialized by gameplay events where each event is one entity
        public GameplayEvent Event = null;
        public GameplayObstacle Obstacle = null;
        
        // use for lists serialized by timing events where each event is one time
        public List<TimingEvent> AdditionalTimingEvents = null;
        public List<GameplayEvent> Events = null;
        public List<GameplayObstacle> Obstacles = null;
        
        public double StartTime {
            get {
                return Phrase.StartTime + TimingEvent.startTime;
            }
        }

        public double FlyTime {
            get {
                return Phrase.TimePerBar;
            }
        }

        public double TimePerBeat {
            get {
                return Phrase.TimePerBeat;
            }
        }

        public WeaponInteraction WeaponInteraction {
            get {
                if (Event.pickupWith != Appendage.Any // Any => use dominant 
                    && GameplayPattern.dominantHand != Event.pickupWith) {
                    return GameplayPattern.weaponInteractionNonDominant;
                } else {
                    return GameplayPattern.weaponInteractionDominant;
                }
            }
            set {
                if (Event.pickupWith != Appendage.Any // Any => use dominant 
                    && GameplayPattern.dominantHand != Event.pickupWith) {
                    GameplayPattern.weaponInteractionNonDominant = value;
                } else {
                    GameplayPattern.weaponInteractionDominant = value;
                }
            }
        }
    }
    
}
