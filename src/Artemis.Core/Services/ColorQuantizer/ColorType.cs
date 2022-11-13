﻿using System;

namespace Artemis.Core.Services;

/// <summary>
///     The types of relevant colors in an image.
/// </summary>
[Obsolete("Use Artemis.Core.ColorScience instead.")]
public enum ColorType
{
    /// <summary>
    ///     Represents a saturated color.
    /// </summary>
    Vibrant,

    /// <summary>
    ///     Represents a saturated and light color.
    /// </summary>
    LightVibrant,

    /// <summary>
    ///     Represents a saturated and dark color.
    /// </summary>
    DarkVibrant,

    /// <summary>
    ///     Represents a desaturated color.
    /// </summary>
    Muted,

    /// <summary>
    ///     Represents a desaturated and light color.
    /// </summary>
    LightMuted,

    /// <summary>
    ///     Represents a desaturated and dark color.
    /// </summary>
    DarkMuted
}