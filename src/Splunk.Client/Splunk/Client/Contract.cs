using System;

namespace Splunk.Client
{
    #if __MonoCS__
    //This doesn't seem like it should not be necessary according to Mono 2.8 notes: http://www.mono-project.com/Release_Notes_Mono_2.8
    //as it states if the CONTRACTS_FULL compiler directive is not set, the calls to Contract should be removed by the compiler.
    //However when testing in Xamarin Studio, this does not appear to be the case.
    public static class Contract
    {
        public static void Requires (bool check, string name="") {
        }

        public static void Requires<T> (bool check, string name="") {
        }
    }
    #endif
}

