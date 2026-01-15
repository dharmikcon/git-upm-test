# Narrative Design Usage Guidelines

## Convai Narrative Design Controller

Populate the Sections list with all the sections ids you want to track, whether its started or ended and want to do something in the application. We have provided two Unity Events which will be invoked when the section starts or end; so you can leverage it to perform various tasks.

### New Event System

The narrative design system now includes a comprehensive event system that allows you to respond to narrative section updates from the server:

#### OnNarrativeSectionReceived Unity Event
- **Purpose**: Fires whenever a narrative section ID is received from the server
- **Parameter**: `string sectionID` - The ID of the narrative section
- **Usage**: Subscribe to this event in the inspector or via code to react to section changes

#### Integration with RTVIHandler
- The system automatically processes server messages containing narrative section IDs
- When a `behavior-tree-response` message is received with a `narrative_section_id`, the event system triggers
- Both static events (RTVIHandler.OnNarrativeSectionReceived) and Unity events are available

## ConvaiNPC Integration

The narrative design system is now directly integrated into the `ConvaiNPC` component:

1. Assign your `ConvaiNarrativeDesignController` to the ConvaiNPC in the inspector
2. The NPC will automatically subscribe to server narrative section updates
3. Section changes will be forwarded to your controller automatically when connected

## Convai Narrative Design Trigger

Populate the Trigger Name if you have already created a trigger in the Narrative Design Editor in the Convai Dashboard, and populate the Trigger Message if you have not created a trigger but still wants to send it.

We have provided a barebone trigger MonoBehaviour, which you can use to invoke the trigger any time, for example on time runout, player going to someplace, etc.

## Setup Instructions

1. **Basic Setup**:
   - Add a `ConvaiNarrativeDesignController` to your ConvaiNPC in the inspector
   - Configure narrative sections with matching Section IDs from your Convai Dashboard
   - Set up Unity Events for `OnSectionStart` and `OnSectionEnd` for each section

2. **Event Subscription Options**:
   - **Unity Events**: Configure directly in the inspector on each narrative section
   - **Code Subscription**: Subscribe to `controller.OnNarrativeSectionReceived` Unity Event
   - **Static Events**: Subscribe to `RTVIHandler.OnNarrativeSectionReceived` for global handling

3. **Testing**:
   - Use the RTVIHandler.SimulateNarrativeSectionReceived() method in editor scripts
   - Test section transitions by calling the method with different section IDs