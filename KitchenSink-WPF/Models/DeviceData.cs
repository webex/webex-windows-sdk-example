#region License
// Copyright (c) 2016-2018 Cisco Systems, Inc.

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenSink
{
    public class DeviceData
    {

        public List<WebexSDK.AVIODevice> MicroPhoneList { get; set; }
        public List<WebexSDK.AVIODevice> SpeakerList { get; set; }
        public List<WebexSDK.AVIODevice> RingerList { get; set; }
        public List<WebexSDK.AVIODevice> CameraList { get; set; }
        public DeviceData(WebexSDK.Webex webex)
        {
            MicroPhoneList = webex.Phone.GetAVIODevices(WebexSDK.AVIODeviceType.Microphone);
            SpeakerList = webex.Phone.GetAVIODevices(WebexSDK.AVIODeviceType.Speaker);
            RingerList = webex.Phone.GetAVIODevices(WebexSDK.AVIODeviceType.Ringer);
            CameraList = webex.Phone.GetAVIODevices(WebexSDK.AVIODeviceType.Camera);

            convertoUTF8(MicroPhoneList);
            convertoUTF8(SpeakerList);
            convertoUTF8(RingerList);
            convertoUTF8(CameraList);

            systemDefault(MicroPhoneList);
            systemDefault(SpeakerList);
            systemDefault(RingerList);
            systemDefault(CameraList);
        }

        private void convertoUTF8(List<WebexSDK.AVIODevice> devices)
        {
            foreach (var i in devices)
            {
                i.Name = Encoding.UTF8.GetString(Encoding.Default.GetBytes(i.Name));
            }
        }

        private void systemDefault(List<WebexSDK.AVIODevice> devices)
        {
            foreach (var i in devices)
            {
                if (i.DefaultDevice == true)
                {
                    i.Name = "system default";
                }
            }
        }

    }
}
