//#define OCULUS_OSP
//#define HAS_DEPENDENCIES

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NarayanaGames.Common.UtilityBehaviours;
using System.Text;
using NarayanaGames.BeatTheRhythm.Maps;

namespace NarayanaGames.Common.Audio {
    /// <summary>
    ///     An audio source that consists of multiple tracks that need to
    ///     be played simultanuously and mixed to create a full song. This
    ///     could be used for ambiences but it's most useful for multitrack
    ///     songs where you have one audio file for each track (e.g. drums,
    ///     bassline, melody and so on).
    /// </summary>
    public class MultiTrackAudioSource : MonoBehaviour {

        public bool doPreload = false;
        public bool disableUnloading = false;

        public float skipIntoSeconds = 0;

        public float skipIntoSecondsForFilming = 0;
        public float skipOutSecondsForFilming = 0;

        /// <summary>
        ///     The actual length of this track in seconds. The reason we
        ///     need this is because sometimes, tracks are longer than the
        ///     music actually plays (e.g. if there's some reverb or echo
        ///     still fading out when the music has stopped).
        /// </summary>
        public float actualLength = 0;

        public float Length {
            get {
                if (individualTracks == null || individualTracks.Count < 1 || individualTracks[0].clip == null) {
                    return 0;
                }
                return individualTracks[0].clip.length;
            }
        }

        /// <summary>
        ///     Can be used to stop the game after seconds for profiling / debugging
        ///     purposes. Set to 0 for production or actual playing ;-)
        /// </summary>
        public float debugBreakAfterSeconds = 0;
        private bool didBreak = false;

#if OCULUS_OSP
        public List<OSPAudioSource> individualTracksOSP = new List<OSPAudioSource>();
#endif

        /// <summary>
        ///     The individual tracks of this multitrack audio source. The
        ///     first AudioSource in this list is used as master and drives
        ///     the others.
        /// </summary>
        public List<AudioSource> individualTracks = new List<AudioSource>();

        /// <summary>
        ///     Gets the current list of individual tracks; this alterates 
        ///     when we're looping segments.
        /// </summary>
        public List<AudioSource> IndividualTracks {
            get {
                return !isAlternateTracks || !IsLoopReady
                    ? individualTracks 
                    : individualTracksLoop;
            }
        }

        /// <summary>
        ///     Gets the other list, which is currently not playing but
        ///     scheduled for playing on the next switch.
        /// </summary>
        public List<AudioSource> IndividualTracksScheduled {
            get {
                return !isAlternateTracks || !IsLoopReady
                    ? individualTracksLoop
                    : individualTracks;
            }
        }

        private List<AudioSource> individualTracksLoop = new List<AudioSource>();

        private bool IsLoopReady {
            get { return individualTracks.Count == individualTracksLoop.Count; }
        }

        private bool isAlternateTracks = false;
        private bool isNextScheduled = false;
        private double nextStartTime = double.MaxValue;
        private double nextSwitchTime = double.MaxValue;

        private SongSegment currentLoopedSegment = null;
        public SongSegment CurrentLoopedSegment {
            get { return currentLoopedSegment; }
            set {
                if (currentLoopedSegment != value) {
                    //Debug.LogFormat("Setting looped segment from '{0}' to '{1}'", currentLoopedSegment, value);
                    currentLoopedSegment = value;
                    RescheduleLoop();
                }
            }
        }

        private void RescheduleLoop() {
            if (IsPlaying && LoopCurrentSegment) {
                if (CurrentLoopedSegment != null) {
                    if (isNextScheduled) {
                        //Debug.LogFormat("Rescheduling loop for {0}", CurrentLoopedSegment.name);
                        if (UpdateNextStartTime()) {
                            UpdateScheduledAlternate(nextStartTime, CurrentLoopedSegment.StartTime);
                            SetScheduledEndTime(nextStartTime);
                            nextSwitchTime = nextStartTime;
                            nextStartTime += CurrentLoopedSegment.DurationSeconds;
                        } else {
                            isNextScheduled = false;
                            nextSwitchTime = double.MaxValue;
                            nextStartTime = double.MaxValue;
                            foreach (AudioSource audioSource in IndividualTracksScheduled) {
                                if (audioSource.isActiveAndEnabled) {
                                    audioSource.Stop();
                                }
                            }
                        }
                    } 
                //    else {
                //        Debug.LogFormat("NOT Rescheduling loop for {0}", CurrentLoopedSegment.name);
                //    }
                //} else {
                //    Debug.LogFormat("NOT Rescheduling loop BECAUSE no segment was selected");
                }
            }
        }

