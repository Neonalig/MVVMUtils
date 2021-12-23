using System.Globalization;
using System.IO;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Converts (nullable) <see cref="string"/> instances to (nullable) <see cref="FileSystemInfo"/> instances.
	/// </summary>
	public class StringToFileSystemInfoConverter : ValueConverter<string?, FileSystemInfo?> {

		/// <inheritdoc />
		public override bool CanReverse => true;

		/// <inheritdoc />
		public override bool CanForwardWhenNull => true;

		/// <inheritdoc />
		public override bool CanReverseWhenNull => true;

		/// <inheritdoc />
		public override FileSystemInfo? Forward( string? From, object? Parameter = null, CultureInfo? Culture = null ) => From?.GetFileSystemInfoOrNull();

		/// <inheritdoc />
		public override string? Reverse( FileSystemInfo? To, object? Parameter = null, CultureInfo? Culture = null ) => To?.FullName;
	}

	/// <summary> Functional inverse of <see cref="StringToFileInfoConverter"/>. </summary>
	public class FileSystemInfoToStringConverter : ReverseValueConverter<StringToFileSystemInfoConverter, string?, FileSystemInfo?> { }
}