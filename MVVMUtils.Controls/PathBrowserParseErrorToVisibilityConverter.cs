using System.Windows;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Provides conversions between <see cref="PathBrowser_ViewModel.ParseError"/> flags and a particular <see cref="Visibility"/>.
	/// </summary>
	public class PathBrowserParseErrorToVisibilityConverter : PathBrowserParseErrorConverter<Visibility> { }
}