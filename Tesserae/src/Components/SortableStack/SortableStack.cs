using System;
using System.Collections.Generic;
using System.Linq;
using H5.Core;
using static H5.Core.dom;

namespace Tesserae
{
    public class SortableStack : IComponent, IHasBackgroundColor, IHasMarginPadding, ISpecialCaseStyling, ICanWrap
    {
        private Stack                   _container;
        private List<SortableStackItem> _items     = new List<SortableStackItem>();
        private List<string>            _itemOrder = new List<string>();
        private Action<string[]>        _onSortingChanged;
        public SortableStack(Stack.Orientation orientation = Stack.Orientation.Vertical)
        {
            _container = new Stack(orientation);

            var sortable = new Sortable(_container.Render(), new SortableOptions()
            {
                animation  = 150,
                invertSwap = true,
                ghostClass = "tss-sortable-ghost",
                onEnd = e =>
                {
                    if (e.oldIndex != e.newIndex)
                    {
                        var old = _itemOrder[e.oldIndex];
                        _itemOrder.RemoveAt(e.oldIndex);
                        _itemOrder.Insert(e.newIndex, old);
                        _onSortingChanged?.Invoke(GetCurrentSorting());
                    }
                }
            });
        }
        public void Add(string identifier, IComponent component)
        {
            _container.Add((IComponent)component);
            _items.Add(new SortableStackItem { Component = component, Identifier = identifier });
            _itemOrder.Add(identifier);
        }
        public void Add(SortableStackItem item)
        {
            _container.Add(item.Component);
            _items.Add(item);
            _itemOrder.Add(item.Identifier);
        }
        public SortableStack Children(params SortableStackItem[] children)
        {
            _container.Clear();
            _items.Clear();

            foreach (var item in children)
            {
                Add(item);
            }
            return this;
        }
        public dom.HTMLElement Render()
        {
            return _container.Render();
        }
        public SortableStack AlignItemsCenter()
        {
            _container.AlignItemsCenter();
            return this;
        }

        //Should be called after all items have been added
        public void LoadSorting(string[] itemOrder)
        {
            _itemOrder = itemOrder.ToList();

            Refresh();
        }
        private void Refresh()
        {
            _container.Clear();

            foreach (var item in _items.OrderBy(i => _itemOrder.IndexOf(i.Identifier)))
            {
                _container.Add(item.Component);
            }
        }
        private string[] GetCurrentSorting()
        {
            return _itemOrder.ToArray();
        }
        public void OnSortingChanged(Action<string[]> onSortingChanged)
        {
            _onSortingChanged = onSortingChanged;
        }
        public string Background
        {
            get => _container.Background;
            set => _container.Background = value;
        }
        public string Margin
        {
            get => _container.Margin;
            set => _container.Margin = value;
        }
        public string Padding
        {
            get => _container.Padding;
            set => _container.Padding = value;
        }
        public dom.HTMLElement StylingContainer           => _container.StylingContainer;
        public bool            PropagateToStackItemParent => _container.PropagateToStackItemParent;
        public bool CanWrap
        {
            get => _container.CanWrap;
            set => _container.CanWrap = value;
        }
        public Stack AlignItems(ItemAlign align)                                  => _container.AlignItems(align);
        public Stack Relative()                                                   => _container.Relative();
        public Stack AlignContent(ItemAlign     align)                            => _container.AlignContent(align);
        public Stack JustifyContent(ItemJustify justify)                          => _container.JustifyContent(justify);
        public Stack JustifyItems(ItemJustify   justify)                          => _container.JustifyItems(justify);
        public Stack RemovePropagation()                                          => _container.RemovePropagation();
        public Stack OnMouseOver(ComponentEventHandler<Stack, Event> onMouseOver) => _container.OnMouseOver(onMouseOver);
        public Stack OnMouseOut(ComponentEventHandler<Stack, Event>  onMouseOut)  => _container.OnMouseOut(onMouseOut);
        public void Clear()
        {
            _items.Clear();
            _itemOrder.Clear();
            _container.Clear();
        }
        public Stack NoDefaultMargin() => _container.NoDefaultMargin();

        public void Remove(string identifer)
        {
            _itemOrder.Remove(identifer);

            foreach (var i in _items)
            {
                if (i.Identifier != identifer)
                {
                    _container.Remove(i.Component);
                }
            }

            _items = _items.Where(i => i.Identifier != identifer).ToList();
        }


    }

    public class SortableStackItem
    {
        public IComponent Component  { get; set; }
        public string     Identifier { get; set; }
    }
}