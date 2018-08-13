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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitchenSink
{
    public class MainViewModel : ViewModelBase
    {

        public RelayCommand LogoutCMD { get; set; }
        public RelayCommand VedioAudioSetupCMD { get; set; }
        public RelayCommand InitialCallCMD { get; set; }
        public RelayCommand CallCMD { get; set; }
        public RelayCommand SendFeedBackCMD { get; set; }
        public RelayCommand WaitingCallCMD { get; set; }
        public RelayCommand ManageSpaceCMD { get; set; }
        public RelayCommand MessageCMD { get; set; }




        public MainViewModel()
        {
            LogoutCMD = new RelayCommand(Logout, IfNotInCall);
            VedioAudioSetupCMD = new RelayCommand(VedioAudioSetup, IfNotInCall);
            InitialCallCMD = new RelayCommand(InitialCall, IfNotInCall);
            SendFeedBackCMD = new RelayCommand(SendFeedBack, IfNotInCall);
            WaitingCallCMD = new RelayCommand(WaitingCall, IfNotInCall);
            ManageSpaceCMD = new RelayCommand(ManageSpace, IfNotInCall);
            MessageCMD = new RelayCommand(Message, IfNotInCall);

            GetUserInfo();
            RegistPhone();
        }

        private void GetUserInfo()
        {
            ApplicationController.Instance.ShellViewModel.LoginInfo = "fetching user profile...";
            var webexManager = ApplicationController.Instance.CurWebexManager;
            webexManager.CurWebex.People.GetMe(r =>
            {
                if (r.IsSuccess)
                {
                    webexManager.CurUser = r.Data;
                    ApplicationController.Instance.ShellViewModel.LoginInfo = "login as: " + webexManager.CurUser.DisplayName;
                }
                else
                {
                    ApplicationController.Instance.ShellViewModel.LoginInfo = "Fetch user profile failed";
                }
            });
        }

        private void RegistPhone()
        {
            ApplicationController.Instance.ShellViewModel.ConnectionInfo = "webex cloud connecting...";
            var webexManager = ApplicationController.Instance.CurWebexManager;
            webexManager.CurWebex.Phone.Register(result =>
            {
                if (result.IsSuccess)
                {
                    ApplicationController.Instance.ShellViewModel.ConnectionInfo = "webex cloud connected";
                }
                else
                {
                    ApplicationController.Instance.ShellViewModel.ConnectionInfo = "webex cloud failed";
                }
            });
        }

        private void WaitingCall(object o)
        {
            ApplicationController.Instance.ChangeState(State.WaitingCall);
        }

        private void SendFeedBack(object o)
        {
            ApplicationController.Instance.ChangeState(State.SendFeedBack);
        }



        private void InitialCall(object o)
        {
            ApplicationController.Instance.ChangeState(State.IntiateCall);
        }

        private void ManageSpace(object o)
        {
            ApplicationController.Instance.ChangeState(State.ManageSpace);
        }
        private void Message(object o)
        {
            ApplicationController.Instance.ChangeState(State.Message);
        }

        private void VedioAudioSetup(object o)
        {
            ApplicationController.Instance.ChangeState(State.VideoAudioSetup);
        }
        private bool IfNotInCall(object o)
        {
            return true;
        }

        private void Logout(object o)
        {
            var webexManager = ApplicationController.Instance.CurWebexManager;
            webexManager.CurAuthenticator.Deauthorize();
            ApplicationController.Instance.AppLogOutput("Logout.");
            ApplicationController.Instance.ChangeState(State.PreLogin);
        }

    }
}
