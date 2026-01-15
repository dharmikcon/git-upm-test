using System;
using Convai.Scripts.LoggerSystem;
using UnityEngine;

namespace Convai.Scripts.Vision
{
    public class ConvaiVisionCapture : MonoBehaviour
    {
        [Header("Texture-Based Settings")] [Tooltip("Camera for texture capture (for texture-based production)")]
        public Camera targetCamera;

        [Header("Output Settings")] public int captureWidth = 1920;

        public int captureHeight = 1080;

        private IConvaiVisionSource _currentVisionSource;
        private ConvaiAsyncTextureVisionSource _textureBasedSource;

        // Public properties for other components to access
        public RenderTexture RenderTexture => _currentVisionSource?.GetRenderTexture();
        public int CaptureWidth => captureWidth;
        public int CaptureHeight => captureHeight;

        private void Start() => InitializeVisionSource();

        private void OnDestroy() => CleanupCurrentSource();

        public event Action<Texture2D> OnTextureReady;
        public event Action<byte[]> OnImageCaptured;
        public event Action<RenderTexture> OnRenderTextureReady;

        private void InitializeVisionSource()
        {
            InitializeTextureBasedSource();

            // Subscribe to events
            if (_currentVisionSource != null)
            {
                _currentVisionSource.OnTextureReady += texture => OnTextureReady?.Invoke(texture);
                _currentVisionSource.OnImageCaptured += data => OnImageCaptured?.Invoke(data);
                _currentVisionSource.OnRenderTextureReady += rt => OnRenderTextureReady?.Invoke(rt);
            }
        }

        private void InitializeTextureBasedSource()
        {
            _textureBasedSource = gameObject.AddComponent<ConvaiAsyncTextureVisionSource>();
            _textureBasedSource.targetCamera = targetCamera;
            _textureBasedSource.captureWidth = captureWidth;
            _textureBasedSource.captureHeight = captureHeight;
            // textureBasedSource.enableFileSynchronization = true; // Enable for LiveKit compatibility

            _currentVisionSource = _textureBasedSource;

            ConvaiUnityLogger.Info($"Texture-based vision source initialized with camera: {targetCamera?.name}",
                LogCategory.SDK);
        }

        private void CleanupCurrentSource()
        {
            if (_currentVisionSource != null)
            {
                _currentVisionSource.Cleanup();
                _currentVisionSource = null;
            }

            if (_textureBasedSource != null)
            {
                DestroyImmediate(_textureBasedSource);
                _textureBasedSource = null;
            }
        }
    }
}
