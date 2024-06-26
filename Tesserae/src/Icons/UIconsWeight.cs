﻿using H5;

namespace Tesserae
{
    [Enum(Emit.StringName)]
    [H5.Name("tss.uiconweight")]
    public enum UIconsWeight
    {
        [Name("fi-rr-")]
        Regular,

        [Name("fi-sr-")]
        Solid,

        [Name("fi-br-")]
        Bold,

        [Name("fi-tr-")]
        Thin,

        [Name("fi-rs-")]
        RegularStraight,

        [Name("fi-ss-")]
        SolidStraight,

        [Name("fi-bs-")]
        BoldStraight,

        [Name("fi-ts-")]
        ThinStraight
    }
}