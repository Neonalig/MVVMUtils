using System.ComponentModel;
using System.Diagnostics;

namespace MVVMUtils {
	/// <summary>
	/// The test class for testing purposes.
	/// </summary>
	public class TestReactive : Reactive {
		/// <summary>
		/// The test string for testing purposes.
		/// </summary>
		public string TestString { get; set; }

		/// <summary>
		/// Default Constructor.
		/// </summary>
		/// <param name="TestString">The test string.</param>
		public TestReactive( string TestString = "egg" ) {
			this.TestString = TestString;
			PropertyChanged += TestReactive_PropertyChanged;
		}

		/// <summary>
		/// OnPropertyChanged callback.
		/// </summary>
		/// <param name="Sender">The sender.</param>
		/// <param name="E">The arguments.</param>
		static void TestReactive_PropertyChanged( object? Sender, PropertyChangedEventArgs E ) => Debug.WriteLine($"{Sender}.PropertyChanged :: {E.PropertyName} was changed.");

		/// <summary>
		/// Test function for testing purposes.
		/// </summary>
		/// <param name="NewTestString">The new <see cref="TestString"/> value to assign.</param>
		public void Test( string NewTestString ) {
			Debug.WriteLine($"Changing {nameof(TestString)} from {TestString} to {NewTestString}... Watch out for property changed calls here:");
			TestString = NewTestString;
			Debug.WriteLine($"Changed {nameof(TestString)}. Was there a property changed call?");
		}
	}
}
