#region Copyright (C) 2017-2021  Starflash Studios

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html

#endregion

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

namespace MVVMUtils; 

/// <summary>
/// Represents the functional inverse of a given <see cref="ValueConverter{TTo, TFrom}"/>.
/// </summary>
/// <typeparam name="TFrom">The type to (originally) convert from. (i.e. The type that will be converted into)</typeparam>
/// <typeparam name="TTo">The type to (originally) convert into. (i.e. The type that will be converted from)</typeparam>
public class ReverseValueConverter<TFrom, TTo> : ValueConverter<TTo, TFrom> {
	/// <summary>
	/// The converter to reverse.
	/// </summary>
	public ValueConverter<TFrom, TTo> Converter { get; }

	/// <summary>
	/// Default constructor.
	/// </summary>
	/// <param name="Converter">The converter to reverse.</param>
	public ReverseValueConverter( ValueConverter<TFrom, TTo> Converter ) => this.Converter = Converter;

	/// <inheritdoc />
	public override bool CanForward => Converter.CanReverse;

	/// <inheritdoc />
	public override bool CanReverse => Converter.CanForward;

	/// <inheritdoc />
	public override bool CanForwardWhenNull => Converter.CanReverseWhenNull;

	/// <inheritdoc />
	public override bool CanReverseWhenNull => Converter.CanForwardWhenNull;

	/// <inheritdoc />
	public override TFrom? Forward( [DisallowNull] TTo From, object? Parameter = null, CultureInfo? Culture = null ) => Converter.Reverse(From, Parameter, Culture);

	/// <inheritdoc />
	public override TTo? Reverse( [DisallowNull] TFrom To, object? Parameter = null, CultureInfo? Culture = null ) => Converter.Forward(To, Parameter, Culture);

	/// <inheritdoc />
	public override TFrom? ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Converter.ReverseWhenNull(Parameter, Culture);

	/// <inheritdoc />
	public override TTo? ReverseWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => Converter.ForwardWhenNull(Parameter, Culture);
}

/// <summary>
/// Retrieves a specific element in a specific collection.
/// </summary>
public class SpecificElementConverter : IMultiValueConverter {

	/// <inheritdoc />
	public object? Convert( object[] Values, Type TargetType, object Parameter, CultureInfo Culture ) => Values.TryGetAt(0, out IList Ls) && Values.TryGetAt(1, out int Index) ? Ls[Index] : Values.TryGetAt(0, out int IndexB) && Values.TryGetAt(1, out IList LsB) ? LsB[IndexB] : null;

	/// <inheritdoc />
	public object[] ConvertBack( object Value, Type[] TargetTypes, object Parameter, CultureInfo Culture ) => throw new NotSupportedException($"{nameof(SpecificElementConverter)} is unable to convert backwards.");
}