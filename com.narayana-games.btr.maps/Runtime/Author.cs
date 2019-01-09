#region Copyright and License Information
/*
 * Copyright (c) 2015-2019 narayana games UG.  All Rights Reserved.
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

namespace NarayanaGames.BeatTheRhythm.Maps {

    /// <summary>
    ///     An author is a person that creates or modifies a map. Usually, 
    ///     authors are also players but the system also supports external
    ///     authors which otherwise don't exist inside our system.
    /// </summary>
    [Serializable]
    public class Author {
        /// <summary>
        ///     Reference to Player.uniquePlayerId, or if the original map was 
        ///     imported (e.g. from osu!), and external player id with a prefix.
        /// </summary>
        public string uniquePlayerId;

        /// <summary>
        ///     Redundant with Player.nickname. Stored here for sake of 
        ///     simplicity and to be able to keep track of external authors 
        ///     that are not in our system.
        /// </summary>
        public string nickname;
    }

}
