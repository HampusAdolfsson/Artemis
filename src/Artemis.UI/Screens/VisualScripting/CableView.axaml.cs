using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Rendering;
using ReactiveUI;

namespace Artemis.UI.Screens.VisualScripting;

public class CableView : ReactiveUserControl<CableViewModel>
{
    private const double CABLE_OFFSET = 24 * 4;
    private readonly Path _cablePath;
    private readonly Border _valueBorder;

    public CableView()
    {
        InitializeComponent();
        _cablePath = this.Get<Path>("CablePath");
        _valueBorder = this.Get<Border>("ValueBorder");

        // Not using bindings here to avoid a warnings
        this.WhenActivated(d =>
        {
            _valueBorder.GetObservable(BoundsProperty).Subscribe(rect => _valueBorder.RenderTransform = new TranslateTransform(rect.Width / 2 * -1, rect.Height / 2 * -1)).DisposeWith(d);

            ViewModel.WhenAnyValue(vm => vm.FromPoint).Subscribe(_ => Update(true)).DisposeWith(d);
            ViewModel.WhenAnyValue(vm => vm.ToPoint).Subscribe(_ => Update(false)).DisposeWith(d);
            Update(true);
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Update(bool from)
    {
        if (ViewModel == null)
            return;

        PathGeometry geometry = new()
        {
            Figures = new PathFigures()
        };
        PathFigure pathFigure = new()
        {
            StartPoint = ViewModel.FromPoint,
            IsClosed = false,
            Segments = new PathSegments
            {
                new BezierSegment
                {
                    Point1 = new Point(ViewModel.FromPoint.X + CABLE_OFFSET, ViewModel.FromPoint.Y),
                    Point2 = new Point(ViewModel.ToPoint.X - CABLE_OFFSET, ViewModel.ToPoint.Y),
                    Point3 = new Point(ViewModel.ToPoint.X, ViewModel.ToPoint.Y)
                }
            }
        };
        geometry.Figures.Add(pathFigure);
        _cablePath.Data = geometry;

        Canvas.SetLeft(_valueBorder, ViewModel.FromPoint.X + (ViewModel.ToPoint.X - ViewModel.FromPoint.X) / 2);
        Canvas.SetTop(_valueBorder, ViewModel.FromPoint.Y + (ViewModel.ToPoint.Y - ViewModel.FromPoint.Y) / 2);

        _cablePath.InvalidateVisual();
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        ViewModel?.UpdateDisplayValue(true);
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        ViewModel?.UpdateDisplayValue(false);
    }
}