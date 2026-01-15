using System;
using UnityEngine;

namespace Convai.Scripts.Vision
{
    public interface IConvaiVisionSource
    {
        /// <summary>
        ///     Gets the render texture for LiveKit video streaming
        /// </summary>
        RenderTexture GetRenderTexture();

        /// <summary>
        ///     Captures the current frame as a Texture2D
        /// </summary>
        Texture2D CaptureFrameAsTexture();

        /// <summary>
        ///     Captures the current frame as byte array (JPG encoded)
        /// </summary>
        byte[] CaptureFrameAsBytes();

        /// <summary>
        ///     Gets the capture dimensions
        /// </summary>
        Vector2Int GetCaptureDimensions();

        /// <summary>
        ///     Event fired when a new texture is ready
        /// </summary>
        event Action<Texture2D> OnTextureReady;

        /// <summary>
        ///     Event fired when image data is captured
        /// </summary>
        event Action<byte[]> OnImageCaptured;

        /// <summary>
        ///     Event fired when render texture is updated
        /// </summary>
        event Action<RenderTexture> OnRenderTextureReady;

        /// <summary>
        ///     Initialize the vision source
        /// </summary>
        void Initialize();

        /// <summary>
        ///     Cleanup resources
        /// </summary>
        void Cleanup();
    }
}
