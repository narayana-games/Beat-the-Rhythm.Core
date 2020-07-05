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
    ///     A special event that changes the target that needs to be
    ///     caught, punched, sliced, shot, whatever.
    /// </summary>
    [Serializable]
    public class GameplayChangeTarget {

        /// <summary>Links this change target event to a timing event.</summary>
        public int timingEventId = 0;

        /// <summary>For which appendage does this change apply!</summary>
        public Appendage pickupWith = Appendage.BothHands;
        
        /// <summary>
        ///     A unique string that defines which prefab should be used.
        ///     The lookup order is: In the folder where the beatmap resides
        ///     (so a beatmap can override the default target prefabs),
        ///     in the active target mod folder (so mods can override the
        ///     targets) and finally, in the default game folder. 
        /// </summary>
        public string targetPrefab = string.Empty;
    }
}
