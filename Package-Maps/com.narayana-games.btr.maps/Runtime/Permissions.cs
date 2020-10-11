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

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     All the information relevant for access to mapping items: Owner, 
    ///     permissions and whether the item has been locked.
    /// </summary>
    [Serializable]
    public class Permissions {

        [Serializable]
        public enum Visibility : int {
            Authors,
            Friends,
            Public
        }
        
        /// <summary>
        ///     Owner of this specific item. This is the person that has
        ///     created this version of the map. Ownership will only be
        ///     officially recognized upon map ranking when the author has
        ///     contributed at least 50% of the content of this map.
        /// </summary>
        public string ownerUniquePlayerId = null;

        /// <summary>
        ///     A list of all authors that have contributed to any part of
        ///     this map. This is included here so we only need the player ids
        ///     in all nested levels. This list must always include owner.
        /// </summary>
        public List<string> authorsUniquePlayerIds = new List<string>();

        /// <summary>
        ///     Who can see this item?
        /// </summary>
        public Visibility visibility = Visibility.Authors;
        
        /// <summary>
        ///     Does the owner allow re-using the contents of this item?
        /// </summary>
        public bool ownerAllowsReuse = true;

        /// <summary>
        ///     Does the owner allow modifying the contents of this item
        ///     (only applies to new/copied instances)?
        /// </summary>
        public bool ownerAllowsModding = true;

        /// <summary>
        ///     SongStructures and MapContainers must be locked before they can
        ///     be used to build gameplay (SongStructures), or record actual
        ///     play sessions (MapContainers). When an item has been locked,
        ///     it cannot be unlocked, so use with care!
        /// </summary>
        public bool isLocked = false;

        public void AddAuthor(string playerId) {
            if (string.IsNullOrEmpty(playerId)) {
                return;
            }
            
            if (string.IsNullOrEmpty(ownerUniquePlayerId)) {
                ownerUniquePlayerId = playerId;
            }

            if (!authorsUniquePlayerIds.Contains(playerId)) {
                authorsUniquePlayerIds.Add(playerId);
            }
        }
    }
}
