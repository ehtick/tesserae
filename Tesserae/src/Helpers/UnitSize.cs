using System;
using System.Collections.Generic;

namespace Tesserae
{
    public sealed class UnitSize
    {
        public UnitSize(double size, Unit unit)
        {
            if (unit == Unit.Default)
            {
                throw new ArgumentException(nameof(unit));
            }

            Size = size;
            Unit = unit;
        }

        private UnitSize()            => Unit = Unit.Auto;

        public static UnitSize Auto() => new UnitSize();
        public static UnitSize Inherit() => new UnitSize() { Unit = Unit.Inherit };

        public double Size            { get; private set; }

        public Unit Unit              { get; private set; }

        [Obsolete("Replace call with .percent, .px or .vh extension methods available on the int and double types")]
        public static string Translate(Unit unit, double size) => new UnitSize(size, unit).ToString();

        internal IEnumerable<string> Select(Func<object, object> p)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            if (Unit == Unit.Auto)
            {
                return $"{Unit}";
            }

            return $"{Size:0.####}{Unit}";
        }
    }
}