        private bool loopCurrentSegment = false;
        public bool LoopCurrentSegment {
            get { return loopCurrentSegment; }
            set {
                loopCurrentSegment = value;
                if (loopCurrentSegment && CurrentLoopedSegment != null) {
                    if (!IsLoopReady) {
                        InstantiateLoopSources();
                    }
                    UpdateNextStartTime();
                } else {
                    nextStartTime = double.MaxValue;
                    nextSwitchTime = double.MaxValue;
                }
            }
        }

        private bool UpdateNextStartTime() {
            if (LoopCurrentSegment && CurrentLoopedSegment != null) {
                //Debug.LogFormat("Setting nextStartTime from {0} to {1}", nextSwitchTime, AudioSettings.dspTime + (CurrentLoopedSegment.EndTime - TimePrecise));
                nextStartTime = AudioSettings.dspTime + (CurrentLoopedSegment.EndTime - TimePrecise);
            }
            return CurrentLoopedSegment.EndTime - TimePrecise > 0F;
        }

        private void CheckLoop() {
            if (IsPlaying && LoopCurrentSegment && CurrentLoopedSegment != null) {
                double time = AudioSettings.dspTime;
                if (!isNextScheduled && time + 0.3f > nextStartTime) {
                    //Debug.LogFormat("Scheduling loop for '{0}' at {1} ({2})", CurrentLoopedSegment.name, nextStartTime, CurrentLoopedSegment.StartTime);
                    isNextScheduled = true;
                    PlayScheduledAlternate(nextStartTime, CurrentLoopedSegment.StartTime);
                    SetScheduledEndTime(nextStartTime);
                    nextSwitchTime = nextStartTime;
                    nextStartTime += CurrentLoopedSegment.DurationSeconds;
                }
                if (isNextScheduled && time >= nextSwitchTime) {
                    //Debug.LogFormat("Switching at {0} (time = {1})", nextSwitchTime, time);
                    isNextScheduled = false;
                    nextSwitchTime = nextStartTime;
                    isAlternateTracks = !isAlternateTracks;
                }
            }
        }

        private void InstantiateLoopSources() {
            // remove any old audio sources
            for (int i = individualTracksLoop.Count - 1; i >= 0; i--) {
                Destroy(individualTracksLoop[i].gameObject);
            }
            individualTracksLoop.Clear();

            foreach (AudioSource source in individualTracks) {
                AudioSource copy = Instantiate(source, source.transform, true);
                individualTracksLoop.Add(copy);

                // remove any children this might have
                for (int i = copy.transform.childCount - 1; i >= 0; i--) {
                    Destroy(copy.transform.GetChild(i).gameObject);
                }

                // we keep other components ... just in case
            }
        }




        /// <summary>
        ///     Overrides playOnAwake of all AudioSources during Awake().
        /// </summary>
        public bool playOnAwake = true;

        /// <summary>
        ///     Overrides loop of all AudioSources during Awake().
        /// </summary>
        public bool loop = false;

        /// <summary>
        ///     Overrides dopplerLevel of all AudioSources during Awake().
        /// </summary>
        public float dopplerLevel = 1;

        /// <summary>
        ///     Overrides priority of all AudioSources during Awake().
        /// </summary>
        public int priority = 128;

        /// <summary>
        ///     Overrides velocityUpdateMode of all AudioSources during Awake().
        /// </summary>
        public AudioVelocityUpdateMode velocityUpdateMode = AudioVelocityUpdateMode.Auto;

        private bool isStopped = true;

        private int SkipIntoSeconds {
            get { return GlobalGameState.SkipIntoSeconds; }
        }

        private bool IsGamePaused {
            get { return GlobalGameState.IsPaused; }
        }


