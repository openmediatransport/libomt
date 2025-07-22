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
    internal class BaseInstance : OMTBase
    {
        protected OMTSendReceiveBase? instance;
        protected IntPtr lastMetadata = IntPtr.Zero;

        public bool GetTally(int millisecondsTimeout, IntPtr pTally)
        {
            bool result = false;
            if (instance != null)
            {
                OMTTally tally = new OMTTally();
                result = instance.GetTally(millisecondsTimeout, ref tally);
                if (pTally != IntPtr.Zero)
                {
                    Marshal.WriteInt32(pTally, tally.Preview);
                    Marshal.WriteInt32(pTally, 4, tally.Program);
                }
            }
            return result;
        }

        public void GetAudioStatistics(IntPtr pStatistics)
        {
            if (instance != null)
            {
                OMTStatistics stats = instance.GetAudioStatistics();
                stats.ToIntPtr(pStatistics);
            }
        }
        public void GetVideoStatistics(IntPtr pStatistics)
        {
            if (instance != null)
            {
                OMTStatistics stats = instance.GetVideoStatistics();
                stats.ToIntPtr(pStatistics);
            }
        }
        protected override void DisposeInternal()
        {
            if (lastMetadata != IntPtr.Zero)
            {
                OMTMediaFrame.FreeIntPtr(lastMetadata);
                lastMetadata = IntPtr.Zero;
            }
            if (instance != null)
            {
                instance.Dispose();
                instance = null;
            }
            base.DisposeInternal();
        }

    }
}
