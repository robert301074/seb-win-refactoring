﻿/*
 * Copyright (c) 2018 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System.Windows;
using System.Windows.Documents;
using SafeExamBrowser.Contracts.Configuration;
using SafeExamBrowser.Contracts.I18n;
using SafeExamBrowser.Contracts.Logging;
using SafeExamBrowser.Contracts.UserInterface.Taskbar.Events;
using SafeExamBrowser.Contracts.UserInterface.Windows;
using SafeExamBrowser.UserInterface.Classic.ViewModels;

namespace SafeExamBrowser.UserInterface.Classic
{
	public partial class RuntimeWindow : Window, IRuntimeWindow
	{
		private bool allowClose;
		private AppConfig appConfig;
		private IText text;
		private RuntimeWindowViewModel model;
		private WindowClosingEventHandler closing;

		public bool TopMost
		{
			get { return Dispatcher.Invoke(() => Topmost); }
			set { Dispatcher.Invoke(() => Topmost = value); }
		}

		event WindowClosingEventHandler IWindow.Closing
		{
			add { closing += value; }
			remove { closing -= value; }
		}

		public RuntimeWindow(AppConfig appConfig, IText text)
		{
			this.appConfig = appConfig;
			this.text = text;

			InitializeComponent();
			InitializeRuntimeWindow();

			Loaded += RuntimeWindow_Loaded;
		}

		public void BringToForeground()
		{
			Dispatcher.Invoke(Activate);
		}

		public new void Close()
		{
			Dispatcher.Invoke(() =>
			{
				allowClose = true;
				base.Close();
			});
		}

		public new void Hide()
		{
			Dispatcher.Invoke(base.Hide);
		}

		public void HideProgressBar()
		{
			model.ProgressBarVisibility = Visibility.Hidden;
		}

		public void Notify(ILogContent content)
		{
			Dispatcher.Invoke(() =>
			{
				model.Notify(content);
				LogScrollViewer.ScrollToEnd();
			});
		}

		public void Progress(int amount = 1)
		{
			model.CurrentProgress += amount;
		}

		public void Regress(int amount = 1)
		{
			model.CurrentProgress -= amount;
		}

		public void SetIndeterminate()
		{
			model.IsIndeterminate = true;
		}

		public void SetMaxValue(int max)
		{
			model.MaxProgress = max;
		}

		public void SetValue(int value)
		{
			model.CurrentProgress = value;
		}

		public void ShowProgressBar()
		{
			model.ProgressBarVisibility = Visibility.Visible;
		}

		public void UpdateText(TextKey key, bool showBusyIndication = false)
		{
			model.StopBusyIndication();
			model.Status = text.Get(key);

			if (showBusyIndication)
			{
				model.StartBusyIndication();
			}
		}

		public new void Show()
		{
			Dispatcher.Invoke(base.Show);
		}

		private void InitializeRuntimeWindow()
		{
			Title = $"{appConfig.ProgramTitle} - Version {appConfig.ProgramVersion}";

			InfoTextBlock.Inlines.Add(new Run($"Version {appConfig.ProgramVersion}") { FontStyle = FontStyles.Italic });
			InfoTextBlock.Inlines.Add(new LineBreak());
			InfoTextBlock.Inlines.Add(new LineBreak());
			InfoTextBlock.Inlines.Add(new Run(appConfig.ProgramCopyright) { FontSize = 10 });

			model = new RuntimeWindowViewModel(LogTextBlock);
			AnimatedBorder.DataContext = model;
			ProgressBar.DataContext = model;
			StatusTextBlock.DataContext = model;

			Closing += (o, args) => args.Cancel = !allowClose;
		}

		private void RuntimeWindow_Loaded(object sender, RoutedEventArgs e)
		{
			Left = (SystemParameters.WorkArea.Right / 2) - (Width / 2);
			Top = (SystemParameters.WorkArea.Bottom / 2) - (Height / 2);
		}
	}
}
