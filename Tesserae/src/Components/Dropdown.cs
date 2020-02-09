﻿using static Tesserae.UI;
using static Retyped.dom;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Retyped;

namespace Tesserae.Components
{
    public class Dropdown : Layer, IContainer<Dropdown, Dropdown.Item>, ICanValidate<Dropdown>
    {
        private readonly HTMLElement _childContainer;

        private readonly HTMLDivElement _container;
        private readonly HTMLSpanElement _errorSpan;

        private HTMLDivElement _spinner;

        private bool _isChanged;
        private bool _callSelectOnAdd = true;

        private List<Item> _selectedChildren;

        public Dropdown()
        {
            InnerElement = Div(_("tss-dropdown"));
            _errorSpan = Span(_("tss-dropdown-error"));

            _container = Div(_("tss-dropdown-container"), InnerElement, _errorSpan);

            _childContainer = Div(_());

            InnerElement.onclick = (e) =>
            {
                if (!IsVisible) Show();
            };
            _selectedChildren = new List<Item>();
        }

        public Dropdown SuppressSelectedOnAddingItem()
        { 
            _callSelectOnAdd = false;
            return this;
        }

        public SelectMode Mode
        {
            get { return _childContainer.classList.contains("tss-dropdown-multi") ? SelectMode.Multi : SelectMode.Single; }
            set
            {
                if (value != Mode)
                {
                    if (value == SelectMode.Single)
                    {
                        _childContainer.classList.remove("tss-dropdown-multi");

                    }
                    else
                    {
                        _childContainer.classList.add("tss-dropdown-multi");
                    }
                }
            }
        }

        public Item[] SelectedItems { get { return _selectedChildren.ToArray(); } }

        public string SelectedText
        {
            get
            {
                return string.Join(", ", _selectedChildren.Select(x => x.Text));
            }
        }

        public string Error
        {
            get { return _errorSpan.innerText; }
            set
            {
                if (_errorSpan.innerText != value)
                {
                    _errorSpan.innerText = value;
                }
            }
        }

        public bool IsInvalid
        {
            get { return _container.classList.contains("invalid"); }
            set
            {
                if (value != IsInvalid)
                {
                    if (value)
                    {
                        _container.classList.add("invalid");
                    }
                    else
                    {
                        _container.classList.remove("invalid");
                    }
                }
            }
        }

        public bool IsEnabled
        {
            get { return !InnerElement.classList.contains("disabled"); }
            set
            {
                if (value != IsEnabled)
                {
                    if (value)
                    {
                        InnerElement.classList.remove("disabled");
                    }
                    else
                    {
                        InnerElement.classList.add("disabled");
                    }
                }
            }
        }

        public bool IsRequired
        {
            get { return _container.classList.contains("tss-required"); }
            set
            {
                if (value != IsInvalid)
                {
                    if (value)
                    {
                        _container.classList.add("tss-required");
                    }
                    else
                    {
                        _container.classList.remove("tss-required");
                    }
                }
            }
        }

        public Func<Task<Item[]>> ItemsSource { get; set; }

        public void Clear()
        {
            ClearChildren(_childContainer);
        }

        public void Replace(Item newComponent, Item oldComponent)
        {
            _childContainer.replaceChild(newComponent.Render(), oldComponent.Render());
        }
        
        public void Add(Item component)
        {
            _childContainer.appendChild(component.Render());
            component.onSelectedChange += OnItemSelected;

            if (component.IsSelected)
            {
                OnItemSelected(null, component);
            }
        }

        public override HTMLElement Render()
        {
            return _container;
        }

        public async Task LoadItemsAsync()
        {
            if (ItemsSource is null) throw new InvalidOperationException("Only valid with async items");

            _spinner = Div(_("tss-spinner"));
            _container.appendChild(_spinner);
            _container.style.pointerEvents = "none";
            Items(await ItemsSource());
            _container.removeChild(_spinner);
            _container.style.pointerEvents = "unset";
        }

