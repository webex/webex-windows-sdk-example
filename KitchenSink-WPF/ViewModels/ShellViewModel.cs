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
using System.Windows.Controls;
using WebexSDK;

namespace KitchenSink
{
    public class ShellViewModel: ViewModelBase, WebexSDK.ILogger
    {
        private string loginInfo = string.Empty;
        public string LoginInfo
        {
            get { return this.loginInfo; }
            set
            {
                if (value != loginInfo)
                {
                    this.loginInfo = value;
                    OnPropertyChanged("LoginInfo");
                }
            }
        }

        private string connInfo = string.Empty;
        public string ConnectionInfo
        {
            get { return this.connInfo; }
            set
            {
                if (value != connInfo)
                {
                    this.connInfo = value;
                    OnPropertyChanged("ConnectionInfo");
                }
            }
        }

        private string appLogOutput;
        public string AppLogOutput
        {
            get
            {
                return this.appLogOutput;
            }
            set
            {
                this.appLogOutput = value;
                OnPropertyChanged("AppLogOutput");
            }
        }

        private string sdkLogOutput;
        public string SDKLogOutput
        {
            get
            {
                return this.sdkLogOutput;
            }
            set
            {
                this.sdkLogOutput = value;
                OnPropertyChanged("SDKLogOutput");
            }
        }
        public void Log(string msg)
        {
            SDKLogOutput += msg + "\n";
        }
        public void Output(string format, params object[] args)
        {
            AppLogOutput += string.Format("{0,-19}", $"{DateTime.UtcNow}") + string.Format(format, args) + "\n";
        }

        public void ShowUserInfo(string info)
        {
            LoginInfo = info;
        }
        public void ShowConnectionInfo(string info)
        {
            ConnectionInfo = info;
        }

        public ShellViewModel()
        {
            ApplicationController.Instance.ShellViewModel = this;
        }
    }
}
