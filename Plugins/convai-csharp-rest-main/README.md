# Convai REST API Client

This folder contains the Convai REST API client implementation for C#. The API provides a modern, type-safe interface for interacting with Convai's services using the `OperationResult<T>` pattern.

## Overview

The REST API client is built around the `OperationResult<T>` pattern, which provides:
- **Type safety**: Each operation returns strongly-typed results
- **Error handling**: Built-in success/failure state management with error messages
- **Async operations**: Non-blocking API calls
- **Consistent interface**: All operations follow the same pattern

## Core Components

### OperationResult<T>

The base class for all API operations. Provides:
- `IsCompleted`: Whether the operation has finished
- `WasSuccess`: Whether the operation succeeded
- `Result`: The operation result (for generic operations)
- `ErrorMessage`: Error message if the operation failed

```csharp
public class OperationResult<T>
{
    public bool IsCompleted { get; private set; }
    public bool WasSuccess { get; private set; }
    public T? Result { get; private set; }
    public string? ErrorMessage { get; protected set; }
}
```

### Models

All API operations use strongly-typed models that inherit from `ConvaiModel`:

```csharp
public class ConvaiModel
{
    public ConvaiModel(string apiKey) => APIKey = apiKey;
    public string APIKey { get; private set; }
}
```

## Available Features

### 1. Character Management

#### Get Character Details
```csharp
var model = new GetCharacterDetailsModel("your-api-key", "character-id");
var operation = new ConvaiREST.GetCharacterDetailsOperation(model);

// Check operation status
if (operation.IsCompleted && operation.WasSuccess)
{
    var characterDetails = operation.Result;
    Console.WriteLine($"Character: {characterDetails.Name}");
}
```

### 2. User Management

#### Validate API Key
```csharp
var model = new ConvaiModel("your-api-key");
var operation = new ConvaiREST.ValidateAPIOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    var status = operation.Result;
    Console.WriteLine($"API Status: {status.Status}");
}
```

#### Update Referral Source
```csharp
var model = new UpdateReferralSourceModel("your-api-key", "source-name");
var operation = new ConvaiREST.UpdateReferralStatusOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    Console.WriteLine("Referral source updated successfully");
}
```

#### Get API Usage Details
```csharp
var model = new ConvaiModel("your-api-key");
var operation = new ConvaiREST.GetAPIUsageDetailsOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    var usage = operation.Result;
    Console.WriteLine($"Usage: {usage.Usage}");
}
```

### 3. Long Term Memory (LTM)

#### Create Speaker ID
```csharp
// deviceId is an optional, client-generated stable identifier for this user or device.
// When provided, repeated calls with the same deviceId will return the same speaker_id.
var deviceId = "your-stable-device-id";
var model = new CreateSpeakerIDModel("your-api-key", "player-name", deviceId);
var operation = new ConvaiREST.CreateSpeakerIDOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    var speakerId = operation.Result;
    Console.WriteLine($"Created Speaker ID: {speakerId}");
}
```

#### Get Speaker ID List
```csharp
var model = new ConvaiModel("your-api-key");
var operation = new ConvaiREST.GetSpeakerIDListOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    var speakerList = operation.Result;
    foreach (var speaker in speakerList)
    {
        Console.WriteLine($"Speaker: {speaker.Name} - {speaker.ID}");
    }
}
```

#### Delete Speaker ID
```csharp
var model = new DeleteSpeakerIDModel("your-api-key", "speaker-id");
var operation = new ConvaiREST.DeleteSpeakerIDOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    Console.WriteLine("Speaker ID deleted successfully");
}
```

#### Get LTM Status
```csharp
var model = new GetCharacterDetailsModel("your-api-key", "character-id");
var operation = new ConvaiREST.GetLTMStatusOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    var isEnabled = operation.Result;
    Console.WriteLine($"LTM Status: {(isEnabled ? "Enabled" : "Disabled")}");
}
```

#### Update LTM Status
```csharp
var model = new UpdateLTMStatusModel("your-api-key", "character-id", true);
var operation = new ConvaiREST.UpdateLTMStatusOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    Console.WriteLine("LTM status updated successfully");
}
```

