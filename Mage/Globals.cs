
namespace Mage {
	
	/// <summary>
	/// Global, static class for informing threads that they should abort processing
	/// </summary>
	public static class Globals {

		/// <summary>
		/// Will be set to true by one of the threads if the user requests that an operation be aborted
		/// </summary>
		public static bool AbortRequested = false;

		static Globals() {
			AbortRequested = false;
		}
	}
}
