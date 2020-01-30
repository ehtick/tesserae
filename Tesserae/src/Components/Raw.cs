﻿using Retyped;
using static Tesserae.UI;

namespace Tesserae.Components
{
    public class Raw : IComponent, IHasMarginPadding, IHasBackgroundColor
    {
        private dom.HTMLElement InnerElement;

        public Raw(dom.HTMLElement element)
        {
            InnerElement = element;
        }

        public Raw Content(IComponent component)
        {
            ClearChildren(InnerElement);
            InnerElement.appendChild(component.Render());
            return this;
        }

        public string Background { get => InnerElement.style.background; set => InnerElement.style.background = value; }
        public string Margin { get => InnerElement.style.margin; set => InnerElement.style.margin = value; }
        public string Padding { get => InnerElement.style.padding; set => InnerElement.style.padding = value; }
        public string Width { get => InnerElement.style.width; set => InnerElement.style.width = value; }
        public string Height { get => InnerElement.style.height; set => InnerElement.style.height = value; }

        public dom.HTMLElement Render()
        {
            return InnerElement;
        }
    }
}