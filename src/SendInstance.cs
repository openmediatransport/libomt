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

namespace libomt
{
    internal class SendInstance : BaseInstance
    {
        private OMTSend? send;

        public SendInstance( string name, int quality)
        {
            send = new OMTSend(name, (OMTQuality)quality);
            instance = send;
        }

        public int Send(OMTMediaFrame frame)
        {
            if (send != null)
            {
                return send.Send(frame);
            }
            return 0;
        }

        public int GetAddress(IntPtr value, int maxLength)
        {
            if (send != null)
            {
                return InstanceHelper.WriteString(send.Address, value, maxLength);
            }
            return 0;
        }

        public void SetRedirect(IntPtr newAddress)
        {
            if (send != null)
            {
                if (newAddress != IntPtr.Zero)
                {
                    send.SetRedirect(OMTUtils.PtrToStringUTF8(newAddress));
                } 
            }
        }

        public IntPtr Receive(int millisecondsTimeout)
        {
            if (send != null)
            {
                OMTMediaFrame mediaFrame = new OMTMediaFrame();
                if (send.Receive(millisecondsTimeout, ref mediaFrame))
                {
                    if (mediaFrame.Type == OMTFrameType.Metadata)
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

        public int Connections { get
            {
                if (send != null)
                {
                    return send.Connections;
                }
                else {
                    return 0;
                }
            } 
        }

        public void SetSenderInformation(IntPtr pInfo)
        {
            if (send != null)
            {
                if (pInfo != IntPtr.Zero)
                {
                    OMTSenderInfo info = new OMTSenderInfo();
                    info.ProductName = OMTUtils.PtrToStringUTF8(pInfo);
                    info.Manufacturer = OMTUtils.PtrToStringUTF8(pInfo + UnmanagedExports.MAX_STRING_LENGTH);
                    info.Version = OMTUtils.PtrToStringUTF8(pInfo + UnmanagedExports.MAX_STRING_LENGTH + UnmanagedExports.MAX_STRING_LENGTH);                    
                    send.SetSenderInformation(info);
                }
            }
        }
        protected override void DisposeInternal()
        {
           send = null;
           base.DisposeInternal();
        }
    }
}
