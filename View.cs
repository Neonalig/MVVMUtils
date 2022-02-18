#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

namespace MVVMUtils; 
///// <summary>
///// Represents a view which uses a <see cref="ViewModel{TView}"/>.
///// </summary>
///// <typeparam name="T">The viewmodel type.</typeparam>
//public interface IView<out T> : IView where T : ViewModel<IView> {
//    /// <summary>
//    /// The viewmodel instance.
//    /// </summary>
//    T VM { get; }
//}

/// <summary>
/// Represents a view which uses a <see cref="ViewModel{TView}"/>.
/// </summary>
/// <typeparam name="T">The viewmodel type.</typeparam>
public interface IView<out T> {
    /// <summary>
    /// The viewmodel instance.
    /// </summary>
    T VM { get; }
}


/// <summary>
/// Represents a view which uses a <see cref="ViewModel{TView}"/>.
/// </summary>
/// <typeparam name="TViewModel">The viewmodel type.</typeparam>
/// <typeparam name="TModel">The model type.</typeparam>
public interface IView<out TViewModel, TModel> where TViewModel : ViewModel<IView<TViewModel, TModel>, TModel> {
    /// <summary>
    /// The viewmodel instance.
    /// </summary>
    TViewModel VM { get; }
}