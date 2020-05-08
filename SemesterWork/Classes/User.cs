using System.Collections.Generic;

namespace SemesterWork
{
    public class User
    {
        public string Name { get; private set; }
        public string AccessLevel { get; private set; }

        public User(List<string> data)
        {
            Name = data[1];
            AccessLevel = data[2];
        }
    }
}
