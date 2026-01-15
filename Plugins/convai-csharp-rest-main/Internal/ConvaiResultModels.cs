using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Convai.RestAPI.Internal
{
    [Serializable]
    internal class CreateSpeakerIDResult
    {
        public CreateSpeakerIDResult(string speakerID)
        {
            SpeakerID = speakerID;
        }

        [JsonProperty("STATUS")] public string Status { get; set; } = string.Empty;

        [JsonProperty("speaker_id")] public string SpeakerID { get; set; } = string.Empty;

        [JsonProperty("name")] public string Name { get; set; } = string.Empty;

        [JsonProperty("device_id")] public string DeviceId { get; set; } = string.Empty;

        [JsonProperty("created")] public bool Created { get; set; }
    }

    [Serializable]
    public class SpeakerIDDetails
    {
        public SpeakerIDDetails(string id, string name)
        {
            ID = id;
            Name = name;
        }

        [JsonProperty("speaker_id")] public string ID { get; set; }
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("device_id")] public string DeviceId { get; set; }
    }

    [Serializable]
    public class MemorySettings
    {
        public MemorySettings(bool isEnabled) => IsEnabled = isEnabled;

        public static MemorySettings Default() => new MemorySettings(false);

        [JsonProperty("enabled")] public bool IsEnabled { get; set; }
    }

    [Serializable]
    public class CharacterUpdateResponse
    {
        public CharacterUpdateResponse(string status) => Status = status;

        [JsonProperty("STATUS")] public string Status { get; private set; }
    }

    [Serializable]
    public class ReferralSourceStatus
    {
        [JsonProperty("referral_source_status")]
        public string ReferralSourceStatusProperty;

        [JsonProperty("status")] public string Status;

        public static ReferralSourceStatus Default()
        {
            return new ReferralSourceStatus(string.Empty, string.Empty);
        }

        public ReferralSourceStatus(string referralSourceStatusProperty = "", string status = "")
        {
            ReferralSourceStatusProperty = referralSourceStatusProperty;
            Status = status;
        }
    }

    [Serializable]
    public class ServerAnimationListResponse
    {
        public ServerAnimationListResponse(List<ServerAnimationItemResponse> animations, string transactionID, int totalPages, int currentPage, int totalItems)
        {
            Animations = animations;
            TransactionID = transactionID;
            TotalPages = totalPages;
            CurrentPage = currentPage;
            TotalItems = totalItems;
        }

        public static ServerAnimationListResponse Default()
        {
            return new ServerAnimationListResponse(new List<ServerAnimationItemResponse>(), string.Empty, 0, 0, 0);
        }

        [JsonProperty("animations")] public List<ServerAnimationItemResponse> Animations { get; private set; }
        [JsonProperty("transaction_id")] public string TransactionID { get; private set; }
        [JsonProperty("total_pages")] public int TotalPages { get; private set; }
        [JsonProperty("page")] public int CurrentPage { get; private set; }
        [JsonProperty("total")] public int TotalItems { get; private set; }
    }

    [Serializable]
    public class ServerAnimationItemResponse
    {
        public ServerAnimationItemResponse(string animationID, string animationName, string status, string thumbnailURL)
        {
            AnimationID = animationID;
            AnimationName = animationName;
            Status = status;
            ThumbnailURL = thumbnailURL;
        }

        [JsonProperty("animation_id")] public string AnimationID { get; private set; }
        [JsonProperty("animation_name")] public string AnimationName { get; private set; }
        [JsonProperty("status")] public string Status { get; private set; }
        [JsonProperty("thumbnail_gcp_file")] public string ThumbnailURL { get; private set; }
    }

    [Serializable]
    public class Animation
    {
        public Animation(string animationId, string userId, string animationName, string status, string csvGcpFile, string fbxGcpFile, string thumbnailGcpFile, int retryCount, AnimationVideos animationVideos, DateTime createdAt)
        {
            AnimationId = animationId;
            UserId = userId;
            AnimationName = animationName;
            Status = status;
            CsvGcpFile = csvGcpFile;
            FbxGcpFile = fbxGcpFile;
            ThumbnailGcpFile = thumbnailGcpFile;
            RetryCount = retryCount;
            AnimationVideos = animationVideos;
            CreatedAt = createdAt;
        }

        public static Animation Default()
        {
            return new Animation(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, AnimationVideos.Default(), DateTime.MinValue);
        }

        [JsonProperty("animation_id")] public string AnimationId { get; private set; }
        [JsonProperty("user_id")] public string UserId { get; private set; }
        [JsonProperty("animation_name")] public string AnimationName { get; private set; }
        [JsonProperty("status")] public string Status { get; private set; }
        [JsonProperty("csv_gcp_file")] public string CsvGcpFile { get; private set; }
        [JsonProperty("fbx_gcp_file")] public string FbxGcpFile { get; private set; }
        [JsonProperty("thumbnail_gcp_file")] public string ThumbnailGcpFile { get; private set; }
        [JsonProperty("retry_count")] public int RetryCount { get; private set; }
        [JsonProperty("animation_videos")] public AnimationVideos AnimationVideos { get; private set; }
        [JsonProperty("created_at")] public DateTime CreatedAt { get; private set; }
    }

    [Serializable]
    public class AnimationVideos
    {

        public AnimationVideos(string fpvVideo, string tpvVideo)
        {
            FpvVideo = fpvVideo;
            TpvVideo = tpvVideo;
        }

        public static AnimationVideos Default()
        {
            return new AnimationVideos(string.Empty, string.Empty);
        }

        [JsonProperty("fpv_video")] public string FpvVideo { get; private set; }
        [JsonProperty("tpv_video")] public string TpvVideo { get; private set; }
    }

    [Serializable]
    public class ServerAnimationDataResponse
    {
        public ServerAnimationDataResponse(string transactionId, Animation animation, UploadUrls uploadUrls)
        {
            TransactionId = transactionId;
            Animation = animation;
            UploadUrls = uploadUrls;
        }


        public static ServerAnimationDataResponse Default()
        {
            return new ServerAnimationDataResponse(string.Empty, Animation.Default(), UploadUrls.Default());
        }


        [JsonProperty("transaction_id")] public string TransactionId { get; private set; }
        [JsonProperty("animation")] public Animation Animation { get; private set; }
        [JsonProperty("upload_urls")] public UploadUrls UploadUrls { get; private set; }
    }

    [Serializable]
    public class UploadUrls
    {
        public UploadUrls(string fpvVideo, string tpvVideo)
        {
            FpvVideo = fpvVideo;
            TpvVideo = tpvVideo;
        }

        public static UploadUrls Default()
        {
            return new UploadUrls(string.Empty, string.Empty);
        }

        [JsonProperty("fpv_video")] public string FpvVideo { get; private set; }
        [JsonProperty("tpv_video")] public string TpvVideo { get; private set; }
    }

    [Serializable]
    public class RoomDetails
    {
        public RoomDetails(string transport, string token, string roomName, string sessionId, string roomURL, string characterSessionId = "")
        {
            Transport = transport;
            Token = token;
            RoomName = roomName;
            SessionId = sessionId;
            RoomURL = roomURL;
            CharacterSessionId = characterSessionId;
        }

        public static RoomDetails Default()
        {
            return new RoomDetails(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        [JsonProperty("transport")]
        public string Transport { get; private set; }

        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("room_name")]
        public string RoomName { get; private set; }

        [JsonProperty("session_id")]
        public string SessionId { get; private set; }

        [JsonProperty("room_url")]
        public string RoomURL { get; private set; }

        [JsonProperty("character_session_id")]
        public string CharacterSessionId { get; private set; }
    }
}