### 4. Narrative Design

#### Toggle Narrative Graph
```csharp
var model = new ToggleNarrativeDrivenModel("your-api-key", "character-id", true);
var operation = new ConvaiREST.ToggleNarrativeDrivenOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    Console.WriteLine($"Narrative graph enabled: {operation.Result.WasSuccessful}");
}
```

#### Create a Section
```csharp
var model = new CreateNarrativeSectionModel(
    "your-api-key",
    "character-id",
    "Intro Section",
    "Greet the player"
);

var operation = new ConvaiREST.CreateNarrativeSectionOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    Console.WriteLine($"Created section: {operation.Result.SectionId}");
}
```

#### Edit or Delete a Section
```csharp
var updateData = new NarrativeSectionUpdateData(sectionName: "Welcome Back");
var editModel = new EditNarrativeSectionModel("your-api-key", "character-id", "section-id", updateData);
var editOperation = new ConvaiREST.EditNarrativeSectionOperation(editModel);

if (editOperation.IsCompleted && editOperation.WasSuccess)
{
    Console.WriteLine("Section updated");
}

var deleteModel = new DeleteNarrativeSectionModel("your-api-key", "character-id", "section-id");
var deleteOperation = new ConvaiREST.DeleteNarrativeSectionOperation(deleteModel);

if (deleteOperation.IsCompleted && deleteOperation.WasSuccess)
{
    Console.WriteLine("Section deleted");
}
```

#### Get or List Sections
```csharp
var listModel = new NarrativeDesignListModel("your-api-key", "character-id");
var listOperation = new ConvaiREST.GetNarrativeDesignSectionsOperation(listModel);

if (listOperation.IsCompleted && listOperation.WasSuccess)
{
    foreach (var section in listOperation.Result)
    {
        Console.WriteLine($"Section: {section.SectionName}");
    }
}

var getModel = new GetNarrativeSectionModel("your-api-key", "character-id", "section-id");
var getOperation = new ConvaiREST.GetNarrativeSectionOperation(getModel);

if (getOperation.IsCompleted && getOperation.WasSuccess && getOperation.Result != null)
{
    Console.WriteLine($"Objective: {getOperation.Result.Objective}");
}
```

#### Manage Decisions
```csharp
var addDecisionModel = new AddNarrativeDecisionModel(
    "your-api-key",
    "character-id",
    fromSectionId: "section-a",
    toSectionId: "section-b",
    criteria: "player_accepts",
    priority: 1);

var addDecisionOperation = new ConvaiREST.AddNarrativeDecisionOperation(addDecisionModel);

if (addDecisionOperation.IsCompleted && addDecisionOperation.WasSuccess)
{
    Console.WriteLine("Decision added");
}
```

#### Manage Triggers
```csharp
var createTriggerModel = new CreateNarrativeTriggerModel(
    "your-api-key",
    "character-id",
    "StartTrigger",
    "Player enters the scene",
    destinationSection: "intro-section"
);

var createTriggerOperation = new ConvaiREST.CreateNarrativeTriggerOperation(createTriggerModel);

if (createTriggerOperation.IsCompleted && createTriggerOperation.WasSuccess)
{
    Console.WriteLine($"Trigger created: {createTriggerOperation.Result.TriggerId}");
}

var triggerListOperation = new ConvaiREST.GetNarrativeDesignTriggersOperation(listModel);

if (triggerListOperation.IsCompleted && triggerListOperation.WasSuccess)
{
    foreach (var trigger in triggerListOperation.Result)
    {
        Console.WriteLine($"Trigger: {trigger.TriggerName}");
    }
}
```

#### Update Node Positions & Current Section
```csharp
var nodePayload = new Newtonsoft.Json.Linq.JArray
{
    new Newtonsoft.Json.Linq.JObject
    {
        ["id"] = "section-id",
        ["x"] = 100,
        ["y"] = 250
    }
};

var nodeModel = new UpdateNarrativeNodePositionModel("your-api-key", "sections", nodePayload);
var nodeOperation = new ConvaiREST.UpdateNarrativeNodePositionOperation(nodeModel);

if (nodeOperation.IsCompleted && nodeOperation.WasSuccess)
{
    Console.WriteLine("Node position updated");
}

var currentSectionModel = new GetCurrentNarrativeSectionModel("your-api-key", "character-id", "session-id");
var currentSectionOperation = new ConvaiREST.GetCurrentNarrativeSectionOperation(currentSectionModel);

if (currentSectionOperation.IsCompleted && currentSectionOperation.WasSuccess && currentSectionOperation.Result != null)
{
    Console.WriteLine($"Current section response: {currentSectionOperation.Result.ToString()}");
}
```

