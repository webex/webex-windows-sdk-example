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
    public class VideoAudioSetupViewModel : ViewModelBase
    {
        private WebexSDK.Webex webex;

        private IntPtr pbHandle;
        private Action refreshViewMtd;

        public List<string> DefaultVideoBandWidths { get; set; }
        public List<string> DefaultShareBandWidths { get; set; }
        public List<string> DefaultAudioBandWidths { get; set; }

        public DeviceData DeviceData
        {
            get;
            set;
        }

        public IList<WebexSDK.LogLevel> LogLevels
        {
            get
            {
                // Will result in a list
                return Enum.GetValues(typeof(WebexSDK.LogLevel)).Cast<WebexSDK.LogLevel>().ToList<WebexSDK.LogLevel>();
            }
        }
        public WebexSDK.LogLevel LogLevel
        {
            get
            {
                return ApplicationController.Instance.CurWebexManager.CurWebex.ConsoleLogger;
            }
            set
            {
                ApplicationController.Instance.CurWebexManager.CurWebex.ConsoleLogger = value;
            }
        }

        public string VideoBandWidth
        {
            get
            {
                return ApplicationController.Instance.CurWebexManager.CurWebex.Phone.VideoMaxBandwidth.ToString();
            }
            set
            {
                ApplicationController.Instance.CurWebexManager.CurWebex.Phone.VideoMaxBandwidth = Convert.ToUInt32(value);
            }
        }
        public string NewVideoBandWidth
        {
            get
            {
                return ApplicationController.Instance.CurWebexManager.CurWebex.Phone.VideoMaxBandwidth.ToString();
            }
            set
            {
                if (value.Length > 0)
                {
                    ApplicationController.Instance.CurWebexManager.CurWebex.Phone.VideoMaxBandwidth = Convert.ToUInt32(value);
                }                
            }
        }
        public string ShareBandWidth
        {
            get
            {
                return ApplicationController.Instance.CurWebexManager.CurWebex.Phone.ShareMaxBandwidth.ToString();
            }
            set
            {
                ApplicationController.Instance.CurWebexManager.CurWebex.Phone.ShareMaxBandwidth = Convert.ToUInt32(value);
            }
        }
        public string NewShareBandWidth
        {
            get
            {
                return ApplicationController.Instance.CurWebexManager.CurWebex.Phone.ShareMaxBandwidth.ToString();
            }
            set
            {
                if (value.Length > 0)
                {
                    ApplicationController.Instance.CurWebexManager.CurWebex.Phone.ShareMaxBandwidth = Convert.ToUInt32(value);
                }
            }
        }

        public string AudioBandWidth
        {
            get
            {
                return ApplicationController.Instance.CurWebexManager.CurWebex.Phone.AudioMaxBandwidth.ToString();
            }
            set
            {
                ApplicationController.Instance.CurWebexManager.CurWebex.Phone.AudioMaxBandwidth = Convert.ToUInt32(value);
            }
        }
        public string NewAudioBandWidth
        {
            get
            {
                return ApplicationController.Instance.CurWebexManager.CurWebex.Phone.AudioMaxBandwidth.ToString();
            }
            set
            {
                if (value.Length > 0)
                {
                    ApplicationController.Instance.CurWebexManager.CurWebex.Phone.AudioMaxBandwidth = Convert.ToUInt32(value);
                }
            }
        }

        private bool ifClosePreview = false;
        public bool IfClosePreview
        {
            get
            {
                return this.ifClosePreview;
            }
            set
            {
                if (value != ifClosePreview)
                {
                    ifClosePreview = value;
                    this.SwitchPreview();
                    OnPropertyChanged("IfClosePreview");
                }
            }
        }

        private WebexSDK.AVIODevice selectedCamera;
        public WebexSDK.AVIODevice SelectedCamera
        {
            get { return this.selectedCamera; }
            set
            {
                if (this.selectedCamera != value)
                {
                    this.selectedCamera = value;
                    webex.Phone.SelectAVIODevice(value);
                    OnPropertyChanged("SelectedCamera");
                }
            }
        }

        private WebexSDK.AVIODevice selectedRinger;
        public WebexSDK.AVIODevice SelectedRinger
        {
            get { return this.selectedRinger; }
            set
            {
                if (this.selectedRinger != value)
                {
                    this.selectedRinger = value;
                    webex.Phone.SelectAVIODevice(value);
                    OnPropertyChanged("SelectedRinger");
                }
            }
        }

        private WebexSDK.AVIODevice selectedSpeaker;
        public WebexSDK.AVIODevice SelectedSpeaker
        {
            get { return this.selectedSpeaker; }
            set
            {
                if (this.selectedSpeaker != value)
                {
                    this.selectedSpeaker = value;
                    webex.Phone.SelectAVIODevice(value);
                    OnPropertyChanged("SelectedSpeaker");
                }
            }
        }

        private WebexSDK.AVIODevice selectedMircoPhone;
        public WebexSDK.AVIODevice SelectedMircoPhone
        {
            get { return this.selectedMircoPhone; }
            set
            {
                if (this.selectedMircoPhone != value)
                {
                    this.selectedMircoPhone = value;
                    webex.Phone.SelectAVIODevice(value);
                    OnPropertyChanged("SelectedMircoPhone");

                }
            }
        }

        public VideoAudioSetupViewModel()
        {
            this.webex = ApplicationController.Instance.CurWebexManager.CurWebex;
            DeviceData = new DeviceData(this.webex);

            DefaultAudioBandWidths = new List<string>();
            DefaultAudioBandWidths.Add(((uint)WebexSDK.Phone.DefaultBandwidth.MaxBandwidthAudio).ToString());

            DefaultVideoBandWidths = new List<string>();
            DefaultVideoBandWidths.Add(((uint)WebexSDK.Phone.DefaultBandwidth.MaxBandwidth90p).ToString());
            DefaultVideoBandWidths.Add(((uint)WebexSDK.Phone.DefaultBandwidth.MaxBandwidth180p).ToString());
            DefaultVideoBandWidths.Add(((uint)WebexSDK.Phone.DefaultBandwidth.MaxBandwidth360p).ToString());
            DefaultVideoBandWidths.Add(((uint)WebexSDK.Phone.DefaultBandwidth.MaxBandwidth720p).ToString());
            DefaultVideoBandWidths.Add(((uint)WebexSDK.Phone.DefaultBandwidth.MaxBandwidth1080p).ToString());

            DefaultShareBandWidths = new List<string>();
            DefaultShareBandWidths.Add(((uint)WebexSDK.Phone.DefaultBandwidth.MaxBandwidthSession).ToString());
        }

        public void OnViewClosed()
        {
            webex.Phone.StopPreview(this.pbHandle);
        }

        public void OnViewReady(IntPtr pbCameraPreviewHandle, Action refreshViewFunc)
        {
            this.pbHandle = pbCameraPreviewHandle;
            webex.Phone.StartPreview(pbCameraPreviewHandle);
            this.refreshViewMtd = refreshViewFunc;
        }

        public void ResetPreviewWindow()
        {
            this.webex.Phone.UpdatePreview(this.pbHandle);
        }

        private void SwitchPreview()
        {
            if (this.ifClosePreview)
            {
                webex.Phone.StopPreview(this.pbHandle);
                this.refreshViewMtd();
            }
            else
            {
                webex.Phone.StartPreview(this.pbHandle);
            }
        }

    }
}