        public override void Show()
        {
            if (_contentHtml == null)
            {
                _contentHtml = Div(_("tss-dropdown-popup"), _childContainer);
                if (ItemsSource != null)
                {
                    LoadItemsAsync().ContinueWith(t => Show()).FireAndForget();
                    return;
                }
            }

            _contentHtml.style.height = "unset";
            _contentHtml.style.left = "-1000px";
            _contentHtml.style.top = "-1000px";

            base.Show();

            _isChanged = false;

            if (!_contentHtml.classList.contains("no-focus")) _contentHtml.classList.add("no-focus");

            ClientRect rect = (ClientRect)InnerElement.getBoundingClientRect();
            var contentRect = (ClientRect)_contentHtml.getBoundingClientRect();
            _contentHtml.style.top = rect.bottom - 1 + "px";
            _contentHtml.style.minWidth = rect.width + "px";

            var finalLeft = rect.left;
            if(rect.left + contentRect.width + 1 > window.innerWidth)
            {
                finalLeft = window.innerWidth - contentRect.width - 1;
            }

            _contentHtml.style.left = finalLeft + "px";

            if (window.innerHeight - rect.bottom - 1 < contentRect.height)
            {
                var top = rect.top - contentRect.height;
                if (top < 0)
                {
                    if (rect.top > window.innerHeight - rect.bottom - 1)
                    {
                        _contentHtml.style.top = "1px";
                        _contentHtml.style.height = rect.top - 1 + "px";
                    }
                    else
                    {
                        _contentHtml.style.height = window.innerHeight - rect.bottom - 1 + "px";
                    }
                }
                else
                {
                    _contentHtml.style.top = top + "px";
                }
            }

            window.setTimeout((e) =>
            {
                document.addEventListener("click", OnWindowClick);
                document.addEventListener("dblclick", OnWindowClick);
                document.addEventListener("contextmenu", OnWindowClick);
                document.addEventListener("wheel", OnWindowClick);
                document.addEventListener("keydown", OnPopupKeyDown);
                if (_selectedChildren.Count > 0) _selectedChildren[_selectedChildren.Count - 1].Render().focus();
            }, 100);
        }

        public override void Hide(Action onHidden = null)
        {
            document.removeEventListener("click", OnWindowClick);
            document.removeEventListener("dblclick", OnWindowClick);
            document.removeEventListener("contextmenu", OnWindowClick);
            document.removeEventListener("wheel", OnWindowClick);
            document.removeEventListener("keydown", OnPopupKeyDown);
            base.Hide(onHidden);
            if (_isChanged) RaiseOnChange(this);
        }

        public void Attach(EventHandler<Event> handler, Validation.Mode mode)
        {
            if (mode == Validation.Mode.OnBlur)
            {
                onChange += (s, e) => handler(this, e);
            }
            else
            {
                onInput += (s, e) => handler(this, e);
            }
        }

        public Dropdown Single()
        {
            Mode = SelectMode.Single;
            return this;
        }

        public Dropdown Multi()
        {
            Mode = SelectMode.Multi;
            return this;
        }

        public Dropdown NoBorder()
        {
            InnerElement.style.border = "none";
            return this;
        }

        public Dropdown Items(params Item[] children)
        {
            children.ForEach(x => Add(x));
            return this;
        }
        public Dropdown Items(Func<Task<Item[]>> itemsSource)
        {
            ItemsSource = itemsSource;
            return this;
        }

        public Dropdown Disabled()
        {
            IsEnabled = false;
            return this;
        }

        public Dropdown Required()
        {
            IsRequired = true;
            return this;
        }

        private void OnWindowClick(Event e)
        {
            if (e.srcElement != _childContainer && !_childContainer.contains(e.srcElement)) Hide();
        }

        private void OnItemSelected(object sender, Item e)
        {
            if (Mode == SelectMode.Single && !e.IsSelected) return;

            if (Mode == SelectMode.Single)
            {
                if (_selectedChildren.Count > 0)
                {
                    foreach (var selectedChild in _selectedChildren.ToArray()) //Need to copy as setting IsSelected will get back here
                    {
                        if(selectedChild != e)
                        {
                            selectedChild.IsSelected = false;
                        }
                    }

                    _selectedChildren.Clear();
                }

                _selectedChildren.Add(e);

                Hide();
            }
            else
            {
                if (_selectedChildren.Contains(e))
                {
                    _selectedChildren.Remove(e);
                }
                else
                {
                    _selectedChildren.Add(e);
                }

                _isChanged = true;
            }

            RenderSelected();

            if (_callSelectOnAdd)
            {
                RaiseOnInput(this);
            }
        }

        private void RenderSelected()
        {
            ClearChildren(InnerElement);

            for (int i = 0; i < SelectedItems.Length; i++)
            {
                Item sel = SelectedItems[i];
                var clone = (HTMLElement)(sel.Render().cloneNode(true));
                clone.classList.remove("tss-dropdown-item");
                clone.classList.remove("selected");
                clone.classList.add("tss-dropdown-item-on-box");
                InnerElement.appendChild(clone);
            }
        }

