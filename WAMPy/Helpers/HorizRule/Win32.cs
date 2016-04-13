//
// Win32.cs
//

using System;
using System.Runtime.InteropServices;

namespace Win32
{
	/// <summary>
	/// Subset of Wesner Moise's Win32 wrapper library that contains just those
	/// definitions needed for the project that contains this file.  The
	/// original library could be found at
	/// http://www.codeproject.com/csharp/win32.asp the last time I looked.
	/// </summary>
	/// <remarks>
	/// After my 10/13/2003 reworking of the the HorizRule and VertRule controls,
	/// I removed unneeded Interop definitions from this class.  There is really
	/// nothing left here that could be considered Moise's original work, but I
	/// left his name on the class because that was the location from which I
	/// cut the WS_CHILD and WS_VISIBLE definitions you see here.
	/// </remarks>
	public class User
	{
		/// <remarks/>
		public const int SS_ETCHEDHORZ = 0x00000010;
		/// <remarks/>
		public const int SS_ETCHEDVERT = 0x00000011;

		/// <remarks/>
		public const int WS_CHILD = 0x40000000;
		/// <remarks/>
		public const int WS_VISIBLE = 0x10000000;
	}
}