### 5. Server Animation

#### Get Animation List
```csharp
var model = new GetAnimationListModel("your-api-key", 1, "active");
var operation = new ConvaiREST.GetAnimationListOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    var animationList = operation.Result;
    foreach (var animation in animationList.Animations)
    {
        Console.WriteLine($"Animation: {animation.AnimationName}");
    }
}
```

#### Get Animation Data
```csharp
var model = new GetAnimationItemModel("your-api-key", "animation-id");
var operation = new ConvaiREST.GetAnimationDataOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    var animationData = operation.Result;
    Console.WriteLine($"Animation: {animationData.Animation.AnimationName}");
    Console.WriteLine($"Status: {animationData.Animation.Status}");
}
```

### 6. Room Connection

#### Get Room Connection Details
```csharp
var model = new ConvaiRoomRequest("your-api-key", "character-id", "transport", "connection-type", "llm-provider", "url");
var operation = new ConvaiREST.GetRoomConnectionOperation(model);

if (operation.IsCompleted && operation.WasSuccess)
{
    var roomDetails = operation.Result;
    Console.WriteLine($"Room: {roomDetails.RoomName}");
    Console.WriteLine($"Session: {roomDetails.SessionId}");
}
```

## Usage Patterns

### Basic Usage
```csharp
// Create the model
var model = new SomeModel("your-api-key", "parameter");

// Create and start the operation
var operation = new ConvaiREST.SomeOperation(model);

// Wait for completion (in a real app, you'd use async/await or polling)
while (!operation.IsCompleted)
{
    await Task.Delay(100);
}

// Check result
if (operation.WasSuccess)
{
    var result = operation.Result;
    // Use the result
}
else
{
    // Handle error
    Console.WriteLine($"Operation failed: {operation.ErrorMessage}");
}
```

### Async/Await Pattern
```csharp
public async Task<SomeResult> GetDataAsync(string apiKey, string parameter)
{
    var model = new SomeModel(apiKey, parameter);
    var operation = new ConvaiREST.SomeOperation(model);
    
    // Wait for completion
    while (!operation.IsCompleted)
    {
        await Task.Delay(100);
    }
    
    return operation.WasSuccess ? operation.Result : null;
}
```

### Error Handling
```csharp
var operation = new ConvaiREST.SomeOperation(model);

if (operation.IsCompleted)
{
    if (operation.WasSuccess)
    {
        // Success case
        var result = operation.Result;
    }
    else
    {
        // Error case
        Console.WriteLine($"Operation failed: {operation.ErrorMessage}");
    }
}
```

## Model Reference

### Base Models
- `ConvaiModel`: Base class for all API models
- `GetCharacterDetailsModel`: For character detail operations
- `NarrativeDesignListModel`: For list operations
- `GetAnimationListModel`: For animation list operations
- `GetAnimationItemModel`: For individual animation operations
- `ConvaiRoomRequest`: For room connection operations

### Narrative Design Models
- `ToggleNarrativeDrivenModel`: Enable/disable the narrative graph
- `CreateNarrativeSectionModel`: Create sections
- `EditNarrativeSectionModel`: Update section fields
- `GetNarrativeSectionModel`: Retrieve a single section
- `DeleteNarrativeSectionModel`: Delete a section
- `NarrativeSectionUpdateData`: Section update payload
- `AddNarrativeDecisionModel`: Add decisions
- `EditNarrativeDecisionModel`: Edit decisions
- `DeleteNarrativeDecisionModel`: Delete decisions
- `NarrativeDecisionData` / `NarrativeDecisionUpdatePayload`: Decision payload helpers
- `UpdateStartNarrativeSectionModel`: Set the default section
- `UpdateNarrativeNodePositionModel`: Persist editor node positions
- `CreateNarrativeTriggerModel`: Create triggers
- `UpdateNarrativeTriggerModel`: Update triggers
- `GetNarrativeTriggerModel`: Retrieve a trigger
- `DeleteNarrativeTriggerModel`: Delete a trigger
- `GetCurrentNarrativeSectionModel`: Lookup the active section for a session

