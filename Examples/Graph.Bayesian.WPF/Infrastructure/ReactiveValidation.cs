// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Formatters.Abstractions;


namespace ReactiveUI.Validation.Helpers
{
    /// <summary>
    /// Base class for ReactiveObjects that support <see cref="INotifyDataErrorInfo"/> validation.
    /// </summary>
    public interface IReactiveValidationObject : IReactiveObject, IValidatableViewModel, INotifyDataErrorInfo
    {
        HashSet<string> MentionedPropertyNames { get; }
        IValidationTextFormatter<string> Formatter { get; }
        public void RaiseErrorsChanged(string propertyName = "");
    }

    public static class ValidationObjectHelper
    {

        /// <summary>
        /// Returns a collection of error messages, required by the INotifyDataErrorInfo interface.
        /// </summary>
        /// <param name="propertyName">Property to search error notifications for.</param>
        /// <returns>A list of error messages, usually strings.</returns>
        /// <inheritdoc />
        public static IEnumerable GetErrors(this IReactiveValidationObject validationObject, string? propertyName) =>
            propertyName is null || string.IsNullOrEmpty(propertyName)
                ? validationObject.ValidationContext.SelectInvalidPropertyValidations()
                    .Select(state => validationObject.Formatter.Format(state.Text ?? ValidationText.None))
                    .ToArray()
                : validationObject.ValidationContext.SelectInvalidPropertyValidations()
                    .Where(validation => validation.ContainsPropertyName(propertyName))
                    .Select(state => validationObject.Formatter.Format(state.Text ?? ValidationText.None))
                    .ToArray();


        /// <summary>
        /// Selects validation components that are invalid.
        /// </summary>
        /// <returns>Returns the invalid property validations.</returns>
        public static IEnumerable<IPropertyValidationComponent> SelectInvalidPropertyValidations(this ValidationContext validationContext) =>
            validationContext.Validations
                .OfType<IPropertyValidationComponent>()
                .Where(validation => !validation.IsValid);


        public static void ListenToValidationStatusChanges(this IReactiveValidationObject validationObject)
        {
            validationObject
                .ValidationContext                .Validations
                .ToObservableChangeSet()
                .ToCollection()
                .Select(components => components.Select(component => component.ValidationStatusChange.Select(_ => component)).Merge().StartWith(validationObject.ValidationContext))
                .Switch()
                .Subscribe(component =>
                OnValidationStatusChange(validationObject, component));
        }

        /// <summary>
        /// Updates the <see cref="HasErrors" /> property before raising the <see cref="ErrorsChanged" />
        /// event, and then raises the <see cref="ErrorsChanged" /> event. This behaviour is required by WPF, see:
        /// https://stackoverflow.com/questions/24518520/ui-not-calling-inotifydataerrorinfo-geterrors/24837028.
        /// </summary>
        /// <remarks>
        /// WPF doesn't understand string.Empty as an argument for the <see cref="ErrorsChanged"/>
        /// event, so we are sending <see cref="ErrorsChanged"/> notifications for every saved property.
        /// This is required for e.g. cases when a <see cref="IValidationComponent"/> is disposed and
        /// detached from the <see cref="ValidationContext"/>, and we'd like to mark all invalid
        /// properties as valid (because the thing that validates them no longer exists).
        /// </remarks>
        private static void OnValidationStatusChange(this IReactiveValidationObject validationObject, IValidationComponent component)
        {
            validationObject.RaisePropertyChanged(new PropertyChangedEventArgs(nameof(validationObject.HasErrors)));

            if (component is IPropertyValidationComponent propertyValidationComponent)
            {
                foreach (var propertyName in propertyValidationComponent.Properties)
                {
                    validationObject.RaiseErrorsChanged(propertyName);
                    validationObject.MentionedPropertyNames.Add(propertyName);
                }
            }
            else
            {
                foreach (var propertyName in validationObject.MentionedPropertyNames)
                {
                    validationObject.RaiseErrorsChanged(propertyName);
                }
            }
        }
    }
}