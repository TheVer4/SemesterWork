using System.Collections.Generic;

namespace SemesterWork
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AccessLevel { get; set; }

        public User(List<string> data)
        {
            Id = data[0];
            Name = data[1];
            AccessLevel = data[2];
        }
    }
}
