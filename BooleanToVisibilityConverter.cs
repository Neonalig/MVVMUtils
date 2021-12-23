#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Globalization;
using System.Windows;

#endregion

namespace MVVMUtils {
	/// <summary>
	/// Converts <see langword="bool"/> values to <see cref="Visibility"/>.
	/// </summary>
	public class BooleanToVisibilityConverter : ValueConverter<bool, Visibility> {
		/// <summary>
		/// The <see cref="Visibility"/> to return when the given boolean is <see langword="true"/>.
		/// </summary>
		public Visibility True { get; set; } = Visibility.Visible;

		/// <summary>
		/// The <see cref="Visibility"/> to return when the given boolean is <see langword="false"/>.
		/// </summary>
		public Visibility False { get; set; } = Visibility.Collapsed;

		/// <inheritdoc />
		public override bool CanReverse => true;

		/// <inheritdoc />
		public override Visibility Forward( bool From, object? Parameter = null, CultureInfo? Culture = null ) => From ? True : False;

		/// <inheritdoc />
		public override bool Reverse( Visibility To, object? Parameter = null, CultureInfo? Culture = null ) => To == True;
	}
}