        /// <summary>
        ///     Adds child audio sources, if individualTracks has no audio
        ///     sources assigned. Writes playOnAwake, loop, dopplerLevel,
        ///     priority and velocityUpdateMode to all individualTracks.
        /// </summary>
        public void Awake() {
            if (individualTracks.Count == 0) {
                individualTracks.AddRange(GetComponentsInChildren<AudioSource>(true));
            }
#if OCULUS_OSP
            if (individualTracksOSP.Count == 0) {
                individualTracksOSP.AddRange(GetComponentsInChildren<OSPAudioSource>());
            }
#endif

            if (individualTracks.Count == 0) {
                Debug.LogWarningFormat(this, "MultiTrackAudioSource '{0}' has no AudioSources assigned and no children with AudioSources; will disable!", this.name);
                this.enabled = false;
                return;
            }

            if (actualLength == 0) {
                actualLength = Length;
            }

            foreach (AudioSource audioSource in individualTracks) {
                audioSource.playOnAwake = false; // this must be handled differently!
#if OCULUS_OSP
                audioSource.playOnAwake = false; // don't ever when using OSP!
#endif
                audioSource.loop = loop;
                audioSource.dopplerLevel = dopplerLevel;
                audioSource.priority = priority;
                audioSource.velocityUpdateMode = velocityUpdateMode;
            }
#if OCULUS_OSP
            foreach (OSPAudioSource audioSource in individualTracksOSP) {
                audioSource.PlayOnAwake = playOnAwake;
            }
#endif
        }

        public void Update() {
            CheckLoop();
            if (!isStopped && !IsGamePaused) {
                isStopped = !IndividualTracks[0].isPlaying;
                if (!didBreak && debugBreakAfterSeconds > 0) {
                    if (IndividualTracks[0].time > debugBreakAfterSeconds) {
                        didBreak = true;
                        Debug.LogFormat(this, "{0} did debug break after {1:0.000} seconds", this.name, IndividualTracks[0].time);
                        Debug.Break();
                    }
                }
            }
        }

        public void OnEnable() {
            if (!IsPlaying && playOnAwake) {
                PlayWhenReady();
            } else if (doPreload) {
                LoadAudio();
            }
        }

        public void OnDisable() {
            //Debug.LogFormat(this, "MultiTrackAudioSource - OnDisable called for '{0}'", this.name);
            UnloadAudioData();
        }

        public void UnloadAudioData() {
            if (!IsPlaying && !disableUnloading) {
                UnloadTracks(individualTracks);
                if (IsLoopReady) {
                    UnloadTracks(individualTracksLoop);
                }
            } else {
                Debug.LogWarningFormat(this, "Not unloading audio data because we're still playing ({0}) or disableUnloading ({1})", IsPlaying, disableUnloading);
            }
        }



        private void UnloadTracks(List<AudioSource> audioSources) {
            //StringBuilder unloadList = new StringBuilder();
            //unloadList.AppendFormat("Unloading audio clips for {0}", this.name).AppendLine();
            for (int i = 0; i < audioSources.Count; i++) {
                AudioSource src = audioSources[i];
                if (src.clip != null) {
                    if (src.clip.loadState == AudioDataLoadState.Loaded) {
                        src.clip.UnloadAudioData();
                        hasLoadedAudio = false;
                        //unloadList.AppendFormat("Unloading {0}", src.name).AppendLine();
                    }
                }
            }
            //Debug.LogFormat(this, "[{0}] Unloaded: {1}", System.DateTime.Now.ToString("HH:mm:ss.fff"), unloadList.ToString());
        }

        private bool isLoading = false;
        public bool IsLoading {
            get { return isLoading; }
        }

        private bool hasLoadedAudio = false;
        public bool HasLoadedAudio {
            get { return hasLoadedAudio; }
        }

        public void AssignClip(AudioClip clip) {
            individualTracks[0].clip = clip;
            if (IsLoopReady) {
                individualTracksLoop[0].clip = clip;
            }
            hasLoadedAudio = true;
            disableUnloading = true;
            //Debug.LogFormat("[{0}] Assigned audio clip: {1}", clip.name, System.DateTime.Now.ToString("HH:mm:ss.fff"));
        }

        public void PlayWhenReady() {
            //Debug.LogFormat(this, "{0} is playing when ready", this.name);
            StartCoroutine(LoadAudioCo(true));
        }

        public void LoadAudio() {
            StartCoroutine(LoadAudioCo(false));
        }

