﻿using System;
using static H5.Core.dom;
using static Tesserae.UI;

namespace Tesserae
{
    public class SidebarCommand : IComponent
    {
        private readonly Button               _button;
        private          Action<Button>       _tooltip;
        private          Func<ISidebarItem[]> _menuGenerator;
        private          bool                 _hookParentContextMenu;

        internal bool ShouldHookToContextMenu => _hookParentContextMenu;

        public SidebarCommand(UIcons icon, UIconsWeight weight = UIconsWeight.Regular) : this(Button().SetIcon(icon, weight: weight)) { }
        public SidebarCommand(Emoji  icon) : this(Button().SetIcon(icon)) { }

        public SidebarCommand(ISidebarIcon image)
        {
            _button = Button().ReplaceContent(image).Class("tss-sidebar-command");
        }

        private SidebarCommand(Button buttonWithIcon)
        {
            _button = buttonWithIcon.Class("tss-sidebar-command");
        }

        public SidebarCommand Foreground(string color)
        {
            _button.Foreground(color);
            return this;
        }

        public SidebarCommand HookToParentContextMenu()
        {
            _hookParentContextMenu = true;
            return this;
        }

        public SidebarCommand Background(string color)
        {
            _button.Background(color);
            return this;
        }

        public SidebarCommand Default()
        {
            _button.IsPrimary = false;
            return this;
        }

        public SidebarCommand Primary()
        {
            _button.IsPrimary = true;
            return this;
        }

        public SidebarCommand Success()
        {
            _button.IsSuccess = true;
            return this;
        }

        public SidebarCommand Danger()
        {
            _button.IsDanger = true;
            return this;
        }

        public SidebarCommand Tooltip(string text)
        {
            _tooltip = (b) => b.Tooltip(text, placement: TooltipPlacement.Top);
            RefreshTooltip();
            return this;
        }

        public SidebarCommand Tooltip(IComponent tooltip)
        {
            _tooltip = (b) => b.Tooltip(tooltip, placement: TooltipPlacement.Top);
            RefreshTooltip();
            return this;
        }

        public SidebarCommand Tooltip(Func<IComponent> tooltip)
        {
            _tooltip = (b) => b.Tooltip(tooltip(), placement: TooltipPlacement.Top);
            RefreshTooltip();
            return this;
        }

        public SidebarCommand OnClickMenu(Func<ISidebarItem[]> generator)
        {
            _menuGenerator = generator;
            _button.OnClick(() => ShowMenu());
            return this;
        }

        public void ShowMenu()
        {
            if (_menuGenerator is null) throw new NullReferenceException("Need to configure the menu first");

            var items = _menuGenerator();

            var menuDiv = Div(_("tss-sidebar-menu"));

            foreach (var item in items)
            {
                if (item is SidebarCommands sbc)
                {
                    menuDiv.appendChild(sbc.RenderOpenFull().Render());
                }
                else
                {
                    menuDiv.appendChild(item.RenderOpen().Render());
                }
            }

            DomObserver.WhenMounted(menuDiv, () =>
            {
                _button.Render().parentElement.classList.add("tss-sidebar-command-menu-is-open");

                DomObserver.WhenRemoved(menuDiv, () =>
                {
                    _button.Render().parentElement.classList.remove("tss-sidebar-command-menu-is-open");
                    RefreshTooltip();
                });
            });

            Tippy.ShowFor(_button.Render(), menuDiv, out Action hide, placement: TooltipPlacement.BottomStart, maxWidth: 500, delayHide: 1000, theme: "tss-sidebar-tippy");

        }

        public SidebarCommand RaiseOnClick(MouseEvent mouseEvent)
        {
            _button.RaiseOnClick(mouseEvent);
            return this;
        }

        public SidebarCommand RaiseOnContextMenu(MouseEvent mouseEvent)
        {
            _button.RaiseOnContextMenu(mouseEvent);
            return this;
        }

        public SidebarCommand OnClick(Action action)
        {
            _button.OnClick(action);
            return this;
        }

        public SidebarCommand OnContextMenu(Action action)
        {
            _button.OnContextMenu(action);
            return this;
        }

        public SidebarCommand OnClick(Action<Button, MouseEvent> action)
        {
            _button.OnClick((b, e) => action(b, e));
            return this;
        }

        public SidebarCommand OnContextMenu(Action<Button, MouseEvent> action)
        {
            _button.OnContextMenu((b, e) => action(b, e));
            return this;
        }

        public SidebarCommand SetIcon(UIcons icon, string color = "", UIconsWeight weight = UIconsWeight.Regular)
        {
            _button.SetIcon(icon, color, weight: weight);
            return this;
        }

        public SidebarCommand SetIcon(Emoji icon)
        {
            _button.SetIcon(icon);
            return this;
        }

        internal void RefreshTooltip() => _tooltip?.Invoke(_button);

        public HTMLElement Render() => _button.Render();
    }
}