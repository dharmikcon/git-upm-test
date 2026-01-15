using System.Collections.Generic;
using Convai.RestAPI.Internal;
using Convai.RestAPI.Internal.PostProcessors;
using Convai.RestAPI.Result;

namespace Convai.RestAPI
{

#nullable enable
    public static partial class ConvaiREST
    {
        public class ValidateAPIOperation : OperationResult<ReferralSourceStatus>
        {
            private ConvaiModel _model;
            public ValidateAPIOperation(ConvaiModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, string.Empty, _model.APIKey, ConvaiURL.ReferralSourceStatus);

                if (!result.Item2)
                {
                    SetCompletion(false, ReferralSourceStatus.Default());
                    return;
                }

                if (!RequestPostProcessor.ProcessAPIValidation(result.Item1, out ReferralSourceStatus? status) || status == null)
                {
                    SetCompletion(false, ReferralSourceStatus.Default());
                    return;
                }

                SetCompletion(true, status);
            }
        }

        public class UpdateReferralStatusOperation : OperationResult
        {
            private UpdateReferralSourceModel _model;
            public UpdateReferralStatusOperation(UpdateReferralSourceModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                Dictionary<string, string> dataToSend = new Dictionary<string, string>() { { "referral_source", _model.Source } };
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, dataToSend, _model.APIKey, ConvaiURL.UpdateReferralSource);

                if (!result.Item2)
                {
                    SetCompletion(false);
                    return;
                }

                SetCompletion(true);
            }
        }

        public class GetAPIUsageDetailsOperation : OperationResult<UserUsageData>
        {
            private ConvaiModel _model;
            public GetAPIUsageDetailsOperation(ConvaiModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, new Dictionary<string, string>(), _model.APIKey, ConvaiURL.UserAPIUsage);

                if (!result.Item2)
                {
                    SetCompletion(false, UserUsageData.Default());
                    return;
                }

                if (!RequestPostProcessor.ProcessUserAPIUsage(result.Item1, out UserUsageData? usage) || usage == null)
                {
                    SetCompletion(false, UserUsageData.Default());
                    return;
                }

                SetCompletion(true, usage);
            }
        }
    }

}