        public IEnumerator LoadAudioCo(bool doPlay) {

            if (individualTracks.Count < 1 || individualTracks[0] == null || individualTracks[0].clip == null) {
                Debug.LogFormat(this, "{0}: No audio assigned, not preloading anything for now!", this.name);
                yield break;
            }

            Awake();

            bool allReady = true;
            bool loggedLoadingError = false;
            float timeStarted = UnityEngine.Time.realtimeSinceStartup;

            while (isLoading && !hasLoadedAudio) {
                yield return null;
            }

            isLoading = true;
            StringBuilder loadingTimes = new StringBuilder();
            float timePassed = UnityEngine.Time.realtimeSinceStartup - timeStarted;

            //Debug.LogFormat("[{0}] MultiTrackAudioSource.PlayWhenReadyCo({1}), hasLoadedAudio={2}", System.DateTime.Now.ToString("HH:mm:ss.fff"), doPlay, hasLoadedAudio);

            if (!hasLoadedAudio) {
                try {
                    bool[] clipsLoaded = new bool[individualTracks.Count];
                    for (int i = 0; i < clipsLoaded.Length; i++) {
                        clipsLoaded[i] = false;
                    }

                    do {
                        //Debug.LogFormat("All ready?");
                        allReady = true;
                        for (int i = 0; i < individualTracks.Count; i++) {
                            AudioSource src = individualTracks[i];
                            if (src == null || src.clip == null) {
                                Debug.LogErrorFormat(this, "{0}: Track {1}/{2} is null!", this.name, i, individualTracks.Count - 1);
                                continue;
                            }
                            if (src.clip.loadState != AudioDataLoadState.Loaded) {
                                if (src.clip.loadState != AudioDataLoadState.Loading && src.clip.loadState != AudioDataLoadState.Failed) {
                                    src.clip.LoadAudioData();
                                    loadingTimes.AppendFormat("{1:0.00}s: Started loading {0} ", src.name, timePassed).AppendLine();
                                }
                                allReady = false;
                            } else if (!clipsLoaded[i]) {
                                clipsLoaded[i] = true;
                                loadingTimes.AppendFormat("{2:0.00}s: Finished loading {0} ({1})", src.name, src.clip.name, timePassed).AppendLine();
                            }
                        }
                        if (!allReady && timePassed > 5F && !loggedLoadingError) {
                            loggedLoadingError = true;
                            StringBuilder msg = new StringBuilder();
                            msg.AppendFormat("{1:0.00}s: Loading audio sources for {0} is taking very long!", this.name, timePassed).AppendLine();
                            int loadedCount = 0;
                            int failedCount = 0;
                            foreach (AudioSource src in individualTracks) {
                                if (src.clip.loadState != AudioDataLoadState.Loaded) {
                                    msg.AppendFormat("'{0}' has loadState: {1}", src.name, src.clip.loadState).AppendLine();
                                } else if (src.clip.loadState != AudioDataLoadState.Failed) { failedCount++; } else if (src.clip.loadState == AudioDataLoadState.Loaded) { loadedCount++; }
                            }
                            msg.AppendFormat("{0} loaded, {1} failed", loadedCount, failedCount);
                            Debug.LogErrorFormat(this, msg.ToString());
                        }
                        //Debug.LogFormat("... now waiting ... Time.timeScale = {0}", UnityEngine.Time.timeScale);
                        yield return null;
                        //yield return new WaitForEndOfFrame();
                        timePassed = UnityEngine.Time.realtimeSinceStartup - timeStarted;
                        //Debug.LogFormat("Still trying to load");
                    } while (!allReady && timePassed < 20F);
                } finally {
                    isLoading = false;
                }
                yield return null;
            } else {
                allReady = true;
                isLoading = false;
                loadingTimes.Append("Audio had previously been loaded.").AppendLine();
            }

            if (IsLoopReady) {
                for (int i=0; i < individualTracks.Count; i++) {
                    individualTracksLoop[i].clip = individualTracks[i].clip;
                }
            }

            //Debug.LogFormat("Loading completed");

            if (allReady) {
                if (doPlay) {
                    Play();
                    loadingTimes.Insert(0, string.Format("{0} is now ready and playing after {1:0.00} seconds{2}", this.name, timePassed, System.Environment.NewLine));
                } else {
                    loadingTimes.Insert(0, string.Format("{0} is now ready and waiting to start playing after {1:0.00} seconds{2}", this.name, timePassed, System.Environment.NewLine));
                }

                loadingTimes.Insert(0, string.Format("[{0}] ", System.DateTime.Now.ToString("HH:mm:ss.fff")));

                hasLoadedAudio = true;

                Debug.LogFormat(this, loadingTimes.ToString());
            } else {
                StringBuilder msg = new StringBuilder();
                msg.AppendFormat("{0} did not successfully load all audio clips after {1:0.00} seconds", this.name, timePassed);
                foreach (AudioSource src in individualTracks) {
                    if (src.clip.loadState != AudioDataLoadState.Loaded) {
                        msg.AppendFormat("Failed to load audio source '{0}', clip '{1}', state: {3}", src.name, src.clip.name, src.clip.loadState).AppendLine();
                    } else {
                        msg.AppendFormat("Did successfully load audio source '{0}', clip '{1}', state: {2}", src.name, src.clip.name, src.clip.loadState).AppendLine();
                    }
                }
                Debug.LogErrorFormat(this, msg.ToString());
#if HAS_DEPENDENCIES
                SimpleLogger.Log(msg.ToString());
#endif
            }
        }

