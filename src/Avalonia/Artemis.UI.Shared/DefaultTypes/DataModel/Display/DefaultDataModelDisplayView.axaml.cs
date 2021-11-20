using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Artemis.UI.Shared.DefaultTypes.DataModel.Display
{
    public partial class DefaultDataModelDisplayView : UserControl
    {
        public DefaultDataModelDisplayView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
