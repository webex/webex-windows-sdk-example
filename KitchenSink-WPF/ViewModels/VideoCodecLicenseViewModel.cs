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
    class VideoCodecLicenseViewModel : ViewModelBase
    {
        public RelayCommand ActivateCMD { get; set; }
        public RelayCommand CancelCMD { get; set; }

        public string VideoCodecLicense
        {
            get
            {
                var webex = ApplicationController.Instance.CurWebexManager?.CurWebex;
                if (webex != null)
                {
                    return webex.Phone.VideoCodecLicense;
                }
                return "";
            }
        }

        public string VideoCodecLicenseURL
        {
            get
            {
                var webex = ApplicationController.Instance.CurWebexManager?.CurWebex;
                if (webex != null)
                {
                    return webex.Phone.VideoCodecLicenseURL;
                }
                return "";
            }
        }

        public VideoCodecLicenseViewModel()
        {
            ActivateCMD = new RelayCommand(Activate);
            CancelCMD = new RelayCommand(Cancel);
        }

        private void Activate(object o)
        {
            ApplicationController.Instance.CurWebexManager.CurWebex.Phone.ActivateVideoCodecLicense(true);
            ApplicationController.Instance.ChangeViewCmd = ChangeViewCmd.None;
            ApplicationController.Instance.ChangeState(State.Call);
        }
        private void Cancel(object o)
        {
            ApplicationController.Instance.CurWebexManager.CurWebex.Phone.ActivateVideoCodecLicense(false);
            ApplicationController.Instance.ChangeViewCmd = ChangeViewCmd.None;
            ApplicationController.Instance.ChangeState(State.Call);
        }
    }
}
