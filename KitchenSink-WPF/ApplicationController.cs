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
using System.Windows;
using System.Windows.Controls;

namespace KitchenSink
{
    public class ApplicationController
    {
        #region Fields
        readonly Dictionary<WorkSpace, Border> dicWorkSpaces;
        #endregion

        #region Properties
        public ShellViewModel ShellViewModel { get; set; }
        public WebexManager CurWebexManager { get;  set; }

        public CallView CurCallView { get; set; }
        public ChangeViewCmd ChangeViewCmd { get; set; }
        
        #endregion

        #region singleton

        static ApplicationController instance;
        public static ApplicationController Instance
        {
            get
            {
                if (instance == null)
                    instance = new ApplicationController();
                return instance;
            }
        }

        private ApplicationController()
        {
            dicWorkSpaces = new Dictionary<WorkSpace, Border>();
            ChangeViewCmd = ChangeViewCmd.None;
        }

        #endregion

        #region Navigation
        public void RegistWorkSpace(WorkSpace workSpace, Border containter)
        {
            if (this.dicWorkSpaces.ContainsKey(workSpace))
            {
                this.dicWorkSpaces.Remove(workSpace);
            }
            this.dicWorkSpaces.Add(workSpace, containter);
        }

        public void ChangeState(State state)
        {
            Application.Current.Dispatcher.Invoke(() => {
                switch (state)
                {
                    case State.Main:
                        this.ShowView(new MainView(), WorkSpace.Main);
                        break;
                    case State.LoginByJWT:
                        this.ShowView(new JWTLoginView(), WorkSpace.Main);
                        break;
                    case State.VideoAudioSetup:
                        this.ShowView(new VideoAudioSetupView(), WorkSpace.Right);
                        break;
                    case State.IntiateCall:
                        this.ShowView(new InitiateCallView(), WorkSpace.Right);
                        break;
                    case State.Call:
                        if (CurCallView == null)
                        {
                            CurCallView = new CallView();
                        }
                        this.ShowView(CurCallView, WorkSpace.Main);
                        break;
                    case State.LoginByOAuth:
                        this.ShowView(new OAuthLoginView(), WorkSpace.Main);
                        break;
                    case State.PreLogin:
                        this.ShowView(new PreLoginView(), WorkSpace.Main);
                        break;
                    case State.WaitingCall:
                        this.ShowView(new WaitingCallView(), WorkSpace.Right);
                        break;
                    case State.SendFeedBack:
                        this.ShowView(new FeedBackView(), WorkSpace.Right);
                        break;
                    case State.VideoCodecLicense:
                        this.ShowView(new VideoCodecLicenseView(), WorkSpace.Main);
                        break;
                    case State.ManageSpace:
                        this.ShowView(new SpaceManageView(), WorkSpace.Right);
                        break;
                    case State.Message:
                        this.ShowView(new MessageView(), WorkSpace.Right);
                        break;
                    case State.MessageSession:
                        this.ShowView(new MessageSessionView(), WorkSpace.Main);
                        break;

                }
            });
            
        }

        private void ShowView(UserControl view, WorkSpace s)
        {
            if (this.dicWorkSpaces.ContainsKey(s))
                this.dicWorkSpaces[s].Child = view;
        }

        public void AppLogOutput(String format, params object[] args)
        {
            ShellViewModel.Output(format, args);
        }

        #endregion

    }
}
