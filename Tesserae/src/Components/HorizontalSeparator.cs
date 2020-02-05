﻿using Retyped;
using Tesserae;
using static Tesserae.UI;
using static Retyped.dom;

namespace Tesserae.Components
{
    public class HorizontalSeparator : IComponent
    {
        private HTMLElement _container;
        private HTMLElement _separator;

        public Align Alignment
        {
            get
            {
                if (_container.classList.contains("left")) return Align.Left;
                if (_container.classList.contains("right")) return Align.Right;
                return Align.Center;
            }
            set
            {
                _container.classList.remove("left");
                _container.classList.remove("right");
                if (value == Align.Left) _container.classList.add("left");
                if (value == Align.Right) _container.classList.add("right");
                //Center is the default, no need for class
            }
        }

        public string Text
        {
            get
            {
                return _separator.textContent;
            }
            set
            {
                _separator.textContent = value ?? "";
            }
        }

        public HorizontalSeparator(string text = string.Empty)
        {
            _separator = Div(_("tss-horizontalseparator"));
            _container = Div(_("tss-horizontalseparator-container"), _separator);
            Text = text;
        }

        public HorizontalSeparator(IComponent component)
        {
            _separator = Div(_("tss-horizontalseparator"));
            _separator.appendChild(component.Render());
            _container = Div(_("tss-horizontalseparator-container"), _separator);
        }

        public HorizontalSeparator SetContent(IComponent component)
        {
            ClearChildren(_separator);
            _separator.appendChild(component.Render());
            return this;
        }

        public HorizontalSeparator Primary()
        {
            _separator.classList.add("primary");
            return this;
        }

        public HorizontalSeparator SetText(string text)
        {
            Text = text;
            return this;
        }

        public HTMLElement Render()
        {
            return _container;
        }

        public enum Align
        {
            Left,
            Center,
            Right
        }

        public HorizontalSeparator Left()
        {
            Alignment = Align.Left;
            return this;
        }

        public HorizontalSeparator Center()
        {
            Alignment = Align.Center;
            return this;
        }

        public HorizontalSeparator Right()
        {
            Alignment = Align.Right;
            return this;
        }
    }
}