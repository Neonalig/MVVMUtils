#region Copyright (C) 2017-2022  Cody Bock
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License (Version 3.0)
// as published by the Free Software Foundation.
// 
// More information can be found here: https://www.gnu.org/licenses/gpl-3.0.en.html
#endregion

#region Using Directives

using System;
using System.Collections;
using System.Linq;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

#endregion

namespace MVVMUtils; 

/// <summary> General MVVM extensions for function shorthands. </summary>
public static class Extensions {

	/// <summary>
	/// Attempts to retrieve the element at the given index in the collection.
	/// </summary>
	/// <param name="Collection">The collection to retrieve from.</param>
	/// <param name="Index">The index to retrieve.</param>
	/// <param name="Found">The found element.</param>
	/// <returns><see langword="true"/> if successful.</returns>
	public static bool TryGetAt( this IList Collection, int Index, out object? Found ) {
		if ( Index < 0 || Index >= Collection.Count ) {
			Found = default!;
			return false;
		}

		Found = Collection[Index];
		return true;
	}

	/// <summary>
	/// Attempts to retrieve the element at the given index in the collection.
	/// </summary>
	/// <typeparam name="T">The collection containing type.</typeparam>
	/// <param name="Collection">The collection to retrieve from.</param>
	/// <param name="Index">The index to retrieve.</param>
	/// <param name="Found">The found element.</param>
	/// <returns><see langword="true"/> if successful.</returns>
	public static bool TryGetAt<T>( this IList<T> Collection, int Index, out T Found ) {
		if ( Index < 0 || Index >= Collection.Count ) {
			Found = default!;
			return false;
		}

		Found = Collection[Index];
		return true;
	}
	/// <summary>
	/// Attempts to retrieve the element at the given index in the collection.
	/// </summary>
	/// <typeparam name="T">The collection containing type.</typeparam>
	/// <param name="Collection">The collection to retrieve from.</param>
	/// <param name="Index">The index to retrieve.</param>
	/// <returns>The found element, or <see langword="null"/>.</returns>
	public static T? GetAtOrNull<T>( this IList<T> Collection, int Index ) => TryGetAt(Collection, Index, out T Found) ? Found : default;

	/// <summary>
	/// Attempts to retrieve the element at the given index in the collection.
	/// </summary>
	/// <typeparam name="T">The collection containing type.</typeparam>
	/// <param name="BaseCollection">The collection to retrieve from.</param>
	/// <param name="Index">The index to retrieve.</param>
	/// <param name="Found">The found element.</param>
	/// <returns><see langword="true"/> if successful.</returns>
	public static bool TryGetAt<T>( this IList<object> BaseCollection, int Index, out T Found ) {
		if ( Index >= 0 && Index < BaseCollection.Count && BaseCollection[Index] is T F ) {
			Found = F;
			return true;
		}

		Found = default!;
		return false;
	}

	/// <summary>
	/// A collection of all assemblies to check for <see cref="IElementTag"/>-inherited classes within.
	/// </summary>
	public static readonly ObservableCollection<Assembly> TagAssemblies;

	/// <summary>
	/// A collection of all known tag classes. Case-insensitive (strings are stored via <see cref="string.ToUpperInvariant"/>).
	/// </summary>
	static readonly Dictionary<string, IElementTag> _KnownTags;

	static Extensions() {
		_KnownTags = new Dictionary<string, IElementTag>();
		TagAssemblies = new ObservableCollection<Assembly>();
		TagAssemblies.CollectionChanged += TagAssemblies_CollectionChanged;
		if (Assembly.GetEntryAssembly() is { } Entry) {
			//Debug.WriteLine($"Appending {Entry} to TagAssemblies.");
			TagAssemblies.Add(Entry);
		}
	}

	/// <summary>
	/// Rebuilds the <see cref="_KnownTags"/> dictionary.
	/// </summary>
	/// <param name="Sender">The sender.</param>
	/// <param name="E">The collection arguments.</param>
	internal static void TagAssemblies_CollectionChanged( object? Sender, NotifyCollectionChangedEventArgs E ) {
		if (E.NewItems is not null) {
			foreach (object Item in E.NewItems) {
				Debug.WriteLine($"Assembly '{Item}' was added.");
			}
		}

		lock (_KnownTags) {
			_KnownTags.Clear();
			Type TagType = typeof(IElementTag);
			//Rebuilds the KnownTags dictionary
			foreach (Type T in TagAssemblies.SelectMany(Ass => Ass.GetTypes())) {
				if (TagType.IsAssignableFrom(T)
				    && T.GetConstructor(Type.EmptyTypes) is { } Const
				    && Const.Invoke(Array.Empty<object>()) is IElementTag Tag) {
					Debug.WriteLine($"\tConstructing {T.Name} as {Tag}");
					_KnownTags.Add(T.Name.TrimEnd("Tag", StringComparison.InvariantCultureIgnoreCase).ToUpperInvariant(), Tag);
				}
			}
		}
	}

