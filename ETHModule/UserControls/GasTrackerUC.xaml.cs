﻿using ETHModule.ViewModels;
using System.Windows.Controls;

namespace ETHModule.UserControls
{
    /// <summary>
    /// Логика взаимодействия для GasTrackerUC.xaml
    /// </summary>
    public partial class GasTrackerUC : UserControl
    {
        public GasTrackerUC(GasTrackerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
