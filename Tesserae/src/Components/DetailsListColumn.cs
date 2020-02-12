using System;
using Tesserae.Helpers.HTML;
using static Tesserae.UI;
using static Retyped.dom;

namespace Tesserae.Components
{
    public class DetailsListColumn : IDetailsListColumn
    {
        public DetailsListColumn(
            string name,
            WidthDimension minWidth,
            WidthDimension maxWidth,
            Action onColumnClick = null)
        {
            Name          = name;
            MinWidth      = minWidth;
            MaxWidth      = maxWidth;
            OnColumnClick = onColumnClick;
        }

        public string Name             { get; }

        public WidthDimension MinWidth { get; }

        public WidthDimension MaxWidth { get; }

        public Action OnColumnClick    { get; }

        public HTMLElement Render()
        {
            var htmlElement = Div(_());

            htmlElement.innerText = Name;

            return htmlElement;
        }
    }
}
