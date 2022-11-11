﻿namespace SharkbotDiscord.Models.Api
{
    [Serializable]
    public class UserData
    {
        private static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

        public UserData()
        {

        }

        public UserData(string name)
        {
            userName = name;
            fileName = new string(name.Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
            nickNames = new List<string>() { name };
            properties = new List<UserProperty>();
            derivedProperties = new List<UserProperty>();
        }

        public string userName { get; set; }

        public string fileName { get; set; }

        public List<string> nickNames { get; set; }

        public List<UserProperty> properties { get; set; }

        public List<UserProperty> derivedProperties { get; set; }

        public string analyzationVersion { get; set; }
    }
}