	/// <summary>
	/// Removes all occurrences of a given string found at the end of <paramref name="S"/>.
	/// </summary>
	/// <param name="S">The string to trim.</param>
	/// <param name="End">The string to remove from the end.</param>
	/// <param name="Comparison">The comparison type.</param>
	/// <returns><c><paramref name="S"/>.Substring(0,<paramref name="S"/>.Length-𝑘*<paramref name="End"/>.Length)</c></returns>
	internal static string TrimEnd( this string S, string End, StringComparison Comparison = StringComparison.Ordinal ) {
		while (S.EndsWith(End, Comparison)) {
			S = S[..^End.Length];
		}

		return S;
	}

	/// <summary>
	/// Initialises all tags on the given <see cref="FrameworkElement"/>, running relevant <see cref="IElementTag.Loaded(FrameworkElement, FrameworkElement)"/> functions.
	/// </summary>
	/// <param name="CC">The parent <see cref="FrameworkElement"/> (i.e. a <see cref="Window"/> or <see cref="UserControl"/>)</param>
	/// <param name="SplitIfRaw">When the tag is a pure <see cref="string"/>, determines whether or not to treat the string as a ';'-delimited array, or just as a singular, verbatim <see cref="string"/>.</param>
	public static void InitialiseTags( this FrameworkElement CC, bool SplitIfRaw = true ) {
		void InitTags(object Sender, RoutedEventArgs E ) {
			Debug.WriteLine($"Initialising tags within '{CC.Name}' ({CC.GetType().Name})");
			foreach (FrameworkElement Child in CC.FindVisualChildren<FrameworkElement>(true)) {
				//Debug.WriteLine($"Checking '{Child.Name}' ({Child.GetType().Name}) -- {Child.Tag} ({Child.Tag?.GetType().Name}) for tags...");
				//CC.property
				foreach (string Tag in Child.GetTags(SplitIfRaw)) {
					//Debug.WriteLine($"Checking for KnownTag with name {Tag}");
					if (_KnownTags.TryGetValue(Tag.ToUpperInvariant(), out IElementTag? FoundTag)
					    && FoundTag is not null) {
						Debug.WriteLine($"\tInitialising {Tag} ({FoundTag}) on {Child} (within {CC})");
						FoundTag.Loaded(Child, CC);
					} else {
						Debug.WriteLine($"\tNo IElementTag found for tag '{Tag}'", "WARNING");
					}
				}
			}
		}
		CC.Loaded += InitTags;
	}

	/// <summary>
	/// Attempts to retrieve all strings on the given <see cref="FrameworkElement"/>, regardless of the collection type.
	/// </summary>
	/// <remarks>Supports:
	/// <list type="bullet">
	/// <item><description><see cref="string"/></description></item>
	/// <item><description><see cref="IEnumerable{T}"/> (where <c>T</c> is type <see cref="string"/>)</description></item>
	/// <item><description><see cref="IEnumerable"/> (where <see cref="object"/> is defined as described below)</description></item>
	/// <item><description><see cref="object"/> (with valid (non-<see langword="null"/> and non-empty) <see cref="object.ToString"/> function)</description></item>
	/// </list></remarks>
	/// <param name="Element">The element to retrieve tags from.</param>
	/// <param name="SplitIfRaw">When the tag is a pure <see cref="string"/>, determines whether or not to treat the string as a ';'-delimited array, or just as a singular, verbatim <see cref="string"/>.</param>
	/// <returns>A collection of all <see cref="string"/> tags on the given element.</returns>
	public static IEnumerable<string> GetTags( this FrameworkElement Element, bool SplitIfRaw = true ) {
		switch (Element.Tag) {
			case string S when !string.IsNullOrEmpty(S):
				if (SplitIfRaw) {
					foreach (string Sp in S.Split(';')) {
						yield return Sp;
					}
				} else {
					yield return S;
				}
				break;
			case IEnumerable<string> EnumS:
				foreach (string S in EnumS) {
					yield return S;
				}
				break;
			case IEnumerable Enum:
				foreach (object Obj in Enum) {
					if (Obj.CanBecomeString(out string ObjTS)) {
						yield return ObjTS;
					}
					break;
				}
				break;
			case { } ET:
				switch (ET) {
					case { } NET when NET.CanBecomeString(out string NETS):
						if (SplitIfRaw) {
							foreach (string Sp in NETS.Split(';')) {
								yield return Sp;
							}
						} else {
							yield return NETS;
						}
						break;
					// ReSharper disable once RedundantEmptySwitchSection
					default:
						break;
				}
				break;
			// ReSharper disable once RedundantEmptySwitchSection
			default:
				break;
		}
	}

