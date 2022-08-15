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
	internal class ViewModel : INotifyPropertyChanged
	{
		public ICommand ExitCommand { get; } = new Command(_ => App.Current.Shutdown(0));
		public ICommand GoForwardCommand { get; }
		public ICommand GoBackwardCommand { get; }
		public Uri? Movie { get; }
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
		readonly static PropertyChangedEventArgs reqArgs = new PropertyChangedEventArgs(nameof(RequestPosition));
		readonly static PropertyChangedEventArgs curArgs = new PropertyChangedEventArgs(nameof(CurrentPosition));

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
			if(Model.TryGetMovie(out var u)) Movie = u;
			else
			{
				MessageBox.Show("File not found.");
				App.Current.Shutdown(0);
			}
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
