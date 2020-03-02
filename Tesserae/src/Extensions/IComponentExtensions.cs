﻿namespace Tesserae.Components
{
    public static class IComponentExtensions
    {
        public static T AlignAuto<T>(this T component) where T : IComponent
        {
            Stack.SetAlign(component, ItemAlign.Auto);
            return component;
        }

        public static T AlignStretch<T>(this T component) where T : IComponent
        {
            Stack.SetAlign(component, ItemAlign.Stretch);
            return component;
        }
        
        public static T AlignBaseline<T>(this T component) where T : IComponent
        {
            Stack.SetAlign(component, ItemAlign.Baseline);
            return component;
        }
        
        public static T AlignStart<T>(this T component) where T : IComponent
        {
            Stack.SetAlign(component, ItemAlign.Start);
            return component;
        }
        
        public static T AlignCenter<T>(this T component) where T : IComponent
        {
            Stack.SetAlign(component, ItemAlign.Center);
            return component;
        }

        public static T AlignEnd<T>(this T component) where T : IComponent
        {
            Stack.SetAlign(component, ItemAlign.End);
            return component;
        }

        public static T Margin<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetMarginLeft(component, unitSize);
            Stack.SetMarginRight(component, unitSize);
            Stack.SetMarginTop(component, unitSize);
            Stack.SetMarginBottom(component, unitSize);
            return component;
        }

        public static T MarginLeft<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetMarginLeft(component, unitSize);
            return component;
        }

        public static T MarginRight<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetMarginRight(component, unitSize);
            return component;
        }

        public static T MarginTop<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetMarginTop(component, unitSize);
            return component;
        }

        public static T MarginBottom<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetMarginBottom(component, unitSize);
            return component;
        }

        public static T Padding<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetPaddingLeft(component, unitSize);
            Stack.SetPaddingRight(component, unitSize);
            Stack.SetPaddingTop(component, unitSize);
            Stack.SetPaddingBottom(component, unitSize);
            return component;
        }

        public static T PaddingLeft<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetPaddingLeft(component, unitSize);
            return component;
        }

        public static T PaddingRight<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetPaddingRight(component, unitSize);
            return component;
        }

        public static T PaddingTop<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetPaddingTop(component, unitSize);
            return component;
        }

        public static T PaddingBottom<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetPaddingBottom(component, unitSize);
            return component;
        }

        public static T WidthAuto<T>(this T component) where T : IComponent
        {
            Stack.SetWidth(component, UnitSize.Auto());
            return component;
        }

        public static T Width<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetWidth(component, unitSize);
            return component;
        }

        public static T MinWidth<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetMinWidth(component, unitSize);
            return component;
        }

        public static T MaxWidth<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetMaxWidth(component, unitSize);
            return component;
        }

        public static T WidthStretch<T>(this T component) where T : IComponent
        {
            Stack.SetWidth(component, 100.percent());
            return component;
        }

        public static T HeightAuto<T>(this T component) where T : IComponent
        {
            Stack.SetHeight(component, UnitSize.Auto());
            return component;
        }

        public static T Height<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetHeight(component, unitSize);
            return component;
        }

        public static T MinHeight<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetMinHeight(component, unitSize);
            return component;
        }

        public static T MaxHeight<T>(this T component, UnitSize unitSize) where T : IComponent
        {
            Stack.SetMaxHeight(component, unitSize);
            return component;
        }

        public static T HeightStretch<T>(this T component) where T : IComponent
        {
            Stack.SetHeight(component, 100.percent());
            return component;
        }

        public static T MinHeightStretch<T>(this T component) where T : IComponent
        {
            Stack.SetMinHeight(component, 100.percent());
            return component;
        }

        public static T Stretch<T>(this T component) where T : IComponent
        {
            Stack.SetWidth(component, 100.percent());
            Stack.SetHeight(component, 100.percent());
            return component;
        }

        public static T Grow<T>(this T component, int grow) where T : IComponent
        {
            Stack.SetGrow(component, grow);
            return component;
        }

        public static T Shrink<T>(this T component) where T : IComponent
        {
            Stack.SetShrink(component, true);
            return component;
        }

        public static T NoShrink<T>(this T component) where T : IComponent
        {
            Stack.SetShrink(component, false);
            return component;
        }

        public static T GridColumn<T>(this T component, int start, int end) where T : IComponent
        {
            Grid.SetGridColumn(component, start, end);
            return component;
        }

        public static T GridColumnStrech<T>(this T component) where T : IComponent
        {
            Grid.SetGridColumn(component, 1, -1);
            return component;
        }

        public static T GridRow<T>(this T component, int start, int end) where T : IComponent
        {
            Grid.SetGridRow(component, start, end);
            return component;
        }

        public static T GridRowStrech<T>(this T component) where T : IComponent
        {
            Grid.SetGridRow(component, 1, -1);
            return component;
        }

        public static T Fade<T>(this T component) where T : IComponent
        {
            component.Render().classList.add("fade");
            component.Render().classList.remove("show");
            return component;
        }

        public static T Show<T>(this T component) where T : IComponent
        {
            component.Render().classList.add("fade", "show");
            return component;
        }
    }
}
