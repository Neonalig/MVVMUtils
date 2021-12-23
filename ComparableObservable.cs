#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System;

#endregion

namespace MVVMUtils; 

/// <summary> Represents a singular observed property with <see cref="IComparable{T}"/> support. </summary>
/// <typeparam name="T">The observed property type.</typeparam>
public class ComparableObservable<T> : Observable<T>, IComparable<T>, IComparable<ComparableObservable<T>>, IComparable where T : IComparable<T> {

	/// <inheritdoc />
	public int CompareTo( ComparableObservable<T>? Other ) => Other is null ? 1 : CompareTo(Other.Value);

	/// <inheritdoc />
	public int CompareTo( T? Other ) => Value is null ? Other is null ? 0 : 1 : Value.CompareTo(Other);

	/// <inheritdoc />
	public int CompareTo( object? Obj ) => Obj switch {
		null => 1,
		_ => ReferenceEquals(this, Obj)
			? 0
			: Obj switch {
				ComparableObservable<T> Obs => CompareTo(Obs),
				T Val                       => CompareTo(Val),
				_                           => throw new ArgumentException($"Object must be of type {nameof(ComparableObservable<T>)}")
			}
	};

	/// <summary> Lesser than comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is lesser than <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator <( ComparableObservable<T>? Left, ComparableObservable<T>? Right ) => Left is null ? Right is null : Left.CompareTo(Right) < 0;

	/// <summary> Greater than comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is greater than <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator >( ComparableObservable<T>? Left, ComparableObservable<T>? Right ) => Left is null ? Right is null : Left.CompareTo(Right) > 0;

	/// <summary> Lesser than or equal to comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is lesser than or equal to <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator <=( ComparableObservable<T>? Left, ComparableObservable<T>? Right ) => Left is null ? Right is null : Left.CompareTo(Right) <= 0;

	/// <summary> Greater than or equal to comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is greater than or equal to <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator >=( ComparableObservable<T>? Left, ComparableObservable<T>? Right ) => Left is null ? Right is null : Left.CompareTo(Right) >= 0;

	/// <summary> Lesser than comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is lesser than <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator <( T? Left, ComparableObservable<T>? Right ) => Left is null ? Right is null || Right.Value is null : Left.CompareTo(Right!.Value) < 0;

	/// <summary> Greater than comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is greater than <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator >( T? Left, ComparableObservable<T>? Right ) => Left is null ? Right is null || Right.Value is null : Left.CompareTo(Right!.Value) > 0;

	/// <summary> Lesser than or equal to comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is lesser than or equal to <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator <=( T? Left, ComparableObservable<T>? Right ) => Left is null ? Right is null || Right.Value is null : Left.CompareTo(Right!.Value) <= 0;

	/// <summary> Greater than or equal to comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is greater than or equal to <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator >=( T? Left, ComparableObservable<T>? Right ) => Left is null ? Right is null || Right.Value is null : Left.CompareTo(Right!.Value) >= 0;

	/// <summary> Lesser than comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is lesser than <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator <( ComparableObservable<T>? Left, T? Right ) => Left is null || Left.Value is null ? Right is null : Left.CompareTo(Right) < 0;

	/// <summary> Greater than comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is greater than <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator >( ComparableObservable<T>? Left, T? Right ) => Left is null || Left.Value is null ? Right is null : Left.CompareTo(Right) > 0;

	/// <summary> Lesser than or equal to comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is lesser than or equal to <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator <=( ComparableObservable<T>? Left, T? Right ) => Left is null || Left.Value is null ? Right is null : Left.CompareTo(Right) <= 0;

	/// <summary> Greater than or equal to comparative. </summary>
	/// <param name="Left">The left value.</param>
	/// <param name="Right">The right value.</param>
	/// <returns><see langword="true"/> if <paramref name="Left"/> is greater than or equal to <paramref name="Right"/>; otherwise <see langword="false"/>.</returns>
	public static bool operator >=( ComparableObservable<T>? Left, T? Right ) => Left is null || Left.Value is null ? Right is null : Left.CompareTo(Right) >= 0;
}