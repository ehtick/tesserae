﻿using System;
using static H5.Core.dom;
using static Tesserae.UI;
using static Tesserae.Tests.Samples.SamplesHelper;
using System.Collections.Generic;
using System.Linq;
using Tesserae.Tests;

namespace Tesserae.Tests.Samples
{
    [SampleDetails(Group = "Utilities", Order = 10, Icon = UIcons.Picture)]
    public class UIconsSample : IComponent, ISample
    {
        private readonly IComponent _content;

        public UIconsSample()
        {
            //TODO: Add dropwdown to select icon weight
            _content = SectionStack().S()
               .Title(SampleHeader(nameof(UIconsSample)))
               .Section(Stack().Children(
                    SampleTitle("Overview"),
                    TextBlock("Tesserae integrates the UIcons icons as part of the package, with an auto-generated strongly typed enum for them.")))
               .Section(Stack().Children(
                    SampleTitle($"enum {nameof(UIcons)}:"),
                    SearchableList(GetAllIcons().ToArray(), 25.percent(), 25.percent(), 25.percent(), 25.percent())).S(), grow: true);
        }

        public HTMLElement Render()
        {
            return _content.Render();
        }

        private IEnumerable<IconItem> GetAllIcons()
        {
            var      names  = Enum.GetNames(typeof(UIcons));
            UIcons[] values = (UIcons[])Enum.GetValues(typeof(UIcons));

            for (int i = 0; i < values.Length; i++)
            {
                yield return new IconItem(values[i], names[i]);
            }
        }

        private class IconItem : ISearchableItem
        {
            private readonly string     _value;
            private readonly IComponent component;
            public IconItem(UIcons icon, string name)
            {
                name   = ToValidName(name.Substring(6));
                _value = name + " " + icon.ToString();

                component = HStack().WS().AlignItemsCenter().PB(4).Children(
                    Icon(icon, size: TextSize.Large).MinWidth(36.px()),
                    TextBlock($"{name}").Ellipsis().Title(icon.ToString()).W(1).Grow());

            }

            public bool IsMatch(string searchTerm) => _value.Contains(searchTerm);

            public IComponent Render() => component;
        }


        //Copy of the logic in the generator code, as we don't have the enum names anymore on  Enum.GetNames(typeof(LineAwesome))
        private static string ToValidName(string icon)
        {
            var words = icon.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(i => i.Substring(0, 1).ToUpper() + i.Substring(1))
               .ToArray();

            var name = string.Join("", words);

            if (char.IsDigit(name[0]))
            {
                return "_" + name;
            }
            else
            {
                return name;
            }
        }
    }
}