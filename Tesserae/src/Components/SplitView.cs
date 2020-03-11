﻿using System;
using static Tesserae.UI;
using static Retyped.dom;
using System.Threading.Tasks;

namespace Tesserae.Components
{
    public class SplitView : IComponent
    {
        private HTMLElement InnerElement;
        private Raw         LeftComponent;
        private Raw         SplitterComponent;
        private Raw         RightComponent;

        public SplitView(UnitSize splitterSize = null)
        {
            LeftComponent     = Raw(Div(_()));
            SplitterComponent = Raw(Div(_("tss-splitter")));
            RightComponent    = Raw(Div(_()));
            SplitterComponent.Width = (splitterSize is object && splitterSize.Unit != Unit.Auto && splitterSize.Unit != Unit.Inherit)
                                          ? splitterSize.ToString()
                                          : "8px";
            LeftComponent.Height     = "100%";
            SplitterComponent.Height = "100%";
            RightComponent.Height    = "100%";
            SplitInMiddle();
            InnerElement = Div(_("tss-splitview"), LeftComponent.Render(), SplitterComponent.Render(), RightComponent.Render());
        }

        public SplitView Left(IComponent component, string background = "")
        {
            LeftComponent.Content(component);

            LeftComponent.Background = background;
            return this;
        }

        public SplitView Right(IComponent component, string background = "")
        {
            RightComponent.Content(component);

            RightComponent.Background = background;
            return this;
        }

        public SplitView Splitter(IComponent component)
        {
            SplitterComponent.Content(component);
            return this;
        }

        public SplitView NoSplitter()
        {
            SplitterComponent.Width = "0px";
            return this;
        }

        public SplitView SplitInMiddle()
        {
            LeftComponent.Width  = $"calc(50% - {SplitterComponent.Width})";
            RightComponent.Width = "50%";
            return this;
        }


        public SplitView LeftIsSmaller(UnitSize value)
        {
            if (value.Unit == Unit.Percent)
            {
                LeftComponent.Width  = $"calc({value.Size}% - {SplitterComponent.Width})";
                RightComponent.Width = $"{100 - value.Size}%";
            }
            else if (value.Unit == Unit.Pixels)
            {
                LeftComponent.Width  = $"calc({value.Size}px - {SplitterComponent.Width})";
                RightComponent.Width = $"calc(100% - {value.Size}px)";
            }
            else
            {
                throw new NotImplementedException($"Size for unit {value.Unit} not implemented");
            }

            return this;
        }

        public SplitView LeftIsSmaller(SizeMode mode = SizeMode.Percent, int value = 30)
        {
            if (mode == SizeMode.Percent)
            {
                LeftComponent.Width  = $"calc({value}% - {SplitterComponent.Width})";
                RightComponent.Width = $"{100 - value}%";
            }
            else
            {
                LeftComponent.Width  = $"calc({value}px - {SplitterComponent.Width})";
                RightComponent.Width = $"calc(100% - {value}px)";
            }

            return this;
        }


        public SplitView RightIsSmaller(UnitSize value)
        {
            if (value.Unit == Unit.Percent)
            {
                RightComponent.Width = $"calc({value.Size}% - {SplitterComponent.Width})";
                LeftComponent.Width  = $"{100 - value.Size}%";
            }
            else if (value.Unit == Unit.Pixels)
            {
                RightComponent.Width = $"calc({value.Size}px - {SplitterComponent.Width})";
                LeftComponent.Width  = $"calc(100% - {value.Size}px)";
            }
            else
            {
                throw new NotImplementedException($"Size for unit {value.Unit} not implemented");
            }

            return this;
        }

        public SplitView RightIsSmaller(SizeMode mode = SizeMode.Percent, int value = 30)
        {
            if (mode == SizeMode.Percent)
            {
                RightComponent.Width = $"calc({value}% - {SplitterComponent.Width})";
                LeftComponent.Width  = $"{100 - value}%";
            }
            else
            {
                RightComponent.Width = $"calc({value}px - {SplitterComponent.Width})";
                LeftComponent.Width  = $"calc(100% - {value}px)";
            }
            return this;
        }

        public HTMLElement Render()
        {
            return InnerElement;
        }
    }

    public enum SizeMode
    {
        Pixels,
        Percent
    }
}
