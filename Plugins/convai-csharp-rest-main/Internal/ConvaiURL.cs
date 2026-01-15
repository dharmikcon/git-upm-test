using System.Collections.Generic;

namespace Convai.RestAPI.Internal
{

    internal static class ConvaiURL
    {
        private const string BETA_SUBDOMAIN = "beta";
        private const string PROD_SUBDOMAIN = "api";
        private const string BASE_URL = "https://{0}.convai.com/";


        internal static string GetEndPoint(string api)
        {
            List<string> betaList = new List<string>()
            {
                // Animation-related endpoints currently use the beta host
                GetAnimationList,
                GetAnimation
            };
            bool onProd = !betaList.Contains(api);
            return string.Format(BASE_URL, onProd ? PROD_SUBDOMAIN : BETA_SUBDOMAIN) + api;
        }

        #region END POINTS

        public static string NewSpeaker { get; } = $"{LTM_SUBDOMAIN}new";
        public static string SpeakerIDList { get; } = $"{LTM_SUBDOMAIN}list";
        public static string DeleteSpeakerID { get; } = $"{LTM_SUBDOMAIN}delete";
        public static string ReferralSourceStatus { get; } = $"{USER_SUBDOMAIN}referral-source-status";
        public static string UpdateReferralSource { get; } = $"{USER_SUBDOMAIN}update-source";
        public static string UserAPIUsage { get; } = $"{USER_SUBDOMAIN}user-api-usage";
        public static string CharacterUpdate { get; } = $"{CHARACTER_SUBDOMAIN}update";
        public static string CharacterGet { get; } = $"{CHARACTER_SUBDOMAIN}get";
        public static string ListCharacterSections { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}list-sections";
        public static string CreateNarrativeSection { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}create-section";
        public static string GetNarrativeSection { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}get-section";
        public static string EditNarrativeSection { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}edit-section";
        public static string DeleteNarrativeSection { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}delete-section";

        public static string AddNarrativeDecision { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}add-decision";
        public static string EditNarrativeDecision { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}edit-decision";
        public static string DeleteNarrativeDecision { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}delete-decision";
        public static string UpdateStartNarrativeSection { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}update-start-section-id";
        public static string UpdateNarrativeNodePosition { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}update-node-position";
        public static string GetCurrentNarrativeSection { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}get-current-section";

        public static string ListCharacterTriggers { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}list-triggers";
        public static string CreateNarrativeTrigger { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}create-trigger";
        public static string GetNarrativeTrigger { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}get-trigger";
        public static string UpdateNarrativeTrigger { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}update-trigger";
        public static string DeleteNarrativeTrigger { get; } = $"{NARRATIVE_DESIGN_SUBDOMAIN}delete-trigger";

        public static string ToggleNarrativeDriven { get; } = $"{CHARACTER_SUBDOMAIN}toggle-is-narrative-driven";

        public static string GetAnimationList { get; } = $"{ANIMATION_SUBDOMAIN}list";
        public static string GetAnimation { get; } = $"{ANIMATION_SUBDOMAIN}get";


        private const string LTM_SUBDOMAIN = "user/speaker/";
        private const string USER_SUBDOMAIN = "user/";
        private const string CHARACTER_SUBDOMAIN = "character/";
        private const string NARRATIVE_DESIGN_SUBDOMAIN = "character/narrative/";
        private const string ANIMATION_SUBDOMAIN = "animations/";

        #endregion
    }

}
