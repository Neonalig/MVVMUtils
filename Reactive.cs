#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using PropertyChanged;
using ReactiveUI;

#endregion

namespace MVVMUtils {

	/// <summary>
	/// Represents a <c>base object</c> supporting automatic <see cref="INotifyPropertyChanging"/> and <see cref="INotifyPropertyChanged"/> callbacks via <see href="https://github.com/reactiveui/ReactiveUI">ReactiveUI</see> and <see href="https://github.com/Fody/PropertyChanged">Fody.PropertyChanged</see>.
	/// </summary>
	public abstract class Reactive : ReactiveObject {
		/// <inheritdoc cref="IReactiveObjectExtensions.RaisePropertyChanging{TSender}(TSender, string?)"/>
		/// <param name="PropertyName">
		/// <inheritdoc cref="IReactiveObjectExtensions.RaisePropertyChanging{TSender}(TSender, string?)"/>
		/// </param>
		public void OnPropertyChanging( [CallerMemberName] string? PropertyName = null ) => this.RaisePropertyChanging(PropertyName);

		/// <inheritdoc cref="IReactiveObjectExtensions.RaisePropertyChanged{TSender}(TSender, string?)"/>
		/// <param name="PropertyName">
		/// <inheritdoc cref="IReactiveObjectExtensions.RaisePropertyChanged{TSender}(TSender, string?)"/>
		/// </param>
		public void OnPropertyChanged( [CallerMemberName] string? PropertyName = null ) => this.RaisePropertyChanged(PropertyName);
	}

	/// <summary>
	/// See the: <see href="https://github.com/Fody/PropertyChanged/wiki/NotificationInterception">PropertyChangedNotificationInterceptor.cs</see> example on GitHub.
	/// </summary>
	public static class PropertyChangedNotificationInterceptor {

		/// <summary>
		/// Whether or not to force property change calls to be run in the UI thread.
		/// </summary>
		[DoNotNotify] public static bool ForceInvokeInUI { get; set; } = true;

		/// <summary>
		/// Sometimes it is helpful to be able to intercept call to OnPropertyChanged. For example:
		/// <list type="bullet">
		///<item><description> Logging all property sets</description></item>
		///<item><description>Performing some action before or after OnPropertyChanged</description></item>
		///<item><description>Choose to not fire OnPropertyChanged under certain circumstances</description></item>
		///<item><description>Executing OnPropertyChanged on the UI thread</description></item>
		/// </list>
		/// </summary>
		/// <param name="Target">The instance of the <see langword="object"/> that OnPropertyChanged is being fired on.</param>
		/// <param name="OnPropertyChangedAction">A <see langword="delegate"/> used to fire OnPropertyChanged.</param>
		/// <param name="PropertyName">The name of the property being notified.</param>
		[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Fody.PropertyChanged interception event. Code will be injected automatically to use parameters.")]
		public static void Intercept( object Target, Action OnPropertyChangedAction, [CallerMemberName] string? PropertyName = null ) {
			switch ( ForceInvokeInUI ) {
				case true:
					Application.Current.Dispatcher.Invoke(OnPropertyChangedAction);
					break;
				default:
					OnPropertyChangedAction();
					break;
			}
		}
	}
}