	/// <summary>
	/// Sets the <paramref name="Element"/> tags to the given <see cref="IEnumerable"/> collection.
	/// </summary>
	/// <param name="Element">The <paramref name="Element"/> to apply the <paramref name="Tags"/> to.</param>
	/// <param name="Tags">The <paramref name="Tags"/> to apply to the given <paramref name="Element"/>.</param>
	public static void SetTags( this FrameworkElement Element, IEnumerable<string> Tags ) => Element.Tag = Tags;

	/// <summary>
	/// Appends the given tag to the <paramref name="Element"/>.
	/// </summary>
	/// <param name="Element">The <paramref name="Element"/> to apply the <paramref name="Tag"/> to.</param>
	/// <param name="Tag">The tag to append upon the <paramref name="Element"/>.</param>
	/// <param name="SplitPreExistingIfRaw">When the existing <paramref name="Element"/> tag is a pure <see cref="string"/>, determines whether or not to treat the string as a ';'-delimited array, or just as a singular, verbatim <see cref="string"/>.</param>
	public static void AddTag( this FrameworkElement Element, string Tag, bool SplitPreExistingIfRaw = true ) => Element.Tag = GetTags(Element, SplitPreExistingIfRaw).With(Tag);

	/// <summary>
	/// Removes the given tag from the <paramref name="Element"/>. If the searched tag is present multiple times, all duplicates will be removed.
	/// </summary>
	/// <param name="Element">The <paramref name="Element"/> to remove the <paramref name="Tag"/> from.</param>
	/// <param name="Tag">The tag to remove from the given <paramref name="Element"/>.</param>
	/// <param name="SplitPreExistingIfRaw">When the existing <paramref name="Element"/> tag is a pure <see cref="string"/>, determines whether or not to treat the string as a ';'-delimited array, or just as a singular, verbatim <see cref="string"/>.</param>
	/// <param name="Comparison">The string comparison type.</param>
	/// <returns><see langword="true"/> if any tags were removed.</returns>
	public static bool RemoveTag( this FrameworkElement Element, string Tag, bool SplitPreExistingIfRaw = true, StringComparison Comparison = StringComparison.Ordinal ) {
		bool StringsAreEqual( string A, string B ) => A.Equals(B, Comparison);
		Element.Tag = GetTags(Element, SplitPreExistingIfRaw).Without(Tag, StringsAreEqual, out int Matches);
		return Matches > 0;
	}

	/// <summary>
	/// Returns a concatenated <see cref="IEnumerable"/> of the given collection and the singular item.
	/// </summary>
	/// <typeparam name="T">The containing type.</typeparam>
	/// <param name="Enum">The main collection.</param>
	/// <param name="AdditionalItem">The additional item.</param>
	/// <returns>The concatenation of <paramref name="Enum"/> and <paramref name="AdditionalItem"/>.</returns>
	public static IEnumerable<T> With<T>( this IEnumerable<T> Enum, T AdditionalItem ) {
		foreach (T A in Enum) {
			yield return A;
		}
		yield return AdditionalItem;
	}

	/// <summary>
	/// Returns a concatenated <see cref="IEnumerable"/> of the singular item and the given collection.
	/// </summary>
	/// <typeparam name="T">The containing type.</typeparam>
	/// <param name="FirstItem">The first item to return.</param>
	/// <param name="AdditionalEnum">The additional collection to iteratively return from.</param>
	/// <returns>The concatenation of <paramref name="FirstItem"/> and <paramref name="AdditionalEnum"/>.</returns>
	public static IEnumerable<T> With<T>( this T FirstItem, IEnumerable<T>? AdditionalEnum ) {
		yield return FirstItem;
		if (AdditionalEnum is not null) {
			foreach (T A in AdditionalEnum) { yield return A; }
		}
	}

	/// <summary>
	/// Returns the given collection without the specified item if present.
	/// </summary>
	/// <typeparam name="T">The containing type.</typeparam>
	/// <param name="Enum">The collection.</param>
	/// <param name="Exclude">The item to exclude.</param>
	/// <param name="Comparer">The equality-comparing function to check items in the collection against.</param>
	/// <param name="TotalExcluded">The number of item matches that were excluded.</param>
	/// <returns>A collection without the given item.</returns>
	internal static IEnumerable<T> Without<T>( this IEnumerable<T> Enum, T Exclude, Func<T, T, bool> Comparer, out int TotalExcluded ) {
		bool Check( T Item ) => !Comparer.Invoke(Item, Exclude);
		IEnumerable<T> Result = Enum.WhereAndEnumerate(Check, out int Matches, out int Total);
		TotalExcluded = Total - Matches;
		return Result;
	}

