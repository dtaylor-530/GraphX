using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PropertyTools.Wpf;

using ColumnDefinition = System.Windows.Controls.ColumnDefinition;

namespace Graph.Bayesian.WPF.Infrastructure
{
    /// <summary>
    /// Provides a custom property control factory.
    /// </summary>
    public class CustomPropertyGridControlFactory : PropertyGridControlFactory
    {
        public override FrameworkElement CreateControl(PropertyItem pi, PropertyControlFactoryOptions options)
        {
            // Check if the property is of type Range
            if (pi.Is(typeof(Message)))
            {
                // Create a control to edit the Range
                return this.CreateMessageControl(pi, options);
            }
            if (pi.Is(typeof(DateTime)))
            {
                // Create a control to edit the Range
                return this.CreateDateTimeControl(pi, options);
            }

            return base.CreateControl(pi, options);
        }


        protected virtual FrameworkElement CreateMessageControl(PropertyItem pi, PropertyControlFactoryOptions options)
        {
            var grid = new Grid();
            int i = 0;
            foreach (var control in CreateChildren(pi))
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                Grid.SetColumn(control, i++);
                grid.Children.Add(control);
            }
            return grid;


            IEnumerable<FrameworkElement> CreateChildren(PropertyItem pi)
            {
                var fromBox = new TextBlock();
                fromBox.SetBinding(TextBlock.TextProperty, new Binding(pi.Descriptor.Name + "." + nameof(Message.From)) { Mode = BindingMode.OneTime });
                yield return fromBox;

                var toBox = new TextBlock();
                toBox.SetBinding(TextBlock.TextProperty, new Binding(pi.Descriptor.Name + "." + nameof(Message.To)) { Mode = BindingMode.OneTime });
                yield return toBox;

                var sentBox = new TextBlock();
                sentBox.SetBinding(TextBlock.TextProperty, new Binding(pi.Descriptor.Name + "." + nameof(Message.Sent)) { Mode = BindingMode.OneTime, StringFormat = "{0:mm-ss}" });
                yield return sentBox;
            }
        }

        protected virtual FrameworkElement CreateDateTimeControl(PropertyItem pi, PropertyControlFactoryOptions options)
        {
            var sentBox = new TextBlock();
            var binding = pi.CreateOneWayBinding();
            binding.StringFormat = "{0:mm-ss}";
            sentBox.SetBinding(TextBlock.TextProperty, binding);
            return sentBox;

        }

        public static CustomPropertyGridControlFactory Instance { get; } = new CustomPropertyGridControlFactory();
    }
}
