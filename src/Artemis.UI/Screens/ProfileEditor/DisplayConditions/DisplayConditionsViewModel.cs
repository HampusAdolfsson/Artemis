﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Artemis.Core;
using Artemis.Core.Services;
using Artemis.UI.Ninject.Factories;
using Artemis.UI.Screens.ProfileEditor.Conditions;
using Artemis.UI.Shared;
using Artemis.UI.Shared.Services;
using Stylet;

namespace Artemis.UI.Screens.ProfileEditor.DisplayConditions
{
    public class DisplayConditionsViewModel : Conductor<DataModelConditionGroupViewModel>, IProfileEditorPanelViewModel
    {
        private readonly IDataModelConditionsVmFactory _dataModelConditionsVmFactory;
        private readonly IProfileEditorService _profileEditorService;
        private readonly INodeService _nodeService;
        private RenderProfileElement _renderProfileElement;
        private bool _displayStartHint;
        private bool _isEventCondition;

        public DisplayConditionsViewModel(IProfileEditorService profileEditorService, INodeService nodeService, IDataModelConditionsVmFactory dataModelConditionsVmFactory)
        {
            _profileEditorService = profileEditorService;
            _nodeService = nodeService;
            _dataModelConditionsVmFactory = dataModelConditionsVmFactory;

            AvailableNodes = _nodeService.AvailableNodes;
            Script = new NodeScript<bool>("Display Condition (TODO)", "TODO");
        }


        private NodeScript<bool> _script;
        public NodeScript<bool> Script
        {
            get => _script;
            private set => SetAndNotify(ref _script, value);
        }

        private IEnumerable<NodeData> _availableNodes;
        public IEnumerable<NodeData> AvailableNodes
        {
            get => _availableNodes;
            set => SetAndNotify(ref _availableNodes, value);
        }

        public bool DisplayStartHint
        {
            get => _displayStartHint;
            set => SetAndNotify(ref _displayStartHint, value);
        }

        public bool IsEventCondition
        {
            get => _isEventCondition;
            set => SetAndNotify(ref _isEventCondition, value);
        }

        public RenderProfileElement RenderProfileElement
        {
            get => _renderProfileElement;
            set
            {
                if (!SetAndNotify(ref _renderProfileElement, value)) return;
                NotifyOfPropertyChange(nameof(DisplayContinuously));
                NotifyOfPropertyChange(nameof(AlwaysFinishTimeline));
                NotifyOfPropertyChange(nameof(EventOverlapMode));
            }
        }

        public bool DisplayContinuously
        {
            get => RenderProfileElement?.Timeline.PlayMode == TimelinePlayMode.Repeat;
            set
            {
                TimelinePlayMode playMode = value ? TimelinePlayMode.Repeat : TimelinePlayMode.Once;
                if (RenderProfileElement == null || RenderProfileElement?.Timeline.PlayMode == playMode) return;
                RenderProfileElement.Timeline.PlayMode = playMode;
                _profileEditorService.SaveSelectedProfileElement();
            }
        }

        public bool AlwaysFinishTimeline
        {
            get => RenderProfileElement?.Timeline.StopMode == TimelineStopMode.Finish;
            set
            {
                TimelineStopMode stopMode = value ? TimelineStopMode.Finish : TimelineStopMode.SkipToEnd;
                if (RenderProfileElement == null || RenderProfileElement?.Timeline.StopMode == stopMode) return;
                RenderProfileElement.Timeline.StopMode = stopMode;
                _profileEditorService.SaveSelectedProfileElement();
            }
        }

        public TimeLineEventOverlapMode EventOverlapMode
        {
            get => RenderProfileElement?.Timeline.EventOverlapMode ?? TimeLineEventOverlapMode.Restart;
            set
            {
                if (RenderProfileElement == null || RenderProfileElement?.Timeline.EventOverlapMode == value) return;
                RenderProfileElement.Timeline.EventOverlapMode = value;
                _profileEditorService.SaveSelectedProfileElement();
            }
        }

        public bool ConditionBehaviourEnabled => RenderProfileElement != null;

        protected override void OnInitialActivate()
        {
            _profileEditorService.SelectedProfileElementChanged += SelectedProfileEditorServiceOnSelectedProfileElementChanged;
            Update(_profileEditorService.SelectedProfileElement);

            base.OnInitialActivate();
        }

        protected override void OnClose()
        {
            _profileEditorService.SelectedProfileElementChanged -= SelectedProfileEditorServiceOnSelectedProfileElementChanged;
            base.OnClose();
        }

        private void SelectedProfileEditorServiceOnSelectedProfileElementChanged(object sender, RenderProfileElementEventArgs e)
        {
            Update(e.RenderProfileElement);
        }

        private void Update(RenderProfileElement renderProfileElement)
        {
            if (RenderProfileElement != null)
            {
                //RenderProfileElement.DisplayCondition.ChildAdded -= DisplayConditionOnChildrenModified;
                //RenderProfileElement.DisplayCondition.ChildRemoved -= DisplayConditionOnChildrenModified;
                RenderProfileElement.Timeline.PropertyChanged -= TimelineOnPropertyChanged;
            }

            RenderProfileElement = renderProfileElement;

            NotifyOfPropertyChange(nameof(DisplayContinuously));
            NotifyOfPropertyChange(nameof(AlwaysFinishTimeline));
            NotifyOfPropertyChange(nameof(ConditionBehaviourEnabled));

            if (renderProfileElement == null)
            {
                ActiveItem = null;
                return;
            }

            // Ensure the layer has a root display condition group
            //if (renderProfileElement.DisplayCondition == null)
            //    renderProfileElement.DisplayCondition = new DataModelConditionGroup(null);

            if (renderProfileElement.DisplayCondition == null)
                renderProfileElement.DisplayCondition = new NodeScript<bool>("Display Condition (TODO)", "-");

            //List<Module> modules = new();
            //if (_profileEditorService.SelectedProfileConfiguration?.Module != null)
            //    modules.Add(_profileEditorService.SelectedProfileConfiguration.Module);
            //ActiveItem = _dataModelConditionsVmFactory.DataModelConditionGroupViewModel(renderProfileElement.DisplayCondition, ConditionGroupType.General, modules);
            //ActiveItem.IsRootGroup = true;

            //DisplayStartHint = !RenderProfileElement.DisplayCondition.Children.Any();
            //IsEventCondition = RenderProfileElement.DisplayCondition.Children.Any(c => c is DataModelConditionEvent);

            //RenderProfileElement.DisplayCondition.ChildAdded += DisplayConditionOnChildrenModified;
            //RenderProfileElement.DisplayCondition.ChildRemoved += DisplayConditionOnChildrenModified;
            RenderProfileElement.Timeline.PropertyChanged += TimelineOnPropertyChanged;
        }

        private void TimelineOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyOfPropertyChange(nameof(DisplayContinuously));
            NotifyOfPropertyChange(nameof(AlwaysFinishTimeline));
            NotifyOfPropertyChange(nameof(EventOverlapMode));
        }

        private void DisplayConditionOnChildrenModified(object sender, EventArgs e)
        {
            //DisplayStartHint = !RenderProfileElement.DisplayCondition.Children.Any();
            //IsEventCondition = RenderProfileElement.DisplayCondition.Children.Any(c => c is DataModelConditionEvent);
        }

        public void EventTriggerModeSelected()
        {
            _profileEditorService.SaveSelectedProfileElement();
        }
    }
}