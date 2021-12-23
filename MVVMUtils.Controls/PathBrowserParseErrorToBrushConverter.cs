using System.Windows.Media;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Provides conversions between <see cref="PathBrowser_ViewModel.ParseError"/> flags and <see cref="Brush"/>es.
	/// </summary>
	public class PathBrowserParseErrorToBrushConverter : PathBrowserParseErrorConverter<Brush> { }
}