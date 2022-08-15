using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MovieViewer
{
	static class ElementSizeInformation
	{


		public static bool GetLoadOnInitialized(DependencyObject obj)
		{
			return (bool)obj.GetValue(LoadOnInitializedProperty);
		}

		public static void SetLoadOnInitialized(DependencyObject obj, bool value)
		{
			obj.SetValue(LoadOnInitializedProperty, value);
		}

		// Using a DependencyProperty as the backing store for LoadOnInitialized.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty LoadOnInitializedProperty =
			DependencyProperty.RegisterAttached("LoadOnInitialized", typeof(bool), typeof(ElementSizeInformation),
				new PropertyMetadata(
					false,
					(d,e) =>
					{
						if (d is not FrameworkElement fe) return;
						static void OnInit(object? sender, RoutedEventArgs _)
						{
							if (sender is not FrameworkElement fe) return;
							fe.Loaded -= OnInit;
							SetWidth(fe, fe.ActualWidth);
							SetHeight(fe, fe.ActualHeight);
						}
						fe.Loaded += OnInit;
					}));



		public static double GetWidth(DependencyObject obj)
		{
			return (double)obj.GetValue(WidthProperty);
		}

		public static void SetWidth(DependencyObject obj, double value)
		{
			obj.SetValue(WidthProperty, value);
		}

		// Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty WidthProperty =
			DependencyProperty.RegisterAttached("Width", typeof(double), typeof(ElementSizeInformation), new PropertyMetadata(0.0));



		public static double GetHeight(DependencyObject obj)
		{
			return (double)obj.GetValue(HeightProperty);
		}

		public static void SetHeight(DependencyObject obj, double value)
		{
			obj.SetValue(HeightProperty, value);
		}

		// Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.RegisterAttached("Height", typeof(double), typeof(ElementSizeInformation), new PropertyMetadata(0.0));


	}
}
