using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MVVMUtils;

/// <summary>
/// Helper methods for <see cref="TextBlock"/> controls.
/// </summary>
/// <remarks>Implementation courtesy of <see href="https://stackoverflow.com/a/53661386/11519246">Vimes</see> and <see href="https://stackoverflow.com/users/270223/pieter-nijs">Pieter Nijs</see>.</remarks>
/// <seealso cref="TextBlockHelper.TrimRunsProperty"/>
public class TextBlockHelper {

    /// <summary>
    /// Gets the <see cref="bool"/> value of the <c>TrimRuns</c> property.
    /// </summary>
    /// <param name="TextBlock">The text block.</param>
    /// <returns>Whether the whitespace between runs should be trimmed or not.</returns>
    public static bool GetTrimRuns( TextBlock TextBlock ) => (bool)TextBlock.GetValue(TrimRunsProperty);

    /// <summary>
    /// Sets the <see cref="bool"/> value of the <c>TrimRuns</c> property.
    /// </summary>
    /// <param name="TextBlock">The text block.</param>
    /// <param name="Value">The new value to apply to the property. If <see langword="true"/>, whitespace characters will be trimmed between runs; otherwise they will be kept.</param>
    public static void SetTrimRuns( TextBlock TextBlock, bool Value ) => TextBlock.SetValue(TrimRunsProperty, Value);

    /// <summary>
    /// The <c>TrimRuns</c> property.
    /// </summary>
    /// <remarks>If set to <see langword="true"/>, whitespace characters between runs will be trimmed; otherwise they will be kept.</remarks>
    public static readonly DependencyProperty TrimRunsProperty =
        DependencyProperty.RegisterAttached("TrimRuns", typeof(bool), typeof(TextBlockHelper),
            new PropertyMetadata(false, OnTrimRunsChanged));

    /// <summary>
    /// Called when the <c>TrimRuns</c> property is changed.
    /// </summary>
    /// <param name="D">The dependency object that had the property changed.</param>
    /// <param name="E">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
    static void OnTrimRunsChanged( DependencyObject D, DependencyPropertyChangedEventArgs E ) {
        if ( D is TextBlock TB ) {
            TB.Loaded += OnTextBlockLoaded;
        }
    }

    /// <summary>
    /// Called when the <see cref="TextBlock"/> is loaded.
    /// </summary>
    /// <param name="Sender">The sender.</param>
    /// <param name="Args">The <see cref="EventArgs"/> instance containing the event data.</param>
    static void OnTextBlockLoaded( object Sender, EventArgs Args ) {
        if (Sender is TextBlock TB ) {
            TB.Loaded -= OnTextBlockLoaded;

            List<Run> Runs = TB.Inlines.OfType<Run>().ToList();
            foreach ( Run Run in Runs ) {
                Run.Text = TrimOne(Run.Text);
            }
        }
    }

    /// <summary>
    /// Trims up to one trailing whitespace character from both sides of the text.
    /// </summary>
    /// <param name="Text">The text.</param>
    /// <returns>The trimmed text with the initial and/or final whitespace character trimmed.</returns>
    static string TrimOne( string Text ) {
        if ( Text.FirstOrDefault() == ' ' ) {
            Text = Text[1..];
        }
        if ( Text.LastOrDefault() == ' ' ) {
            Text = Text[..^1];
        }

        return Text;
    }
}