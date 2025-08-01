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
    internal class ReceiveInstance : BaseInstance
    {
        private OMTReceive? receive;
        private IntPtr lastVideo = IntPtr.Zero;
        private IntPtr lastAudio = IntPtr.Zero;
 
        public ReceiveInstance(string name, OMTFrameType frameTypes, OMTPreferredVideoFormat format, OMTReceiveFlags flags) {
            receive = new OMTReceive(name, frameTypes, format, flags);
            instance = receive;
        }

        public void SetFlags(int flags)
        {
            if (receive != null)
            {
               receive.SetFlags((OMTReceiveFlags)flags);
            }
        }

        public void SetSuggestedQuality(int quality)
        {
            if (receive != null)
            {
                receive.SetSuggestedQuality((OMTQuality)quality);
            }
        }

        public void SetTally(IntPtr pTally)
        {
            if (receive != null)
            {
                if (pTally != IntPtr.Zero)
                {
                    int preview = Marshal.ReadInt32(pTally);
                    int program = Marshal.ReadInt32(pTally, 4);
                    OMTTally t = new OMTTally(preview, program);
                    receive.SetTally(t);
                }
            }
        }

        public int Send(OMTMediaFrame frame)
        {
            if (receive != null)
            {
                return receive.Send(frame);
            }
            return 0;
        }

        public void GetSenderInformation(IntPtr pInfo)
        {
            if (receive != null)
            {
                OMTSenderInfo info = receive.GetSenderInformation();
                if (info != null)
                {
                    if (pInfo != IntPtr.Zero)
                    {
                        OMTUtils.WriteStringToPtrUTF8(info.ProductName, pInfo, UnmanagedExports.MAX_STRING_LENGTH);
                        OMTUtils.WriteStringToPtrUTF8(info.Manufacturer, pInfo + UnmanagedExports.MAX_STRING_LENGTH, UnmanagedExports.MAX_STRING_LENGTH);
                        OMTUtils.WriteStringToPtrUTF8(info.Version, pInfo + UnmanagedExports.MAX_STRING_LENGTH + UnmanagedExports.MAX_STRING_LENGTH, UnmanagedExports.MAX_STRING_LENGTH);                       
                    }
                }
            }
         }

        public IntPtr Receive(OMTFrameType frameTypes, int millisecondsTimeout)
        {
            if (receive != null)
            {
                OMTMediaFrame mediaFrame = new OMTMediaFrame();
                if (receive.Receive(frameTypes, millisecondsTimeout, ref mediaFrame))
                {
                    if (mediaFrame.Type == OMTFrameType.Video)
                    {
                        if (lastVideo != IntPtr.Zero)
                        {
                            OMTMediaFrame.FreeIntPtr(lastVideo);
                            lastVideo = IntPtr.Zero;
                        }
                        lastVideo = OMTMediaFrame.ToIntPtr(mediaFrame);
                        return lastVideo;
                    } else if (mediaFrame.Type == OMTFrameType.Audio) 
                    {
                        if (lastAudio != IntPtr.Zero)
                        {
                            OMTMediaFrame.FreeIntPtr(lastAudio);
                            lastAudio = IntPtr.Zero;
                        }
                        lastAudio = OMTMediaFrame.ToIntPtr(mediaFrame);
                        return lastAudio;
                    } else if (mediaFrame.Type == OMTFrameType.Metadata)
                    {
                        if (lastMetadata != IntPtr.Zero)
                        {
                            OMTMediaFrame.FreeIntPtr(lastMetadata);
                            lastMetadata = IntPtr.Zero;
                        }
                        lastMetadata = OMTMediaFrame.ToIntPtr(mediaFrame);
                        return lastMetadata;
                    }
                }
            }
            return IntPtr.Zero;
        }

        protected override void DisposeInternal()
        {
            if (lastVideo != IntPtr.Zero)
            {
                OMTMediaFrame.FreeIntPtr(lastVideo);
                lastVideo = IntPtr.Zero;
            }
            if (lastAudio != IntPtr.Zero) {
                OMTMediaFrame.FreeIntPtr(lastAudio);
                lastAudio = IntPtr.Zero;
            }
            receive = null;
            base.DisposeInternal();
        }
    }
}
