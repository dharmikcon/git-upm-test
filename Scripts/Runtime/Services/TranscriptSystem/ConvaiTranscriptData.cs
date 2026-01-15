namespace Convai.Scripts.Services.TranscriptSystem
{
    public struct ConvaiTranscriptData
    {
        public string Identifier;
        public string Name;
        public string Message;
        public bool IsLastChunk;


        public ConvaiTranscriptData(string identifier, string name, string message, bool isLastChunk)
        {
            Identifier = identifier;
            Name = name;
            Message = message;
            IsLastChunk = isLastChunk;
        }
    }
}
