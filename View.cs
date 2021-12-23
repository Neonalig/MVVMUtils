namespace MVVMUtils {
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
}
