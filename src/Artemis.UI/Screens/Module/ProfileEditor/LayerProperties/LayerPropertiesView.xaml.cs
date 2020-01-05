﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Artemis.UI.Screens.Module.ProfileEditor.LayerProperties
{
    /// <summary>
    /// Interaction logic for LayerPropertiesView.xaml
    /// </summary>
    public partial class LayerPropertiesView : UserControl
    {
        public LayerPropertiesView()
        {
            InitializeComponent();
        }

        // Keeping the scroll viewers in sync is up to the view, not a viewmodel concern
    }
}