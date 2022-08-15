using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MovieViewer
{
	enum VideoStretch { Normal, Fill }
	internal class ViewModel : INotifyPropertyChanged
	{
		public ICommand ExitCommand { get; } = new Command(_ => App.Current.Shutdown(0));
		public ICommand GoForwardCommand { get; }
		public ICommand GoBackwardCommand { get; }
		public ICommand VideoStretchCommand { get; }
		public ICommand LocationCommand { get; }
		public Uri? Movie { get; }

		public VideoStretch VideoStretch { get; private set; }

		public double WindowWidth { get; set; }
		public double WindowHeight { get; set; }
		public int VideoWidth { get; set; }
		public int VideoHeight { get; set; }
		public double ScreenLeft { get; private set; }
		public double ScreenTop { get; private set; }
		public double ScreenWidth { get; private set; }
		public double ScreenHeight { get; private set; }

		TimeSpan _currentposition;
		public TimeSpan CurrentPosition
		{
			get => _currentposition;
			set
			{
				_currentposition = value;
				PropertyChanged?.Invoke(this, curArgs);
			}
		}
		public TimeSpan RequestPosition { get; private set; } = TimeSpan.FromMilliseconds(-1);

		public event PropertyChangedEventHandler? PropertyChanged;
		readonly static PropertyChangedEventArgs reqArgs = new(nameof(RequestPosition));
		readonly static PropertyChangedEventArgs curArgs = new(nameof(CurrentPosition));
		readonly static PropertyChangedEventArgs strArgs = new(nameof(VideoStretch));
		readonly static PropertyChangedEventArgs lftArgs = new(nameof(ScreenLeft));
		readonly static PropertyChangedEventArgs topArgs = new(nameof(ScreenTop));
		readonly static IEnumerable<PropertyChangedEventArgs> rectArgs = new PropertyChangedEventArgs[]
		{
			new (nameof(ScreenLeft)), new (nameof(ScreenTop)),
			new (nameof(ScreenWidth)), new (nameof(ScreenHeight))
		};

		public ViewModel()
		{
			GoForwardCommand = new Command(
				p =>
				{
					var skip = TimeSpan.TryParse(p as string, out var ts) ? ts : TimeSpan.FromSeconds(1);
					RequestPosition = CurrentPosition + skip;
					RaiseChanged();
					RequestPosition = TimeSpan.FromMilliseconds(-1);
					RaiseChanged();
				});
			GoBackwardCommand = new Command(
				p =>
				{
					var skip = TimeSpan.TryParse(p as string, out var ts) ? ts : TimeSpan.FromSeconds(1);
					var c = CurrentPosition - skip;
					RequestPosition = c > TimeSpan.Zero ? c : TimeSpan.Zero;
					RaiseChanged();
					RequestPosition = TimeSpan.FromMilliseconds(-1);
					RaiseChanged();
				});
			VideoStretchCommand = new Command(
				_ =>
				{
					(ScreenLeft, ScreenTop, ScreenWidth, ScreenHeight) = ResetLocation(VideoWidth, VideoHeight, WindowWidth, WindowHeight);
					foreach (var a in rectArgs)
						PropertyChanged?.Invoke(this, a);
					VideoStretch = VideoStretch == VideoStretch.Fill ? VideoStretch.Normal : VideoStretch.Fill;
					PropertyChanged?.Invoke(this, strArgs);
				});
			LocationCommand = new Command(
				p =>
				{
					if (VideoStretch is not VideoStretch.Fill) return;
					var range = int.TryParse(p as string, out var i) ? i : 0;
					if (ScreenWidth - WindowWidth > Math.Abs(range))
					{
						ScreenLeft += range;
						PropertyChanged?.Invoke(this, lftArgs);
					}
					else if(ScreenHeight-WindowHeight > Math.Abs(range))
					{
						ScreenTop += range;
						PropertyChanged?.Invoke(this, topArgs);
					}
				});

			if(Model.TryGetMovie(out var u)) Movie = u;
			else
			{
				MessageBox.Show("File not found.");
				App.Current.Shutdown(0);
			}
		}

		static (double ScreenLeft, double ScreenTop, double ScreenWidth, double ScreenHeight) ResetLocation(int videoWidth, int videoHeight, double windowWidth, double windowHeight)
		{
			(double screenW, double screenH) = videoWidth / videoHeight < windowWidth / windowHeight ?
				(windowWidth, videoHeight * windowWidth / videoWidth) :
				(videoWidth * windowHeight / videoHeight, windowHeight);
			return ((windowWidth - screenW) / 2, (windowHeight - screenH) / 2, screenW, screenH);
		}

		void RaiseChanged() => PropertyChanged?.Invoke(this, reqArgs);
		class Command : ICommand
		{
			public event EventHandler? CanExecuteChanged;
			readonly Action<object?> execute;
			readonly Func<object?,bool> canExecute;
			public Command(Action<object?> execute, Func<object?, bool>? canExecute = default) =>
				(this.execute, this.canExecute) = (execute, canExecute ?? (_ => true));
			public bool CanExecute(object? parameter) => canExecute(parameter);
			public void Execute(object? parameter) => execute(parameter);
			public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
