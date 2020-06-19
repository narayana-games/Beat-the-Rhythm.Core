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

namespace NarayanaGames.BeatTheRhythm.Maps.Tracks {

    /// <summary>
    ///     Represents a gameplay obstacle.
    /// </summary>
    [Serializable]
    public class GameplayObstacle {

        /// <summary>
        ///     Links this obstacle to a rhythm event.
        /// </summary>
        public int timingEventId = 0;

        /// <summary>The area covered at the beginning of this obstacle.</summary>
        public ObstacleArea areaBegin;

        /// <summary>The area covered at the end of this obstacle.</summary>
        public ObstacleArea areaEnd;
    }
    
    [Serializable]
    public class ObstacleArea {
        public Vector3Float topLeftPos;
        public Vector3Int topLeftRasterPos;
        public Vector3Float bottomRightPos;
        public Vector3Int bottomRightRasterPos;
    }
    
}
