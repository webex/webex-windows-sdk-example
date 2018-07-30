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

using WebexSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenSink
{
    public class JWTLoginViewModel : ViewModelBase
    {
        public bool IsAuthenticated { get; set; }

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return this.isBusy; }
            set
            {
                if (value != this.isBusy)
                {
                    this.isBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }

        public RelayCommand AuthenticateByJWTCMD { get; set; }

        private string jwtStr;
        public string JwtStr
        {
            get
            {
                return this.jwtStr;
            }
            set
            {
                if (value != this.jwtStr)
                {
                    this.jwtStr = value;
                    OnPropertyChanged("JwtStr");
                }
            }
        }

        public JWTLoginViewModel()
        {
            this.AuthenticateByJWTCMD = new RelayCommand(this.AuthenticateByJWT, CanAuthenticateByJWT);
        }

        void AuthenticateByJWT(object o)
        {
            this.IsBusy = true;
            JWTAuthenticator auth = ApplicationController.Instance.CurWebexManager.CurWebex.Authenticator as JWTAuthenticator;

            auth?.AuthorizeWith(this.jwtStr, result =>
            {
                this.IsBusy = false;
                if (result.IsSuccess)
                {
                    ApplicationController.Instance.AppLogOutput("authorize success!");
                    ApplicationController.Instance.ChangeState(State.Main);
                }
                else
                {
                    ApplicationController.Instance.AppLogOutput("authorize failed!");
                }
            });

        }

        bool CanAuthenticateByJWT(object ignore)
        {
            return !string.IsNullOrEmpty(this.JwtStr);
        }

    }
}
