using System;

namespace POS.ServerApi
{
    public class TitleAttribute:Attribute
    {
        public string Name { get; set; }

        public TitleAttribute(string name)
        {
            Name = name;
        }
    }
}