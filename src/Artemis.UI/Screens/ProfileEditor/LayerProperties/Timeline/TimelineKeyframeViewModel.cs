﻿using System;
using System.ComponentModel;
using System.Linq;
using Artemis.Core;
using Artemis.UI.Shared.Services;
using Stylet;

namespace Artemis.UI.Screens.ProfileEditor.LayerProperties.Timeline
{
    public class TimelineKeyframeViewModel<T> : TimelineKeyframeViewModel
    {
        private readonly IProfileEditorService _profileEditorService;

        public TimelineKeyframeViewModel(IProfileEditorService profileEditorService, LayerPropertyKeyframe<T> layerPropertyKeyframe)
            : base(profileEditorService, layerPropertyKeyframe)
        {
            _profileEditorService = profileEditorService;
            LayerPropertyKeyframe = layerPropertyKeyframe;
        }

        public LayerPropertyKeyframe<T> LayerPropertyKeyframe { get; }

        #region Context menu actions

        public override void Copy()
        {
            var newKeyframe = new LayerPropertyKeyframe<T>(
                LayerPropertyKeyframe.Value,
                LayerPropertyKeyframe.Position,
                LayerPropertyKeyframe.EasingFunction,
                LayerPropertyKeyframe.LayerProperty
            );
            // If possible, shift the keyframe to the right by 11 pixels
            var desiredPosition = newKeyframe.Position + TimeSpan.FromMilliseconds(1000f / _profileEditorService.PixelsPerSecond * 11);
            if (desiredPosition <= newKeyframe.LayerProperty.ProfileElement.TimelineLength)
                newKeyframe.Position = desiredPosition;
            // Otherwise if possible shift it to the left by 11 pixels
            else
            {
                desiredPosition = newKeyframe.Position - TimeSpan.FromMilliseconds(1000f / _profileEditorService.PixelsPerSecond * 11);
                if (desiredPosition > TimeSpan.Zero)
                    newKeyframe.Position = desiredPosition;
            }

            LayerPropertyKeyframe.LayerProperty.AddKeyframe(newKeyframe);
            _profileEditorService.UpdateSelectedProfileElement();
        }

        public override void Delete()
        {
            LayerPropertyKeyframe.LayerProperty.RemoveKeyframe(LayerPropertyKeyframe);
            _profileEditorService.UpdateSelectedProfileElement();
        }

        #endregion
    }

    public abstract class TimelineKeyframeViewModel : PropertyChangedBase, IDisposable
    {
        private readonly IProfileEditorService _profileEditorService;
        private BindableCollection<TimelineEasingViewModel> _easingViewModels;
        private bool _isSelected;
        private int _pixelsPerSecond;
        private string _timestamp;
        private double _x;

        protected TimelineKeyframeViewModel(IProfileEditorService profileEditorService, BaseLayerPropertyKeyframe baseLayerPropertyKeyframe)
        {
            _profileEditorService = profileEditorService;
            BaseLayerPropertyKeyframe = baseLayerPropertyKeyframe;
            EasingViewModels = new BindableCollection<TimelineEasingViewModel>();

            BaseLayerPropertyKeyframe.PropertyChanged += BaseLayerPropertyKeyframeOnPropertyChanged;
        }

        public BaseLayerPropertyKeyframe BaseLayerPropertyKeyframe { get; }

        public BindableCollection<TimelineEasingViewModel> EasingViewModels
        {
            get => _easingViewModels;
            set => SetAndNotify(ref _easingViewModels, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetAndNotify(ref _isSelected, value);
        }

        public double X
        {
            get => _x;
            set => SetAndNotify(ref _x, value);
        }

        public string Timestamp
        {
            get => _timestamp;
            set => SetAndNotify(ref _timestamp, value);
        }

        public void Dispose()
        {
            BaseLayerPropertyKeyframe.PropertyChanged -= BaseLayerPropertyKeyframeOnPropertyChanged;
        }

        public void Update(int pixelsPerSecond)
        {
            _pixelsPerSecond = pixelsPerSecond;

            X = pixelsPerSecond * BaseLayerPropertyKeyframe.Position.TotalSeconds;
            Timestamp = $"{Math.Floor(BaseLayerPropertyKeyframe.Position.TotalSeconds):00}.{BaseLayerPropertyKeyframe.Position.Milliseconds:000}";
        }

        public abstract void Copy();

        public abstract void Delete();

        private void BaseLayerPropertyKeyframeOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BaseLayerPropertyKeyframe.Position))
                Update(_pixelsPerSecond);
        }

        #region Easing

        public void CreateEasingViewModels()
        {
            EasingViewModels.AddRange(Enum.GetValues(typeof(Easings.Functions)).Cast<Easings.Functions>().Select(v => new TimelineEasingViewModel(this, v)));
        }

        public void SelectEasingMode(TimelineEasingViewModel easingViewModel)
        {
            BaseLayerPropertyKeyframe.EasingFunction = easingViewModel.EasingFunction;
            // Set every selection to false except on the VM that made the change
            foreach (var propertyTrackEasingViewModel in EasingViewModels.Where(vm => vm != easingViewModel))
                propertyTrackEasingViewModel.IsEasingModeSelected = false;


            _profileEditorService.UpdateSelectedProfileElement();
        }

        #endregion

        #region Movement

        private TimeSpan? _offset;

        public void ReleaseMovement()
        {
            _offset = null;
        }

        public void SaveOffsetToKeyframe(TimelineKeyframeViewModel keyframeViewModel)
        {
            if (keyframeViewModel == this)
            {
                _offset = null;
                return;
            }

            if (_offset != null)
                return;

            _offset = BaseLayerPropertyKeyframe.Position - keyframeViewModel.BaseLayerPropertyKeyframe.Position;
        }

        public void ApplyOffsetToKeyframe(TimelineKeyframeViewModel keyframeViewModel)
        {
            if (keyframeViewModel == this || _offset == null)
                return;

            UpdatePosition(keyframeViewModel.BaseLayerPropertyKeyframe.Position + _offset.Value);
        }

        public void UpdatePosition(TimeSpan position)
        {
            if (position < TimeSpan.Zero)
                BaseLayerPropertyKeyframe.Position = TimeSpan.Zero;
            else if (position > _profileEditorService.SelectedProfileElement.TimelineLength)
                BaseLayerPropertyKeyframe.Position = _profileEditorService.SelectedProfileElement.TimelineLength;
            else
                BaseLayerPropertyKeyframe.Position = position;

            Update(_pixelsPerSecond);
        }

        #endregion
    }
}