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
    ///     A special event where the player needs to change the weapon.
    /// </summary>
    [Serializable]
    public class GameplayChangeWeapon {

        /// <summary>Links this gameplay change weapon event to a timing event.</summary>
        public int timingEventId = 0;

        /// <summary>With which appendage - only Left, Right and Any (for Archery)!</summary>
        public Appendage pickupWith = Appendage.Any;

        /// <summary>The weapon to change to: Catcher, Laserblade, Gun or BowAndArrow.</summary>
        public WeaponType weapon = WeaponType.Catcher;
    }
}
