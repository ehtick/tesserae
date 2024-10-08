﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H5.Core;
using static H5.Core.dom;
using static Tesserae.UI;

namespace Tesserae
{
    [H5.Name("tss.SearchableList")]
    public class SearchableList<T> : IComponent, ISpecialCaseStyling where T : ISearchableItem
    {
        private readonly IDefer           _defered;
        private readonly Stack            _searchBoxContainer;
        private readonly List<IComponent> _searchBoxContainerComponents;
        private readonly Stack            _stack;
        private readonly SearchBox        _searchBox;
        private readonly ItemsList        _list;

        private int               _minimumItemsToShowBox = 0;
        private Func<string, Task<T[]>> _backgroundSearcher;

        public  HTMLElement       StylingContainer           => _stack.InnerElement;
        public  bool              PropagateToStackItemParent => true;
        public  ObservableList<T> Items                      { get; }

        public bool ShowNotMatchingItems { get; set; }
        public SearchableList(T[] items, params UnitSize[] columns) : this(new ObservableList<T>(initialValues: items ?? new T[0]), columns)
        {

        }

        public SearchableList(ObservableList<T> items, params UnitSize[] columns)
        {
            Items      = items ?? new ObservableList<T>();
            _searchBox = new SearchBox().Underlined().SetPlaceholder("Type to search").SearchAsYouType().Width(100.px()).Grow();
            _list      = ItemsList(new IComponent[0], columns);
            object marker;
            _defered =
                DeferSync(
                        Items,
                        item =>
                        {
                            var searchTerms = (_searchBox.Text ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            var includeItems = new List<IComponent>();
                            var includeItemsBackground = new List<IComponent>();
                            var excludeItems = new List<IComponent>();

                            foreach (var i in Items)
                            {
                                if (searchTerms.Length == 0 || searchTerms.All(st => i.IsMatch(st)))
                                {
                                    includeItems.Add(i.Render().RemoveClass("tss-searchable-list-no-match"));
                                }
                                else if (ShowNotMatchingItems)
                                {
                                    excludeItems.Add(i.Render().Class("tss-searchable-list-no-match"));
                                }
                            }

                            if (_backgroundSearcher is object && !string.IsNullOrEmpty(_searchBox.Text))
                            {
                                var markerLocal = new object();
                                marker = markerLocal;
                                _backgroundSearcher(_searchBox.Text).ContinueWith(t =>
                                {
                                    if (markerLocal != marker) return;
                                    if (t.IsCompleted)
                                    {
                                        foreach(var bi in t.Result)
                                        {
                                            includeItemsBackground.Add(bi.Render().RemoveClass("tss-searchable-list-no-match"));
                                        }

                                        var filteredItemsWithBackground = includeItems.Concat(includeItemsBackground)
                                                                                      .Concat(excludeItems).ToArray();

                                        _list.Items.Clear();

                                        if (filteredItemsWithBackground.Any())
                                        {
                                            _list.Items.AddRange(filteredItemsWithBackground);
                                        }

                                        _searchBox.Show();
                                    }
                                }).FireAndForget();
                            }

                            var filteredItems = includeItems.Concat(excludeItems).ToArray();

                            _list.Items.Clear();

                            if (filteredItems.Any())
                            {
                                _list.Items.AddRange(filteredItems);
                            }

                            if (filteredItems.Length >= _minimumItemsToShowBox || _backgroundSearcher is object)
                            {
                                _searchBox.Show();
                            }
                            else
                            {
                                _searchBox.Collapse();
                            }

                            return _list.S();
                        }
                    )
                   .WS()
                   .Grow(1);

            _searchBox.OnSearch((_, __) => _defered.Refresh());
            _searchBoxContainer           = Stack().Horizontal().WS().Children(_searchBox).AlignItems(ItemAlign.Center);
            _searchBoxContainerComponents = new List<IComponent>() { _searchBox };
            _stack                        = Stack().Children(_searchBoxContainer, _defered.Scroll()).WS().MaxHeight(100.percent());
        }

        public SearchableList<T> WithNoResultsMessage(Func<IComponent> emptyListMessageGenerator)
        {
            _list.WithEmptyMessage(emptyListMessageGenerator ?? throw new ArgumentNullException(nameof(emptyListMessageGenerator)));
            _defered.Refresh();
            return this;
        }

        public SearchableList<T> SearchBox(Action<SearchBox> sb)
        {
            sb(_searchBox);
            return this;
        }

        public SearchableList<T> CaptureSearchBox(out SearchBox sb)
        {
            sb = _searchBox;
            return this;
        }

        public SearchableList<T> WithBackgroundSearch(Func<string, Task<T[]>> searcher)
        {
            _backgroundSearcher = searcher;
            _minimumItemsToShowBox = 0;
            return this;
        }

        public SearchableList<T> HideSearchBoxIfLessThan(int items)
        {
            _minimumItemsToShowBox = items;
            return this;
        }

        public SearchableList<T> ShowNotMatching()
        {
            ShowNotMatchingItems = true;
            return this;
        }

        public SearchableList<T> BeforeSearchBox(params IComponent[] beforeComponents)
        {
            foreach (var component in beforeComponents.Reverse<IComponent>())
            {
                _searchBoxContainerComponents.Insert(0, component);
            }
            _searchBoxContainer.Children(_searchBoxContainerComponents);
            return this;
        }

        public SearchableList<T> AfterSearchBox(params IComponent[] afterComponents)
        {
            _searchBoxContainerComponents.AddRange(afterComponents);
            _searchBoxContainer.Children(_searchBoxContainerComponents);
            return this;
        }

        public dom.HTMLElement Render() => _stack.Render();
    }

    [H5.Name("tss.ISearchableItem")]
    public interface ISearchableItem
    {
        bool       IsMatch(string searchTerm);
        IComponent Render();
    }
}