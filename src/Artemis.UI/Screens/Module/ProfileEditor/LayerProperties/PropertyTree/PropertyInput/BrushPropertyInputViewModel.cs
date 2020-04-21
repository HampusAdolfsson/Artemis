﻿using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core.Events;
using Artemis.Core.Models.Profile;
using Artemis.Core.Plugins.LayerBrush;
using Artemis.Core.Services.Interfaces;
using Artemis.UI.Services.Interfaces;
using Artemis.UI.Shared.Utilities;
using Stylet;

namespace Artemis.UI.Screens.Module.ProfileEditor.LayerProperties.PropertyTree.PropertyInput
{
    public class BrushPropertyInputViewModel : PropertyInputViewModel
    {
        private readonly ILayerService _layerService;
        private readonly IPluginService _pluginService;

        public BrushPropertyInputViewModel(IProfileEditorService profileEditorService, ILayerService layerService, IPluginService pluginService) : base(profileEditorService)
        {
            _layerService = layerService;
            _pluginService = pluginService;
            EnumValues = new BindableCollection<ValueDescription>();

            _pluginService.PluginLoaded += PluginServiceOnPluginLoaded;
        }

        public BindableCollection<ValueDescription> EnumValues { get; }

        public sealed override List<Type> CompatibleTypes { get; } = new List<Type> {typeof(LayerBrushReference)};

        public LayerBrushReference BrushInputValue
        {
            get => (LayerBrushReference) InputValue;
            set
            {
                InputValue = value;
                _layerService.InstantiateLayerBrush(LayerPropertyViewModel.LayerProperty.Layer);
            }
        }

        public void UpdateEnumValues()
        {
            var layerBrushProviders = _pluginService.GetPluginsOfType<LayerBrushProvider>();
            var descriptors = layerBrushProviders.SelectMany(l => l.LayerBrushDescriptors).ToList();
            
            var enumValues = new List<ValueDescription>();
            foreach (var layerBrushDescriptor in descriptors)
            {
                var brushName = layerBrushDescriptor.LayerBrushType.Name;
                var brushGuid = layerBrushDescriptor.LayerBrushProvider.PluginInfo.Guid;
                if (BrushInputValue != null && BrushInputValue.BrushType == brushName && BrushInputValue.BrushPluginGuid == brushGuid)
                    enumValues.Add(new ValueDescription {Description = layerBrushDescriptor.DisplayName, Value = BrushInputValue});
                else
                    enumValues.Add(new ValueDescription {Description = layerBrushDescriptor.DisplayName, Value = new LayerBrushReference {BrushType = brushName, BrushPluginGuid = brushGuid}});
            }
            EnumValues.Clear();
            EnumValues.AddRange(enumValues);
        }

        public override void Update()
        {
            NotifyOfPropertyChange(() => BrushInputValue);
        }

        public override void Dispose()
        {
            _pluginService.PluginLoaded -= PluginServiceOnPluginLoaded;
            base.Dispose();
        }

        protected override void OnInitialized()
        {
            UpdateEnumValues();
            base.OnInitialized();
        }

        private void PluginServiceOnPluginLoaded(object sender, PluginEventArgs e)
        {
            UpdateEnumValues();
        }
    }
}