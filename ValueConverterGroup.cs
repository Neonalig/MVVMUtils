#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System;
using System.Linq;

using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

#endregion

namespace MVVMUtils;

/// <summary>
/// Provides a method for performing multiple conversions on a type of object.
/// </summary>
/// <remarks>
/// <b>Usage example:</b>
/// <code lang="XAML">
/// &lt;ValueConverterGroup x:Key="BoolInvToVisConv"&gt;
///     &lt;InverseBooleanConverter&gt;
///     &lt;BooleanToVisibilityConverter&gt;
/// &lt;/ValueConverterGroup&gt;
/// </code>
/// </remarks>
[ValueConversion(typeof(object), typeof(object))]
public class ValueConverterGroup : List<IValueConverter>, IValueConverter {

    #region IValueConverter Members

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <param name="TargetType">The type of the target.</param>
    /// <param name="Parameter">The parameter.</param>
    /// <param name="Culture">The culture.</param>
    /// <returns>The forwards conversion result.</returns>
    public object Convert( object Value, Type TargetType, object Parameter, CultureInfo Culture ) => this.Aggregate(Value, ( Current, Converter ) => Converter.Convert(Current, TargetType, Parameter, Culture)!);

    /// <summary>
    /// Converts the specified value.
    /// </summary>
    /// <param name="Value">The value.</param>
    /// <param name="TargetType">The type of the target.</param>
    /// <param name="Parameter">The parameter.</param>
    /// <param name="Culture">The culture.</param>
    /// <returns>The reverse conversion result.</returns>
    /// <exception cref="System.NotImplementedException">This method is not yet implemented.</exception>
    public object ConvertBack( object Value, Type TargetType, object Parameter, CultureInfo Culture ) => throw new NotImplementedException();

    #endregion
}