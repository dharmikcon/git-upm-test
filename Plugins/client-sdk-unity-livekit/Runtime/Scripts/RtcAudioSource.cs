using System;
using System.Collections;
using LiveKit.Proto;
using LiveKit.Internal;
using LiveKit.Internal.FFIClients.Requests;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System.Diagnostics;

namespace LiveKit
{
    /// <summary>
    /// Defines the type of audio source, influencing processing behavior.
    /// </summary>
    public enum RtcAudioSourceType
    {
        AudioSourceCustom = 0,
        AudioSourceMicrophone = 1
    }

    /// <summary>
    /// Capture source for a local audio track.
    /// </summary>
    public abstract class RtcAudioSource : IRtcSource, IDisposable
    {
        // Global processing toggles; set before constructing sources to avoid double-processing
        public static bool UseSoftwareEchoCancellation = true;
        public static bool UseAutoGainControl = true;
        public static bool UseNoiseSuppression = true;

        /// <summary>
        /// Event triggered when audio samples are captured from the underlying source.
        /// Provides the audio data, channel count, and sample rate.
        /// </summary>
        /// <remarks>
        /// This event is not guaranteed to be called on the main thread.
        /// </remarks>
        public abstract event Action<float[], int, int> AudioRead;
        // Thread-safe cache for Unity's audio sample rate to avoid accessing AudioSettings from non-main threads
        private static volatile uint _cachedUnitySampleRate;
        private static volatile bool _cacheInitialized;

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitAudioConfigCache()
        {
            UpdateCachedUnitySampleRate();
            // Subscribe to Unity audio configuration changes on the main thread
            UnityEngine.AudioSettings.OnAudioConfigurationChanged += OnUnityAudioConfigChanged;
        }

        private static void OnUnityAudioConfigChanged(bool deviceWasChanged)
        {
            UpdateCachedUnitySampleRate();
        }

        private static void UpdateCachedUnitySampleRate()
        {
            try
            {
                _cachedUnitySampleRate = (uint)UnityEngine.AudioSettings.outputSampleRate;
                _cacheInitialized = true;
                UnityEngine.Debug.Log($"[LiveKit] Cached Unity output sample rate: {_cachedUnitySampleRate} Hz");
            }
            catch (Exception ex)
            {
                // Fallback to a sane default without calling into Unity APIs from non-main thread
                _cachedUnitySampleRate = DefaultSampleRate;
                _cacheInitialized = true;
                UnityEngine.Debug.LogWarning($"[LiveKit] Failed to read AudioSettings.outputSampleRate: {ex.Message}. Using fallback {_cachedUnitySampleRate} Hz");
            }
        }


        /// <summary>
        /// Gets the appropriate sample rate for audio sources based on Unity's audio configuration.
        /// This ensures compatibility between Unity's audio system and LiveKit.
        /// </summary>
        public static uint GetUnityCompatibleSampleRate(RtcAudioSourceType sourceType)
        {
            // Use cached value to avoid touching Unity APIs from non-main threads
            uint unitySampleRate = _cacheInitialized ? _cachedUnitySampleRate : DefaultSampleRate;

#if UNITY_IOS && !UNITY_EDITOR
            // iOS has specific microphone constraints
            if (sourceType == RtcAudioSourceType.AudioSourceMicrophone)
            {
                // For iOS microphones, prefer 24kHz if Unity allows it, otherwise use Unity's rate
                uint preferredRate = 24000;
                if (unitySampleRate == preferredRate || unitySampleRate == 48000)
                {
                    return preferredRate;
                }
                else
                {
                    return unitySampleRate;
                }
            }
#endif

            // For all other cases, use Unity's configured sample rate to ensure compatibility
            return unitySampleRate;
        }

#if UNITY_IOS && !UNITY_EDITOR
        // iOS microphone sample rate is 24k (fallback)
        public static uint DefaultMicrophoneSampleRate = 24000;
        public static uint DefaultSampleRate = 48000;
#else
        public static uint DefaultSampleRate = 48000;
        public static uint DefaultMicrophoneSampleRate = DefaultSampleRate;
#endif
        public static uint DefaultChannels = 2;
        private readonly int _configuredChannels;

        private readonly RtcAudioSourceType _sourceType;
        public RtcAudioSourceType SourceType => _sourceType;

        internal readonly FfiHandle Handle;
        protected AudioSourceInfo _info;

        /// <summary>
        /// Temporary frame buffer for invoking the FFI capture method.
        /// </summary>
        private NativeArray<short> _frameData;

        private bool _muted = false;
        public override bool Muted => _muted;

        private bool _started = false;
        private bool _disposed = false;

