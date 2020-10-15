using UnityEngine;

using NarayanaGames.Common.Audio;

namespace NarayanaGames.Common.Audio {

    public static class GameplayTime {

        public static bool UseGlobalTime = false;
        private static MultiTrackAudioSource timingAudioSource = null;

        private static bool pauseSystems = false;
        public static bool PauseSystems {
            get => pauseSystems;
            set {
                Debug.Log($"PauseSystems {pauseSystems} => {value}");
                pauseSystems = value;
            }
        }
        
        private static LoopedTime lastLoopedTime = new LoopedTime();
        
        public static LoopedTime ElapsedTime {
            get {
                lastLoopedTime.CurrentStartTime = lastLoopedTime.NextStartTime = 0;
                
                if (timingAudioSource != null) {
                    lastLoopedTime.Time = timingAudioSource.TimePrecise;
                    bool noLoop = false;
                    if (timingAudioSource.IsPreRolling && timingAudioSource.CurrentSegment != null) {
                        lastLoopedTime.Time = timingAudioSource.CurrentSegment.StartTime + timingAudioSource.TimePrecise;
                        noLoop = true;
                    }
                    if (!noLoop && timingAudioSource.LoopCurrentSegment && timingAudioSource.CurrentLoopedSegment != null) {
                        lastLoopedTime.CurrentStartTime = lastLoopedTime.NextStartTime 
                            = timingAudioSource.CurrentLoopedSegment.StartTime;

                        lastLoopedTime.CurrentEndTime = lastLoopedTime.NextEndTime 
                            = timingAudioSource.CurrentLoopedSegment.EndTime;
                        
                        lastLoopedTime.CurrentDuration = lastLoopedTime.NextDuration 
                            = timingAudioSource.CurrentLoopedSegment.DurationSeconds;
                        
                    } else { // TODO: Skip segment
                        lastLoopedTime.CurrentEndTime = lastLoopedTime.NextEndTime =
                        lastLoopedTime.CurrentDuration = lastLoopedTime.NextDuration 
                            = timingAudioSource.Length;
                    }
                } else {
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
