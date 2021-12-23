#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Windows;

#endregion

namespace MVVMUtils; 

/// <summary>
/// Represents a known tag for <see cref="FrameworkElement"/>s.
/// </summary>
public interface IElementTag {
	/// <summary>
	/// Invoked on all <see cref="FrameworkElement"/>s when the containing window loads.
	/// </summary>
	/// <param name="Sender">The element which loaded.</param>
	/// <param name="Container">The container which loaded the element.</param>
	void Loaded( FrameworkElement Sender, FrameworkElement Container );
}