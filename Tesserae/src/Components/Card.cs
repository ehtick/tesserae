﻿using System;
using static H5.Core.dom;
using static Tesserae.UI;

namespace Tesserae
{
    [H5.Name("tss.Card")]
    public sealed class Card : ComponentBase<Card, HTMLElement>
    {
        private readonly HTMLElement _cardContainer;
        public Card(IComponent content)
        {
            InnerElement   = Div(_("tss-card"),           content.Render());
            _cardContainer = Div(_("tss-card-container"), InnerElement);
            DomObserver.WhenMounted(InnerElement, () => InnerElement.classList.add("tss-ismounted"));
            AttachClick();
            AttachContextMenu();
        }

        /// <summary>
        /// Gets or set whenever the card is rendered in a compact form
        /// </summary>
        public bool IsCompact
        {
            get => _cardContainer.classList.contains("tss-small");
            set => _cardContainer.UpdateClassIf(value, "tss-small");
        }

        public override Card OnClick(ComponentEventHandler<Card, MouseEvent> onClick, bool clearPrevious = true)
        {
            InnerElement.style.cursor = "pointer";
            return base.OnClick(onClick, clearPrevious);
        }

        public Card OnClick(Action action) => OnClick((_, __) => action.Invoke());

        public Card SetContent(IComponent content)
        {
            ClearChildren(InnerElement);
            InnerElement.appendChild(content.Render());
            return this;
        }

        public Card Compact()
        {
            IsCompact = true;
            return this;
        }

        public Card NoAnimation()
        {
            InnerElement.classList.add("tss-noanimation", "tss-ismounted");
            return this;
        }

        public Card BackgroundColor(string color)
        {
            InnerElement.style.backgroundColor = color;
            return this;
        }

        public Card Border(string color, UnitSize size = null)
        {
            size                           = size ?? 1.px();
            InnerElement.style.borderColor = color;
            InnerElement.style.borderWidth = size.ToString();
            InnerElement.style.borderStyle = "solid";
            return this;
        }

        public Card NoPadding()
        {
            InnerElement.style.padding = "0px";
            return this;
        }

        public Card HoverColor(bool enabled = true)
        {
            if (enabled)
            {
                InnerElement.classList.add("tss-card-hover");
            }
            else
            {
                InnerElement.classList.remove("tss-card-hover");
            }
            return this;
        }

        public override HTMLElement Render()
        {
            return _cardContainer;
        }
    }
}