﻿using VehicleDefence.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace VehicleDefence.View
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class VehicleDefencePage : VehicleDefence.Common.LayoutAwarePage
    {
        public VehicleDefencePage()
        {
            this.InitializeComponent();

            SettingsPane.GetForCurrentView().CommandsRequested += AircraftsPage_CommandsRequested;
        }

        void AircraftsPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            UICommandInvokedHandler invokeHandler = new UICommandInvokedHandler(AboutInvokeHandler);
            SettingsCommand aboutCommand = new SettingsCommand("About", "About Invaders", invokeHandler);
            args.Request.ApplicationCommands.Add(aboutCommand);
        }

        private void AboutInvokeHandler(IUICommand command)
        {
            viewModel.Paused = true;
            aboutPopup.IsOpen = true;
        }

        private void CloseAboutPopup(object sender, RoutedEventArgs e)
        {
            aboutPopup.IsOpen = false;
            viewModel.Paused = false;
        }

        private void LearnButton(object sender, RoutedEventArgs e)
        {
            learnMorePopup.IsOpen = true;
        }

        private void CloseLearnMorePopup(object sender, RoutedEventArgs e)
        {
            learnMorePopup.IsOpen = false;
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            aboutPopup.IsOpen = false;
            learnMorePopup.IsOpen = false;
            viewModel.StartGame();
            firstTapOfGame = true;
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            viewModel.KeyDown(e.VirtualKey);
        }

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            viewModel.KeyUp(e.VirtualKey);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += KeyDownHandler;
            Window.Current.CoreWindow.KeyUp += KeyUpHandler;
            base.OnNavigatedTo(e);
        }

        private void pageRoot_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            viewModel.LeftGestureCompleted();
            viewModel.RightGestureCompleted();
        }

        private void pageRoot_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            viewModel.LeftGestureCompleted();
            viewModel.RightGestureCompleted();
        }

        bool firstTapOfGame = false;
        private void pageRoot_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!firstTapOfGame)
                viewModel.Tapped();

            firstTapOfGame = false;
        }

        private void playArea_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePlayAreaSize(playArea.RenderSize);
        }

        private void pageRoot_SizeChanged(Object sender, SizeChangedEventArgs e)
        {
            UpdatePlayAreaSize(new Size(e.NewSize.Width, e.NewSize.Height - 100));
        }

        private void UpdatePlayAreaSize(Size newPlayAreaSize)
        {
            double targetWidth;
            double targetHeight;
            if (newPlayAreaSize.Width > newPlayAreaSize.Height)
            {
                targetWidth = newPlayAreaSize.Height * 4 / 3;
                targetHeight = newPlayAreaSize.Height;
                double leftRightMargin = (newPlayAreaSize.Width - targetWidth) / 2;
                playArea.Margin = new Thickness(leftRightMargin, 0, leftRightMargin, 0);
            }
            else
            {
                targetHeight = newPlayAreaSize.Width * 3 / 4;
                targetWidth = newPlayAreaSize.Width;
                double topBottomMargin = (newPlayAreaSize.Height - targetHeight) / 2;
                playArea.Margin = new Thickness(0, topBottomMargin, 0, topBottomMargin);
            }
            playArea.Width = targetWidth;
            playArea.Height = targetHeight;
            viewModel.PlayAreaSize = new Size(targetWidth, targetHeight);
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
        }

        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }
    }
}
