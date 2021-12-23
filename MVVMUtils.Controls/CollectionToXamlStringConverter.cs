using System.Collections;
using System.Globalization;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Stores a collection as a string.
	/// </summary>
	public class CollectionToXamlStringConverter : ValueConverter<IList, string> {

		/// <summary>
		/// The character used for element separation purposes.
		/// </summary>
		public char SeparationCharacter { get; set; } = (char)30;

		/// <inheritdoc />
		public override bool CanReverse => true;

		/// <inheritdoc />
		public override string? Forward( IList From, object? Parameter = null, CultureInfo? Culture = null ) => string.Join(SeparationCharacter, From);

		/// <inheritdoc />
		public override IList? Reverse( string To, object? Parameter = null, CultureInfo? Culture = null ) => To.Split(SeparationCharacter);
	}
}