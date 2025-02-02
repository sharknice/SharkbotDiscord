using Newtonsoft.Json.Linq;

namespace SharkbotDiscord.Models.Api
{
    [Serializable]
    public class ConversationData
    {
        public string name { get; set; }
        public List<AnalyzedChat> responses { get; set; }
        public bool groupChat { get; set; }
    }

    [Serializable]
    public class AnalyzedChat
    {
        public Chat chat { get; set; }
        public NaturalLanguageData naturalLanguageData { get; set; }

        public string botName { get; set; }
    }

    [Serializable]
    public class NaturalLanguageData
    {
        public List<Sentence> sentences { get; set; }

        public List<ConversationSubject> subjects { get; set; }
        /// <summary>
        /// confidence level that this chat has a response
        /// </summary>
        public double responseConfidence { get; set; }
        public List<ConversationSubject> responseSubjects { get; set; }
        public List<ConversationSubject> proximitySubjects { get; set; }
        public string userlessMessage { get; set; }
        public string AnalyzationVersion { get; set; }
    }

    [Serializable]
    public class ConversationSubject
    {
        public string Lemmas { get; set; }
        public int OccurenceCount { get; set; }
    }

    [Serializable]
    public class Sentence
    {
        public string Source { get; set; }
        public double Sentiment { get; set; }
        public SentenceType SentenceType { get; set; }
        public Voice Voice { get; set; }
    }

    public enum SentenceType { Unidentifiable, Declarative, Interrogative, Imperative, Exclamatory };
    public enum Voice { Unidentifiable, Active, Passive };
}
