﻿#region Copyright and License Information
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

    public enum Mechanic : int {
        MultiMechanic = -1, // ADDED - used for tracks that have multiple mechanics
        Catch = 0,        // fist, only hands (because requires "catch" gesture)  
        PunchFlying = 1,  // fists/feet/head => "classic"
        PunchInplace = 2, // fists/feet/head
        Slice = 3,        // sword, laser blade; only hands
        ShootGun = 4,     // immediate reaction: bullets, lasers; only hands
        ShootCharged = 5, // immediate reaction, but first needs to "charge" (like an energy gun)
        ShootArrow = 6,   // delayed reaction, physics: archery/bow and arrow; requires both hands
        ShootMagic = 7    // delayed reaction, needs charge, can use left/right hands
        
        // ShootCharged can also be used for "magic hands" - in that case,
        // closing the hand charges, opening the hand hits immediately
        
        // all except ShootCharged and ShootArrow can handle held notes
    }

}
