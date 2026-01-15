using System;
using System.Threading.Tasks;
using Convai.Scripts.LoggerSystem;
using LiveKit;
using LiveKit.Proto;
using UnityEngine;

namespace Convai.Scripts.Vision
{
    public class ConvaiVideoPublisher : MonoBehaviour
    {
        [Header("Video Settings")] public string videoTrackName = "unity-scene";

        public int frameRate = 15;
        private bool isPublishing;
        private LocalVideoTrack localVideoTrack;

        private Room room;
        private TextureVideoSource textureVideoSource;
        private Coroutine updateCoroutine;
        private ConvaiVisionCapture visionCapture;

        private void Start()
        {
            visionCapture = GetComponent<ConvaiVisionCapture>();
            if (visionCapture == null)
            {
                ConvaiUnityLogger.Error("ConvaiVisionCapture component not found!", LogCategory.SDK);
                return;
            }

            ConvaiRoomManager.Instance.OnRoomConnectionSuccessful.AddListener(OnRoomConnected);
        }

        private void OnDestroy()
        {
            isPublishing = false;

            // Stop the update coroutine
            if (updateCoroutine != null)
            {
                StopCoroutine(updateCoroutine);
                updateCoroutine = null;
            }

            // Clean up video source (no explicit Dispose method)
            textureVideoSource = null;
            localVideoTrack = null;
        }

        private async void OnRoomConnected()
        {
            room = ConvaiRoomManager.Instance.Room;
            ConvaiUnityLogger.Info($"Unity player joined room: {room.Name}", LogCategory.SDK);
            await SetupVideoTrack();
        }

        private async Task SetupVideoTrack()
        {
            try
            {
                // Get the render texture from vision capture
                RenderTexture renderTexture = visionCapture.RenderTexture;
                if (renderTexture == null)
                {
                    ConvaiUnityLogger.Error("Failed to get render texture from vision capture", LogCategory.SDK);
                    return;
                }

                // Create TextureVideoSource using the render texture
                textureVideoSource = new TextureVideoSource(renderTexture);

                // Create local video track
                localVideoTrack = LocalVideoTrack.CreateVideoTrack(videoTrackName, textureVideoSource, room);

                // Configure publish options
                TrackPublishOptions options = new()
                {
                    Source = TrackSource.SourceScreenshare, // Custom source indicator
                    VideoCodec = VideoCodec.Vp8,
                    VideoEncoding = new VideoEncoding
                    {
                        MaxBitrate = 1000000, // 1 Mbps
                        MaxFramerate = (uint)frameRate
                    },
                    Simulcast = false // Disable simulcast for custom sources
                };

                // Publish the track
                PublishTrackInstruction publishInstruction =
                    room.LocalParticipant.PublishTrack(localVideoTrack, options);

                // Wait for completion
                while (!publishInstruction.IsDone)
                {
                    await Task.Delay(100);
                }

                if (publishInstruction.IsError)
                {
                    // ConvaiUnityLogger.Error($"Failed to publish video track: {publishInstruction.ErrorMessage}", LogCategory.SDK);
                }
                else
                {
                    ConvaiUnityLogger.Info($"Video track '{videoTrackName}' published successfully", LogCategory.SDK);

                    // Start the video source
                    textureVideoSource.Start();

                    // Start the update coroutine
                    updateCoroutine = StartCoroutine(textureVideoSource.Update());

                    isPublishing = true;
                }
            }
            catch (Exception ex)
            {
                ConvaiUnityLogger.Error($"Error setting up video track: {ex.Message}", LogCategory.SDK);
            }
        }
    }
}
