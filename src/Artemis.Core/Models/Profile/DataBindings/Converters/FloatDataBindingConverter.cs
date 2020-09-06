﻿using System;

namespace Artemis.Core
{
    /// <inheritdoc />
    public class FloatDataBindingConverter : IDataBindingConverter
    {
        /// <inheritdoc />
        public Type SupportedType => typeof(float);

        /// <inheritdoc />
        public bool SupportsSum => true;

        /// <inheritdoc />
        public bool SupportsInterpolate => true;

        /// <inheritdoc />
        public object Sum(BaseLayerProperty layerProperty, object a, object b)
        {
            return (float) a + (float) b;
        }

        /// <inheritdoc />
        public object Interpolate(BaseLayerProperty layerProperty, object a, object b, float progress)
        {
            var diff = (float) b - (float) a;
            return diff * progress;
        }

        /// <inheritdoc />
        public void ApplyValue(BaseLayerProperty layerProperty, object value)
        {
            layerProperty.CurrentValue = value;
        }

        /// <inheritdoc />
        public object GetValue(BaseLayerProperty layerProperty)
        {
            return layerProperty.CurrentValue;
        }
    }
}