	/// <summary>
	/// Enumerates the given <paramref name="Enum"/> then returns the <paramref name="Count"/> and continue.
	/// </summary>
	/// <typeparam name="T">The enumerable containing type.</typeparam>
	/// <param name="Enum">The enumerable to enumerate.</param>
	/// <param name="Count">The length of the given <paramref name="Enum"/>.</param>
	/// <returns><paramref name="Enum"/> enumerated as an array.</returns>
	internal static IEnumerable<T> RetrieveCount<T>( this IEnumerable<T> Enum, out int Count ) {
		T[] Arr = Enum.ToArray();
		Count = Arr.Length;
		return Arr;
	}

	/// <summary>
	/// Finds all <paramref name="Predicate"/> matches in the given <paramref name="Enum"/>, returning the number of <paramref name="Matches"/> and count of <paramref name="Total"/> items analysed.
	/// </summary>
	/// <typeparam name="T">The enumerable containing type.</typeparam>
	/// <param name="Enum">The enumerable to check for matches.</param>
	/// <param name="Predicate">The match-checking function.</param>
	/// <param name="Matches">The number of found matches.</param>
	/// <param name="Total">The total number of items analysed.</param>
	/// <returns>An <see cref="IEnumerable"/> of all <paramref name="Predicate"/> matches.</returns>
	internal static IEnumerable<T> WhereAndEnumerate<T>(this IEnumerable<T> Enum, Func<T, bool> Predicate, out int Matches, out int Total ) {
		T[] Original = Enum.ToArray();
		Total = Original.Length;

		Matches = 0;
		List<T> FoundMatches = new List<T>();
		foreach (T Item in Original.Where(Predicate)) {
			Matches++;
			FoundMatches.Add(Item);
		}
		return FoundMatches;
	}

	/// <summary>
	/// Returns <see langword="true"/> if the given <see langword="object"/> can safely become a non-<see langword="null"/> and non-empty string.
	/// </summary>
	/// <param name="O">The <see langword="object"/> to check.</param>
	/// <param name="Result">The resultant string. Only use if function returns <see langword="true"/>.</param>
	/// <returns><see langword="true"/> if the <see langword="object"/> is either: a string, or has a valid <see cref="object.ToString"/> function.</returns>
	internal static bool CanBecomeString( this object? O, out string Result ) {
		Result = string.Empty;
		switch (O) {
			case string S when !string.IsNullOrEmpty(S):
				Result = S;
				return true;
			case { } Obj when Obj.ToString().HasValue(out string ObjTS) && !string.IsNullOrEmpty(ObjTS):
				Result = ObjTS;
				return true;
			//case null:
			default:
				return false;
		}
	}

	/// <summary>
	/// Attempts to get the first child of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The child type to search for.</typeparam>
	/// <param name="Element">The parent.</param>
	/// <param name="Child">The found child.</param>
	/// <param name="Recurse">Whether or not to retrieve children of children, etc.</param>
	/// <returns><see langword="true"/> if a child was found; otherwise <see langword="false"/>.</returns>
	public static bool TryGetChild<T>( this DependencyObject Element, out T Child, bool Recurse = false ) where T : DependencyObject {
		T? PossibleChild = FindVisualChildren<T>(Element, Recurse).FirstOrDefault();
		if ( PossibleChild is null ) {
			Child = default!;
			return false;
		}
		Child = PossibleChild;
		return true;
	}

	/// <summary>
	/// Retrieves all <see cref="DependencyObject"/> children in the given parent.
	/// </summary>
	/// <param name="Element">The parent.</param>
	/// <param name="Recurse">Whether or not to retrieve children of children, etc.</param>
	/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DependencyObject"/> children.</returns>
	public static IEnumerable<DependencyObject> FindVisualChildren( this DependencyObject Element, bool Recurse = true ) => FindVisualChildren<DependencyObject>(Element, Recurse);

	/// <summary>
	/// Retrieves all <see cref="DependencyObject"/> children in the given parent.
	/// </summary>
	/// <param name="Element">The parent.</param>
	/// <param name="Recurse">Whether or not to retrieve children of children, etc.</param>
	/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="DependencyObject"/> children.</returns>
	public static IEnumerable<T> FindVisualChildren<T>( this DependencyObject Element, bool Recurse = true ) where T : DependencyObject {
		if (TryGetChildrenCount(Element, out int C)) {
			for (int I = 0; I < C; I++) {
				DependencyObject Child = VisualTreeHelper.GetChild(Element, I);
				switch (Child) {
					case T CDO:
						yield return CDO;
						break;
				}

				if (Recurse) {
					foreach (T ChildOfChild in FindVisualChildren<T>(Child)) {
						yield return ChildOfChild;
					}
				}
			}
		}
	}

