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


namespace NarayanaGames.BeatTheRhythm.Maps.Enums {

    [System.Serializable] // this is currently still part of another package
    public enum GameMechanic : int {
        MultiMechanic = -1, // ADDED - used for tracks that have multiple mechanics
        CatchersCasual = 0,
        Catchers = 1,
        Laserblade = 2, // single hand
        Laserblades = 3,
        Gun = 4, // single hand
        Guns = 5,
        BowAndArrow = 6 // single hand
    }


    [System.Serializable] // this is currently still part of another package
    public enum TrackedAppendages : int {
        SingleHand = 0,       // SH
        TwoHands = 1,         // TH
        HeadAndHands = 2,     // HAH
        HeadHandsAndFeet = 3, // HHAF
        HandsAndFeet = 4,     // HAF
        TwoFeet = 5,          // TF
        SingleFoot = 6,       // SF
        HeadOnly = 7          // HO
    }

    [System.Serializable] // this is currently still part of another package
    public enum PickupType {
        Any = 0, // catch it however you like; also used when not caught
        Left = 1,
        Right = 2,
        Head = 3,
        LeftFoot = 4, // when you have Vive trackers ...
        RightFoot = 5 // when you have Vive trackers ...
    }

    [System.Serializable]
    public enum TouchType {
        Orb = 0,
        Arc = 1, // aka "Slider"
        Random = 2, // this mechanic is currently not supported
        Spinner = 3 // added to have full osu! support
    }

}
