/*
* MIT License
*
* Copyright (c) 2025 Open Media Transport Contributors
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*
*/

using libomtnet;
using System.Runtime.InteropServices;

namespace libomt
{
    internal class DiscoveryInstance : OMTBase
    {
        private OMTDiscovery instance;
        private static IntPtr lastAddresses = IntPtr.Zero;
        private static int lastAddressesLength = 0;
        public DiscoveryInstance()
        {
            instance = OMTDiscovery.GetInstance();
        }
        public OMTDiscovery Instance { get { return instance; } }

        public IntPtr GetAddresses(IntPtr addressCount)
        {
            if (addressCount == IntPtr.Zero) return IntPtr.Zero;
            string[] addresses = instance.GetAddresses();
            if (addresses.Length > 0)
            {
                if (lastAddresses != IntPtr.Zero)
                {
                    InstanceHelper.FreeStringArray(lastAddresses, lastAddressesLength);
                    lastAddressesLength = 0;
                    lastAddresses = IntPtr.Zero;
                }
                Marshal.WriteInt32(addressCount, addresses.Length);
                lastAddressesLength = addresses.Length;
                lastAddresses = InstanceHelper.AllocStringArray(addresses);
                return lastAddresses;
            }
            return IntPtr.Zero;
        }

        protected override void DisposeInternal()
        {
            if (lastAddresses != IntPtr.Zero)
            {
                InstanceHelper.FreeStringArray(lastAddresses, lastAddressesLength);
                lastAddressesLength = 0;
                lastAddresses = IntPtr.Zero;
            }
            base.DisposeInternal();
        }
    }
}
