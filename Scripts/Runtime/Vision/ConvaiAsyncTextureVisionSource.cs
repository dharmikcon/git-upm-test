using System;
using Convai.Scripts.LoggerSystem;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Convai.Scripts.Vision
{
    public class ConvaiAsyncTextureVisionSource : MonoBehaviour, IConvaiVisionSource
    {
        [Header("Capture")] public Camera targetCamera;

        public int captureWidth = 640;
        public int captureHeight = 480;
        public int targetFps = 1;

        private float _nextCaptureTime;
        private RenderTexture _rtA, _rtB; // ping-pong render targets

        private RenderTexture _rtFlipped;
        private Texture2D _stagingTex; // stays on CPU side
        private bool _useA;

        private void Awake()
        {
            if (!targetCamera)
            {
                targetCamera = Camera.main;
            }

            _rtA = NewRT();
            _rtB = NewRT();
            _rtFlipped = new RenderTexture(captureWidth, captureHeight, 0, RenderTextureFormat.ARGB32);
            _rtFlipped.Create();
            _stagingTex = new Texture2D(captureWidth, captureHeight, TextureFormat.RGBA32, false);
        }

        private void LateUpdate()
        {
            // throttle to targetFps
            if (Time.time < _nextCaptureTime)
            {
                return;
            }

            _nextCaptureTime = Time.time + 1f / targetFps;

            // ---------- safety ----------
            if (_rtFlipped == null) // make sure we have a flip target
            {
                _rtFlipped = new RenderTexture(captureWidth, captureHeight, 0, RenderTextureFormat.ARGB32);
                _rtFlipped.Create();
            }

            // ---------- render scene into the active ping-pong RT ----------
            RenderTexture rt = _useA ? _rtA : _rtB; // ping-pong
            _useA = !_useA;

            RenderTexture prev = targetCamera.targetTexture;
            targetCamera.targetTexture = rt;
            targetCamera.Render(); // draw the frame
            targetCamera.targetTexture = prev;

            // ---------- GPU flip (verts) ----------
            //   scale  = ( 1,-1) flips Y
            //   offset = ( 0, 1) re-aligns to top-left origin
            Graphics.Blit(rt, _rtFlipped, new Vector2(1, -1), new Vector2(0, 1));

            // ---------- async GPU readback of the flipped RT ----------
            AsyncGPUReadback.Request(_rtFlipped, 0, request =>
            {
                if (request.hasError)
                {
                    ConvaiUnityLogger.Error("GPU readback failed", LogCategory.SDK);
                    return;
                }

                NativeArray<byte> data = request.GetData<byte>();

                // push into the staging Texture2D (already allocated in Awake)
                _stagingTex.LoadRawTextureData(data);
                _stagingTex.Apply(); // upload to CPU texture

                // fire events with the *flipped* frame
                OnRenderTextureReady?.Invoke(_rtFlipped);
                OnTextureReady?.Invoke(_stagingTex);
                OnImageCaptured?.Invoke(_stagingTex.EncodeToJPG(75));
            });
        }

        public event Action<Texture2D> OnTextureReady;
        public event Action<byte[]> OnImageCaptured;
        public event Action<RenderTexture> OnRenderTextureReady;

        // --- IConvaiVisionSource implementation ------------------------------

        public RenderTexture GetRenderTexture() => _useA ? _rtB : _rtA; // the most recent
        public Texture2D CaptureFrameAsTexture() => _stagingTex; // last CPU copy
        public byte[] CaptureFrameAsBytes() => _stagingTex.EncodeToJPG(75);
        public Vector2Int GetCaptureDimensions() => new(captureWidth, captureHeight);
        public void Initialize() { }

        public void Cleanup()
        {
            Destroy(_rtA);
            Destroy(_rtB);
            Destroy(_stagingTex);
            Destroy(_rtFlipped);
        }

        private RenderTexture NewRT()
        {
            RenderTexture rt = new(captureWidth, captureHeight, 0, RenderTextureFormat.ARGB32);
            rt.Create();
            return rt;
        }
    }
}
