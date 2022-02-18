#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace MVVMUtils; 

/// <summary>
/// <inheritdoc cref="ReverseValueConverter{TFrom, TTo}"/>
/// <br/>Based on a specific converter type.
/// </summary>
/// <typeparam name="T">The specific converter type.</typeparam>
/// <typeparam name="TFrom"><inheritdoc cref="ReverseValueConverter{TFrom, TTo}"/></typeparam>
/// <typeparam name="TTo"><inheritdoc cref="ReverseValueConverter{TFrom, TTo}"/></typeparam>
public class ReverseValueConverter<T, TFrom, TTo> : ReverseValueConverter<TFrom, TTo> where T : ValueConverter<TFrom, TTo>, new() {

	/// <inheritdoc />
	public ReverseValueConverter( ValueConverter<TFrom, TTo> Converter ) : base(Converter) { }

	/// <summary>
	/// Default constructor.
	/// </summary>
	public ReverseValueConverter() : this(new T()) { }
}