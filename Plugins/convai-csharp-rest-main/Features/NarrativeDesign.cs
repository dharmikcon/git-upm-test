using System.Collections.Generic;
using Convai.RestAPI.Internal;
using Convai.RestAPI.Internal.Models;
using Convai.RestAPI.Internal.PostProcessors;
using Convai.RestAPI.Result;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Convai.RestAPI
{
#nullable enable
    public static partial class ConvaiREST
    {
        private static readonly JsonSerializerSettings _narrativeSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        private static string SerializeModel(object model) => JsonConvert.SerializeObject(model, _narrativeSerializerSettings);

        public class GetNarrativeDesignSectionsOperation : OperationResult<List<SectionData>>
        {
            private NarrativeDesignListModel _model;
            public GetNarrativeDesignSectionsOperation(NarrativeDesignListModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                Dictionary<string, string> dataToSend = new Dictionary<string, string>() { { "character_id", _model.CharacterID } };
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, dataToSend, _model.APIKey, ConvaiURL.ListCharacterSections);

                if (!result.Item2)
                {
                    SetCompletion(false, new List<SectionData>());
                    return;
                }

                if (!RequestPostProcessor.ProcessListSections(result.Item1, out List<SectionData>? list) || list == null)
                {
                    SetCompletion(false, new List<SectionData>());
                    return;
                }

                SetCompletion(true, list);
            }
        }

        public class GetNarrativeDesignTriggersOperation : OperationResult<List<TriggerData>>
        {
            private NarrativeDesignListModel _model;
            public GetNarrativeDesignTriggersOperation(NarrativeDesignListModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                Dictionary<string, string> dataToSend = new Dictionary<string, string>() { { "character_id", _model.CharacterID } };
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, dataToSend, _model.APIKey, ConvaiURL.ListCharacterTriggers);

                if (!result.Item2)
                {
                    SetCompletion(false, new List<TriggerData>());
                    return;
                }

                if (!RequestPostProcessor.ProcessListTriggers(result.Item1, out List<TriggerData>? list) || list == null)
                {
                    SetCompletion(false, new List<TriggerData>());
                    return;
                }

                SetCompletion(true, list);
            }
        }

        public class ToggleNarrativeDrivenOperation : OperationResult<StatusResponse>
        {
            private readonly ToggleNarrativeDrivenModel _model;

            public ToggleNarrativeDrivenOperation(ToggleNarrativeDrivenModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.ToggleNarrativeDriven);

                if (!result.Item2)
                {
                    SetCompletion(false, new StatusResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessStatusResponse(result.Item1, out StatusResponse? response) || response == null)
                {
                    SetCompletion(false, new StatusResponse(), "Failed to parse toggle narrative driven response.");
                    return;
                }

                if (!response.WasSuccessful)
                {
                    SetCompletion(false, response, response.Message ?? response.Status ?? "Toggle narrative driven failed.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class CreateNarrativeSectionOperation : OperationResult<CreateSectionResponse>
        {
            private readonly CreateNarrativeSectionModel _model;

            public CreateNarrativeSectionOperation(CreateNarrativeSectionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.CreateNarrativeSection);

                if (!result.Item2)
                {
                    SetCompletion(false, new CreateSectionResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessCreateSection(result.Item1, out CreateSectionResponse? response) || response == null)
                {
                    SetCompletion(false, new CreateSectionResponse(), "Failed to parse create section response.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class GetNarrativeSectionOperation : OperationResult<SectionData?>
        {
            private readonly GetNarrativeSectionModel _model;

            public GetNarrativeSectionOperation(GetNarrativeSectionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.GetNarrativeSection);

                if (!result.Item2)
                {
                    SetCompletion(false, null, result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessGetSection(result.Item1, out SectionData? section))
                {
                    SetCompletion(false, null, "Failed to parse get section response.");
                    return;
                }

                SetCompletion(true, section);
            }
        }

        public class EditNarrativeSectionOperation : OperationResult<EditSectionResponse>
        {
            private readonly EditNarrativeSectionModel _model;

            public EditNarrativeSectionOperation(EditNarrativeSectionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.EditNarrativeSection);

                if (!result.Item2)
                {
                    SetCompletion(false, new EditSectionResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessEditSection(result.Item1, out EditSectionResponse? response) || response == null)
                {
                    SetCompletion(false, new EditSectionResponse(), "Failed to parse edit section response.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class DeleteNarrativeSectionOperation : OperationResult<StatusResponse>
        {
            private readonly DeleteNarrativeSectionModel _model;

            public DeleteNarrativeSectionOperation(DeleteNarrativeSectionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.DeleteNarrativeSection);

                if (!result.Item2)
                {
                    SetCompletion(false, new StatusResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessStatusResponse(result.Item1, out StatusResponse? response) || response == null)
                {
                    SetCompletion(false, new StatusResponse(), "Failed to parse delete section response.");
                    return;
                }

                if (!response.WasSuccessful)
                {
                    SetCompletion(false, response, response.Message ?? "Delete section failed.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class AddNarrativeDecisionOperation : OperationResult<StatusResponse>
        {
            private readonly AddNarrativeDecisionModel _model;

            public AddNarrativeDecisionOperation(AddNarrativeDecisionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.AddNarrativeDecision);

                if (!result.Item2)
                {
                    SetCompletion(false, new StatusResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessStatusResponse(result.Item1, out StatusResponse? response) || response == null)
                {
                    SetCompletion(false, new StatusResponse(), "Failed to parse add decision response.");
                    return;
                }

                if (!response.WasSuccessful)
                {
                    SetCompletion(false, response, response.Message ?? "Add decision failed.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class EditNarrativeDecisionOperation : OperationResult<StatusResponse>
        {
            private readonly EditNarrativeDecisionModel _model;

            public EditNarrativeDecisionOperation(EditNarrativeDecisionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.EditNarrativeDecision);

                if (!result.Item2)
                {
                    SetCompletion(false, new StatusResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessStatusResponse(result.Item1, out StatusResponse? response) || response == null)
                {
                    SetCompletion(false, new StatusResponse(), "Failed to parse edit decision response.");
                    return;
                }

                if (!response.WasSuccessful)
                {
                    SetCompletion(false, response, response.Message ?? "Edit decision failed.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class DeleteNarrativeDecisionOperation : OperationResult<StatusResponse>
        {
            private readonly DeleteNarrativeDecisionModel _model;

            public DeleteNarrativeDecisionOperation(DeleteNarrativeDecisionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.DeleteNarrativeDecision);

                if (!result.Item2)
                {
                    SetCompletion(false, new StatusResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessStatusResponse(result.Item1, out StatusResponse? response) || response == null)
                {
                    SetCompletion(false, new StatusResponse(), "Failed to parse delete decision response.");
                    return;
                }

                if (!response.WasSuccessful)
                {
                    SetCompletion(false, response, response.Message ?? "Delete decision failed.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class UpdateStartNarrativeSectionOperation : OperationResult<StatusResponse>
        {
            private readonly UpdateStartNarrativeSectionModel _model;

            public UpdateStartNarrativeSectionOperation(UpdateStartNarrativeSectionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.UpdateStartNarrativeSection);

                if (!result.Item2)
                {
                    SetCompletion(false, new StatusResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessStatusResponse(result.Item1, out StatusResponse? response) || response == null)
                {
                    SetCompletion(false, new StatusResponse(), "Failed to parse update start section response.");
                    return;
                }

                if (!response.WasSuccessful)
                {
                    SetCompletion(false, response, response.Message ?? "Update start section failed.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class CreateNarrativeTriggerOperation : OperationResult<TriggerData>
        {
            private readonly CreateNarrativeTriggerModel _model;

            public CreateNarrativeTriggerOperation(CreateNarrativeTriggerModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.CreateNarrativeTrigger);

                if (!result.Item2)
                {
                    SetCompletion(false, TriggerDataDefault(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessCreateTrigger(result.Item1, out TriggerData? trigger) || trigger == null)
                {
                    SetCompletion(false, TriggerDataDefault(), "Failed to parse create trigger response.");
                    return;
                }

                SetCompletion(true, trigger);
            }

            private static TriggerData TriggerDataDefault() => new TriggerData(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public class GetNarrativeTriggerOperation : OperationResult<TriggerData?>
        {
            private readonly GetNarrativeTriggerModel _model;

            public GetNarrativeTriggerOperation(GetNarrativeTriggerModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.GetNarrativeTrigger);

                if (!result.Item2)
                {
                    SetCompletion(false, null, result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessGetTrigger(result.Item1, out TriggerData? trigger))
                {
                    SetCompletion(false, null, "Failed to parse get trigger response.");
                    return;
                }

                SetCompletion(true, trigger);
            }
        }

        public class UpdateNarrativeTriggerOperation : OperationResult<StatusResponse>
        {
            private readonly UpdateNarrativeTriggerModel _model;

            public UpdateNarrativeTriggerOperation(UpdateNarrativeTriggerModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.UpdateNarrativeTrigger);

                if (!result.Item2)
                {
                    SetCompletion(false, new StatusResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessStatusResponse(result.Item1, out StatusResponse? response) || response == null)
                {
                    SetCompletion(false, new StatusResponse(), "Failed to parse update trigger response.");
                    return;
                }

                if (!response.WasSuccessful)
                {
                    SetCompletion(false, response, response.Message ?? "Update trigger failed.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class DeleteNarrativeTriggerOperation : OperationResult<StatusResponse>
        {
            private readonly DeleteNarrativeTriggerModel _model;

            public DeleteNarrativeTriggerOperation(DeleteNarrativeTriggerModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.DeleteNarrativeTrigger);

                if (!result.Item2)
                {
                    SetCompletion(false, new StatusResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessStatusResponse(result.Item1, out StatusResponse? response) || response == null)
                {
                    SetCompletion(false, new StatusResponse(), "Failed to parse delete trigger response.");
                    return;
                }

                if (!response.WasSuccessful)
                {
                    SetCompletion(false, response, response.Message ?? "Delete trigger failed.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class UpdateNarrativeNodePositionOperation : OperationResult<StatusResponse>
        {
            private readonly UpdateNarrativeNodePositionModel _model;

            public UpdateNarrativeNodePositionOperation(UpdateNarrativeNodePositionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.UpdateNarrativeNodePosition);

                if (!result.Item2)
                {
                    SetCompletion(false, new StatusResponse(), result.Item1);
                    return;
                }

                if (!RequestPostProcessor.ProcessStatusResponse(result.Item1, out StatusResponse? response) || response == null)
                {
                    SetCompletion(false, new StatusResponse(), "Failed to parse update node position response.");
                    return;
                }

                if (!response.WasSuccessful)
                {
                    SetCompletion(false, response, response.Message ?? "Update node position failed.");
                    return;
                }

                SetCompletion(true, response);
            }
        }

        public class GetCurrentNarrativeSectionOperation : OperationResult<JObject?>
        {
            private readonly GetCurrentNarrativeSectionModel _model;

            public GetCurrentNarrativeSectionOperation(GetCurrentNarrativeSectionModel model)
            {
                _model = model;
                Execute();
            }

            private async void Execute()
            {
                string payload = SerializeModel(_model);
                (string, bool) result = await ConvaiREST.Request(RequestDispatcher.RequestType.Post, payload, _model.APIKey, ConvaiURL.GetCurrentNarrativeSection);

                if (!result.Item2)
                {
                    SetCompletion(false, null, result.Item1);
                    return;
                }

                JObject? response;
                try
                {
                    response = JsonConvert.DeserializeObject<JObject>(result.Item1);
                }
                catch (JsonException)
                {
                    SetCompletion(false, null, "Failed to parse current narrative section response.");
                    return;
                }

                SetCompletion(true, response);
            }
        }
    }
}
