using System;
using System.Collections;
using UnityEngine;

namespace LiveKit
{
    /// <summary>
    /// An audio source which captures from the device's microphone.
    /// </summary>
    /// <remarks>
    /// Ensure microphone permissions are granted before calling <see cref="Start"/>.
    /// </remarks>
    sealed public class MicrophoneSource : RtcAudioSource
    {
        private readonly GameObject _sourceObject;
        private readonly string _deviceName;

        public override event Action<float[], int, int> AudioRead;

        private bool _disposed = false;
        private bool _started = false;

        // Optional hook: if assigned, mic frames will be passed to this callback
        // before being forwarded to LiveKit capture.
        public static Action<float[], int, int> OnBeforeCapture; // data, channels, sampleRate

        /// <summary>
        /// Creates a new microphone source for the given device.
        /// </summary>
        /// <param name="deviceName">The name of the device to capture from. Use <see cref="Microphone.devices"/> to
        /// get the list of available devices.</param>
        /// <param name="sourceObject">The GameObject to attach the AudioSource to. The object must be kept in the scene
        /// for the duration of the source's lifetime.</param>
        public MicrophoneSource(string deviceName, GameObject sourceObject) : base(2, RtcAudioSourceType.AudioSourceMicrophone)
        {
            _deviceName = deviceName;
            _sourceObject = sourceObject;
        }

        /// <summary>
        /// Begins capturing audio from the microphone.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the microphone is not available or unauthorized.
        /// </exception>
        /// <remarks>
        /// Ensure microphone permissions are granted before calling this method
        /// by calling <see cref="Application.RequestUserAuthorization"/>.
        /// </remarks>
        public override void Start()
        {
            base.Start();
            if (_started) return;


            if (!Application.HasUserAuthorization(mode: UserAuthorization.Microphone))
                throw new InvalidOperationException("Microphone access not authorized");

            MonoBehaviourContext.OnApplicationPauseEvent += OnApplicationPause;
            MonoBehaviourContext.RunCoroutine(StartMicrophone());

            _started = true;
        }

        private IEnumerator StartMicrophone()
        {
            // Use Unity-compatible sample rate to ensure consistency with RtcAudioSource
            uint compatibleSampleRate = GetUnityCompatibleSampleRate(RtcAudioSourceType.AudioSourceMicrophone);

            var clip = Microphone.Start(
                _deviceName,
                loop: true,
                lengthSec: 1,
                frequency: (int)compatibleSampleRate
            );

            UnityEngine.Debug.Log($"Microphone started with sample rate: {compatibleSampleRate} Hz, Device: {_deviceName}");
            if (clip == null)
                throw new InvalidOperationException("Microphone start failed");

            var source = _sourceObject.GetComponent<AudioSource>();
            if (source == null)
                source = _sourceObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;

            var probe = _sourceObject.GetComponent<AudioProbe>();
            if (probe == null)
                probe = _sourceObject.AddComponent<AudioProbe>();
            // Clear the audio data after it is read as to not play it through the speaker locally.
            probe.ClearAfterInvocation();
            probe.AudioRead += OnAudioRead;

            var waitUntilReady = new WaitUntil(() => Microphone.GetPosition(_deviceName) > 0);
            yield return waitUntilReady;
            source.Play();
        }

        /// <summary>
        /// Stops capturing audio from the microphone.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            MonoBehaviourContext.RunCoroutine(StopMicrophone());
            MonoBehaviourContext.OnApplicationPauseEvent -= OnApplicationPause;
            _started = false;
        }

        private IEnumerator StopMicrophone()
        {
            if (Microphone.IsRecording(_deviceName))
                Microphone.End(_deviceName);

            var probe = _sourceObject.GetComponent<AudioProbe>();
            probe.AudioRead -= OnAudioRead;
            UnityEngine.Object.Destroy(probe);

            var source = _sourceObject.GetComponent<AudioSource>();
            UnityEngine.Object.Destroy(source);
            yield return null;
        }

        private void OnAudioRead(float[] data, int channels, int sampleRate)
        {
            // Optional AEC hook - allows pre-processing before capture
            OnBeforeCapture?.Invoke(data, channels, sampleRate);
            AudioRead?.Invoke(data, channels, sampleRate);
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause && _started)
                MonoBehaviourContext.RunCoroutine(RestartMicrophone());
        }

        private IEnumerator RestartMicrophone()
        {
            yield return StopMicrophone();
            yield return StartMicrophone();
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing) Stop();
            _disposed = true;
            base.Dispose(disposing);
        }

        ~MicrophoneSource()
        {
            Dispose(false);
        }
    }
}