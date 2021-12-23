using System.Globalization;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Converts an <see langword="object"/> to a boolean representing its nullability.
	/// </summary>
	public class NullabilityToBooleanConverter : ValueConverter<object?, bool> {

		/// <summary>
		/// The value to return when not <see langword="null"/>.
		/// </summary>
		public bool WhenNotNull { get; set; } = true;

		/// <summary>
		/// The value to return when <see langword="null"/>.
		/// </summary>
		public bool WhenNull { get; set; }

		/// <inheritdoc />
		public override bool CanForwardWhenNull => true;

		/// <inheritdoc />
		public override bool CanReverse => false;

		/// <inheritdoc />
		public override bool CanReverseWhenNull => false;

		/// <inheritdoc />
		public override bool Forward( object? From, object? Parameter = null, CultureInfo? Culture = null ) => WhenNotNull;

		/// <inheritdoc />
		public override bool ForwardWhenNull( object? Parameter = null, CultureInfo? Culture = null ) => WhenNull;

		/// <inheritdoc />
		public override object? Reverse( bool To, object? Parameter = null, CultureInfo? Culture = null ) => null;
	}
}