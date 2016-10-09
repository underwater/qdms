// -----------------------------------------------------------------------
// <copyright file="SessionTemplatesWindow.xaml.cs" company="">
// Copyright 2013 Alexander Soffronow Pagonidis
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using EntityData;
using MahApps.Metro.Controls;
using QDMS;
using QDMSServer.ViewModels;
using ReactiveUI;
using System.Reactive.Linq;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for SessionTemplatesWindow.xaml
    /// </summary>
    public partial class ScheduledJobsWindow : MetroWindow
    {
        public ScheduledJobsViewModel ViewModel { get; set; }
        public ScheduledJobsWindow()
        {
            InitializeComponent();

            ViewModel = new ScheduledJobsViewModel();
            this.WhenAnyObservable(x => x.ViewModel.DeleteJobCommand)
                .Subscribe(_ =>
                {
                    var dialogResult = MessageBox.Show(
                          $"Are you sure you want to delete {ViewModel.SelectedJob.Name}?"
                        , "Delete Job"
                        , MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.Yes)
                        ViewModel.ConfirmDeleteJobCommand.Execute(null);
                });
            MessageBus.Current.Listen<string>().Subscribe(x => 
            {
                MessageBox.Show(x);
            });
            DataContext = ViewModel;          

        }

        
    }
}
