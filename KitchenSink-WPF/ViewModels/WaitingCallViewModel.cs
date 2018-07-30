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
using WebexSDK;

namespace KitchenSink
{
    public class WaitingCallViewModel : ViewModelBase
    {
        Call currentCall;
        public RelayCommand DeclineCMD { get; set; }
        public RelayCommand AnswerCMD { get; set; }

        private Person caller;
        public Person Caller
        {
            get
            {
                return this.caller;
            }
            set
            {
                this.caller = value;
                OnPropertyChanged("Caller");
            }
        }

        private bool isIncomingCall = false;
        public bool IsIncomingCall
        {
            get { return this.isIncomingCall; }
            set
            {
                if (value != this.isIncomingCall)
                {
                    this.isIncomingCall = value;
                    OnPropertyChanged("IsIncomingCall");
                }
            }
        }
        private bool isNoCall = true;
        public bool IsNoCall {
            get { return this.isNoCall; }
            set
            {
                if (value != this.isNoCall)
                {
                    this.isNoCall = value;
                    OnPropertyChanged("IsNoCall");
                }
            }
        }
        public WaitingCallViewModel()
        {
            var webex = ApplicationController.Instance.CurWebexManager.CurWebex;
            if (webex != null)
            {
                webex.Phone.OnIncoming += Phone_onIncoming;
            }

            DeclineCMD = new RelayCommand(DeclineCall, CanCall);
            AnswerCMD = new RelayCommand(AnswerCall, CanCall);
        }

        private void Phone_onIncoming(WebexSDK.Call obj)
        {
            IsIncomingCall = true;
            IsNoCall = false;
            ApplicationController.Instance.CurWebexManager.currentCall = obj;
            currentCall = obj;
            currentCall.OnCallMembershipChanged += CurrentCall_onCallMembershipChanged;
        }

        private void CurrentCall_onCallMembershipChanged(CallMembershipChangedEvent obj)
        {
            FetchCaller();
        }


        private void FetchCaller()
        {
            CallMembership from = ApplicationController.Instance.CurWebexManager.currentCall.From;
            var webex = ApplicationController.Instance.CurWebexManager.CurWebex;
            webex?.People.Get(from?.PersonId, r =>
            {
                if (r.IsSuccess)
                {
                    Caller = (Person)(r.Data);
                }
            });

        }

        private void DeclineCall(object o)
        {
            unRegistEvent();
            ApplicationController.Instance.ChangeViewCmd = ChangeViewCmd.CallViewDecline;
            ApplicationController.Instance.ChangeState(State.Call);
        }
        private void AnswerCall(object o)
        {
            unRegistEvent();
            ApplicationController.Instance.ChangeViewCmd = ChangeViewCmd.CallViewAnswer;
            ApplicationController.Instance.ChangeState(State.Call);
        }
        private bool CanCall(object o)
        {
            //return (caller != null);
            return true;
        }

        private void unRegistEvent()
        {
            var webex = ApplicationController.Instance.CurWebexManager.CurWebex;
            if (webex != null)
            {
                webex.Phone.OnIncoming -= Phone_onIncoming;
            }
            if (currentCall != null)
            {
                currentCall.OnCallMembershipChanged -= CurrentCall_onCallMembershipChanged;
            }
        }
    }
}
