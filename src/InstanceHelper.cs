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

using System.Runtime.InteropServices;

namespace libomt
{
    internal class InstanceHelper
    {
        public static Object? FromIntPtr(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                GCHandle gh = GCHandle.FromIntPtr(handle);
                return gh.Target;
            }
            return null;
        }
        public static IntPtr ToIntPtr(Object? obj)
        {
            if (obj == null) return IntPtr.Zero;
            return GCHandle.ToIntPtr(GCHandle.Alloc(obj));
        }

        public static IntPtr AllocStringArray(string[] values)
        {
            IntPtr m = Marshal.AllocCoTaskMem(values.Length * IntPtr.Size);
            for (int i = 0; i < values.Length; i++)
            {
                Marshal.WriteIntPtr(m, i * IntPtr.Size, Marshal.StringToCoTaskMemUTF8(values[i]));
            }
            return m;
        }
        public static void FreeStringArray(IntPtr ptr, int length)
        {
            if (ptr != IntPtr.Zero)
            {
                for (int i = 0; i < length; i++)
                {
                    Marshal.ZeroFreeCoTaskMemUTF8(Marshal.ReadIntPtr(ptr, i * IntPtr.Size));
                }
                Marshal.FreeCoTaskMem(ptr);
            }
        }
    }
}