        private void OnPopupKeyDown(Event e)
        {
            var ev = e as KeyboardEvent;
            if (ev.key == "ArrowUp")
            {
                if (_contentHtml.classList.contains("no-focus")) _contentHtml.classList.remove("no-focus");
                if (document.activeElement != null && _childContainer.contains(document.activeElement))
                {
                    var el = (_childContainer.children.TakeWhile(x => !x.Equals(document.activeElement)).LastOrDefault(x => (x as HTMLElement).tabIndex != -1) as HTMLElement);
                    if (el != null) el.focus();
                    else (_childContainer.children.Last(x => (x as HTMLElement).tabIndex != -1) as HTMLElement).focus();
                }
                else
                {
                    (_childContainer.children.Last(x => (x as HTMLElement).tabIndex != -1) as HTMLElement).focus();
                }
            }
            else if (ev.key == "ArrowDown")
            {
                if (_contentHtml.classList.contains("no-focus")) _contentHtml.classList.remove("no-focus");
                if (document.activeElement != null && _childContainer.contains(document.activeElement))
                {
                    var el = (_childContainer.children.SkipWhile(x => !x.Equals(document.activeElement)).Skip(1).FirstOrDefault(x => (x as HTMLElement).tabIndex != -1) as HTMLElement);
                    if (el != null) el.focus();
                    else (_childContainer.children.First(x => (x as HTMLElement).tabIndex != -1) as HTMLElement).focus();
                }
                else
                {
                    (_childContainer.children.First(x => (x as HTMLElement).tabIndex != -1) as HTMLElement).focus();
                }
            }
        }

        public enum SelectMode
        {
            Single,
            Multi
        }

        public enum ItemType
        {
            Item,
            Header,
            Divider
        }

        public class Item : ComponentBase<Item, HTMLButtonElement>
        {
            public Item(string text = string.Empty)
            {
                InnerElement = Button(_("tss-dropdown-item", text: text));
                InnerElement.addEventListener("click", OnItemClick);
                InnerElement.addEventListener("mouseover", OnItemMouseOver);
            }

            public Item(IComponent content)
            {
                InnerElement = Button(_("tss-dropdown-item"));
                InnerElement.appendChild(content.Render());
                InnerElement.addEventListener("click", OnItemClick);
                InnerElement.addEventListener("mouseover", OnItemMouseOver);
            }


            public event BeforeSelectEventHandler<Item> onBeforeSelected;
            internal event EventHandler<Item> onSelectedChange;

            public ItemType Type
            {
                get
                {
                    if (InnerElement.classList.contains("tss-dropdown-item")) return ItemType.Item;
                    if (InnerElement.classList.contains("tss-dropdown-header")) return ItemType.Header;
                    return ItemType.Divider;
                }

                set
                {
                    if (value != Type)
                    {
                        InnerElement.classList.remove($"tss-dropdown-{Type.ToString().ToLower()}");
                        InnerElement.classList.add($"tss-dropdown-{value.ToString().ToLower()}");

                        if (value == ItemType.Item) InnerElement.tabIndex = 0;
                        else InnerElement.tabIndex = -1;
                    }
                }
            }

            public bool IsEnabled
            {
                get { return !InnerElement.classList.contains("disabled"); }
                set
                {
                    if (value != IsEnabled)
                    {
                        if (value)
                        {
                            InnerElement.classList.remove("disabled");
                            if (Type == ItemType.Item) InnerElement.tabIndex = 0;
                        }
                        else
                        {
                            InnerElement.classList.add("disabled");
                            InnerElement.tabIndex = -1;
                        }
                    }
                }
            }

            public bool IsSelected
            {
                get { return InnerElement.classList.contains("selected"); }
                set
                {
                    if (value != IsSelected)
                    {
                        if(value && onBeforeSelected is object)
                        {
                            var shouldSelect = onBeforeSelected(this);
                            if (!shouldSelect) return;
                        }

                        if (value) InnerElement.classList.add("selected");
                        else InnerElement.classList.remove("selected");

                        onSelectedChange?.Invoke(this, this);
                    }
                }
            }

            public string Text
            {
                get { return InnerElement.innerText; }
                set { InnerElement.innerText = value; }
            }

            public override HTMLElement Render()
            {
                return InnerElement;
            }

            public Item Header()
            {
                Type = ItemType.Header;
                return this;
            }
            public Item Divider()
            {
                Type = ItemType.Divider;
                return this;
            }
            public Item Disabled()
            {
                IsEnabled = false;
                return this;
            }

            public Item Selected()
            {
                IsSelected = true;
                return this;
            }

            public Item OnSelected(EventHandler<Item> onSelected, EventHandler<Item> onDeselected = null)
            {
                this.onSelectedChange += (s,e) => 
                {
                    if(e.IsSelected)
                    {
                        onSelected?.Invoke(s, e);
                    }
                    else
                    {
                        onDeselected?.Invoke(s, e);
                    }
                };
                return this;
            }

            public Item OnBeforeSelected(BeforeSelectEventHandler<Item> onBeforeSelect)
            {
                onBeforeSelected += onBeforeSelect;
                return this;
            }

            private void OnItemClick(Event e)
            {
                if (Type == ItemType.Item)
                {
                    if (InnerElement.parentElement.classList.contains("tss-dropdown-multi")) IsSelected = !IsSelected;
                    else IsSelected = true;
                }
            }

            private void OnItemMouseOver(Event ev)
            {
                if (Type == ItemType.Item) InnerElement.focus();
            }
        }
    }
}
