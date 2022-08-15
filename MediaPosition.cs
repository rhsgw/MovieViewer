using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MovieViewer
{
	internal static class MediaPosition
	{


		static IDisposable GetDisposable(DependencyObject obj)
		{
			return (IDisposable)obj.GetValue(DisposableProperty);
		}

		static void SetDisposable(DependencyObject obj, IDisposable value)
		{
			obj.SetValue(DisposableProperty, value);
		}

		// Using a DependencyProperty as the backing store for Disposable.  This enables animation, styling, binding, etc...
		static readonly DependencyProperty DisposableProperty =
			DependencyProperty.RegisterAttached("Disposable", typeof(IDisposable), typeof(MediaPosition),
				new PropertyMetadata(default,
					(d,e) =>
					{
						var disposable = (IDisposable)e.OldValue;
						disposable?.Dispose();
					}));



		public static TimeSpan GetInterval(DependencyObject obj)
		{
			return (TimeSpan)obj.GetValue(IntervalProperty);
		}

		public static void SetInterval(DependencyObject obj, TimeSpan value)
		{
			obj.SetValue(IntervalProperty, value);
		}

		// Using a DependencyProperty as the backing store for Interval.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IntervalProperty =
			DependencyProperty.RegisterAttached("Interval", typeof(TimeSpan), typeof(MediaPosition),
				new PropertyMetadata(
					TimeSpan.FromMilliseconds(-1),
					(d,e) =>
					{
						if (d is not MediaElement me) return;
						var t = new Timer(_ => me.Dispatcher.BeginInvoke(() => SetCurrent(me, me.Position)), null, (TimeSpan)e.NewValue, (TimeSpan)e.NewValue);
						SetDisposable(me, t);
					}));




		public static TimeSpan GetCurrent(DependencyObject obj)
		{
			return (TimeSpan)obj.GetValue(CurrentProperty);
		}

		public static void SetCurrent(DependencyObject obj, TimeSpan value)
		{
			obj.SetValue(CurrentProperty, value);
		}

		// Using a DependencyProperty as the backing store for Current.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CurrentProperty =
			DependencyProperty.RegisterAttached("Current", typeof(TimeSpan), typeof(MediaPosition), new PropertyMetadata(TimeSpan.Zero));



		public static TimeSpan GetRequired(DependencyObject obj)
		{
			return (TimeSpan)obj.GetValue(RequiredProperty);
		}

		public static void SetRequired(DependencyObject obj, TimeSpan value)
		{
			obj.SetValue(RequiredProperty, value);
		}

		// Using a DependencyProperty as the backing store for Required.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty RequiredProperty =
			DependencyProperty.RegisterAttached("Required", typeof(TimeSpan), typeof(MediaPosition),
				new PropertyMetadata(
					TimeSpan.Zero,
					(d,e) =>
					{
						if(d is not MediaElement me) return;
						var req = (TimeSpan)e.NewValue;
						if (req < TimeSpan.Zero) return;
						me.Position = req;
					}));


	}
}