        protected RtcAudioSource(int channels = 2, RtcAudioSourceType audioSourceType = RtcAudioSourceType.AudioSourceCustom)
        {
            _sourceType = audioSourceType;
            _configuredChannels = channels;

            using var request = FFIBridge.Instance.NewRequest<NewAudioSourceRequest>();
            var newAudioSource = request.request;
            newAudioSource.Type = AudioSourceType.AudioSourceNative;
            newAudioSource.NumChannels = (uint)channels;

            // Use Unity-compatible sample rate to prevent mismatches
            newAudioSource.SampleRate = GetUnityCompatibleSampleRate(_sourceType);

            UnityEngine.Debug.Log($"NewAudioSource configured - Channels: {newAudioSource.NumChannels}, SampleRate: {newAudioSource.SampleRate}, SourceType: {_sourceType}");

            newAudioSource.Options = request.TempResource<AudioSourceOptions>();
            newAudioSource.Options.EchoCancellation = UseSoftwareEchoCancellation;
            newAudioSource.Options.AutoGainControl = UseAutoGainControl;
            newAudioSource.Options.NoiseSuppression = UseNoiseSuppression;
            using var response = request.Send();
            FfiResponse res = response;
            _info = res.NewAudioSource.Source.Info;
            Handle = FfiHandle.FromOwnedHandle(res.NewAudioSource.Source.Handle);
        }

        /// <summary>
        /// Begin capturing audio samples from the underlying source.
        /// </summary>
        public virtual void Start()
        {
            if (_started) return;
            AudioRead += OnAudioRead;
            _started = true;
        }

        /// <summary>
        /// Stop capturing audio samples from the underlying source.
        /// </summary>
        public virtual void Stop()
        {
            if (!_started) return;
            AudioRead -= OnAudioRead;
            _started = false;
        }

        private void OnAudioRead(float[] data, int channels, int sampleRate)
        {
            if (_muted) return;

            // Validate audio parameters to prevent mismatches
            uint expectedSampleRate = GetUnityCompatibleSampleRate(_sourceType);
            if ((uint)sampleRate != expectedSampleRate)
            {
                UnityEngine.Debug.LogWarning($"Audio sample rate mismatch detected! " +
                    $"Expected: {expectedSampleRate} Hz, Received: {sampleRate} Hz, " +
                    $"SourceType: {_sourceType}. This may cause audio capture failures.");
            }

            if ((uint)channels != (uint)_configuredChannels)
            {
                UnityEngine.Debug.LogWarning($"Audio channel count mismatch detected! " +
                    $"Expected: {_configuredChannels}, Received: {channels}. " +
                    $"This may cause audio capture failures.");
            }

            // The length of the data buffer corresponds to the DSP buffer size.
            if (_frameData.Length != data.Length)
            {
                if (_frameData.IsCreated) _frameData.Dispose();
                 _frameData = new NativeArray<short>(data.Length, Allocator.Persistent);
            }

            // Copy from the audio read buffer into the frame buffer, converting
            // each sample to a 16-bit signed integer.
            static short FloatToS16(float v)
            {
                v *= 32768f;
                v = Math.Min(v, 32767f);
                v = Math.Max(v, -32768f);
                return (short)(v + Math.Sign(v) * 0.5f);
            }
            for (int i = 0; i < data.Length; i++)
                _frameData[i] = FloatToS16(data[i]);

            // Capture the frame.
            using var request = FFIBridge.Instance.NewRequest<CaptureAudioFrameRequest>();
            using var audioFrameBufferInfo = request.TempResource<AudioFrameBufferInfo>();

            var pushFrame = request.request;
            pushFrame.SourceHandle = (ulong)Handle.DangerousGetHandle();
            pushFrame.Buffer = audioFrameBufferInfo;
            unsafe
            {
                 pushFrame.Buffer.DataPtr = (ulong)NativeArrayUnsafeUtility
                    .GetUnsafePtr(_frameData);
            }
            pushFrame.Buffer.NumChannels = (uint)channels;
            pushFrame.Buffer.SampleRate = (uint)sampleRate;
            pushFrame.Buffer.SamplesPerChannel = (uint)data.Length / (uint)channels;

            using var response = request.Send();
            FfiResponse res = response;

            // Wait for async callback, log an error if the capture fails.
            var asyncId = res.CaptureAudioFrame.AsyncId;
            void Callback(CaptureAudioFrameCallback callback)
            {
                if (callback.AsyncId != asyncId) return;
                if (callback.HasError)
                {
                    Utils.Error($"Audio capture failed: {callback.Error}. " +
                        $"Audio parameters - Channels: {channels}, SampleRate: {sampleRate} Hz, " +
                        $"Expected SampleRate: {GetUnityCompatibleSampleRate(_sourceType)} Hz, " +
                        $"SourceType: {_sourceType}, DataLength: {data.Length}");
                }
                FfiClient.Instance.CaptureAudioFrameReceived -= Callback;
            }
            FfiClient.Instance.CaptureAudioFrameReceived += Callback;
        }

        /// <summary>
        /// Mutes or unmutes the audio source.
        /// </summary>
        public override void SetMute(bool muted)
        {
            _muted = muted;
        }

        /// <summary>
        /// Disposes of the audio source, stopping it first if necessary.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing) Stop();
            if (_frameData.IsCreated) _frameData.Dispose();
            _disposed = true;
        }

        ~RtcAudioSource()
        {
            Dispose(false);
        }

        [Obsolete("No longer used, audio sources should perform any preparation in Start() asynchronously")]
        public virtual IEnumerator Prepare(float timeout = 0) { yield break; }

        [Obsolete("Use Start() instead")]
        public IEnumerator PrepareAndStart()
        {
            Start();
            yield break;
        }
    }
}
