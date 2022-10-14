using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ReactiveUI;

namespace Artemis.UI.Screens.Root;

public class SplashView : ReactiveWindow<SplashViewModel>
{
    public SplashView()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.WhenActivated(d =>
        {
            if (ViewModel == null)
                return;
            
            // Close when initialized
            Observable.FromEventPattern(x => ViewModel.CoreService.Initialized += x, x => ViewModel!.CoreService.Initialized -= x).Subscribe(_ => Dispatcher.UIThread.Post(Close)).DisposeWith(d);
            // Close if already initialized (can happen when there are no plugins)
            if (ViewModel.CoreService.IsInitialized)
                Close();
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}