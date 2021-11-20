﻿using System;
using Artemis.Core;
using Artemis.UI.Shared.DataModelVisualization;
using ReactiveUI;

namespace Artemis.UI.Shared.DefaultTypes.DataModel.Display
{
    /// <summary>
    ///     Represents the default data model display view model that is used when no display viewmodel specific for the type
    ///     is registered
    /// </summary>
    internal class DefaultDataModelDisplayViewModel : DataModelDisplayViewModel<object>
    {
        private bool _showNull;
        private bool _showToString;

        internal DefaultDataModelDisplayViewModel()
        {
            ShowNull = true;
        }

        public bool ShowToString
        {
            get => _showToString;
            private set => this.RaiseAndSetIfChanged(ref _showToString, value);
        }

        public bool ShowNull
        {
            get => _showNull;
            set => this.RaiseAndSetIfChanged(ref _showNull, value);
        }

        protected override void OnDisplayValueUpdated()
        {
            if (DisplayValue is Enum enumDisplayValue)
                DisplayValue = EnumUtilities.HumanizeValue(enumDisplayValue);

            ShowToString = DisplayValue != null;
            ShowNull = DisplayValue == null;
        }
    }
}