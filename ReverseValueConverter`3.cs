namespace MVVMUtils {
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
}