### LTM Models
- `CreateSpeakerIDModel`: For creating speaker IDs
- `DeleteSpeakerIDModel`: For deleting speaker IDs
- `UpdateLTMStatusModel`: For updating LTM status

### User Models
- `UpdateReferralSourceModel`: For updating referral sources

## Response Types

### Character Management
- `CharacterDetails`: Character information and settings

### User Management
- `ReferralSourceStatus`: API validation status
- `UserUsageData`: API usage statistics

### LTM
- `string`: Speaker ID (for creation)
- `List<SpeakerIDDetails>`: List of speaker IDs
- `bool`: LTM status

### Narrative Design
- `List<SectionData>`: Narrative design sections
- `CreateSectionResponse`: Section creation result
- `EditSectionResponse`: Section update details
- `StatusResponse`: Generic success/error wrapper (toggle, delete, triggers, decisions, node positions)
- `List<TriggerData>` / `TriggerData`: Trigger payloads
- `Newtonsoft.Json.Linq.JObject`: Raw payload for current-section lookups

### Server Animation
- `ServerAnimationListResponse`: List of animations with pagination
- `ServerAnimationDataResponse`: Individual animation data with upload URLs

### Room Connection
- `RoomDetails`: Room connection information including transport, token, room name, and session ID

## Project Structure

```
convai-csharp-rest/
├── ConvaiREST.cs              # Main API class with character operations
├── ConvaiRestModels.cs        # All model definitions
├── CharacterDetails.cs        # Character details response model
├── UserUsageData.cs          # User usage data response model
├── Features/
│   ├── User.cs               # User management operations
│   ├── LongTermMemory.cs     # LTM operations
│   ├── NarrativeDesign.cs    # Narrative design operations
│   ├── ServerAnimation.cs    # Server animation operations
│   └── RoomConnection.cs     # Room connection operations
├── Internal/
│   ├── ConvaiResultModels.cs # Internal response models
│   ├── ConvaiURL.cs          # API endpoint URLs
│   ├── RequestDispatcher.cs  # HTTP request handling
│   ├── RequestPreprocessor.cs # Request preprocessing
│   └── PostProcessors/       # Response processing
├── Result/
│   └── OperationResult.cs    # Base operation result classes
└── README.md                 # This documentation
```

## Best Practices

1. **Always check completion**: Wait for `IsCompleted` before accessing results
2. **Handle errors**: Check `WasSuccess` before using the result and handle `ErrorMessage`
3. **Use appropriate models**: Each operation requires a specific model type
4. **Async operations**: The operations are async, so use proper async/await patterns
5. **Resource management**: Operations are fire-and-forget, no disposal needed

## Error Handling

All operations provide built-in error handling through the `WasSuccess` property and `ErrorMessage`. When an operation fails:
- `IsCompleted` will be `true`
- `WasSuccess` will be `false`
- `Result` will be `null` or the default value
- `ErrorMessage` will contain the error details

```csharp
var operation = new ConvaiREST.SomeOperation(model);

if (operation.IsCompleted)
{
    if (!operation.WasSuccess)
    {
        // Handle the error
        Console.WriteLine($"Operation failed: {operation.ErrorMessage}");
        return;
    }
    
    // Use the successful result
    var result = operation.Result;
}
```

## Thread Safety

The `OperationResult<T>` classes are not thread-safe. If you need to access operation results from multiple threads, implement appropriate synchronization mechanisms.

## Dependencies

- `Newtonsoft.Json` (v13.0.3): For JSON serialization
- `System.Net.Http`: For HTTP requests
- `System.Threading.Tasks`: For async operations

## Target Framework

- **.NET Standard 2.1**: Ensures compatibility across multiple .NET platforms 