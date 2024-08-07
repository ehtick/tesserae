using System;
using System.Collections.Generic;
using System.Linq;
using static H5.Core.dom;

namespace Tesserae
{
    [H5.Name("tss.ComponentCache")]
    public class ComponentCache<TComponent> : ComponentCacheBase<TComponent> where TComponent : class
    {
        private readonly Func<(int Key, TComponent Component), HTMLElement> _createComponentExpression;

        private readonly List<(int Key, HTMLElement HtmlElement)> _componentCache;

        public ComponentCache(Func<(int Key, TComponent Component), HTMLElement> createComponentExpression)
        {
            _createComponentExpression = createComponentExpression ?? throw new ArgumentNullException(nameof(createComponentExpression));

            _componentCache = new List<(int Key, HTMLElement HtmlElement)>();
        }

        public ComponentCache<TComponent> AddComponents(IEnumerable<TComponent> components)
        {
            AddToComponents(components);

            return this;
        }

        public IEnumerable<HTMLElement> GetAllRenderedComponentsFromCache()
        {
            foreach (var componentAndKey in _componentsAndKeys)
            {
                var (Key, HtmlElement) = _componentCache.SingleOrDefault(component => component.Key == componentAndKey.Key);

                if (HtmlElement != null)
                {
                    yield return HtmlElement;
                }
                else
                {
                    var htmlElement = _createComponentExpression(componentAndKey);

                    _componentCache.Add((componentAndKey.Key, htmlElement));

                    yield return htmlElement;
                }
            }
        }

        public ComponentCache<TComponent> SortComponents(Comparison<TComponent> comparison)
        {
            if (HasComponents)
            {
                _componentsAndKeys.Sort((componentAndKey, otherComponentAndKey) => comparison(componentAndKey.Component, otherComponentAndKey.Component));
            }

            return this;
        }

        public ComponentCache<TComponent> ReverseComponentOrder()
        {
            _componentsAndKeys.Reverse();

            return this;
        }

        public ComponentCache<TComponent> Clear()
        {
            _componentsAndKeys.Clear();
            _componentCache.Clear();

            return this;
        }
    }
}