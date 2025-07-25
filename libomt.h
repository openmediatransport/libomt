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

#pragma once
#pragma comment(lib, "libomt.lib")

const int OMT_MAX_STRING_LENGTH = 1024;

enum OMTFrameType : int
{
    OMTFrameType_None = 0,
    OMTFrameType_Metadata = 1,
    OMTFrameType_Video = 2,
    OMTFrameType_Audio = 4
};
enum OMTCodec : int
{
    OMTCodec_VMX1 = 0x31584D56,
    OMTCodec_FPA1 = 0x31415046,
    OMTCodec_UYVY = 0x59565955,
    OMTCodec_BGRA = 0x41524742
};
enum OMTQuality : int
{
    OMTQuality_Default = 0,
    OMTQuality_Low = 1,
    OMTQuality_Medium = 50,
    OMTQuality_High = 100
};
enum OMTColorSpace : int
{
    OMTColorSpace_Undefined = 0,
    OMTColorSpace_BT601 = 601,
    OMTColorSpace_BT709 = 709
};
enum OMTVideoFlags : int
{
    OMTVideoFlags_None = 0,
    OMTVideoFlags_Interlaced = 1,
    OMTVideoFlags_Alpha = 2,
    OMTVideoFlags_Preview = 4
};

enum OMTPreferredVideoFormat : int
{
    OMTPreferredVideoFormat_UYVY = 0,
    OMTPreferredVideoFormat_UYVYorBGRA = 1,
    OMTPreferredVideoFormat_BGRA = 2
};
enum OMTReceiveFlags : int
{
    OMTReceiveFlags_None = 0,
    OMTReceiveFlags_Preview = 1,
    OMTReceiveFlags_IncludeCompressed = 2
};

struct OMTTally
{
    int preview;
    int program;
};

struct OMTSenderInfo
{
    char ProductName[OMT_MAX_STRING_LENGTH];
    char Manufacturer[OMT_MAX_STRING_LENGTH];
    char Version[OMT_MAX_STRING_LENGTH];
    char Reserved1[OMT_MAX_STRING_LENGTH];
    char Reserved2[OMT_MAX_STRING_LENGTH];
    char Reserved3[OMT_MAX_STRING_LENGTH];
};

struct OMTStatistics
{
    int64_t BytesSent;
    int64_t BytesReceived;
    int64_t BytesSentSinceLast;
    int64_t BytesReceivedSinceLast;

    int64_t Frames;
    int64_t FramesSinceLast;
    int64_t FramesDropped;

    int64_t CodecTime;
    int64_t CodecTimeSinceLast;

    int64_t Reserved1;
    int64_t Reserved2;
    int64_t Reserved3;
    int64_t Reserved4;
    int64_t Reserved5;
    int64_t Reserved6;
    int64_t Reserved7;
};

struct OMTMediaFrame
{
    enum OMTFrameType Type;
    int64_t Timestamp;
    int Codec;

    //Video Properties
    int Width;
    int Height;
    int Stride;
    enum OMTVideoFlags Flags;
    int FrameRateN;
    int FrameRateD;
    float AspectRatio;
    enum OMTColorSpace ColorSpace;

    //Audio Properties
    int SampleRate;
    int Channels;
    int SamplesPerChannel;

    //Data Properties
    void* Data;
    int DataLength;
    void* CompressedData;
    int CompressedLength;
};

typedef long long omt_receive_t;
typedef long long omt_send_t;


/**
* Retrieve the current value of a string setting.
* Returns the length in bytes of the UTF-8 encoded value including null terminator.
* maxLength specifies the maximum amount of bytes allocated to value by the caller.
*/
extern "C" int omt_settings_get_string(const char* name, char* value, int maxLength);

/**
* Set a string setting that will be used for the duration of this process.
* Value should be a null terminated UTF-8 encoded string.
*/
extern "C" void omt_settings_set_string(const char* name, const char* value);


extern "C" int omt_settings_get_integer(const char* name);
extern "C" void omt_settings_set_integer(const char* name, int value);


extern "C" omt_receive_t* omt_receive_create(const char* name, OMTFrameType frameTypes, OMTPreferredVideoFormat format, OMTReceiveFlags flags);
extern "C" void omt_receive_destroy(omt_receive_t* instance);
extern "C" OMTMediaFrame* omt_receive(omt_receive_t* instance, OMTFrameType frameTypes, int timeoutMilliseconds);
extern "C" int omt_receive_send(omt_receive_t* instance, OMTMediaFrame* frame);
extern "C" void omt_receive_settally(omt_receive_t* instance, OMTTally* tally);
extern "C" void omt_receive_getsenderinformation(omt_receive_t* instance, OMTSenderInfo* info);
extern "C" void omt_receive_getvideostatistics(omt_receive_t* instance, OMTStatistics* stats);
extern "C" void omt_receive_getaudiostatistics(omt_receive_t* instance, OMTStatistics* stats);

extern "C" omt_send_t* omt_send_create(const char* name, OMTQuality quality);
extern "C" void omt_send_setsenderinformation(omt_send_t* instance, OMTSenderInfo* info);
extern "C" void omt_send_destroy(omt_send_t* instance);
extern "C" int omt_send(omt_send_t* instance, OMTMediaFrame* frame);
extern "C" int omt_send_connections(omt_send_t* instance);
extern "C" OMTMediaFrame* omt_send_receive(omt_send_t* instance, int timeoutMilliseconds);
extern "C" bool omt_send_gettally(omt_send_t* instance, int timeoutMilliseconds, OMTTally* tally);
extern "C" void omt_send_getvideostatistics(omt_send_t* instance, OMTStatistics* stats);
extern "C" void omt_send_getaudiostatistics(omt_send_t* instance, OMTStatistics* stats);

extern "C" void omt_setloggingfilename(const char* filename);

