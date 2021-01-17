using UnityEngine;

using NarayanaGames.Common.Audio;

namespace NarayanaGames.Common.Audio {

    public static class GameplayTime {

        public static bool UseGlobalTime = false;
        public static bool IsLooping = false; // only used for testing!
        private static MultiTrackAudioSource timingAudioSource = null;

        private static bool pauseSystems = false;
        public static bool PauseSystems {
            get => pauseSystems;
            set {
                Debug.Log($"PauseSystems {pauseSystems} => {value}");
                pauseSystems = value;
            }
        }

        public static bool IsShortLoop {
            get {
                return lastLoopedTime.IsLooping
                       && timingAudioSource.CurrentLoopedSegment != null
                       && timingAudioSource.CurrentLoopedSegment.DurationBars == 1;
            }
        }
        
        private static LoopedTime lastLoopedTime = new LoopedTime();
        
        public static LoopedTime ElapsedTime {
            get {
                lastLoopedTime.CurrentStartTime = lastLoopedTime.NextStartTime = 0;
                
                if (timingAudioSource != null) {
                    lastLoopedTime.IsLooping = timingAudioSource.LoopSegment;
                    lastLoopedTime.Time = (float) timingAudioSource.TimePrecise;
                    lastLoopedTime.Pitch = timingAudioSource.Pitch;
                    bool noLoop = false;
                    if (timingAudioSource.IsPreRolling && timingAudioSource.CurrentSegment != null) {
                        lastLoopedTime.Time = (float) (timingAudioSource.CurrentSegment.StartTime + timingAudioSource.TimePrecise);
                        noLoop = true;
                    }
                    if (!noLoop && timingAudioSource.LoopSegment) {
                        lastLoopedTime.CurrentStartTime = lastLoopedTime.NextStartTime 
                            = (float) timingAudioSource.CurrentLoopedSegment.StartTime;

                        lastLoopedTime.CurrentEndTime = lastLoopedTime.NextEndTime 
                            = (float) timingAudioSource.CurrentLoopedSegment.EndTime;
                        
                        lastLoopedTime.CurrentDuration = lastLoopedTime.NextDuration 
                            = (float) timingAudioSource.CurrentLoopedSegment.DurationSeconds;
                        
                    } else { // TODO: Skip segment
                        lastLoopedTime.CurrentEndTime = lastLoopedTime.NextEndTime =
                        lastLoopedTime.CurrentDuration = lastLoopedTime.NextDuration 
                            = timingAudioSource.Length;
                    }
                } else {
                    lastLoopedTime.IsLooping = IsLooping;
                    
                    lastLoopedTime.Time = UseGlobalTime ? Time.realtimeSinceStartup : -10F;
                    
                    lastLoopedTime.CurrentEndTime = lastLoopedTime.NextEndTime 
                        = UseGlobalTime ? Time.realtimeSinceStartup + 100F : 100F;

                    lastLoopedTime.CurrentDuration = lastLoopedTime.NextDuration
                        = lastLoopedTime.CurrentEndTime - lastLoopedTime.CurrentStartTime;
                }
                
                return lastLoopedTime;
            }
        }
        
        public static MultiTrackAudioSource TimingSource {
            get => timingAudioSource;
            set {
                timingAudioSource = value;
                // if (timingAudioSource != null) {
                //     Debug.Log($"<b>[GameplayTime]</b> Set to audio source. {timingAudioSource.name}!", timingAudioSource);
                // }
            }
        }
    }
}