        /// <summary>
        ///     Is this multitrack audio source currently playing? Uses
        ///     the master track (first track in the list).
        /// </summary>
        /// <remarks>
        ///     This intentionally uses PascalCase (upper-case first letter)
        ///     because that's how properties should be spelled, even if
        ///     AudioSource uses camelCase (lower-case first letter).
        /// </remarks>
        public bool IsPlaying {
            get {
                if (CheckStartTime()) {
                    return true;
                }
                return IsValid 
                    ? IndividualTracks[0].isPlaying || (isNextScheduled && IndividualTracksScheduled[0].isPlaying)
                    : false;
            }
        }

        /// <summary>
        ///     Returns if the track is either at its end, or Stop was called.
        ///     This will still return true, if the track is playing but paused.
        /// </summary>
        public bool IsStopped {
            get { return isStopped || (!loop && Time > actualLength); }
        }

        public bool IsPaused { get; private set; }

        /// <summary>
        ///     Gets or sets whether this track is muted. Uses the master
        ///     track (first track in the list) to get but sets to all tracks.
        /// </summary>
        /// <remarks>
        ///     This intentionally uses PascalCase (upper-case first letter)
        ///     because that's how properties should be spelled, even if
        ///     AudioSource uses camelCase (lower-case first letter).
        /// </remarks>
        public bool Mute {
            get { return IsValid ? IndividualTracks[0].mute : false; }
            set {
                foreach (AudioSource audioSource in IndividualTracks) {
                    audioSource.mute = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the pitch. Uses the master
        ///     track (first track in the list) to get but sets to all tracks.
        /// </summary>
        /// <remarks>
        ///     This intentionally uses PascalCase (upper-case first letter)
        ///     because that's how properties should be spelled, even if
        ///     AudioSource uses camelCase (lower-case first letter).
        /// </remarks>
        public float Pitch {
            get { return IsValid ? IndividualTracks[0].pitch : 1F; }
            set {
                for (int i=0; i < IndividualTracks.Count; i++) {
                    if (IndividualTracks[i] != null) {
                        IndividualTracks[i].pitch = value;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets the current time of this multi track audio source.
        ///     Uses the master track (first track in the list) to get but sets
        ///     to all tracks. Be aware of potential limitations this has with
        ///     compressed audio (see Unity Scripting API AudioSource.time).
        /// </summary>
        /// <remarks>
        ///     This intentionally uses PascalCase (upper-case first letter)
        ///     because that's how properties should be spelled, even if
        ///     AudioSource uses camelCase (lower-case first letter).
        /// </remarks>
        public float Time {
            get {
                if (CheckStartTime()) {
                    return (float) PreRollTime;
                }
                return IsValid ? IndividualTracks[0].time : 0;
            }
            set {
                foreach (AudioSource audioSource in IndividualTracks) {
                    audioSource.time = value;
                }
            }
        }

        private bool CheckStartTime() {
            if (startTime > 0) {
                if (AudioSettings.dspTime > startTime) {
                    startTime = -1;
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        }

        private double PreRollTime {
            get { return AudioSettings.dspTime - startTime; }
        }

        /// <summary>
        ///     Gets or sets the current time of this multi track audio source.
        ///     Uses the master track (first track in the list) to get but sets
        ///     to all tracks.
        ///     IMPORTANT: This does not work with preroll - use TimePrecise
        ///     in that case instead!
        /// </summary>
        /// <remarks>
        ///     This intentionally uses PascalCase (upper-case first letter)
        ///     because that's how properties should be spelled, even if
        ///     AudioSource uses camelCase (lower-case first letter).
        /// </remarks>
        public int TimeSamples {
            get {
                return IsValid ? IndividualTracks[0].timeSamples : 0;
            }
            set {
                foreach (AudioSource audioSource in IndividualTracks) {
                    audioSource.timeSamples = value;
                }
            }
        }

        /// <summary>
        ///     This should get the precise time as double value representing seconds.
        /// </summary>
        public double TimePrecise {
            get {
                if (CheckStartTime()) {
                    return (float)PreRollTime;
                }
                if (IndividualTracks == null || IndividualTracks.Count == 0 || IndividualTracks[0].clip == null) {
                    return 0.0;
                }
                return IsValid ? SamplesToTime(IndividualTracks[0].clip, IndividualTracks[0].timeSamples) : 0.0;
            }
            set {
                //Debug.LogFormat("TimePrecise set from: {0} to {1}", TimePrecise, value);
                foreach (AudioSource audioSource in IndividualTracks) {
                    audioSource.timeSamples = TimeToSamples(IndividualTracks[0].clip, value);
                }
                RescheduleLoop();
            }
        }

        public double TimePreciseScheduled {
            get {
                if (CheckStartTime()) {
                    return (float)PreRollTime;
                }
                if (IndividualTracksScheduled == null || IndividualTracksScheduled.Count == 0 || IndividualTracksScheduled[0].clip == null) {
                    return 0.0;
                }
                return IsValid ? SamplesToTime(IndividualTracksScheduled[0].clip, IndividualTracksScheduled[0].timeSamples) : 0.0;
            }
        }


        public static int TimeToSamples(AudioClip clip, double time) {
            return (int)(time * clip.frequency);
        }

        public static double SamplesToTime(AudioClip clip, int timeSamples) {
            return ((double)timeSamples) / ((double)clip.frequency);
        }


        /* NOTE: Instead of using volume, you should usually use the AudioMixer!
         * If you have a use case for having volume, just let me know by sending
         * me an email to jashan@narayana-games.net and I can add it if you
         * convince me ;-)
         */

        /// <summary>
        ///     This one is used for multiplayer time synchronisation and called
        ///     by NBMultiTrackAudioSource.
        /// </summary>
        /// <param name="audioSourceId"></param>
        /// <param name="serverTimeWhenSent"></param>
        /// <param name="delayTimeMS"></param>
        public void SyncAudioTime(int audioSourceId, float serverTimeWhenSent, int delayTimeMS) {
            float currentTimeOnServer = serverTimeWhenSent + (delayTimeMS / 100F);

            if (IsValid) {
                isStopped = false;
                AudioSource audioSource = IndividualTracks[audioSourceId];
                if (audioSource.isActiveAndEnabled) {
                    if (!audioSource.isPlaying) {
                        audioSource.Play();
                    }
                    // use TimePrecise here instead of "time" (dspTime and friends ;-) )
                    audioSource.time = currentTimeOnServer;

#if HAS_DEPENDENCIES
                    if (GetComponentInChildren<NBMultiTrackAudioSource>().debugLogging) {
                        SimpleLogger.Log("Received SyncAudioTime for {0}: {1}", audioSource.name, currentTimeOnServer);
                        Debug.LogFormat("Received SyncAudioTime for {0}: {1}", audioSource.name, currentTimeOnServer);
                    }
#endif

                }
            }
        }

        /// <summary>
        ///     Plays the clips of all AudioSources of this multitrack audio source.
        /// </summary>
        public void Play() {
            Debug.LogFormat("Play was called, first audio clip: {0}", IndividualTracks[0].clip);
            if (IsValid) {
                isStopped = false;
                IsPaused = false;
#if !OCULUS_OSP
                foreach (AudioSource audioSource in IndividualTracks) {
                    if (audioSource.isActiveAndEnabled) {
                        audioSource.Play();
                        audioSource.timeSamples = 0;
                        if (skipIntoSeconds > 0) {
                            audioSource.time = skipIntoSeconds;
                            Debug.LogFormat("[{0}] Calling Play() on {1} - {2}, skipIntoSeconds: {3}",
                                System.DateTime.Now.ToString("HH:mm:ss.fff"), audioSource.name, audioSource.clip.name, skipIntoSeconds);
                        }
                        if (SkipIntoSeconds > 0) {
                            audioSource.time = SkipIntoSeconds;
                            Debug.LogFormat("[{0}] Calling Play() on {1} - {2}, AudioSettingsUI.SkipIntoSeconds: {3}",
                                System.DateTime.Now.ToString("HH:mm:ss.fff"), audioSource.name, audioSource.clip.name, SkipIntoSeconds);
                        }
                    }
                }
                //disableUnloading = false;
#else
                foreach (OSPAudioSource audioSource in individualTracksOSP) {
                    audioSource.Play();
                    audioSource.GetComponent<AudioSource>().timeSamples = 0;
                    //Debug.LogFormat("{0}.Play()", audioSource.name);
                }
#endif
            }
        }

        /// <summary>
        ///     Stops the clips of all AudioSources of this multitrack audio source.
        /// </summary>
        public void Stop() {
            isStopped = true;
            IsPaused = false;
            if (IsValid) {
#if !OCULUS_OSP
                foreach (AudioSource audioSource in individualTracks) {
                    if (audioSource.isActiveAndEnabled) {
                        //Debug.LogFormat("Stopped {0} - {1}", audioSource.name, audioSource.clip.name);
                        audioSource.Stop();
                    }
                }
                foreach (AudioSource audioSource in individualTracksLoop) {
                    if (audioSource.isActiveAndEnabled) {
                        //Debug.LogFormat("Stopped {0} - {1}", audioSource.name, audioSource.clip.name);
                        audioSource.Stop();
                    }
                }

#else
                foreach (OSPAudioSource audioSource in individualTracksOSP) {
                    audioSource.Stop();
                }
#endif
            }
        }


        /// <summary>
        ///     Pauses all AudioSources of this multitrack audio source.
        /// </summary>
        public void Pause() {
            if (IsValid) {
                IsPaused = true;
#if !OCULUS_OSP
                foreach (AudioSource audioSource in individualTracks) {
                    if (audioSource.isActiveAndEnabled) {
                        //Debug.LogFormat("Paused {0} - {1}", audioSource.name, audioSource.clip.name);
                        audioSource.Pause();
                    }
                }
                foreach (AudioSource audioSource in individualTracksLoop) {
                    if (audioSource.isActiveAndEnabled) {
                        //Debug.LogFormat("Stopped {0} - {1}", audioSource.name, audioSource.clip.name);
                        audioSource.Pause();
                    }
                }
#else
                foreach (OSPAudioSource audioSource in individualTracksOSP) {
                    audioSource.Pause();
                }
#endif
            }
        }

        /// <summary>
        ///     UnPauses all AudioSources of this multitrack audio source.
        /// </summary>
        public void UnPause() {
            if (IsValid) {
                IsPaused = false;
//#if !OCULUS_OSP
                int firstTrackTime = IndividualTracks[0].timeSamples;
                foreach (AudioSource audioSource in IndividualTracks) {
                    if (audioSource.isActiveAndEnabled) {
                        audioSource.UnPause();
                        audioSource.timeSamples = firstTrackTime;
                    }
                }
//#else // currently not supported by OSP
//                foreach (OSPAudioSource audioSource in individualTracksOSP) {
//                    audioSource.UnPause();
//                }
//#endif
            }
        }

        /// <summary>
        ///     Plays the clips of all AudioSources of this multitrack audio
        ///     source with a delay specified in seconds.
        /// </summary>
        public void PlayDelayed(float delaySeconds) {
            if (IsValid) {
                isStopped = false;
                IsPaused = false;
#if !OCULUS_OSP
                foreach (AudioSource audioSource in IndividualTracks) {
                    if (audioSource.isActiveAndEnabled) {
                        audioSource.PlayDelayed(delaySeconds);
                    }
                }
#else
                foreach (OSPAudioSource audioSource in individualTracksOSP) {
                    audioSource.PlayDelayed(delaySeconds);
                }
#endif
            }
        }

        /// <summary>
        ///     Similar to PlayDelayed but uses PlayScheduled to make sure all
        ///     clips are started exactly at the same sample.
        /// </summary>
        /// <param name="delaySeconds"></param>
        public void PlayAfterPreciseDelay(double delaySeconds) {
            PlayScheduled(AudioSettings.dspTime + delaySeconds);
        }

        private double startTime = -1;

        /// <summary>
        ///     Plays the clips of all AudioSources of this multitrack audio
        ///     source scheduled. See
        ///     Unity Scripting API, AudioSource.PlayScheduled
        ///     for more info.
        /// </summary>
        public void PlayScheduled(double time) {
            if (IsValid) {
                isStopped = false;
                IsPaused = false;
                startTime = time;
#if !OCULUS_OSP
                foreach (AudioSource audioSource in IndividualTracks) {
                    if (audioSource.isActiveAndEnabled) {
                        audioSource.PlayScheduled(time);
                    }
                }
#else
                foreach (OSPAudioSource audioSource in individualTracksOSP) {
                    audioSource.PlayScheduled(time);
                }
#endif
            }
        }

        private void PlayScheduledAlternate(double timeToStart, double startAtTime) {
            foreach (AudioSource audioSource in IndividualTracksScheduled) {
                if (audioSource.isActiveAndEnabled) {
                    audioSource.PlayScheduled(timeToStart);
                    audioSource.timeSamples = TimeToSamples(IndividualTracksScheduled[0].clip, startAtTime);
                }
            }
        }

        private void UpdateScheduledAlternate(double timeToStart, double startAtTime) {
            foreach (AudioSource audioSource in IndividualTracksScheduled) {
                if (audioSource.isActiveAndEnabled) {
                    audioSource.SetScheduledStartTime(timeToStart);
                    audioSource.timeSamples = TimeToSamples(IndividualTracksScheduled[0].clip, startAtTime);
                }
            }
        }

        /// <summary>
        ///     Schedules the end of all AudioSources of this multitrack audio
        ///     source. See
        ///     Unity Scripting API, AudioSource.SetScheduledEndTime
        ///     for more info.
        /// </summary>
        public void SetScheduledEndTime(double time) {
            if (IsValid) {
//#if !OCULUS_OSP
                foreach (AudioSource audioSource in IndividualTracks) {
                    if (audioSource.isActiveAndEnabled) {
                        audioSource.SetScheduledEndTime(time);
                    }
                }
//#else // currently not supported by OSP
//                foreach (OSPAudioSource audioSource in individualTracksOSP) {
//                    audioSource.SetScheduledEndTime(time);
//                }
//#endif
            }
        }

        /// <summary>
        ///     Re-Schedules the start of all AudioSources of this multitrack audio
        ///     source. See
        ///     Unity Scripting API, AudioSource.SetScheduledEndTime
        ///     for more info.
        /// </summary>
        public void SetScheduledStartTime(double time) {
            if (IsValid) {
//#if !OCULUS_OSP
                foreach (AudioSource audioSource in IndividualTracks) {
                    if (audioSource.isActiveAndEnabled) {
                        audioSource.SetScheduledStartTime(time);
                    }
                }
//#else  // currently not supported by OSP
//                foreach (OSPAudioSource audioSource in individualTracksOSP) {
//                    audioSource.SetScheduledStartTime(time);
//                }
//#endif
            }
        }


        public bool IsValid {
            get {
                if (IndividualTracks.Count == 0) {
                    Debug.LogWarning(
                        "MultiTrackAudioSource has no audio sources assigned "
                        + "and no children with AudioSources!", this);
                    return false;
                }
#if OCULUS_OSP
                if (individualTracksOSP.Count == 0) {
                    Debug.LogWarning(
                        "MultiTrackAudioSource has no OSP audio sources assigned "
                        + "and no children with OSPAudioSources!", this);
                    return false;
                }
#endif
                return true;
            }
        }

    }

}