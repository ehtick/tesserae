﻿using System;
using Tesserae;
using static H5.Core.dom;
using static Tesserae.UI;
using static Tesserae.Tests.Samples.SamplesHelper;
using Tesserae.Tests;

namespace Tesserae.Tests.Samples
{
    [SampleDetails(Group = "Components", Order = 20, Icon = UIcons.HorizontalRule)]
    public class HorizontalSeparatorSample : IComponent, ISample
    {
        private readonly IComponent _content;

        public HorizontalSeparatorSample()
        {
            _content = SectionStack()
               .Title(SampleHeader(nameof(HorizontalSeparatorSample)))
               .Section(Stack().Children(
                    SampleTitle("Overview"),
                    TextBlock("A separator visually separates content into groups."),
                    TextBlock("You can render content in the separator by specifying the component's children. The component's children can be plain text or a component like Icon. The content is center-aligned by default.")))
               .Section(Stack().Children(
                    SampleTitle("Best Practices"),
                    HStack().Children(
                        Stack().Width(40.percent()).Children(
                            SampleSubTitle("Do"),
                            SampleDo("Explain what is the group this separator introduces"),
                            SampleDo("Be short and concise.")
                        ),
                        Stack().Width(40.percent()).Children(
                            SampleSubTitle("Don't"),
                            SampleDont("Use long group names")))))
               .Section(Stack().Children(
                    SampleTitle("Usage"),
                    HorizontalSeparator("Center"),
                    HorizontalSeparator("Left").Left(),
                    HorizontalSeparator("Right").Right(),
                    SampleTitle("Custom Separators"),
                    HorizontalSeparator(HStack().Children(Icon(UIcons.Plane).AlignCenter().PaddingRight(8.px()), TextBlock("Custom Center").SemiBold().MediumPlus().AlignCenter())).Primary(),
                    HorizontalSeparator(HStack().Children(Icon(UIcons.Plane).AlignCenter().PaddingRight(8.px()), TextBlock("Custom Left").SemiBold().MediumPlus().AlignCenter())).Primary().Left(),
                    HorizontalSeparator(HStack().Children(Icon(UIcons.Plane).AlignCenter().PaddingRight(8.px()), TextBlock("Custom Right").SemiBold().MediumPlus().AlignCenter())).Primary().Right()));
        }

        public HTMLElement Render()
        {
            return _content.Render();
        }
    }
}