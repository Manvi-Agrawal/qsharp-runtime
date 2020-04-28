﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Quantum.CsharpGeneration.EntryPointDriver
{
    /// <summary>
    /// Represents either a success or a failure of a process.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    public readonly struct Validation<T>
    {
        /// <summary>
        /// True if the validation succeeded.
        /// </summary>
        internal bool IsSuccess { get; }
        
        /// <summary>
        /// True if the validation failed.
        /// </summary>
        internal bool IsFailure => !IsSuccess;

        /// <summary>
        /// The success value of the validation.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the validation failed.</exception>
        internal T Value => IsSuccess ? ValueOrDefault : throw new InvalidOperationException();

        /// <summary>
        /// The success value of the validation or a default value if the validation failed.
        /// </summary>
        internal T ValueOrDefault { get; }
        
        /// <summary>
        /// The error message of the validation.
        /// </summary>
        internal string ErrorMessage { get; }

        /// <summary>
        /// Creates a new validation.
        /// </summary>
        /// <param name="isSuccess">True if the validation succeeded.</param>
        /// <param name="value">The success value or a default value</param>
        /// <param name="errorMessage">The error message.</param>
        private Validation(bool isSuccess, T value, string errorMessage)
        {
            IsSuccess = isSuccess;
            ValueOrDefault = value;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Creates a successful validation.
        /// </summary>
        /// <param name="value">The success value.</param>
        /// <returns>The successful validation.</returns>
        internal static Validation<T> Success(T value) =>
            new Validation<T>(true, value, default);

        /// <summary>
        /// Creates a failed validation.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The failed validation.</returns>
        internal static Validation<T> Failure(string errorMessage = null) =>
            new Validation<T>(false, default, errorMessage);
    }

    /// <summary>
    /// Extension methods for <see cref="Validation{T}"/>.
    /// </summary>
    internal static class ValidationExtensions
    {
        /// <summary>
        /// Sequentially composes two validations, passing the value of the first validation to another
        /// validation-producing function if the first validation is a success.
        /// </summary>
        /// <typeparam name="T">The type of the first validation's success value.</typeparam>
        /// <typeparam name="U">The type of the second validation's success value.</typeparam>
        /// <param name="validation">The first validation.</param>
        /// <param name="bind">
        /// A function that takes the value of the first validation and returns a second validation.
        /// </param>
        /// <returns>
        /// The first validation if the first validation is a failure; otherwise, the return value of calling the bind
        /// function on the first validation's success value.
        /// </returns>
        internal static Validation<U> Bind<T, U>(this Validation<T> validation, Func<T, Validation<U>> bind) =>
            validation.IsFailure ? Validation<U>.Failure(validation.ErrorMessage) : bind(validation.Value);

        /// <summary>
        /// Converts an enumerable of validations into a validation of an enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the validation success values.</typeparam>
        /// <param name="validations">The validations to sequence.</param>
        /// <returns>
        /// A validation that contains an enumerable of the validation values if all of the validations are a success,
        /// or the first error message if one of the validations is a failure.
        /// </returns>
        internal static Validation<IEnumerable<T>> Sequence<T>(this IEnumerable<Validation<T>> validations) =>
            validations.All(validation => validation.IsSuccess)
            ? Validation<IEnumerable<T>>.Success(validations.Select(validation => validation.Value))
            : Validation<IEnumerable<T>>.Failure(validations.First(validation => validation.IsFailure).ErrorMessage);

        /// <summary>
        /// Calls the action on the validation value if the validation is a success.
        /// </summary>
        /// <typeparam name="T">The type of the validation's success value.</typeparam>
        /// <param name="validation">The validation.</param>
        /// <param name="onSuccess">The action to call if the validation is a success.</param>
        internal static void Then<T>(this Validation<T> validation, Action<T> onSuccess)
        {
            if (validation.IsSuccess)
            {
                onSuccess(validation.Value);
            }
        }
    }
}
