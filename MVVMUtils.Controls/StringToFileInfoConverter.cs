using System.Globalization;
using System.IO;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Converts (nullable) <see cref="string"/> instances to (nullable) <see cref="FileInfo"/> instances.
	/// </summary>
	public class StringToFileInfoConverter : ValueConverter<string?, FileInfo?> {

		/// <inheritdoc />
		public override bool CanReverse => true;

		/// <inheritdoc />
		public override bool CanForwardWhenNull => true;

		/// <inheritdoc />
		public override bool CanReverseWhenNull => true;

		/// <inheritdoc />
		public override FileInfo? Forward( string? From, object? Parameter = null, CultureInfo? Culture = null ) => From?.GetFileInfoOrNull();

		/// <inheritdoc />
		public override string? Reverse( FileInfo? To, object? Parameter = null, CultureInfo? Culture = null ) => To?.FullName;
	}

	/// <summary> Functional inverse of <see cref="StringToFileInfoConverter"/>. </summary>
	public class FileInfoToStringConverter : ReverseValueConverter<StringToFileInfoConverter, string?, FileInfo?> { }
}