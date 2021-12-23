using System.Collections;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Inverse of <see cref="CollectionToXamlStringConverter"/> (which '<inheritdoc cref="CollectionToXamlStringConverter"/>')
	/// </summary>
	public class XamlStringToCollectionConverter : ReverseValueConverter<CollectionToXamlStringConverter, IList, string> { }
}