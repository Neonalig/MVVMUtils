#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Mvvm.ComponentModel;

#endregion

namespace MVVMUtils; 

/// <summary> Represents a singular observed property. </summary>
/// <typeparam name="T">The property type.</typeparam>
public class Observable<T> : ObservableObject, IEquatable<T?>, IEquatable<Observable<T>> {
	/// <summary>
	/// The internally stored value.
	/// <br/> Do not set directly accept for during construction, or invoke both <see cref="ObservableObject.OnPropertyChanging(string?)"/> and <see cref="ObservableObject.OnPropertyChanged(string?)"/> immediately before and after value change.
	/// </summary>
	T? _Value;

	/// <summary>
	/// Represents the bound value, invoking <see cref="ObservableObject.OnPropertyChanging(string?)"/> and <see cref="ObservableObject.OnPropertyChanged(string?)"/> upon value change.
	/// </summary>
	public T? Value {
		get => _Value;
		set => SetProperty(ref _Value, value);
	}

	/// <summary>
	/// Returns <see langword="true"/> if <see cref="Value"/> is not <see langword="null"/>; otherwise <see langword="false"/>.
	/// </summary>
	public bool HasValue => _Value is not null;

	/// <summary> The default constructor for a new <see cref="Observable{T}"/>. </summary>
	/// <param name="Value">The initial value of the bound object.</param>
	public Observable( T? Value = default ) {
		OnPropertyChanging(nameof(Value));
		_Value = Value;
		OnPropertyChanged(nameof(Value));
	}

	/// <inheritdoc/>
	public override string ToString() => _Value is { } V ? V.ToString() ?? string.Empty : "<NULL>";

	/// <summary>
	/// Implicit <see langword="operator"/> shorthand for the <see cref="Observable{T}.Value"/> getter.
	/// </summary>
	/// <param name="Obv"> The <see cref="Observable{T}"/> to access the getter from. </param>
	public static implicit operator T?(Observable<T> Obv) => Obv.Value;

	/// <summary>
	/// Implicit <see langword="operator"/> shorthand for the <see cref="Observable{T}"/> default constructor.
	/// </summary>
	/// <param name="Val">The value to create a new <see cref="Observable{T}"/> object with.</param>
	public static implicit operator Observable<T>( T? Val ) => new Observable<T>(Val);

	/// <inheritdoc />
	public bool Equals( Observable<T>? Other ) => Other switch {
		null => this is null,
		_ => ReferenceEquals(this, Other)
		     || EqualityComparer<T?>.Default.Equals(_Value, Other._Value)
	};

	/// <inheritdoc />
	public override bool Equals( object? Obj ) => Obj switch {
		//Ambiguous case (unsure if object is Observable<T> or T
		null => this is null || _Value is null, 
		_ => ReferenceEquals(this, Obj)
		     || Obj.GetType() == GetType() && Equals((Observable<T>)Obj)
		     || Obj.GetType() == typeof(T) && Equals((T?)Obj)
	};

	/// <inheritdoc />
	public override int GetHashCode() => _Value is null ? 0 : EqualityComparer<T?>.Default.GetHashCode(_Value);

	/// <inheritdoc />
	public bool Equals( T? Other ) => Other switch {
		null => !HasValue,
		_ => ReferenceEquals(_Value, Other)
		     || EqualityComparer<T?>.Default.Equals(_Value, Other)
	};

	/// <summary> Equivalent to <see cref="Observable{T}.Equals(object?)"/> </summary>
	/// <param name="Left">The bound object.</param>
	/// <param name="Right">The value.</param>
	/// <returns><see langword="true"/> if <see cref="Value"/> (of <paramref name="Left"/>) equals <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator ==( Observable<T>? Left, T? Right ) => Left?.Equals(Right) ?? false;

	/// <summary> Inverse to <see cref="Observable{T}.Equals(object?)"/> </summary>
	/// <param name="Left">The bound object.</param>
	/// <param name="Right">The value.</param>
	/// <returns><see langword="false"/> if <see cref="Value"/> (of <paramref name="Left"/>) equals <paramref name="Right"/>; otherwise <see langword="true"/>.</returns>
	public static bool operator !=( Observable<T>? Left, T? Right ) => !(Left ==  Right);

	/// <summary> Equivalent to <see cref="Observable{T}.Equals(object?)"/> </summary>
	/// <param name="Left">The value.</param>
	/// <param name="Right">The bound object.</param>
	/// <returns><see langword="true"/> if <see cref="Value"/> (of <paramref name="Right"/>) equals <paramref name="Left"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator ==( T? Left, Observable<T>? Right ) => Right?.Equals(Left) ?? false;


	/// <summary> Inverse to <see cref="Observable{T}.Equals(object?)"/> </summary>
	/// <param name="Left">The value.</param>
	/// <param name="Right">The bound object.</param>
	/// <returns><see langword="false"/> if <see cref="Value"/> (of <paramref name="Right"/>) equals <paramref name="Left"/>; otherwise <see langword="true"/>.</returns>
	public static bool operator !=( T? Left, Observable<T>? Right ) => !(Left == Right);


	/// <summary> Equivalent to <see cref="Observable{T}.Equals(Observable{T}?)"/> </summary>
	/// <param name="Left">The bound object.</param>
	/// <param name="Right">The other bound object.</param>
	/// <returns><see langword="true"/> if <see cref="Value"/> (of <paramref name="Left"/>) equals <see cref="Value"/> (of <paramref name="Right"/>); otherwise <see langword="false"/>.</returns>
	public static bool operator ==( Observable<T>? Left, Observable<T>? Right ) => Equals(Left, Right);

	/// <summary> Inverse to <see cref="Observable{T}.Equals(Observable{T}?)"/> </summary>
	/// <param name="Left">The bound object.</param>
	/// <param name="Right">The other bound object.</param>
	/// <returns><see langword="false"/> if <see cref="Value"/> (of <paramref name="Left"/>) equals <see cref="Value"/> (of <paramref name="Right"/>); otherwise <see langword="true"/>.</returns>
	public static bool operator !=( Observable<T>? Left, Observable<T>? Right ) => !Equals(Left, Right);
}