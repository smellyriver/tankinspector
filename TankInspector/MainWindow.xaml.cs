using Smellyriver.TankInspector.Modeling;
using Smellyriver.TankInspector.UIComponents;
using Smellyriver.Wpf.Interop;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Smellyriver.TankInspector
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		internal MainWindowViewModel ViewModel
		{
			get => this.DataContext as MainWindowViewModel;
			set => this.DataContext = value;
		}

		private bool _isFirstRun;

		public MainWindow()
		{
			InitializeComponent();

			this.Loaded += MainWindow_Loaded;

			this.ViewModel = new MainWindowViewModel(this.CommandBindings);
			this.ViewModel.LoadComplete += ViewModel_LoadComplete;

			if (this.ProcessFirstRun())
			{
				this.ViewModel.BeginLoad();
			}
			else
				Application.Current.Shutdown();

			var screen = Screen.GetScreenFrom(this);

			if (_isFirstRun)
			{
				ApplicationSettings.Default.MainWindowWidth = this.Width = Math.Min(1366, screen.WorkingArea.Width);
				ApplicationSettings.Default.MainWindowHeight = this.Height = Math.Min(720, screen.WorkingArea.Height);
				ApplicationSettings.Default.MainWindowX = this.Left = (screen.WorkingArea.Width - this.Width) / 2;
				ApplicationSettings.Default.MainWindowY = this.Top = (screen.WorkingArea.Height - this.Height) / 2;
				ApplicationSettings.Default.MainWindowMaximized = false;
				ApplicationSettings.Default.Save();
			}
			else
			{
				ApplicationSettings.Default.MainWindowWidth = this.Width = Math.Min(ApplicationSettings.Default.MainWindowWidth, screen.WorkingArea.Width);
				ApplicationSettings.Default.MainWindowHeight = this.Height = Math.Min(ApplicationSettings.Default.MainWindowHeight, screen.WorkingArea.Height);
				this.Left = ApplicationSettings.Default.MainWindowX;
				this.Top = ApplicationSettings.Default.MainWindowY;
				this.WindowState = ApplicationSettings.Default.MainWindowMaximized ? WindowState.Maximized : WindowState.Normal;
				ApplicationSettings.Default.Save();
			}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);

			ApplicationSettings.Default.MainWindowWidth = this.Width;
			ApplicationSettings.Default.MainWindowHeight = this.Height;
			ApplicationSettings.Default.MainWindowX = this.Left;
			ApplicationSettings.Default.MainWindowY = this.Top;
			ApplicationSettings.Default.MainWindowMaximized = this.WindowState == WindowState.Maximized;
			ApplicationSettings.Default.Save();
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (_isFirstRun && this.ViewModel.IsLoaded)
				this.ShowTutorial();
		}

		private bool ProcessFirstRun()
		{
			if (AppState.Default.FirstRun)
			{
				AppState.Default.Upgrade();
				ApplicationSettings.Default.Upgrade();
			}

			if (AppState.Default.FirstRun)
			{
				_isFirstRun = true;

				if (ApplicationSettings.Default.GamePathes.Count == 0
					|| !Database.IsPathValid(ApplicationSettings.Default.GamePathes[0]))
				{
					var firstRunWindow = new FirstRunWindow();
					if (firstRunWindow.ShowDialog() != true)
						return false;
				}
			}
			else
			{
				this.UnloadTutorial();
			}

			return true;
		}
		private void ShowTutorial()
		{
			var showStoryboard = (Storyboard)this.TutorialView.FindResource("ShowStoryboard");
			showStoryboard.Begin(this.TutorialView);
		}

		private void ViewModel_LoadComplete(object sender, EventArgs e)
		{
			this.LoadingView.Visibility = Visibility.Hidden;
			var parent = (Grid)VisualTreeHelper.GetParent(this.LoadingView);
			parent.Children.Remove(this.LoadingView);

			if (_isFirstRun && this.IsLoaded)
			{
				this.ShowTutorial();
			}
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Escape:
					if (this.ViewModel.SettingsViewModel.IsShown)
					{
						this.ViewModel.SettingsViewModel.Hide();
						e.Handled = true;
					}
					else if (this.ViewModel.TankGallery != null && this.ViewModel.TankGallery.IsShown)
					{
						this.ViewModel.TankGallery.Hide();
						e.Handled = true;
					}
					break;

			}

			// todo: manage key mapping
			if (!e.Handled)
			{
				if (this.ViewModel.SettingsViewModel.IsShown)
				{
					if (!this.SettingsView.IsKeyboardFocusWithin)
						this.SettingsView.Focus();
				}
				else if (!this.HangarView.IsKeyboardFocusWithin)
					this.HangarView.Focus();
			}

		}

		private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (!e.Handled)
			{
				if (!this.HangarView.IsKeyboardFocusWithin)
					this.HangarView.Focus();
			}
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			this.ViewModel.IsWindowActive = true;
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			this.ViewModel.IsWindowActive = false;
		}

		private void ReadyButton_Click(object sender, RoutedEventArgs e)
		{
			var hideStoryboard = (Storyboard) this.TutorialView.FindResource("HideStoryboard");
			hideStoryboard.Begin(this.TutorialView);
		}

		private void UnloadTutorial()
		{
			var owner = VisualTreeHelper.GetParent(this.TutorialView) as Grid;
			owner?.Children.Remove(this.TutorialView);
		}

		private void TutorialViewHideStoryboard_Completed(object sender, EventArgs e)
		{
			this.UnloadTutorial();
		}

		private void HeaderImage_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

	}
}
