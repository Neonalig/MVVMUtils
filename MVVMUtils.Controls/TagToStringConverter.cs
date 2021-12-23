using System.Globalization;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Converts <see cref="Tag"/> instances to <see cref="string"/> instances.
	/// </summary>
	public class TagToStringConverter : ValueConverter<Tag, string> {

		/// <inheritdoc />
		public override bool CanReverse => true;

		/// <inheritdoc />
		public override string Forward( Tag From, object? Parameter = null, CultureInfo? Culture = null ) => From.Value.Replace(' ', ' ');

		/// <inheritdoc />
		public override Tag Reverse( string To, object? Parameter = null, CultureInfo? Culture = null ) => new Tag(To, Parameter as TagBox_ViewModel);
	}
}