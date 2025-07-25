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
using System.Text;
using libomtnet;

namespace libomt
{
    public class UnmanagedExports
    {      
        private static object lockSync = new object();
        private static DiscoveryInstance? discoveryInstance = null;
        internal const int MAX_STRING_LENGTH = 1024;

        static UnmanagedExports()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OMTLogging.Write(e.ExceptionObject.ToString(), "UnhandledException");
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_settings_set_string")]
        public static void OMTSettingsSetString(IntPtr name, IntPtr value)
        {
            try
            {
                if (name != IntPtr.Zero && value != IntPtr.Zero)
                {
                    string? szName = Marshal.PtrToStringUTF8(name);
                    string? szValue = Marshal.PtrToStringUTF8(value);
                    if (szName != null && szValue != null)
                    {
                        OMTSettings.GetInstance().SetString(szName, szValue);
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_settings_set_string");
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_settings_get_string")]
        public static int OMTSettingsGetString(IntPtr name, IntPtr value, int maxLength)
        {
            try
            {
                if (name != IntPtr.Zero)
                {
                    string? szName = Marshal.PtrToStringUTF8(name);
                    if (szName != null)
                    {
                        string szValue = OMTSettings.GetInstance().GetString(szName, null);
                        if (szValue != null)
                        {
                            byte[] b = UTF8Encoding.UTF8.GetBytes(szValue);
                            if (b != null)
                            {
                                int len = b.Length + 1;
                                if (value == IntPtr.Zero) return len;
                                if (maxLength > len)
                                {
                                    Marshal.Copy(b, 0, value, b.Length);
                                    Marshal.WriteByte(value, b.Length, 0);
                                    return len;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_settings_get_string");
            }
            return 0;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_settings_set_integer")]
        public static void OMTSettingsSetInteger(IntPtr name, int value)
        {
            try
            {
                if (name != IntPtr.Zero)
                {
                    string? szName = Marshal.PtrToStringUTF8(name);
                    if (szName != null)
                    {
                        OMTSettings.GetInstance().SetInteger(szName, value);
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_settings_set_integer");
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_settings_get_integer")]
        public static int OMTSettingsGetInteger(IntPtr name)
        {
            try
            {
                if (name != IntPtr.Zero)
                {
                    string? szName = Marshal.PtrToStringUTF8(name);
                    if (szName != null)
                    {
                        return OMTSettings.GetInstance().GetInteger(szName, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_settings_get_integer");
            }
            return 0;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_send_create")]
        public static IntPtr OMTSendCreate(IntPtr name, int profile)
        {
            try
            {
                if (name != IntPtr.Zero)
                {
                    string? szName = Marshal.PtrToStringUTF8(name);
                    if (szName != null)
                    {
                        return InstanceHelper.ToIntPtr(new SendInstance(szName, profile));
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_send_create");
            }
            return IntPtr.Zero;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_receive_create")]
        public static IntPtr OMTReceiveCreate(IntPtr name, OMTFrameType frameTypes, OMTPreferredVideoFormat format, OMTReceiveFlags receiveFlags)
        {
            try
            {
                if (name != IntPtr.Zero)
                {
                    string? szName = Marshal.PtrToStringUTF8(name);
                    if (szName != null)
                    {
                        return InstanceHelper.ToIntPtr(new ReceiveInstance(szName, frameTypes, format, receiveFlags));
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_receive_create");
            }
            return IntPtr.Zero;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_send_destroy")]
        public static void OMTSendDestroy(IntPtr instance)
        {
            try
            {
                if (instance != IntPtr.Zero)
                {
                    SendInstance? sendInstance = (SendInstance?)InstanceHelper.FromIntPtr(instance);
                    if (sendInstance != null)
                    {
                        sendInstance.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_send_destroy");
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "omt_receive_destroy")]
        public static void OMTReceiveDestroy(IntPtr instance)
        {
            try
            {
                if (instance != IntPtr.Zero)
                {
                    ReceiveInstance? receiveInstance = (ReceiveInstance?)InstanceHelper.FromIntPtr(instance);
                    if (receiveInstance != null)
                    {
                        receiveInstance.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_receive_destroy");
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_send")]
        public static int OMTSend(IntPtr instance, IntPtr frame)
        {
            try
            {
                SendInstance? sendInstance = (SendInstance?)InstanceHelper.FromIntPtr(instance);
                if (sendInstance != null)
                {
                    if (frame != IntPtr.Zero)
                    {
                        OMTMediaFrame v = OMTMediaFrame.FromIntPtr(frame);
                        return sendInstance.Send(v);
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_send");
            }
            return 0;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_send_connections")]
        public static int OMTSendConnections(IntPtr instance)
        {
            try
            {
                SendInstance? sendInstance = (SendInstance?)InstanceHelper.FromIntPtr(instance);
                if (sendInstance != null)
                {
                    return sendInstance.Connections;
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_send_connections");
            }
            return 0;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_receive_send")]
        public static int OMTReceiveSend(IntPtr instance, IntPtr frame)
        {
            try
            {
                ReceiveInstance? receiveInstance = (ReceiveInstance?)InstanceHelper.FromIntPtr(instance);
                if (receiveInstance != null)
                {
                    if (frame != IntPtr.Zero)
                    {
                        OMTMediaFrame v = OMTMediaFrame.FromIntPtr(frame);
                        return receiveInstance.Send(v);
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_receive_sendmetadata");
            }
            return 0;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_send_receive")]
        public static IntPtr OMTSendReceive(IntPtr instance, int millisecondsTimeout)
        {
            try
            {
                SendInstance? sendInstance = (SendInstance?)InstanceHelper.FromIntPtr(instance);
                if (sendInstance != null)
                {
                    return sendInstance.Receive(millisecondsTimeout);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_send_receive");
            }
            return IntPtr.Zero;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_receive")]
        public static IntPtr OMTReceive(IntPtr instance, int frameTypes, int millisecondsTimeout)
        {
            try
            {
                ReceiveInstance? receiveInstance = (ReceiveInstance?)InstanceHelper.FromIntPtr(instance);
                if (receiveInstance != null)
                {
                    return receiveInstance.Receive((OMTFrameType)frameTypes, millisecondsTimeout);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_receive");
            }
            return IntPtr.Zero;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_receive_settally")]
        public static void OMTReceiveSetTally(IntPtr instance, IntPtr tally)
        {
            try
            {
                ReceiveInstance? receiveInstance = (ReceiveInstance?)InstanceHelper.FromIntPtr(instance);
                if (receiveInstance != null)
                {
                    receiveInstance.SetTally(tally);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_receive_settally");
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_send_getvideostatistics")]
        public static void OMTSendGetVideoStatistics(IntPtr instance, IntPtr statistics)
        {
            try
            {
                SendInstance? sendInstance = (SendInstance?)InstanceHelper.FromIntPtr(instance);
                if (sendInstance != null)
                {
                    sendInstance.GetVideoStatistics(statistics);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_send_getvideostatistics");
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_send_getaudiostatistics")]
        public static void OMTSendGetAudioStatistics(IntPtr instance, IntPtr statistics)
        {
            try
            {
                SendInstance? sendInstance = (SendInstance?)InstanceHelper.FromIntPtr(instance);
                if (sendInstance != null)
                {
                    sendInstance.GetAudioStatistics(statistics);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_send_getaudiostatistics");
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "omt_receive_getvideostatistics")]
        public static void OMTReceiveGetVideoStatistics(IntPtr instance, IntPtr statistics)
        {
            try
            {
                ReceiveInstance? receiveInstance = (ReceiveInstance?)InstanceHelper.FromIntPtr(instance);
                if (receiveInstance != null)
                {
                    receiveInstance.GetVideoStatistics(statistics);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_receive_getvideostatistics");
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_receive_getaudiostatistics")]
        public static void OMTReceiveGetAudioStatistics(IntPtr instance, IntPtr statistics)
        {
            try
            {
                ReceiveInstance? receiveInstance = (ReceiveInstance?)InstanceHelper.FromIntPtr(instance);
                if (receiveInstance != null)
                {
                    receiveInstance.GetAudioStatistics(statistics);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_receive_getaudiostatistics");
            }
        }


        [UnmanagedCallersOnly(EntryPoint = "omt_send_setsenderinformation")]
        public static void OMTSendSetSenderInformation(IntPtr instance, IntPtr info)
        {
            try
            {
                SendInstance? sendInstance = (SendInstance?)InstanceHelper.FromIntPtr(instance);
                if (sendInstance != null)
                {
                    sendInstance.SetSenderInformation(info);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_send_setsenderinformation");
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_receive_getsenderinformation")]
        public static void OMTReceiveGetSenderInformation(IntPtr instance, IntPtr info)
        {
            try
            {
                ReceiveInstance? receiveInstance = (ReceiveInstance?)InstanceHelper.FromIntPtr(instance);
                if (receiveInstance != null)
                {
                    receiveInstance.GetSenderInformation(info);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_receive_getsenderinformation");
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_receive_gettally")]
        public static bool OMTReceiveGetTally(IntPtr instance, int millisecondsTimeout, IntPtr tally)
        {
            try
            {
                if (tally != IntPtr.Zero)
                {
                    ReceiveInstance? receiveInstance = (ReceiveInstance?)InstanceHelper.FromIntPtr(instance);
                    if (receiveInstance != null)
                    {
                        return receiveInstance.GetTally(millisecondsTimeout, tally);
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_receive_gettally");
            }
            return false;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_send_gettally")]
        public static bool OMTSendGetTally(IntPtr instance, int millisecondsTimeout, IntPtr tally)
        {
            try
            {
                SendInstance? sendInstance = (SendInstance?)InstanceHelper.FromIntPtr(instance);
                if (sendInstance != null)
                {
                    return sendInstance.GetTally(millisecondsTimeout, tally);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_send_gettally");
            }
            return false;
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_setloggingfilename")]        
        public static void OMTSetLoggingFilename(IntPtr filename)
        {
            if (filename != IntPtr.Zero)
            {
                string? fn = Marshal.PtrToStringUTF8(filename);
                if (fn != null)
                {
                    OMTLogging.SetFilename(fn);
                }
            }
        }

        [UnmanagedCallersOnly(EntryPoint = "omt_discovery_getaddresses")]
        private static IntPtr OMTDiscoveryGetAddresses(IntPtr addressCount)
        {
            try
            {
                DiscoveryInstance? instance = CreateDiscovery();
                if (instance != null)
                {
                    return instance.GetAddresses(addressCount);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_discovery_getaddresses");
            }
            return IntPtr.Zero;
        }

        private static DiscoveryInstance? CreateDiscovery()
        {
            try
            {
                lock (lockSync)
                {
                    if (discoveryInstance == null)
                    {
                        discoveryInstance = new DiscoveryInstance();
                    }
                    return discoveryInstance;
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "omt_creatediscovery");
            }
            return null;
        }
       
    }
}