	/// <summary>
	/// Attempts to find all logical children of type <typeparamref name="T"/> on the <paramref name="Parent"/>.
	/// </summary>
	/// <typeparam name="T">The child type to search for.</typeparam>
	/// <param name="Parent">The parent to search within.</param>
	/// <returns>All found children of type <typeparamref name="T"/>.</returns>
	public static IEnumerable<T> FindLogicalChildren<T>( this DependencyObject Parent ) where T : DependencyObject {
		Queue<DependencyObject> Queue = new Queue<DependencyObject>(new[] {Parent});

		while ( Queue.Any() ) {
			DependencyObject Reference = Queue.Dequeue();
			int Count = VisualTreeHelper.GetChildrenCount(Reference);

			for ( int I = 0; I < Count; I++ ) {
				DependencyObject Child = VisualTreeHelper.GetChild(Reference, I);
				if ( Child is T Children ) {
					yield return Children;
				}

				Queue.Enqueue(Child);
			}
		}
	}

	/// <summary>
	/// Safely retrieves the children count on the given <see cref="DependencyObject"/>.
	/// </summary>
	/// <param name="DO">The reference <see langword="object"/>.</param>
	/// <param name="C">The number of found children.</param>
	/// <returns>true if successfully retrieved child count.</returns>
	public static bool TryGetChildrenCount( this DependencyObject? DO, out int C ) {
		C = -1;
		if (DO is null) { return false; }
		try {
			C = VisualTreeHelper.GetChildrenCount(DO);
			return true;
		} catch (InvalidOperationException) { return false; }
	}

	/// <inheritdoc cref="IntGetParent(DependencyObject)"/>
	public static DependencyObject? GetParent( this DependencyObject Child ) => IntGetParent(Child);

	/// <summary>
	/// Recursively gets all ancestors of the <see cref="DependencyObject"/>.
	/// </summary>
	/// <param name="Child">The child to search.</param>
	/// <returns>The ancestors of the given <paramref name="Child"/> up until the root.</returns>
	public static IEnumerable<DependencyObject> GetParents( this DependencyObject Child ) {
		DependencyObject? P = IntGetParent(Child);
		while ( P is not null ) {
			yield return P;
			P = IntGetParent(P);
		}
	}

	/// <summary>
	/// Attempts to get the parent of the <paramref name="Child"/>.
	/// </summary>
	/// <param name="Child">The child.</param>
	/// <returns>The parent object, or <see langword="null"/>.</returns>
	internal static DependencyObject? IntGetParent( this DependencyObject Child ) {
		try {
			return Child switch {
				FrameworkContentElement FCE => FCE.Parent,
				Visual V                    => VisualTreeHelper.GetParent(V),
				Visual3D VThree             => VisualTreeHelper.GetParent(VThree),
				_                           => null
			};
		} catch {
			return null;
		}
	}

	/// <summary>
	/// Recursively gets all ancestors of the <see cref="DependencyObject"/> of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of parents to return.</typeparam>
	/// <param name="Child">The child to search.</param>
	/// <returns>The ancestors of the given <paramref name="Child"/> of type <typeparamref name="T"/> (up until the root).</returns>
	public static IEnumerable<T> GetParents<T>( this DependencyObject Child ) where T : DependencyObject {
		foreach (DependencyObject Parent in GetParents(Child) ) {
			if ( Parent is T P ) {
				yield return P;
			}
		}
	}

	/// <summary>
	/// Attempts to get the first ancestor of type <typeparamref name="T"/> of the <see cref="DependencyObject"/>.
	/// </summary>
	/// <typeparam name="T">The type of parent to find.</typeparam>
	/// <param name="Child">The child to search.</param>
	/// <param name="Parent">The first found ancestor of type <typeparamref name="T"/>.</param>
	/// <returns><see langword="true"/> if a <paramref name="Parent"/> was found; otherwise <see langword="false"/>.</returns>
	public static bool TryGetParent<T>(this DependencyObject Child, [NotNullWhen(true)] out T Parent) where T : DependencyObject {
		foreach ( DependencyObject P in GetParents(Child) ) {
			if ( P is T Pa ) {
				Parent = Pa;
				return true;
			}
		}
		Parent = null!;
		return false;
	}

