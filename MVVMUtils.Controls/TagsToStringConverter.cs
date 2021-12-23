using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Stores a collection of <see cref="Tag"/>s as a string.
	/// </summary>
	public class TagsToStringConverter : ValueConverter<IEnumerable<Tag>, string> {

		/// <summary>
		/// The character used for element separation purposes.
		/// </summary>
		public char SeparationCharacter { get; set; } = (char)30;

		/// <inheritdoc />
		public override bool CanReverse => true;

		/// <inheritdoc />
		public override string Forward( IEnumerable<Tag> From, object? Parameter = null, CultureInfo? Culture = null ) => string.Join(SeparationCharacter, From);

		/// <inheritdoc />
		public override IEnumerable<Tag> Reverse( string To, object? Parameter = null, CultureInfo? Culture = null ) => To.Split(SeparationCharacter).Select(Str => new Tag(Str));
	}
}