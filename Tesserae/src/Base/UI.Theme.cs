﻿using System.Text;
using static H5.Core.dom;

namespace Tesserae
{
    public static partial class UI
    {
        public static class Theme
        {
            private static HTMLStyleElement _primaryStyleElement;
            private static HTMLStyleElement _backgroundStyleElement;

            public static void Dark()
            {
                document.body.classList.add("tss-dark-mode");
            }

            public static void Light()
            {
                document.body.classList.remove("tss-dark-mode");
            }

            public static bool IsDark
            {
                get
                {
                    return document.body.classList.contains("tss-dark-mode");
                }
                set
                {
                    if (value)
                    {
                        Dark();
                    }
                    else
                    {
                        Light();
                    }
                }
            }

            public static bool IsLight
            {
                get
                {
                    return !IsDark;
                }
                set
                {
                    IsDark = !value;
                }
            }


            public static void SetBackground(Color defaultLight, Color defaultDark)
            {
                if (_backgroundStyleElement is object)
                {
                    _backgroundStyleElement.remove();
                    _backgroundStyleElement = null;
                }

                var secondaryLight = (HSLColor)defaultLight;
                var sidebarLight   = (HSLColor)defaultLight;
                var hoverLight     = (HSLColor)defaultLight;
                var activeLight    = (HSLColor)defaultLight;
                var progressLight  = (HSLColor)defaultLight;

                var secondaryDark = (HSLColor)defaultDark;
                var hoverDark     = (HSLColor)defaultDark;
                var activeDark    = (HSLColor)defaultDark;
                var progressDark  = (HSLColor)defaultDark;


                secondaryLight.Luminosity -= 4;
                hoverLight.Luminosity     -= 12;
                activeLight.Luminosity    -= 9;
                progressLight.Luminosity  -= 9;

                secondaryDark.Luminosity += 7;
                hoverDark.Luminosity     += 2;
                activeDark.Luminosity    += 13;
                progressDark.Luminosity  += 13;


                var sb = new StringBuilder();
                sb.AppendLine(":root {");
                //sb.Append("  --tss-default-background-color-root: ").Append(defaultLight.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-secondary-background-color-root: ").Append(secondaryLight.ToRGBvar()).AppendLine(";");
                //sb.Append("  --tss-default-background-hover-color-root: ").Append(hoverLight.ToRGBvar()).AppendLine(";");
                //sb.Append("  --tss-default-background-active-color-root: ").Append(activeLight.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-progress-background-color-root: ").Append(progressLight.ToRGBvar()).AppendLine(";");

                //We need to redefine the variables again here otherwise the values won't change as they're derived twice from variables
                sb.Append(@"
    --tss-default-background-color: rgb(var(--tss-default-background-color-root ));
    --tss-secondary-background-color: rgb(var(--tss-secondary-background-color-root ));
    --tss-default-background-hover-color: rgb(var(--tss-default-background-hover-color-root ));
    --tss-default-background-active-color: rgb(var(--tss-default-background-active-color-root ));
    --tss-progress-background-color: rgb(var(--tss-progress-background-color-root ));
");

                sb.AppendLine("}");

                sb.AppendLine(".tss-dark-mode {");
                //sb.Append("  --tss-default-background-color-root: ").Append(defaultDark.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-secondary-background-color-root: ").Append(secondaryDark.ToRGBvar()).AppendLine(";");
                //sb.Append("  --tss-default-background-hover-color-root: ").Append(hoverDark.ToRGBvar()).AppendLine(";");
                //sb.Append("  --tss-default-background-active-color-root: ").Append(activeDark.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-progress-background-color-root: ").Append(progressDark.ToRGBvar()).AppendLine(";");

                //We need to redefine the variables again here otherwise the values won't change as they're derived twice from variables
                sb.Append(@"
    --tss-default-background-color: rgb(var(--tss-default-background-color-root ));
    --tss-secondary-background-color: rgb(var(--tss-secondary-background-color-root ));
    --tss-default-background-hover-color: rgb(var(--tss-default-background-hover-color-root ));
    --tss-default-background-active-color: rgb(var(--tss-default-background-active-color-root ));
    --tss-progress-background-color: rgb(var(--tss-progress-background-color-root ));
");

                sb.AppendLine("}");

                _backgroundStyleElement      = (HTMLStyleElement)document.createElement("style");
                _backgroundStyleElement.type = "text/css";
                _backgroundStyleElement.appendChild(document.createTextNode(sb.ToString()));

                var head = document.getElementsByTagName("head")[0];
                head.appendChild(_backgroundStyleElement);
            }


            public static void SetPrimary(Color primaryLightColor, Color primaryDarkColor)
            {
                if (_primaryStyleElement is object)
                {
                    _primaryStyleElement.remove();
                    _primaryStyleElement = null;
                }

                var borderColorLight      = (HSLColor)primaryLightColor;
                var borderColorDark       = (HSLColor)primaryDarkColor;
                var backgroundActiveLight = (HSLColor)primaryLightColor;
                var backgroundActiveDark  = (HSLColor)primaryDarkColor;

                // rgb(0, 120, 212)  = hsl(206, 100, 41.6)
                // rgb(16, 110, 190) = hsl(208, 85.5, 40.4)
                // rgb(0, 90, 158)   = hsl(206, 100, 31)

                borderColorLight.Luminosity -= (100  - 85.5); //Uses the same delta as in the current template
                borderColorLight.Saturation -= (41.6 - 40.4); //TODO: get real values instead using Color.EvalVar
                borderColorLight.Hue        -= (206  - 208); // Main problem is just how to handle the .tss-dark-mode eval, as it will change the return value

                borderColorDark.Luminosity -= (100  - 85.5);
                borderColorDark.Saturation -= (41.6 - 40.4);
                borderColorDark.Hue        -= (206  - 208);

                backgroundActiveLight.Luminosity -= (100  - 100);
                backgroundActiveLight.Saturation -= (41.6 - 31);
                backgroundActiveLight.Hue        -= (206  - 206);

                backgroundActiveDark.Luminosity -= (100  - 100);
                backgroundActiveDark.Saturation -= (41.6 - 31);
                backgroundActiveDark.Hue        -= (206  - 206);

                var sb = new StringBuilder();
                sb.AppendLine(":root {");
                sb.Append("  --tss-primary-background-color-root: ").Append(primaryLightColor.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-link-color-root: ").Append(primaryLightColor.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-primary-border-color-root: ").Append(borderColorLight.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-primary-background-hover-color-root: ").Append(borderColorLight.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-primary-background-active-color-root: ").Append(backgroundActiveLight.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-primary-shadow-color-root: ").Append(primaryLightColor.ToRGBvar()).AppendLine(";");

                //We need to redefine the variables again here otherwise the values won't change as they're derived twice from variables
                sb.Append(@"
    --tss-primary-background-color: rgb(var(--tss-primary-background-color-root ));
    --tss-link-color: rgb(var(--tss-link-color-root ));
    --tss-primary-border-color: rgb(var(--tss-primary-border-color-root ));
    --tss-primary-background-hover-color: rgb(var(--tss-primary-background-hover-color-root ));
    --tss-primary-background-active-color: rgb(var(--tss-primary-background-active-color-root ));
    --tss-primary-shadow: 0 1.6px 3.6px 0 rgba(var(--tss-primary-shadow-color-root),0.132), 0 0.3px 0.9px 0 rgba(var(--tss-primary-shadow-color-root),0.108);
");

                sb.AppendLine("}");

                sb.AppendLine(".tss-dark-mode {");
                sb.Append("  --tss-primary-background-color-root: ").Append(primaryDarkColor.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-link-color-root: ").Append(primaryDarkColor.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-primary-border-color-root: ").Append(borderColorDark.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-primary-background-hover-color-root: ").Append(borderColorDark.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-primary-background-active-color-root: ").Append(backgroundActiveDark.ToRGBvar()).AppendLine(";");
                sb.Append("  --tss-primary-shadow-color-root: ").Append(primaryDarkColor.ToRGBvar()).AppendLine(";");

                //We need to redefine the variables again here otherwise the values won't change as they're derived twice from variables
                sb.Append(@"
    --tss-primary-background-color: rgb(var(--tss-primary-background-color-root ));
    --tss-link-color: rgb(var(--tss-link-color-root ));
    --tss-primary-border-color: rgb(var(--tss-primary-border-color-root ));
    --tss-primary-background-hover-color: rgb(var(--tss-primary-background-hover-color-root ));
    --tss-primary-background-active-color: rgb(var(--tss-primary-background-active-color-root ));
    --tss-primary-shadow: 0 3.6px 5.6px 0 rgba(var(--tss-primary-shadow-color-root),0.132), 2px 2.3px 5.9px 0 rgba(var(--tss-primary-shadow-color-root),0.108);
");


                sb.AppendLine("}");

                _primaryStyleElement      = (HTMLStyleElement)document.createElement("style");
                _primaryStyleElement.type = "text/css";
                _primaryStyleElement.appendChild(document.createTextNode(sb.ToString()));

                var head = document.getElementsByTagName("head")[0];
                head.appendChild(_primaryStyleElement);
            }

            //Variables from tesserae.common.css
            public static class Default
            {
                public const string Background       = "var(--tss-default-background-color)";
                public const string Foreground       = "var(--tss-default-foreground-color)";
                public const string Border           = "var(--tss-default-border-color)";
                public const string DarkBorder       = "var(--tss-dark-border-color)";
                public const string InvalidBorder    = "var(--tss-invalid-border-color)";
                public const string BackgroundHover  = "var(--tss-default-background-hover-color)";
                public const string ForegroundHover  = "var(--tss-default-foreground-hover-color)";
                public const string BackgroundActive = "var(--tss-default-background-active-color)";
                public const string ForegroundActive = "var(--tss-default-foreground-active-color)";

                public const string Slider         = "var(--tss-slider-color)";
                public const string SliderActive   = "var(--tss-slider-active-color)";
                public const string SliderDisabled = "var(--tss-slider-disabled-color)";
                public const string OverlayLight   = "var(--tss-overlay-light)";
                public const string OverlayDark    = "var(--tss-overlay-dark)";
                public const string CardShadow     = "var(--tss-card-shadow, 0 0.3px 0.9px 0 rgba(0,0,0,0.108))";
            }

            public static class Sidebar
            {
                public const string Background = "var(--tss-sidebar-background-color)";
                public const string Foreground = "var(--tss-sidebar-foreground-color)";
            }

            public static class Secondary
            {
                public const string Background = "var(--tss-secondary-background-color)";
                public const string Foreground = "var(--tss-secondary-foreground-color)";
            }
            public static class Disabled
            {
                public const string Background = "var(--tss-disabled-background-color)";
                public const string Foreground = "var(--tss-disabled-foreground-color)";
            }

            public static class Primary
            {
                public const string Background       = "var(--tss-primary-background-color)";
                public const string Foreground       = "var(--tss-primary-foreground-color)";
                public const string Border           = "var(--tss-primary-border-color)";
                public const string BackgroundHover  = "var(--tss-primary-background-hover-color)";
                public const string ForegroundHover  = "var(--tss-primary-foreground-hover-color)";
                public const string BackgroundActive = "var(--tss-primary-background-active-color)";
                public const string ForegroundActive = "var(--tss-primary-foreground-active-color)";
            }

            public static class Danger
            {
                public const string Background       = "var(--tss-danger-background-color)";
                public const string Foreground       = "var(--tss-danger-foreground-color)";
                public const string Border           = "var(--tss-danger-border-color)";
                public const string BackgroundHover  = "var(--tss-danger-background-hover-color)";
                public const string ForegroundHover  = "var(--tss-danger-foreground-hover-color)";
                public const string BackgroundActive = "var(--tss-danger-background-active-color)";
                public const string ForegroundActive = "var(--tss-danger-foreground-active-color)";
            }

            public static class Success
            {
                public const string Background       = "var(--tss-success-background-color)";
                public const string Foreground       = "var(--tss-success-foreground-color)";
                public const string Border           = "var(--tss-success-border-color)";
                public const string BackgroundHover  = "var(--tss-success-background-hover-color)";
                public const string ForegroundHover  = "var(--tss-success-foreground-hover-color)";
                public const string BackgroundActive = "var(--tss-success-background-active-color)";
                public const string ForegroundActive = "var(--tss-success-foreground-active-color)";
            }
        }
    }
}