	/// <summary>
	/// Retrieves all <see cref="MarkupProperty"/> values in the given <paramref name="ObjElement"/>.
	/// </summary>
	/// <param name="ObjElement">The element to retrieve properties in.</param>
	/// <param name="OnlyAttached">Whether or not the property must be attached to be returned.</param>
	/// <returns>An <see cref="IEnumerable"/> of type <see cref="MarkupProperty"/></returns>
	internal static IEnumerable<MarkupProperty> GetMarkupProperties( this object ObjElement, bool OnlyAttached = false) {
		if (MarkupWriter.GetMarkupObjectFor(ObjElement) is { } MO) {
			// ReSharper disable once LoopCanBePartlyConvertedToQuery
			foreach (MarkupProperty MP in MO.Properties) {
				if (!OnlyAttached || MP.IsAttached) {
					yield return MP;
				}
			}
		}
	}

	/// <inheritdoc cref="GetMarkupProperties(object, bool)"/>
	/// <param name="DOObject">The element to retrieve properties in.</param>
	/// <param name="OnlyAttached"><inheritdoc cref="GetDependencyProperties(object, bool)"/></param>
	public static IEnumerable<MarkupProperty> GetMarkupProperties( this DependencyObject DOObject, bool OnlyAttached = false ) => GetMarkupProperties(ObjElement: DOObject, OnlyAttached);

	/// <inheritdoc cref="GetMarkupProperties(object, bool)"/>
	/// <param name="DPObject">The element to retrieve properties in.</param>
	/// <param name="OnlyAttached"><inheritdoc cref="GetDependencyProperties(object, bool)"/></param>
	public static IEnumerable<MarkupProperty> GetMarkupProperties( this DependencyProperty DPObject, bool OnlyAttached = false ) => GetMarkupProperties(ObjElement: DPObject, OnlyAttached);

	/// <summary>
	/// Retrieves all <see cref="DependencyProperty"/> values in the given <paramref name="ObjElement"/>.
	/// </summary>
	/// <param name="ObjElement">The element to retrieve properties in.</param>
	/// <param name="OnlyAttached">Whether or not the property must be attached to be returned.</param>
	/// <returns>An <see cref="IEnumerable"/> of type <see cref="DependencyProperty"/></returns>
	internal static IEnumerable<DependencyProperty> GetDependencyProperties(this object ObjElement, bool OnlyAttached = false) {
		foreach (MarkupProperty MP in GetMarkupProperties(ObjElement, OnlyAttached)) {
			if (MP.DependencyProperty is { } DP) {
				//Debug.WriteLine($"Found {MP.Name}: {DP}");
				yield return DP;
			}
		}
	}

	/// <inheritdoc cref="GetDependencyProperties(object, bool)"/>
	/// <param name="DOObject">The element to retrieve properties in.</param>
	/// <param name="OnlyAttached"><inheritdoc cref="GetDependencyProperties(object, bool)"/></param>
	public static IEnumerable<DependencyProperty> GetDependencyProperties( this DependencyObject DOObject, bool OnlyAttached = false ) => GetDependencyProperties(ObjElement: DOObject, OnlyAttached);

	/// <inheritdoc cref="GetDependencyProperties(object, bool)"/>
	/// <param name="DPObject">The element to retrieve properties in.</param>
	/// <param name="OnlyAttached"><inheritdoc cref="GetDependencyProperties(object, bool)"/></param>
	public static IEnumerable<DependencyProperty> GetDependencyProperties( this DependencyProperty DPObject, bool OnlyAttached = false ) => GetDependencyProperties(ObjElement: DPObject, OnlyAttached);

