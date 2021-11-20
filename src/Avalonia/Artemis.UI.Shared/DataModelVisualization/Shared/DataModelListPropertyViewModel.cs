﻿using System;
using Artemis.UI.Shared.Services;
using ReactiveUI;

namespace Artemis.UI.Shared.DataModelVisualization.Shared
{
    /// <summary>
    ///     Represents a view model that visualizes a single data model property contained in a
    ///     <see cref="DataModelListViewModel" />
    /// </summary>
    public class DataModelListPropertyViewModel : DataModelPropertyViewModel
    {
        private int _index;
        private Type? _listType;

        internal DataModelListPropertyViewModel(Type listType, DataModelDisplayViewModel displayViewModel, string? name) : base(null, null, null)
        {
            ListType = listType;
            DisplayViewModel = displayViewModel;
        }

        internal DataModelListPropertyViewModel(Type listType, string? name) : base(null, null, null)
        {
            ListType = listType;
        }

        /// <summary>
        ///     Gets the index of the element within the list
        /// </summary>
        public int Index
        {
            get => _index;
            internal set => this.RaiseAndSetIfChanged(ref _index, value);
        }

        /// <summary>
        ///     Gets the type of elements contained in the list
        /// </summary>
        public Type? ListType
        {
            get => _listType;
            private set => this.RaiseAndSetIfChanged(ref _listType, value);
        }

        /// <inheritdoc />
        public override object? GetCurrentValue()
        {
            return DisplayValue;
        }

        /// <inheritdoc />
        public override void Update(IDataModelUIService dataModelUIService, DataModelUpdateConfiguration? configuration)
        {
            // Display value gets updated by parent, don't do anything if it is null
            if (DisplayValue == null)
                return;

            if (DisplayViewModel == null)
                DisplayViewModel = dataModelUIService.GetDataModelDisplayViewModel(DisplayValue.GetType(), PropertyDescription, true);

            ListType = DisplayValue.GetType();
            DisplayViewModel?.UpdateValue(DisplayValue);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[List item {Index}] {DisplayPath ?? Path} - {DisplayValue}";
        }
    }
}