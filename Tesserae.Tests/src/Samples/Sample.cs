﻿using System;

namespace Tesserae.Tests
{
    internal class Sample
    {
        public string           Type             { get; }
        public string           Name             { get; }
        public string           Group            { get; }
        public int              Order            { get; }
        public UIcons           Icon             { get; }
        public Func<IComponent> ContentGenerator { get; }

        public Sample(string type, string name, string group, int order, UIcons icon, Func<IComponent> contentGenerator)
        {
            Type             = type;
            Name             = name;
            Group            = group;
            Order            = order;
            Icon             = icon;
            ContentGenerator = contentGenerator;
        }
    }
}