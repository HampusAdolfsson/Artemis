﻿using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;

namespace Artemis.UI.Screens.Sidebar;

public class SidebarCategoryView : ReactiveUserControl<SidebarCategoryViewModel>
{
    private static Image? _dragAdorner;
    private Point _dragStartPosition;
    private Point _elementDragOffset;
    private ListBox _listBox;

    public SidebarCategoryView()
    {
        InitializeComponent();
        _listBox = this.Get<ListBox>("SidebarListBox");

        AddHandler(DragDrop.DragEnterEvent, HandleDragEnterEvent, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        AddHandler(DragDrop.DragOverEvent, HandleDragOver, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
        AddHandler(PointerEnteredEvent, HandlePointerEnter, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble, true);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Left)
            ViewModel?.ToggleCollapsed.Execute().Subscribe();
    }

    #region Dragging

    private void HandlePointerEnter(object? sender, PointerEventArgs e)
    {
        DisposeDragAdorner();
    }

    private void HandleDragEnterEvent(object? sender, DragEventArgs e)
    {
        CreateDragAdorner(e);
    }

    private void CreateDragAdorner(DragEventArgs e)
    {
        if (_dragAdorner != null)
            return;

        if (e.Source is not Control c)
            return;

        // Get the list box item that raised the event
        ListBoxItem? container = c.FindLogicalAncestorOfType<ListBoxItem>();
        if (container == null)
            return;

        // Take a snapshot of said tree view item and add it as an adorner
        ITransform? originalTransform = container.RenderTransform;
        try
        {
            _dragStartPosition = e.GetPosition(this.FindAncestorOfType<Window>());
            _elementDragOffset = e.GetPosition(container);

            RenderTargetBitmap renderTarget = new(new PixelSize((int) container.Bounds.Width, (int) container.Bounds.Height));
            container.RenderTransform = new TranslateTransform(container.Bounds.X * -1, container.Bounds.Y * -1);
            renderTarget.Render(container);
            _dragAdorner = new Image
            {
                Source = renderTarget,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Stretch = Stretch.None,
                IsHitTestVisible = false
            };
            AdornerLayer.GetAdornerLayer(this)!.Children.Add(_dragAdorner);
        }
        finally
        {
            container.RenderTransform = originalTransform;
        }
    }

    private void HandleDragOver(object? sender, DragEventArgs e)
    {
        UpdateDragAdorner(e.GetPosition(this.FindAncestorOfType<Window>()));
    }

    private void HandleLeaveEvent(object? sender, RoutedEventArgs e)
    {
        // If there is currently an adorner, dispose of it
        DisposeDragAdorner();
    }

    private void HandleDrop(object? sender, DragEventArgs e)
    {
        // If there is currently an adorner, dispose of it
        DisposeDragAdorner();
    }

    private void DisposeDragAdorner()
    {
        if (_dragAdorner == null)
            return;

        AdornerLayer.GetAdornerLayer(this)!.Children.Remove(_dragAdorner);
        (_dragAdorner.Source as RenderTargetBitmap)?.Dispose();
        _dragAdorner = null;
    }

    private void UpdateDragAdorner(Point position)
    {
        if (_dragAdorner == null)
            return;

        _dragAdorner.RenderTransform = new TranslateTransform(_dragStartPosition.X - _elementDragOffset.X, position.Y - _elementDragOffset.Y);
    }

    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        DisposeDragAdorner();
    }

    #endregion
}