﻿#region Copyright and License Information
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
using UnityEngine;

namespace NarayanaGames.BeatTheRhythm.Maps.Tracks {

    /// <summary>
    ///     Represents a gameplay event. Positions are stored in two
    ///     representations in parallel: A normalized, and a rasterized
    ///     position. The rasterized position is primarily used to make
    ///     it easy to recognize similar patterns, while the normalized
    ///     position is there to fine-tune the flow in a given pattern.
    /// </summary>
    [Serializable]
    public class GameplayEvent {

        /// <summary>
        ///     Links this gameplay event to a rhythm event. Be aware that
        ///     you can skip IDs to skip "beats" (e.g. if there are too
        ///     many rhythmic events for the difficulty you're working on,
        ///     or if you want a pattern that requires more time to be
        ///     more distributed), and you can also assign multiple gameplay
        ///     events to a single timing event when they should arrive
        ///     at the exact same time, e.g. one for left, one for right,
        ///     or even multiple for one hand. 
        /// </summary>
        public int timingEventId = 0;

        /// <summary>
        ///     Normalized position with floating point coordinates from
        ///     -1 to +1 on X, with 0 being the center, and from -2 to +1
        ///     on Y (with values between -2 and -1 primarily reserved for
        ///     feet gameplay).
        /// </summary>
        public Vector3Float pos;
        
        /// <summary>
        ///     Rasterized position represented by integers from -3 to +3 on
        ///     X, which allows seven "half steps" from left to right, or four
        ///     full positions (-3, -1, +1, +3).
        ///     For Y, the rasterized position is from -8 to +2, with values
        ///     from -8 to -3 reserved for the feet (-8, -6, -4 being the full
        ///     steps), and -2, 0, +2 being the full steps for hands.
        /// </summary>
        public Vector3Int rasterPos;

        /// <summary>Is this a directional gameplay event?</summary>
        public bool hasDirection = false;

        /// <summary>
        ///     If hasDirection, the direction in degrees.
        ///     0 is down, 180 is up, -90 is left, 90 is right.
        /// </summary>
        public float direction = 0;
        //public bool noDirectionFail; // TBD: do we need this?
        //public bool isRotationEvent; // Should probably go into its own type, e.g. GameplayChangeSourceRotation

        /// <summary>With which appendage.</summary>
        public Appendage pickupWith = Appendage.Any;

        /// <summary>What is the mechanic of this event?</summary>
        public GameplayEventType eventType = GameplayEventType.Beat;
        
        /// <summary>
        ///     Path for GameplayEventType.Trace (time is always from 0 to 1, actual
        ///     steps are interpolated based on the event duration).
        /// </summary>
        public List<PositionAtTime> path = new List<PositionAtTime>();

        public GameplayEvent Copy() {
            return new GameplayEvent() {
                timingEventId = timingEventId,
                pos = pos.Copy(),
                rasterPos = rasterPos.Copy(),
                hasDirection = hasDirection,
                direction = direction,
                pickupWith =  pickupWith,
                eventType = eventType,
                path = CopyPath()
            };
        }

        private List<PositionAtTime> CopyPath() {
            List<PositionAtTime> copiedPath = new List<PositionAtTime>(path.Count);
            for (int i = 0; i < path.Count; i++) {
                copiedPath.Add(path[i].Copy());
            }
            return copiedPath;
        }
    }

    [Serializable]
    public class Vector3Float {
        public float x = 0;
        public float y = 0;
        public float z = 0;
        
        public Vector3Int ToInt() {
            Vector3Int result = new Vector3Int();
            result.x = (int)Math.Round(x*3F);
            result.y = (int)Math.Round(y*2F);
            result.z = (int)Math.Round(z*3F);
            return result;
        }

        public Vector3Float Copy() => new Vector3Float() { x = x, y = y, z = z };
        
        public static implicit operator Vector3(Vector3Float v) => new Vector3(v.x, v.y, v.z);
        public static implicit operator Vector3Float(Vector3 v) => new Vector3Float(){ x = v.x, y = v.y, z = v.z };
        
        public static implicit operator Vector2(Vector3Float v) => new Vector2(v.x, v.y);
        public static implicit operator Vector3Float(Vector2 v) => new Vector3Float(){ x = v.x, y = v.y, z = 0 };
    }

    [Serializable]
    public class PositionAtTime {
        /// <summary>Time between 0 and 1.</summary>
        public float time = 0;
        /// <summary>Position relative to event pos/rasterPos.</summary>
        public Vector3Float position;

        public PositionAtTime Copy() => new PositionAtTime(){ time = time, position = position.Copy() }; 
    }
    
    [Serializable]
    public class Vector3Int {
        public int x = 0;
        public int y = 0;
        public int z = 0;

        public Vector3Float ToFloat() {
            Vector3Float result = new Vector3Float();
            result.x = ((float)x) / 3F;
            result.y = ((float)y) / 2F;
            result.z = ((float)z) / 3F;
            return result;
        }

        public Vector3Int Copy() => new Vector3Int() { x = x, y = y, z = z };
        
        public static implicit operator Vector3Float(Vector3Int v) => v.ToFloat();
        public static implicit operator Vector3Int(Vector3Float v) => v.ToInt();

        public override string ToString() {
            return $"[{x}, {y}, {z}]";
        }
    }
    
}
