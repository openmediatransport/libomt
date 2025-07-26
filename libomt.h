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

#define OMT_MAX_STRING_LENGTH 1024

typedef long long int64_t;

typedef enum OMTFrameType
{
    OMTFrameType_None = 0,
    OMTFrameType_Metadata = 1,
    OMTFrameType_Video = 2,
    OMTFrameType_Audio = 4,
    OMTFrameType_INT32 = 0x7fffffff //Ensure int type in C
} OMTFrameType;
typedef enum OMTCodec
{
    OMTCodec_VMX1 = 0x31584D56,
    OMTCodec_FPA1 = 0x31415046, //Planar audio
    OMTCodec_UYVY = 0x59565955,
    OMTCodec_YUY2 = 0x32595559,
    OMTCodec_BGRA = 0x41524742,
    OMTCodec_NV12 = 0x3231564E,
    OMTCodec_YV12 = 0x32315659,
    OMTCodec_UYVA = 0x41565955,

} OMTCodec;
typedef enum OMTQuality
{
    OMTQuality_Default = 0,
    OMTQuality_Low = 1,
    OMTQuality_Medium = 50,
    OMTQuality_High = 100,
    OMTQuality_INT32 = 0x7fffffff //Ensure int type in C
} OMTQuality;
typedef enum OMTColorSpace
{
    OMTColorSpace_Undefined = 0,
    OMTColorSpace_BT601 = 601,
    OMTColorSpace_BT709 = 709,
    OMTColorSpace_INT32 = 0x7fffffff //Ensure int type in C
} OMTColorSpace;
typedef enum OMTVideoFlags
{
    OMTVideoFlags_None = 0,
    OMTVideoFlags_Interlaced = 1,
    OMTVideoFlags_Alpha = 2,
    OMTVideoFlags_Preview = 4,
    OMTVideoFlags_INT32 = 0x7fffffff //Ensure int type in C
} OMTVideoFlags;

typedef enum OMTPreferredVideoFormat
{
    OMTPreferredVideoFormat_UYVY = 0,
    OMTPreferredVideoFormat_UYVYorBGRA = 1,
    OMTPreferredVideoFormat_BGRA = 2,
    OMTPreferredVideoFormat_UYVYorUYVA = 3,
    OMTPreferredVideoFormat_INT32 = 0x7fffffff //Ensure int type in C
} OMTPreferredVideoFormat;
typedef enum OMTReceiveFlags
{
    OMTReceiveFlags_None = 0,
    OMTReceiveFlags_Preview = 1,
    OMTReceiveFlags_IncludeCompressed = 2,
    OMTReceiveFlags_CompressedOnly = 4,
    OMTReceiveFlags_INT32 = 0x7fffffff //Ensure int type in C
} OMTReceiveFlags;

typedef struct OMTTally
{
    int preview;
    int program;
} OMTTally;

typedef struct OMTSenderInfo
{
    char ProductName[OMT_MAX_STRING_LENGTH];
    char Manufacturer[OMT_MAX_STRING_LENGTH];
    char Version[OMT_MAX_STRING_LENGTH];
    char Reserved1[OMT_MAX_STRING_LENGTH];
    char Reserved2[OMT_MAX_STRING_LENGTH];
    char Reserved3[OMT_MAX_STRING_LENGTH];
} OMTSenderInfo;

typedef struct OMTStatistics
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
} OMTStatistics;

typedef struct OMTMediaFrame
{
    int Type; //OMTFrameType
    int64_t Timestamp;
    int Codec; //OMTCodec

    //Video Properties
    int Width;
    int Height;
    int Stride;
    int Flags; //OMTVideoFlags
    int FrameRateN;
    int FrameRateD;
    float AspectRatio;
    int ColorSpace; //OMTColorSpace

    //Audio Properties
    int SampleRate;
    int Channels;
    int SamplesPerChannel;

    //Data Properties
    void* Data;
    int DataLength;
    void* CompressedData;
    int CompressedLength;

    //Frame MetaData Properties
    void* FrameMetadata;
    int FrameMetadataLength;

} OMTMediaFrame;

typedef long long omt_receive_t;
typedef long long omt_send_t;

#ifdef __cplusplus
extern "C" {
#endif

    /**
    * Retrieve the current value of a string setting.
    * Returns the length in bytes of the UTF-8 encoded value including null terminator.
    * maxLength specifies the maximum amount of bytes allocated to value by the caller.
    */
    int omt_settings_get_string(const char* name, char* value, int maxLength);

    /**
    * Set a string setting that will be used for the duration of this process.
    * Value should be a null terminated UTF-8 encoded string.
    */
    void omt_settings_set_string(const char* name, const char* value);


    int omt_settings_get_integer(const char* name);
    void omt_settings_set_integer(const char* name, int value);


    omt_receive_t* omt_receive_create(const char* name, OMTFrameType frameTypes, OMTPreferredVideoFormat format, OMTReceiveFlags flags);
    void omt_receive_destroy(omt_receive_t* instance);
    OMTMediaFrame* omt_receive(omt_receive_t* instance, OMTFrameType frameTypes, int timeoutMilliseconds);
    int omt_receive_send(omt_receive_t* instance, OMTMediaFrame* frame);
    void omt_receive_settally(omt_receive_t* instance, OMTTally* tally);
    void omt_receive_getsenderinformation(omt_receive_t* instance, OMTSenderInfo* info);
    void omt_receive_getvideostatistics(omt_receive_t* instance, OMTStatistics* stats);
    void omt_receive_getaudiostatistics(omt_receive_t* instance, OMTStatistics* stats);

    omt_send_t* omt_send_create(const char* name, OMTQuality quality);
    void omt_send_setsenderinformation(omt_send_t* instance, OMTSenderInfo* info);
    void omt_send_destroy(omt_send_t* instance);
    int omt_send(omt_send_t* instance, OMTMediaFrame* frame);
    int omt_send_connections(omt_send_t* instance);
    OMTMediaFrame* omt_send_receive(omt_send_t* instance, int timeoutMilliseconds);

    /**
    * Receives the current tally state across all connections to a Sender.
    * If this function times out, the last known tally state will be received.
    * Returns 0 if timed out or tally didn't change. 1 otherwise.
    */
    int omt_send_gettally(omt_send_t* instance, int timeoutMilliseconds, OMTTally* tally);
    void omt_send_getvideostatistics(omt_send_t* instance, OMTStatistics* stats);
    void omt_send_getaudiostatistics(omt_send_t* instance, OMTStatistics* stats);

    void omt_setloggingfilename(const char* filename);

#ifdef __cplusplus
}
#endif