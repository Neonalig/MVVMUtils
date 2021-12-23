using System.Collections.Generic;

namespace MVVMUtils.Controls {
	/// <summary>
	/// Inverse of <see cref="TagsToStringConverter"/> (which '<inheritdoc cref="TagsToStringConverter"/>')
	/// </summary>
	public class StringToTagsConverter : ReverseValueConverter<TagsToStringConverter, IEnumerable<Tag>, string> { }
}