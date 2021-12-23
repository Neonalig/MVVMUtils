using System.Globalization;
using System.IO;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Converts (nullable) <see cref="string"/> instances to (nullable) <see cref="DirectoryInfo"/> instances.
	/// </summary>
	public class StringToDirectoryInfoConverter : ValueConverter<string?, DirectoryInfo?> {

		/// <inheritdoc />
		public override bool CanReverse => true;

		/// <inheritdoc />
		public override bool CanForwardWhenNull => true;

		/// <inheritdoc />
		public override bool CanReverseWhenNull => true;

		/// <inheritdoc />
		public override DirectoryInfo? Forward( string? From, object? Parameter = null, CultureInfo? Culture = null ) => From?.GetDirectoryInfoOrNull();

		/// <inheritdoc />
		public override string? Reverse( DirectoryInfo? To, object? Parameter = null, CultureInfo? Culture = null ) => To?.FullName;
	}

	/// <summary> Functional inverse of <see cref="StringToDirectoryInfoConverter"/>. </summary>
	public class DirectoryInfoToStringConverter : ReverseValueConverter<StringToDirectoryInfoConverter, string?, DirectoryInfo?> { }
}