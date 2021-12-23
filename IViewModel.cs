#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.ComponentModel;

#endregion

namespace MVVMUtils; 

/// <summary> Represents the VM in the MVVM model. </summary>
/// <typeparam name="TView">The view type.</typeparam>
public interface IViewModel<out TView> : INotifyPropertyChanging, INotifyPropertyChanged {
	/// <summary>
	/// Gets the view.
	/// </summary>
	/// <value>
	/// The view.
	/// </value>
	public TView View { get; }
}