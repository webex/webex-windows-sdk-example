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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenSink
{
    public class OAuthLoginViewModel : ViewModelBase
    {
        public string RedirectionUrl
        {
            get
            {
                return "KitchenSink://response/";
            }
        }

        public string AuthorizationUrl
        {
            get
            {
                OAuthAuthenticator auth = ApplicationController.Instance.CurWebexManager.CurAuthenticator as OAuthAuthenticator;
                if (auth != null)
                {
                    output($"AuthorizationUrl: {auth.AuthorizationUrl}");
                    return auth.AuthorizationUrl;
                }
                output($"Get AuthorizationUrl failed. OAuth authenticator is null.");
                return "";
            }
        }

        public RelayCommand BackToLoginViewCommand { get; set; }

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
        private bool gettingAuthCode = true;
        public bool GettingAuthCode
        {
            get { return this.gettingAuthCode; }
            set
            {
                if (value != this.gettingAuthCode)
                {
                    this.gettingAuthCode = value;
                    OnPropertyChanged("GettingAuthCode");
                }
            }
        }

        public OAuthLoginViewModel()
        {
            BackToLoginViewCommand = new RelayCommand(BackToLoginView);
        }
      
        public void AuthorizeByOAuthAccessToken(string authCode)
        {
            GettingAuthCode = false;
            IsBusy = true;

            output($"begin to authorize with auth code.");
            OAuthAuthenticator auth = ApplicationController.Instance.CurWebexManager.CurAuthenticator as OAuthAuthenticator;
            auth?.Authorize(authCode, result =>
            {
                IsBusy = false;
                if (result.IsSuccess)
                {
                    output("authorize success!");
                    ApplicationController.Instance.ChangeState(State.Main);
                }
                else
                {
                    output("authorize failed!");
                }
            });
        }

        internal void TryToAnalyzeUrl(string uri)
        {
            if (uri.StartsWith(this.RedirectionUrl.ToLower()))
            {
                int start = uri.IndexOf("code=");
                string authCode = uri.Substring(start + "code=".Length, 64);
                output($"Get the auth code: {authCode}");
                AuthorizeByOAuthAccessToken(authCode);
            }
        }

        internal void TryToAnalyzeTitle(string title)
        {
            if (title != null && title.Length == 64)
            {
                AuthorizeByOAuthAccessToken(title);
            }
        }

        void BackToLoginView(object o)
        {
            ApplicationController.Instance.ChangeState(State.PreLogin);
        }

        private void output(String format, params object[] args)
        {
            ApplicationController.Instance.AppLogOutput(format, args);
        }
    }
}
