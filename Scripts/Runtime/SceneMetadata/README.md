# Convai Scene Metadata System

A system for collecting and managing metadata from GameObjects in Unity scenes. This system lets you to add descriptive information to objects in your scene and efficiently send that metadata to Convai's WebRTC service for better AI environment awareness.

## Overview

This new metadata system allows you to add descriptive information to GameObjects in your Unity scene, which can then be collected and sent to Convai's AI service to provide better context for interactions. The system consists of three main components:

1. **ConvaiObjectMetadata** - Component you attach to GameObjects to store name and description
2. **ConvaiMetadataRegistry** - Efficient registry system that tracks all metadata components
3. **ConvaiSceneMetadataCollector** - Service that collects and sends metadata to RTVI

## Quick Start

### 1. Add Metadata to GameObjects

```csharp
// Method 1: Add component in Inspector
// Select GameObject -> Add Component -> Convai/Object Metadata

// Method 2: Add programmatically
var metadata = gameObject.AddComponent<ConvaiObjectMetadata>();
metadata.ObjectName = "Magic Sword";
metadata.ObjectDescription = "A legendary sword with mystical powers";
```

### 2. Set Up the Metadata Collector

Add a `ConvaiSceneMetadataCollector` component to a GameObject in your scene and assign the `ConvaiRoomManager`:

```csharp
// The collector will auto-find ConvaiRoomManager if not assigned
// Or assign it manually in the inspector or via code:
var collector = GetComponent<ConvaiSceneMetadataCollector>();
var roomManager = FindFirstObjectByType<ConvaiRoomManager>();
collector.SetRoomManager(roomManager);
```

### 3. Collect and Send Metadata

```csharp
// Get the collector component
var collector = FindFirstObjectByType<ConvaiSceneMetadataCollector>();

// Check if ready to send (room connected)
if (collector.IsReadyToSendMetadata())
{
    // Collect and send all metadata
    collector.CollectAndSendSceneMetadata();
}

// Or just get the count
int metadataCount = collector.GetMetadataCount();
```

### 4. Access Registry Directly

```csharp
// Get all valid metadata
var validMetadata = ConvaiMetadataRegistry.GetValidMetadata();

// Get metadata as SceneMetadata objects for RTVI
var sceneMetadataList = ConvaiMetadataRegistry.GetSceneMetadataList();

// Get statistics
var stats = ConvaiMetadataRegistry.GetStatistics();
```

## Components

### ConvaiObjectMetadata

A MonoBehaviour component that stores metadata for GameObjects.

**Properties:**

- `ObjectName` - Display name for the object (required)
- `ObjectDescription` - Description of the object (optional)
- `IncludeInMetadata` - Whether to include in collection (default: true)

**Features:**

- Automatic registration/unregistration with registry
- Input validation with helpful error messages
- Auto-populates name from GameObject if empty
- Inspector tooltips and validation

### ConvaiMetadataRegistry

Static registry that efficiently tracks all metadata components.

**Key Methods:**

- `GetValidMetadata()` - Returns all valid metadata components
- `GetSceneMetadataList()` - Returns RTVI-compatible metadata list
- `GetStatistics()` - Returns registry statistics for debugging
- `CleanupNullReferences()` - Removes invalid references

**Performance Benefits:**

- No `FindObjectsOfType` calls during runtime
- Automatic registration/cleanup
- Thread-safe operations
- Efficient memory usage

### ConvaiSceneMetadataCollector

Collects metadata and sends it via RTVI through ConvaiRoomManager.

**Configuration:**

- `roomManager` - Reference to ConvaiRoomManager (auto-detected if not assigned)
- `collectOnStart` - Auto-collect on Start (default: false)
- `logStatistics` - Log collection stats (default: true)

**Methods:**

- `CollectAndSendSceneMetadata()` - Main collection method
- `GetMetadataCount()` - Get current metadata count
- `ValidateAllMetadata()` - Validate all registered metadata
- `IsReadyToSendMetadata()` - Check if system is ready to send
- `SetRoomManager()` - Assign room manager reference

### Error Handling

```csharp
// Validate metadata before sending
var collector = FindFirstObjectByType<ConvaiSceneMetadataCollector>();
collector.ValidateAllMetadata();

// Check for validation errors on individual components
var metadata = GetComponent<ConvaiObjectMetadata>();
var errors = metadata.GetValidationErrors();
if (errors.Count > 0)
{
    Debug.LogWarning($"Validation errors: {string.Join(", ", errors)}");
}
```

## Editor Integration

The metadata system integrates seamlessly with the Unity Editor through the component inspector and provides validation feedback during development.

## Best Practices

### Setup Guidelines

1. **Meaningful Names**: Use descriptive, unique names for objects that the AI can understand
2. **Concise Descriptions**: Keep descriptions informative but under 500 characters
3. **Consistent Naming**: Use consistent naming conventions across your project
4. **Selective Metadata**: Only add metadata to objects that are relevant for AI interactions

### Performance Optimization

1. **Use the Registry**: Always use `ConvaiMetadataRegistry` methods for collection
2. **Batch Operations**: Collect metadata once per interaction, not continuously
3. **Validate Early**: Use validation methods to catch issues during development

## Troubleshooting

### Common Issues

1. **No metadata collected**: Ensure objects have `ConvaiObjectMetadata` components attached
2. **Validation errors**: Check object names are not empty and within length limits
3. **Room not connected**: Ensure `ConvaiRoomManager` is connected before sending metadata
4. **Missing room manager**: Assign `ConvaiRoomManager` to the collector component
5. **Empty metadata list**: Verify that metadata components have `IncludeInMetadata` enabled

### Debug Information

```csharp
// Log detailed statistics
var stats = ConvaiMetadataRegistry.GetStatistics();
Debug.Log($"Total: {stats["TotalRegistered"]}, Valid: {stats["ValidMetadata"]}");

// Clean up null references
int cleaned = ConvaiMetadataRegistry.CleanupNullReferences();
Debug.Log($"Cleaned {cleaned} null references");
```

## Performance

The registry-based design provides efficient metadata collection with constant-time lookup and minimal memory overhead. The system is optimized to handle large numbers of objects (100+) without performance degradation.

## API Reference

See the inline documentation in each class for detailed API information. All public methods and properties are fully documented with XML comments.
