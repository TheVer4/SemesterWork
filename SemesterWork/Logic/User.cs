using System.Collections.Generic;

namespace SemesterWork
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AccessLevel { get; set; }

        public User(List<string> data) : this(data[0], data[1], data[2]) { }

        public User(string id, string name, string accessLevel)
        {
            Id = id;
            Name = name;
            AccessLevel = accessLevel;
        }
    }
}
