using System.Collections.Generic;
using Convai.RestAPI.Internal;
using Convai.RestAPI.Internal.PostProcessors;
using Convai.RestAPI.Result;
using Newtonsoft.Json;
using UnityEngine;


namespace Convai.RestAPI
{

#nullable enable
    public static partial class ConvaiREST
    {
        public class CreateSpeakerIDOperation : OperationResult<string>
        {
            private CreateSpeakerIDModel _model;
            public CreateSpeakerIDOperation(CreateSpeakerIDModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string endpoint = ConvaiURL.GetEndPoint(ConvaiURL.NewSpeaker);
                Debug.Log($"CreateSpeakerID endpoint = {endpoint}");

                (string, bool) result;

                if (!string.IsNullOrEmpty(_model.DeviceId))
                {
                    var payload = new
                    {
                        name = _model.PlayerName,
                        deviceId = _model.DeviceId
                    };
                    string json = JsonConvert.SerializeObject(payload);
                    Debug.Log($"CreateSpeakerID payload (with deviceId) => {json}");
                    result = await Request(RequestDispatcher.RequestType.Post, json, _model.APIKey, ConvaiURL.NewSpeaker);
                }
                else
                {
                    Dictionary<string, string> dataToSend = new Dictionary<string, string>() { { "name", _model.PlayerName } };
                    string json = JsonConvert.SerializeObject(dataToSend);
                    Debug.Log($"CreateSpeakerID payload (without deviceId) => {json}");
                    result = await Request(RequestDispatcher.RequestType.Post, json, _model.APIKey, ConvaiURL.NewSpeaker);
                }

                Debug.Log($"CreateSpeakerID raw response => Success:{result.Item2} Body:{result.Item1}");

                if (!result.Item2)
                {
                    string error = string.IsNullOrWhiteSpace(result.Item1)
                        ? "CreateSpeakerID request failed with unknown error (empty response)."
                        : $"CreateSpeakerID request failed: {result.Item1}";

                    SetCompletion(false, string.Empty, error);
                    return;
                }

                if (!RequestPostProcessor.ProcessCreateSpeakerID(result.Item1, out string id))
                {
                    string raw = result.Item1 ?? string.Empty;
                    if (raw.Length > 500)
                    {
                        raw = raw.Substring(0, 500) + "...";
                    }

                    string error = string.IsNullOrWhiteSpace(raw)
                        ? "CreateSpeakerID response did not contain a valid speaker_id."
                        : $"CreateSpeakerID response did not contain a valid speaker_id. Raw response: {raw}";

                    SetCompletion(false, string.Empty, error);
                    return;
                }

                SetCompletion(true, id);
            }
        }

        public class GetSpeakerIDListOperation : OperationResult<List<SpeakerIDDetails>>
        {
            private ConvaiModel _model;
            public GetSpeakerIDListOperation(ConvaiModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                (string, bool) result = await Request(RequestDispatcher.RequestType.Post, new Dictionary<string, string>(), _model.APIKey, ConvaiURL.SpeakerIDList);

                if (!result.Item2)
                {
                    SetCompletion(false, new List<SpeakerIDDetails>());
                    return;
                }

                if (!RequestPostProcessor.ProcessSpeakerIDList(result.Item1, out List<SpeakerIDDetails> detailsList))
                {
                    SetCompletion(false, new List<SpeakerIDDetails>());
                    return;
                }

                SetCompletion(true, detailsList);
            }
        }

        public class DeleteSpeakerIDOperation : OperationResult
        {
            private DeleteSpeakerIDModel _model;
            public DeleteSpeakerIDOperation(DeleteSpeakerIDModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                Dictionary<string, string> dataToSend = new Dictionary<string, string>() { { "speakerId", _model.SpeakerID } };

                (string, bool) result = await Request(RequestDispatcher.RequestType.Post, dataToSend, _model.APIKey, ConvaiURL.DeleteSpeakerID);

                if (!result.Item2)
                {
                    SetCompletion(false);
                    return;
                }

                SetCompletion(true);
            }
        }

        public class GetLTMStatusOperation : OperationResult<bool>
        {
            private GetCharacterDetailsModel _model;
            public GetLTMStatusOperation(GetCharacterDetailsModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                Dictionary<string, string> dataToSend = new Dictionary<string, string>() { { "charID", _model.CharacterID } };

                (string, bool) result = await Request(RequestDispatcher.RequestType.Post, dataToSend, _model.APIKey, ConvaiURL.CharacterGet);

                if (!result.Item2)
                {
                    SetCompletion(false, false);
                    return;
                }

                if (!RequestPostProcessor.ProcessLTMStatus(result.Item1, out bool status))
                {
                    SetCompletion(false, false);
                    return;
                }

                SetCompletion(true, status);
            }
        }

        public class UpdateLTMStatusOperation : OperationResult
        {
            private UpdateLTMStatusModel _model;
            public UpdateLTMStatusOperation(UpdateLTMStatusModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                CharacterUpdateRequest dataModel = new CharacterUpdateRequest(_model.CharacterID, _model.Status);
                string json = JsonConvert.SerializeObject(dataModel);

                (string, bool) result = await Request(RequestDispatcher.RequestType.Post, json, _model.APIKey, ConvaiURL.CharacterUpdate);

                if (!result.Item2)
                {
                    SetCompletion(false);
                    return;
                }

                if (!RequestPostProcessor.ProcessLTMUpdateStatus(result.Item1, out bool status))
                {
                    SetCompletion(false);
                    return;
                }

                SetCompletion(true);
            }
        }
    }

}
