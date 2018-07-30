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
using System.Threading;
using System.Threading.Tasks;

namespace KitchenSink
{
    public class WebexManager
    {
        #region Properties

        public IAuthenticator CurAuthenticator { get; private set; }
        public Webex CurWebex { get; private set; }
        public string CurCalleeAddress { get; set; }
        public bool Authenticated { get; private set; }
        private Person curUser;
        public Person CurUser
        {
            get
            {
                return this.curUser;
            }
            set
            {
                this.curUser = value;
                RecentContacts = new RecentContacts();
            }
        }
        public Call currentCall { get; set; }
        public RecentContacts RecentContacts { get; set; }



        #endregion

        #region Constructors
        public WebexManager(IAuthenticator authenticator)
        {
            this.CurAuthenticator = authenticator;
            this.CurWebex = new Webex(authenticator);
            RegistPhoneEvent();

        }

        #endregion

        #region Methods
        private void RegistPhoneEvent()
        {
            this.CurWebex.Phone.OnRequestVideoCodecActivation += Phone_onRequestVideoCodecActivation;
        }
        private void Phone_onRequestVideoCodecActivation()
        {
            ApplicationController.Instance.ChangeState(State.VideoCodecLicense);
        }

        #endregion
    }
}
