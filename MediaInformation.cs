using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MovieViewer
{
	static class MediaInformation
	{

		static IDisposable? GetDisposable(DependencyObject obj)
		{
			return (IDisposable?)obj.GetValue(DisposableProperty);
		}

		static void SetDisposable(DependencyObject obj, IDisposable? value)
		{
			obj.SetValue(DisposableProperty, value);
		}

		// Using a DependencyProperty as the backing store for Disposable.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DisposableProperty =
			DependencyProperty.RegisterAttached("Disposable", typeof(IDisposable), typeof(MediaInformation), new PropertyMetadata(default));

		class Disposer : IDisposable
		{
			readonly Action dispose;
			public Disposer(Action dispose) => this.dispose = dispose;
			public void Dispose() => dispose();
		}


		public static bool GetLoadOnMediaOpened(DependencyObject obj)
		{
			return (bool)obj.GetValue(LoadOnMediaOpenedProperty);
		}

		public static void SetLoadOnMediaOpened(DependencyObject obj, bool value)
		{
			obj.SetValue(LoadOnMediaOpenedProperty, value);
		}

		// Using a DependencyProperty as the backing store for LoadOnMediaOpened.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LoadOnMediaOpenedProperty =
			DependencyProperty.RegisterAttached("LoadOnMediaOpened", typeof(bool), typeof(MediaInformation),
				new PropertyMetadata(
					false,
					(d,e) =>
					{
						if (d is not MediaElement me) return;
						GetDisposable(me)?.Dispose();
						SetDisposable(me, new Disposer(() => me.MediaOpened -= OnMediaOpend));
						me.MediaOpened += OnMediaOpend;
					}));

		static void OnMediaOpend(object sender, RoutedEventArgs e)
		{
			if (sender is not MediaElement me) return;
			SetVideoWidth(me, me.NaturalVideoWidth);
			SetVideoHeight(me, me.NaturalVideoHeight);
			SetVideoDuration(me, me.NaturalDuration.TimeSpan);
		}



		public static int GetVideoWidth(DependencyObject obj)
		{
			return (int)obj.GetValue(VideoWidthProperty);
		}

		public static void SetVideoWidth(DependencyObject obj, int value)
		{
			obj.SetValue(VideoWidthProperty, value);
		}

		// Using a DependencyProperty as the backing store for VideoWidth.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty VideoWidthProperty =
			DependencyProperty.RegisterAttached("VideoWidth", typeof(int), typeof(MediaInformation), new PropertyMetadata(0));



		public static int GetVideoHeight(DependencyObject obj)
		{
			return (int)obj.GetValue(VideoHeightProperty);
		}

		public static void SetVideoHeight(DependencyObject obj, int value)
		{
			obj.SetValue(VideoHeightProperty, value);
		}

		// Using a DependencyProperty as the backing store for VideoHeight.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty VideoHeightProperty =
			DependencyProperty.RegisterAttached("VideoHeight", typeof(int), typeof(MediaInformation), new PropertyMetadata(0));



		public static TimeSpan GetVideoDuration(DependencyObject obj)
		{
			return (TimeSpan)obj.GetValue(VideoDurationProperty);
		}

		public static void SetVideoDuration(DependencyObject obj, TimeSpan value)
		{
			obj.SetValue(VideoDurationProperty, value);
		}

		// Using a DependencyProperty as the backing store for VideoDuration.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty VideoDurationProperty =
			DependencyProperty.RegisterAttached("VideoDuration", typeof(TimeSpan), typeof(MediaInformation), new PropertyMetadata(TimeSpan.Zero));


	}
}
