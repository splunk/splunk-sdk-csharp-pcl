using System;

namespace Splunk.Client
{
	#if __MonoCS__
	public static class Contract
	{
		public static void Requires (bool check, string name="") {
		}

		public static void Requires<T> (bool check, string name="") {

		}
	}
	#endif
}

