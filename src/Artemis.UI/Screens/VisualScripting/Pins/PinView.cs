using System;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;

namespace Artemis.UI.Screens.VisualScripting.Pins;

public class PinView : ReactiveUserControl<PinViewModel>
{
    private Canvas? _container;
    private bool _dragging;
    private Border? _pinPoint;

    protected void InitializePin(Border pinPoint)
    {
        _pinPoint = pinPoint;
        _pinPoint.PointerMoved += PinPointOnPointerMoved;
        _pinPoint.PointerReleased += PinPointOnPointerReleased;

        this.WhenActivated(d =>
        {
            if (ViewModel == null)
                return;

            // This is a bit of a nasty workaround that ensures the position of the pin has updated
            ViewModel.Pin.Node.WhenAnyValue(n => n.X).Delay(TimeSpan.FromMilliseconds(1), AvaloniaScheduler.Instance).Subscribe(_ => UpdatePosition()).DisposeWith(d);
            ViewModel.Pin.Node.WhenAnyValue(n => n.Y).Delay(TimeSpan.FromMilliseconds(1), AvaloniaScheduler.Instance).Subscribe(_ => UpdatePosition()).DisposeWith(d);
        });
    }

    private void PinPointOnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || _container == null || _pinPoint == null)
            return;

        NodeScriptViewModel? nodeScriptViewModel = this.FindAncestorOfType<NodeScriptView>()?.ViewModel;
        PointerPoint point = e.GetCurrentPoint(_container);
        if (nodeScriptViewModel == null || !point.Properties.IsLeftButtonPressed)
            return;

        if (!_dragging)
            e.Pointer.Capture(_pinPoint);

        PinViewModel? targetPin = (_container.InputHitTest(point.Position) as Border)?.DataContext as PinViewModel;
        if (targetPin == ViewModel)
            targetPin = null;

        _pinPoint.Cursor = new Cursor(nodeScriptViewModel.UpdatePinDrag(ViewModel, targetPin, point.Position) ? StandardCursorType.Hand : StandardCursorType.No);
        _dragging = true;
        e.Handled = true;
    }

    private void PinPointOnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_dragging || ViewModel == null || _container == null || _pinPoint == null)
            return;

        _dragging = false;
        e.Pointer.Capture(null);

        PointerPoint point = e.GetCurrentPoint(_container);
        PinViewModel? targetPin = (_container.InputHitTest(point.Position) as Border)?.DataContext as PinViewModel;
        if (targetPin == ViewModel)
            targetPin = null;

        this.FindAncestorOfType<NodeScriptView>()?.ViewModel?.FinishPinDrag(ViewModel, targetPin, point.Position);
        _pinPoint.Cursor = new Cursor(StandardCursorType.Hand);
        e.Handled = true;
    }

    #region Overrides of Visual

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _container = this.FindAncestorOfType<Canvas>();
    }

    private void UpdatePosition()
    {
        if (_container == null || _pinPoint == null || ViewModel == null)
            return;

        Matrix? transform = _pinPoint.TransformToVisual(_container);
        if (transform != null)
            ViewModel.Position = new Point(_pinPoint.Bounds.Width / 2, _pinPoint.Bounds.Height / 2).Transform(transform.Value);
    }

    #endregion
}