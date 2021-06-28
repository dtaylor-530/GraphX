// Decompiled with JetBrains decompiler
// Type: Elysium.Parameters.General
// Assembly: Elysium, Version=2.0.1042.4, Culture=neutral, PublicKeyToken=afa220db249e5b15
// MVID: F3B07FED-9CD6-4976-A4D1-B7D4AB0D7923
// Assembly location: D:\Repos\Elysium-Extra\packages\Elysium.Theme.2.0.4\lib\net45\Elysium.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media;

namespace Elysium.Parameters {
   /// <summary>
   ///   Represents a class that manages the settings of the styles of the controls.
   /// </summary>
   public static class General {



      public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.RegisterAttached("HeaderFontSize", typeof(double), typeof(General), new FrameworkPropertyMetadata(16d)
      );
      public static void SetHeaderFontSize(UIElement element, double value) {
         element.SetValue(HeaderFontSizeProperty, value);
      }
      public static double GetHeaderFontSize(UIElement element) {
         return (double)element.GetValue(HeaderFontSizeProperty);
      }
      
      public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.RegisterAttached("TextFontSize", typeof(double), typeof(General), new FrameworkPropertyMetadata(10d)
      );
      public static void SetTextFontSize(UIElement element, double value) {
         element.SetValue(TextFontSizeProperty, value);
      }
      public static double GetTextFontSize(UIElement element) {
         return (double)element.GetValue(TextFontSizeProperty);
      }


      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.TitleFontSize" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty TitleFontSizeProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.HeaderFontSize" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty HeaderFontSizeProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.ContentFontSize" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty ContentFontSizeProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.TextFontSize" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty TextFontSizeProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.DefaultThickness" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty DefaultThicknessProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.SemiBoldThickness" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty SemiBoldThicknessProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.BoldThickness" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty BoldThicknessProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.DefaultThicknessValue" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty DefaultThicknessValueProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.SemiBoldThicknessValue" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty SemiBoldThicknessValueProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.BoldThicknessValue" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty BoldThicknessValueProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.DefaultPadding" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty DefaultPaddingProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.SemiBoldPadding" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty SemiBoldPaddingProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.BoldPadding" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty BoldPaddingProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.DefaultPaddingValue" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty DefaultPaddingValueProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.SemiBoldPaddingValue" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty SemiBoldPaddingValueProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.BoldPaddingValue" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty BoldPaddingValueProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.DefaultDuration" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty DefaultDurationProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.MinimumDuration" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty MinimumDurationProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.OptimumDuration" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty OptimumDurationProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.MaximumDuration" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty MaximumDurationProperty;
      ///// <summary>
      /////   Identifies the <see cref="P:Elysium.Parameters.General.ShadowBrush" /> dependency property.
      ///// </summary>
      //public static readonly DependencyProperty ShadowBrushProperty;
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.TitleFontSize" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>The size of the font of the window's title.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetTitleFontSize(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.TitleFontSize" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">The size of the font of the window's title.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetTitleFontSize(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.HeaderFontSize" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>The size of the font of the control's title.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetHeaderFontSize(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.HeaderFontSize" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">The size of the font of the control's title.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetHeaderFontSize(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.ContentFontSize" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>The size of the font of the control's content.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetContentFontSize(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.ContentFontSize" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">The size of the font of the control's content.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetContentFontSize(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.TextFontSize" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>The size of the font of the text.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetTextFontSize(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.TextFontSize" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">The size of the font of the text.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetTextFontSize(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.DefaultThickness" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>Толщина линии по умолчанию.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static Thickness GetDefaultThickness(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.DefaultThickness" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">Default thickness of the line.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetDefaultThickness(DependencyObject obj, Thickness value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.SemiBoldThickness" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>A thickness of the semibold line.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static Thickness GetSemiBoldThickness(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.SemiBoldThickness" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">A thickness of the semibold line.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetSemiBoldThickness(DependencyObject obj, Thickness value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.BoldThickness" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>A thickness of the bold line.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static Thickness GetBoldThickness(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.BoldThickness" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">A thickness of the bold line.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetBoldThickness(DependencyObject obj, Thickness value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.DefaultThicknessValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>Defalut thickness of the line.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetDefaultThicknessValue(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.DefaultThicknessValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">Default thickness of the line.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetDefaultThicknessValue(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.SemiBoldThicknessValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>A thickness of the semibold line.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetSemiBoldThicknessValue(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.SemiBoldThicknessValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">A thickness of the semibold line.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetSemiBoldThicknessValue(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.BoldThicknessValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>A thickness of the bold line.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetBoldThicknessValue(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.BoldThicknessValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">A thickness of the bold line.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetBoldThicknessValue(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.DefaultPadding" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>Default value of padding.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static Thickness GetDefaultPadding(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.DefaultPadding" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">Default value of padding.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetDefaultPadding(DependencyObject obj, Thickness value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.SemiBoldPadding" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>The value of the medium-size padding.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static Thickness GetSemiBoldPadding(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.SemiBoldPadding" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">The value of medium-size padding.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetSemiBoldPadding(DependencyObject obj, Thickness value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.BoldPadding" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>The value of the biggest padding.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static Thickness GetBoldPadding(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.BoldPadding" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">The value of the biggest padding.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetBoldPadding(DependencyObject obj, Thickness value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.DefaultPaddingValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>Default value of padding.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetDefaultPaddingValue(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.DefaultPaddingValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">Default value of padding.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetDefaultPaddingValue(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.SemiBoldPaddingValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>The value of the medium-size padding.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetSemiBoldPaddingValue(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.SemiBoldPaddingValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">The value of medium-size padding.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetSemiBoldPaddingValue(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.BoldPaddingValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>The value of the biggest padding.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //[SuppressMessage("Microsoft.Contracts", "Ensures", Justification = "Can\'t be proven.")]
      //public static double GetBoldPaddingValue(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.BoldPaddingValue" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">The value of the biggest padding.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetBoldPaddingValue(DependencyObject obj, double value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.DefaultDuration" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>Default duration of the animation.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static Duration GetDefaultDuration(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.DefaultDuration" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">Default duration of the animation.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetDefaultDuration(DependencyObject obj, Duration value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.MinimumDuration" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>Minimum duration of the animation.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static Duration GetMinimumDuration(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.MinimumDuration" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">Minimum duration of the animation.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetMinimumDuration(DependencyObject obj, Duration value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.OptimumDuration" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>Optimum duration of the animation.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static Duration GetOptimumDuration(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.OptimumDuration" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">Optimum duration of the animation.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetOptimumDuration(DependencyObject obj, Duration value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.MaximumDuration" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>Maximum duration of the animation.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static Duration GetMaximumDuration(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.MaximumDuration" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">Maximum duration of the animation.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetMaximumDuration(DependencyObject obj, Duration value);
      ///// <summary>
      /////   Returns the value of the <see cref="P:Elysium.Parameters.General.ShadowBrush" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being got.</param>
      ///// <returns>A brush, used for shadow creation.</returns>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static SolidColorBrush GetShadowBrush(DependencyObject obj);
      ///// <summary>
      /////   Sets the value of the <see cref="P:Elysium.Parameters.General.ShadowBrush" /> dependency property for the specified control.
      ///// </summary>
      ///// <param name="obj">The  control, for which the value of the property is being set.</param>
      ///// <param name="value">A brush used for shadow creation.</param>
      ///// <exception cref="T:System.ArgumentNullException">
      /////   The parameter <paramref name="obj" /> has a value null.
      ///// </exception>
      //public static void SetShadowBrush(DependencyObject obj, SolidColorBrush value);
   }
}
