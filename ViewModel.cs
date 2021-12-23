#region Copyright (C) 2017-2021  Starflash Studios
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System.Runtime.CompilerServices;

using Microsoft.Toolkit.Mvvm.ComponentModel;

#endregion

namespace MVVMUtils; 

/// <summary> Represents the VM in the MVVM model. </summary>
/// <typeparam name="TView">The view type.</typeparam>
/// <typeparam name="TModel">The model type.</typeparam>
public class ViewModel<TView, TModel> : ObservableObject, IViewModel<TView> {

	/// <summary> A reference to the view in the MVVM model. </summary>
	TView _View;

	/// <inheritdoc cref="_View"/>
	public TView View {
		get => _View;
		set => SetProperty(ref _View, value);
	}

	/// <summary> A reference to the model in the MVVM model. </summary>
	TModel? _Model;

	/// <inheritdoc cref="_Model"/>
	public TModel? Model {
		get => _Model;
		set => SetProperty(ref _Model, value);
	}

	/// <summary> Default constructor for the ViewModel instance. </summary>
	/// <param name="View">The view.</param>
	/// <param name="Model">The model.</param>
	public ViewModel( TView View, TModel? Model = default ) {
		OnPropertyChanging(nameof(View));
		_View = View;
		OnPropertyChanged(nameof(View));
		OnPropertyChanging(nameof(Model));
		_Model = Model;
		OnPropertyChanged(nameof(Model));
	}

	/// <summary> Empty constructor for XAML usage only. </summary>
	public ViewModel() {
		_View = default!;
		_Model = default;
	}

}

/// <summary> Represents the VM in the MVVM model. </summary>
/// <typeparam name="TView">The view type.</typeparam>
public class ViewModel<TView> : ObservableObject, IViewModel<TView> {

	/// <inheritdoc cref="ObservableObject.OnPropertyChanging(string?)"/>
	public void RaisePropertyChanging( [CallerMemberName] string? PropertyName = null ) => OnPropertyChanging(PropertyName);

	/// <inheritdoc cref="ObservableObject.OnPropertyChanged(string?)"/>
	public void RaisePropertyChanged( [CallerMemberName] string? PropertyName = null ) => OnPropertyChanged(PropertyName);

	/// <summary> A reference to the view in the MVVM model. </summary>
	TView _View;

	/// <inheritdoc cref="_View"/>
	public TView View {
		get => _View;
		set => SetProperty(ref _View, value);
	}

	/// <summary> Default constructor for the ViewModel instance. </summary>
	/// <param name="View">The view.</param>
	public ViewModel( TView View ) {
		OnPropertyChanging(nameof(View));
		_View = View;
		OnPropertyChanged(nameof(View));
	}

	/// <summary> Empty constructor for XAML usage only. </summary>
	public ViewModel() => _View = default!;

	/// <summary> Initialises setup for the current <see cref="ViewModel{TView}"/>. </summary>
	/// <param name="View">The view to set.</param>
	public void Setup( TView View ) {
		this.View = View;
		OnSetup();
	}

	/// <summary> Invoked when the view is setup and initialised/initialising. </summary>
	public virtual void OnSetup() { }

}