	/// <summary>
	/// Gets the dependency property.
	/// </summary>
	/// <typeparam name="T">The property value type.</typeparam>
	/// <param name="DOObject">The dependency object.</param>
	/// <param name="DPName">The name of the dependency property.</param>
	/// <param name="OnlyAttached"><inheritdoc cref="GetDependencyProperties(object, bool)"/></param>
	/// <returns>The found dependency property, or <see langword="null"/>.</returns>
	public static TypedDependencyProperty<T>? GetDependencyProperty<T>( this DependencyObject DOObject, string DPName, bool OnlyAttached = false ) {
		// ReSharper disable once LoopCanBePartlyConvertedToQuery
		foreach ( DependencyProperty Property in DOObject.GetDependencyProperties(OnlyAttached) ) {
			if ( Property.Name == DPName ) {
				if ( Property.PropertyType.IsAssignableFrom(typeof(T)) ) {
					return new TypedDependencyProperty<T>(Property);
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Gets the dependency property.
	/// </summary>
	/// <typeparam name="T">The property value type.</typeparam>
	/// <param name="DOObject">The dependency object.</param>
	/// <param name="PropertyReference">A reference to the dependency property.</param>
	/// <param name="OnlyAttached"><inheritdoc cref="GetDependencyProperties(object, bool)"/></param>
	/// <param name="PropertyName">The name of the dependency property.</param>
	/// <returns>The found dependency property, or <see langword="null"/>.</returns>
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used by attributes")]
	public static TypedDependencyProperty<T>? GetDependencyProperty<T>( this DependencyObject DOObject, T PropertyReference, bool OnlyAttached = false, [CallerArgumentExpression("PropertyReference")] string PropertyName = "" ) => GetDependencyProperty<T>(DOObject, LastField(PropertyName), OnlyAttached);

	/// <summary>
	/// Attempts to find the children element of type <typeparamref name="T"/> with the given <paramref name="ChildName"/>.
	/// </summary>
	/// <typeparam name="T">The child type to search for.</typeparam>
	/// <param name="Element">The parent.</param>
	/// <param name="ChildName">The name of the child.</param>
	/// <param name="Recurse">Whether to iteratively retrieve children in subsequent <see cref="FrameworkElement"/>s or just return the raw element and continue.</param>
	/// <param name="Comparison">The string comparison type.</param>
	/// <returns>The found child or <see langword="null"/>.</returns>
	public static T? FindElementByName<T>( this FrameworkElement Element, string ChildName, bool Recurse = true, StringComparison Comparison = StringComparison.Ordinal ) where T : FrameworkElement => FindVisualChildren<T>(Element).FirstOrDefault(Child => Child.Name.Equals(ChildName, Comparison));

	/// <summary>
	/// Returns <see langword="true"/> if the given <paramref name="Value"/> is not <see langword="null"/>, passing out the non-<see langword="null"/> value if so.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <param name="Value">The (possibly <see langword="null"/>) value.</param>
	/// <param name="NotNull">The not <see langword="null"/> value. Only use if function returns <see langword="true"/>.</param>
	/// <returns>True if <paramref name="Value"/> is not <see langword="null"/>; otherwise <see langword="false"/>.</returns>
	public static bool HasValue<T>( this T? Value, out T NotNull ) {
		if (Value is null) {
			NotNull = default!;
			return false;
		}
		NotNull = Value;
		return true;
	}

	/// <summary>
	/// Gets all text after the last period (.) symbol.
	/// </summary>
	/// <param name="Expression">The expression.</param>
	/// <returns>The substring.</returns>
	[return: NotNullIfNotNull(nameof(Expression))]
	internal static string? LastField( string? Expression ) => Expression?[(Expression.LastIndexOf('.') + 1)..];

	/// <inheritdoc cref="TypedDependencyProperty{T}.Set(DependencyObject, T)"/>
	/// <typeparam name="T">The property value type.</typeparam>
	/// <param name="Object">The dependency object.</param>
	/// <param name="PropertyReference">A reference to the dependency property.</param>
	/// <param name="Value">The new value.</param>
	/// <param name="PropertyName">The name of the dependency property.</param>
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used by attributes")]
	public static void SetValue<T>( this DependencyObject Object, T PropertyReference, T Value, [CallerArgumentExpression("PropertyReference")] string PropertyName = "") => SetValue(Object, Object.GetDependencyProperty<T>(LastField(PropertyName))!, Value);


	/// <inheritdoc cref="TypedDependencyProperty{T}.Get(DependencyObject)"/>
	/// <typeparam name="T">The property value type.</typeparam>
	/// <param name="Object">The dependency object.</param>
	/// <param name="PropertyReference">A reference to the dependency property.</param>
	/// <param name="PropertyName">The name of the dependency property.</param>
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used by attributes")]
	public static T GetValue<T>( this DependencyObject Object, T PropertyReference, [CallerArgumentExpression("PropertyReference")] string PropertyName = "" ) => GetValue<T>(Object, Object.GetDependencyProperty<T>(LastField(PropertyName))!);

	/// <inheritdoc cref="TypedDependencyProperty{T}.Set(DependencyObject, T)"/>
	/// <typeparam name="T">The property value type.</typeparam>
	/// <param name="Object">The dependency object.</param>
	/// <param name="Property">The dependency property.</param>
	/// <param name="Value">The new value.</param>
	public static void SetValue<T>( this DependencyObject Object, DependencyProperty Property, T Value ) => SetValue(Object, new TypedDependencyProperty<T>(Property), Value);

	/// <inheritdoc cref="TypedDependencyProperty{T}.Get(DependencyObject)"/>
	/// <typeparam name="T">The property value type.</typeparam>
	/// <param name="Object">The dependency object.</param>
	/// <param name="Property">The dependency property.</param>
	public static T GetValue<T>( this DependencyObject Object, DependencyProperty Property ) => GetValue(Object, new TypedDependencyProperty<T>(Property));

	/// <inheritdoc cref="TypedDependencyProperty{T}.Set(DependencyObject, T)"/>
	/// <typeparam name="T">The property value type.</typeparam>
	/// <param name="Object">The dependency object.</param>
	/// <param name="Property">The dependency property.</param>
	/// <param name="Value">The new value.</param>
	public static void SetValue<T>( this DependencyObject Object, TypedDependencyProperty<T> Property, T Value ) => Property.Set(Object, Value);

	/// <inheritdoc cref="TypedDependencyProperty{T}.Get(DependencyObject)"/>
	/// <typeparam name="T">The property value type.</typeparam>
	/// <param name="Object">The dependency object.</param>
	/// <param name="Property">The dependency property.</param>
	public static T GetValue<T>( this DependencyObject Object, TypedDependencyProperty<T> Property ) => Property.Get(Object);

	/// <summary>
	/// Gets the developer-friendly short-name/alias of the type.
	/// </summary>
	/// <param name="Type">The type.</param>
	/// <returns>The alias/name of the type.</returns>
	public static string GetTypeName( this Type Type ) => Type.GetTypeCode(Type) switch {
		TypeCode.Empty    => "<null>",
		TypeCode.Object   => Type.Name,
		TypeCode.DBNull   => nameof(DBNull),
		TypeCode.Boolean  => "bool",
		TypeCode.Char     => "char",
		TypeCode.SByte    => "sbyte",
		TypeCode.Byte     => "byte",
		TypeCode.Int16    => "short",
		TypeCode.UInt16   => "ushort",
		TypeCode.Int32    => "int",
		TypeCode.UInt32   => "uint",
		TypeCode.Int64    => "long",
		TypeCode.UInt64   => "ulong",
		TypeCode.Single   => "float",
		TypeCode.Double   => "double",
		TypeCode.Decimal  => "decimal",
		TypeCode.DateTime => nameof(DateTime),
		TypeCode.String   => "string",
		_                 => throw new NotImplementedException()
	};
}

/// <summary>
/// An implementation of <see cref="DependencyProperty"/> of which the value type is known at compile-time.
/// </summary>
/// <typeparam name="T">The property value type.</typeparam>
public readonly struct TypedDependencyProperty<T> {
	/// <summary>
	/// The dependency property.
	/// </summary>
	public readonly DependencyProperty Property;

	/// <summary>
	/// Initialises a new instance of the <see cref="TypedDependencyProperty{T}"/> struct.
	/// </summary>
	/// <param name="Property">The property.</param>
	/// <exception cref="System.InvalidOperationException">The property must inherit from type '<typeparamref name="T"/>'.</exception>
	public TypedDependencyProperty( DependencyProperty Property ) {
		if ( Property.PropertyType.IsAssignableFrom(typeof(T)) ) {
			this.Property = Property;
		} else {
			throw new InvalidOperationException($"The property must inherit from type '{typeof(T)}'.");
		}
	}

	/// <summary>
	/// Gets the value of the property.
	/// </summary>
	/// <param name="Object">The dependency object.</param>
	/// <returns>The property value in the given dependency object.</returns>
	public T Get(DependencyObject Object) => (T)Object.GetValue(Property);

	/// <summary>
	/// Sets the value of the property on the given dependency object.
	/// </summary>
	/// <param name="Object">The dependency object.</param>
	/// <param name="Value">The new value.</param>
	public void Set( DependencyObject Object, T Value ) => Object.SetValue(Property, Value);

	/// <summary>
	/// Performs an <see langword="implicit"/> conversion from <see cref="TypedDependencyProperty{T}"/> to <see cref="DependencyProperty"/>.
	/// </summary>
	/// <param name="Typed">The typed property.</param>
	/// <returns>
	/// The result of the conversion.
	/// </returns>
	public static implicit operator DependencyProperty(TypedDependencyProperty<T> Typed) => Typed.Property;

	/// <summary>
	/// Performs an explicit conversion from <see cref="DependencyProperty"/> to <see cref="TypedDependencyProperty{T}"/>.
	/// </summary>
	/// <param name="Property">The property.</param>
	/// <returns>
	/// The result of the conversion.
	/// </returns>
	public static explicit operator TypedDependencyProperty<T>( DependencyProperty Property ) => new TypedDependencyProperty<T>(Property);

	/// <inheritdoc />
	public override string ToString() => $"[{Property.PropertyType.GetTypeName()} {Property.OwnerType.GetTypeName()}.{Property.Name}]";
}