﻿using System;
using System.Linq;
using Artemis.Core.Models.Profile.Conditions;
using Artemis.UI.Ninject.Factories;
using Artemis.UI.Screens.Module.ProfileEditor.DisplayConditions.Abstract;
using Humanizer;

namespace Artemis.UI.Screens.Module.ProfileEditor.DisplayConditions
{
    public class DisplayConditionGroupViewModel : DisplayConditionViewModel
    {
        private readonly IDisplayConditionsVmFactory _displayConditionsVmFactory;
        private bool _isRootGroup;

        public DisplayConditionGroupViewModel(DisplayConditionGroup displayConditionGroup, DisplayConditionViewModel parent, IDisplayConditionsVmFactory displayConditionsVmFactory) : base(
            displayConditionGroup, parent)
        {
            _displayConditionsVmFactory = displayConditionsVmFactory;
        }

        public DisplayConditionGroup DisplayConditionGroup => (DisplayConditionGroup) Model;

        public bool IsRootGroup
        {
            get => _isRootGroup;
            set => SetAndNotify(ref _isRootGroup, value);
        }

        public string SelectedBooleanOperator => DisplayConditionGroup.BooleanOperator.Humanize();

        public void SelectBooleanOperator(string type)
        {
            var enumValue = Enum.Parse<BooleanOperator>(type);
            DisplayConditionGroup.BooleanOperator = enumValue;
            NotifyOfPropertyChange(nameof(SelectedBooleanOperator));
        }

        public void AddCondition(string type)
        {
            if (type == "Static")
                DisplayConditionGroup.AddChild(new DisplayConditionPredicate {PredicateType = PredicateType.Static});
            else if (type == "Dynamic")
                DisplayConditionGroup.AddChild(new DisplayConditionPredicate {PredicateType = PredicateType.Dynamic});
            Update();
        }

        public void AddGroup()
        {
            DisplayConditionGroup.AddChild(new DisplayConditionGroup());
            Update();
        }

        public override void Update()
        {
            NotifyOfPropertyChange(nameof(SelectedBooleanOperator));

            // Remove VMs of effects no longer applied on the layer
            var toRemove = Children.Where(c => !DisplayConditionGroup.Children.Contains(c.Model)).ToList();
            Children.RemoveRange(toRemove);

            foreach (var childModel in Model.Children)
            {
                if (Children.Any(c => c.Model == childModel))
                    continue;

                switch (childModel)
                {
                    case DisplayConditionGroup displayConditionGroup:
                        Children.Add(_displayConditionsVmFactory.DisplayConditionGroupViewModel(displayConditionGroup, this));
                        break;
                    case DisplayConditionListPredicate displayConditionListPredicate:
                        Children.Add(_displayConditionsVmFactory.DisplayConditionListPredicateViewModel(displayConditionListPredicate, this));
                        break;
                    case DisplayConditionPredicate displayConditionPredicate:
                        Children.Add(_displayConditionsVmFactory.DisplayConditionPredicateViewModel(displayConditionPredicate, this));
                        break;
                }
            }

            foreach (var childViewModel in Children)
                childViewModel.Update();
        }
    }
}