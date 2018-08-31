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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KitchenSink
{
    partial class CallViewModel : ViewModelBase
    {
        #region Fields

        readonly WebexSDK.Webex webex;
        WebexSDK.Call currentCall
        {
            get
            {
                return ApplicationController.Instance.CurWebexManager.currentCall;
            }
            set
            {
                ApplicationController.Instance.CurWebexManager.currentCall = value;
            }
        }
        readonly CallView curCallView;

        #endregion

        #region Properties

        public RelayCommand BackCommand { get; set; }
        public RelayCommand EndCallCMD { get; set; }
        public RelayCommand KeyboardCMD { get; set; }      
        public RelayCommand StopShareCMD { get; set; }

        public RelayCommand DtmfCMD { get; set; }

        private double aspectRatioLocalVedio = 16.0 / 9.0;
        public double AspectRatioLocalVedio
        {
            get
            {
                return aspectRatioLocalVedio;
            }
            set
            {
                if(value != aspectRatioLocalVedio)
                {
                    aspectRatioLocalVedio = value;
                    OnPropertyChanged("AspectRatioLocalVedio");
                }
            }
        }

        private double aspectRatioRemoteVedio = 16.0 / 9.0;
        public double AspectRatioRemoteVedio
        {
            get
            {
                return aspectRatioRemoteVedio;
            }
            set
            {
                if (value != aspectRatioRemoteVedio)
                {
                    aspectRatioRemoteVedio = value;
                    OnPropertyChanged("AspectRatioRemoteVedio");
                }
            }
        }

        private double aspectRatioShareVedio = 16.0 / 9.0;
        public double AspectShareScreenVideo
        {
            get
            {
                return aspectRatioShareVedio;
            }
            set
            {
                if (value != aspectRatioShareVedio)
                {
                    aspectRatioShareVedio = value;
                    OnPropertyChanged("AspectShareScreenVideo");
                }
            }
        }

        public AuxStreamView RemoteAux1VideoView { get; set; }
        public AuxStreamView RemoteAux2VideoView { get; set; }
        public AuxStreamView RemoteAux3VideoView { get; set; }
        public AuxStreamView RemoteAux4VideoView { get; set; }
        public ObservableCollection<AuxStreamView> AuxStreamViews { get; set; }

        private ObservableCollection<CallMembership> callMemberships;
        public ObservableCollection<CallMembership> CallMemberships
        {
            get
            {
                return this.callMemberships;
            }
            set
            {
                this.callMemberships = value;
                OnPropertyChanged("CallMemberships");
            }
        }
        public bool IsRemoteSendingVideo
        {
            get
            {
                if (this.currentCall != null)
                {
                    return this.currentCall.IsRemoteSendingVideo;
                }
                return false;
            }
        }
        public bool IsRemoteSendingAudio
        {
            get
            {
                if (this.currentCall != null)
                {
                    return this.currentCall.IsRemoteSendingAudio;
                }
                return false;
            }
        }

        public bool IfSendAudio
        {
            get
            {
                if (this.currentCall != null)
                {
                    return this.currentCall.IsSendingAudio;
                }
                return false;
            }
            set
            {
                if (this.currentCall != null && value != this.currentCall.IsSendingAudio)
                {
                    this.currentCall.IsSendingAudio = value;
                    OnPropertyChanged("IfSendAudio");
                }
            }
        }

        public bool IfSendVedio
        {
            get
            {
                if (this.currentCall != null)
                {
                    return this.currentCall.IsSendingVideo;
                }
                return false;
            }
            set
            {
                if (this.currentCall != null && value != this.currentCall.IsSendingVideo)
                {
                    this.currentCall.IsSendingVideo = value;
                    OnPropertyChanged("IfSendVedio");
                }                
            }
        }

        public bool IfReceiveVedio
        {
            get
            {
                if (this.currentCall != null)
                {
                    return this.currentCall.IsReceivingVideo;
                }
                return false;
            }
            set
            {
                if (this.currentCall != null && value != this.currentCall.IsReceivingVideo)
                {
                    this.currentCall.IsReceivingVideo = value;
                    OnPropertyChanged("IfReceiveVedio");
                }
            }
        }

        public bool IfReceiveAudio
        {
            get
            {
                if (this.currentCall != null)
                {
                    return this.currentCall.IsReceivingAudio;
                }
                return false;
            }
            set
            {
                if (this.currentCall != null&&value != this.currentCall.IsReceivingAudio)
                {
                    this.currentCall.IsReceivingAudio = value;
                    OnPropertyChanged("IfReceiveAudio");
                }
            }
        }

        private bool ifIncludeLog = false;
        public bool IfIncludeLog
        {
            get
            {
                return this.ifIncludeLog;
            }
            set
            {
                this.ifIncludeLog = value;
                OnPropertyChanged("IfIncludeLog");
            }
        }
        public string CallStatus
        {
            get
            {
                if (this.currentCall != null)
                {
                    return "Call Status: " + this.currentCall.Status.ToString();
                }
                return null;
            }
        }

        private string activeSpeaker;
        public string ActiveSpeaker
        {
            get
            {
                return activeSpeaker;
            }
            set
            {
                activeSpeaker = value;
                OnPropertyChanged("ActiveSpeaker");
            }
        }

        private string inputKey;
        public string InputKey
        {
            get
            {
                return this.inputKey;
            }
            set
            {
                this.inputKey = value;
                OnPropertyChanged("InputKey");
            }
        }

        private ObservableCollection<ShareSource> shareSourceList;
        public ObservableCollection<ShareSource> ShareSourceList
        {
            get
            {
                return this.shareSourceList;
            }
            set
            {
                this.shareSourceList = value;
                OnPropertyChanged("ShareSourceList");
            }
        }

        private ShareSource selectedSource;
        public ShareSource SelectedSource
        {
            get
            {
                return this.selectedSource;
            }
            set
            {
                this.selectedSource = value;
                if (this.selectedSource != null)
                {
                    this.currentCall.StartShare(this.selectedSource.SourceId, r =>
                    {
                        if (!r.IsSuccess)
                        {
                            Output($"Start share failed! Error: {r.Error?.ErrorCode.ToString()} {r.Error?.Reason}");
                        }
                        IfShowStopShareButton = true;
                    });
                }
            }
        }

        public void FetchShareSources()
        {
            ShareSourceList.Clear();
            if (this.currentCall != null)
            {
                this.currentCall.FetchShareSources(ShareSourceType.Desktop, result =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (result.IsSuccess)
                        {
                            foreach (var i in result.Data)
                                ShareSourceList.Add(i);
                        }
                    });

                });

                this.currentCall.FetchShareSources(ShareSourceType.Application, r =>
                {
                    if (r.IsSuccess)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            if (r.IsSuccess)
                            {
                                foreach (var i in r.Data)
                                    ShareSourceList.Add(i);
                            }
                        });
                    }
                });
            }
        }

        private void StopShare(object o)
        {
            if (currentCall == null)
            {
                return;
            }

            if (!currentCall.IsSendingShare)
            {
                return;
            }

            currentCall.StopShare(r=>
            {
                if (r.IsSuccess)
                {
                    IfShowStopShareButton = false;
                }
            });
        }

        #region Rating

        public RelayCommand HideRatingViewCMD { get; set; }
        public RelayCommand SendFeedBackCMD { get; set; }

        private int ratingValue = int.MinValue;
        public int RatingValue
        {
            get {
                return this.ratingValue;
            }
            set
            {
                if (value != this.ratingValue)
                {
                    this.ratingValue = value;
                    OnPropertyChanged("RatingValue");
                }
            }
        }

        private string comment = string.Empty;

        public string Comment
        {
            get
            {
                return this.comment;
            }
            set
            {
                if (value != this.comment)
                {
                    this.comment = value;
                    OnPropertyChanged("Comment");
                }
            }
        }

        private bool ifShowRatingView = false;
        public bool IfShowRatingView
        {
            get
            {
                return this.ifShowRatingView;
            }
            set
            {
                if (value != ifShowRatingView)
                {
                    this.ifShowRatingView = value;
                    OnPropertyChanged("IfShowRatingView");
                }
            }
        }

        private bool ifShowkeyboard = false;
        public bool IfShowkeyboard
        {
            get
            {
                return this.ifShowkeyboard;
            }
            set
            {
                if (value != ifShowkeyboard)
                {
                    this.ifShowkeyboard = value;
                    OnPropertyChanged("IfShowkeyboard");
                }
            }
        }

        private bool ifShowStopShareButton = false;
        public bool IfShowStopShareButton
        {
            get
            {
                return this.ifShowStopShareButton;
            }
            set
            {
                if (value != ifShowStopShareButton)
                {
                    this.ifShowStopShareButton = value;
                    OnPropertyChanged("IfShowStopShareButton");
                }
            }
        }

        private bool ifShowRemoteView = false;
        public bool IfShowRemoteView
        {
            get
            {
                return this.ifShowRemoteView;
            }
            set
            {
                if (value != ifShowRemoteView)
                {
                    this.ifShowRemoteView = value;
                    OnPropertyChanged("IfShowRemoteView");
                }
            }
        }

        #endregion

        #endregion

        public CallViewModel() { }
        public CallViewModel(CallView callView):base()
        {
            curCallView = callView;
            EndCallCMD = new RelayCommand(EndCall);
            BackCommand = new RelayCommand(BackToMain);
            HideRatingViewCMD = new RelayCommand(HideRatingView);
            SendFeedBackCMD = new RelayCommand(this.SendFeedBack,this.CanSendFeedBack);
            KeyboardCMD = new RelayCommand(ShowHideKeyboard);
            DtmfCMD = new RelayCommand(SendDTMF);
            StopShareCMD = new RelayCommand(StopShare);
            webex = ApplicationController.Instance.CurWebexManager.CurWebex;
            shareSourceList = new ObservableCollection<ShareSource>();
            AuxStreamViews = new ObservableCollection<AuxStreamView>();
            LoadAuxStreamViews();
        }

        void LoadAuxStreamViews()
        {
            AuxStreamViews.Clear();

            RemoteAux1VideoView = new AuxStreamView()
            {
                Name = "RemoteVideoViewAux1",
                Handle = curCallView.RemoteAux1Handle,
            };
            AuxStreamViews.Add(RemoteAux1VideoView);

            RemoteAux2VideoView = new AuxStreamView()
            {
                Name = "RemoteVideoViewAux2",
                Handle = curCallView.RemoteAux2Handle,
            };
            AuxStreamViews.Add(RemoteAux2VideoView);

            RemoteAux3VideoView = new AuxStreamView()
            {
                Name = "RemoteVideoViewAux3",
                Handle = curCallView.RemoteAux3Handle,
            };
            AuxStreamViews.Add(RemoteAux3VideoView);

            RemoteAux4VideoView = new AuxStreamView()
            {
                Name = "RemoteVideoViewAux4",
                Handle = curCallView.RemoteAux4Handle,
            };
            AuxStreamViews.Add(RemoteAux4VideoView);
        }

        #region method

        public void Dial(string calleeAddress)
        {
            webex?.Phone.Dial(calleeAddress, MediaOption.AudioVideoShare(curCallView.LocalViewHandle, curCallView.RemoteViewHandle, curCallView.RemoteShareViewHandle), result =>
            {
                if (result.IsSuccess)
                {
                    currentCall = result.Data;

                    RegisterCallEvent();
                    this.curCallView.RefreshViews();
                }
                else
                {
                    Output($"Error: {result.Error?.ErrorCode.ToString()} {result.Error?.Reason}");
                }
            });
        }

        public void Answer()
        {
            if (currentCall == null)
            {
                return;
            }
            RegisterCallEvent();
            currentCall.Answer(MediaOption.AudioVideoShare(curCallView.LocalViewHandle, curCallView.RemoteViewHandle, curCallView.RemoteShareViewHandle), result =>
            {
                if (!result.IsSuccess)
                {
                    Output($"Error: {result.Error?.ErrorCode.ToString()} {result.Error?.Reason}");
                }               
            });
        }
        public void Reject()
        {
            if (currentCall == null)
            {
                return;
            }
            RegisterCallEvent();
            currentCall.Reject(result =>
            {
                if (!result.IsSuccess)
                {
                    Output($"Error: {result.Error?.ErrorCode.ToString()} {result.Error?.Reason}");
                }
            });
        }

        private void EndCall(object o)
        {
            if (this.currentCall != null)
            {
                currentCall.Hangup(result =>
                {
                    if (!result.IsSuccess)
                    {
                        Output("Hangup failed.");
                    }
                });
            }
            ApplicationController.Instance.CurCallView = null;
        }
        public void UpdateLocalVideoView()
        {
            if (currentCall == null)
            {
                return;
            }
            
            Application.Current.Dispatcher.Invoke(() =>
            {
                currentCall.RefreshLocalView(curCallView.LocalViewHandle);
            });
        }
        public void UpdateRemoteVideoView()
        {
            if (currentCall == null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                currentCall.RefreshRemoteView(curCallView.RemoteViewHandle);
            });
        }
        public void UpdateRemoteAllAuxVideoView()
        {
            if (currentCall == null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in currentCall.AuxStreams)
                {
                    item.RefreshView();
                }
            });
        }
        public void UpdateAuxStreamView(AuxStream AuxStream)
        {
            if (currentCall == null && AuxStream == null)
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                AuxStream.RefreshView();
            });
        }

        public void UpdateRemoteShareVideoView()
        {
            if (currentCall == null)
            {
                return;
            }
            
            Application.Current.Dispatcher.Invoke(() =>
            {
                currentCall.RefreshRemoteShareView(curCallView.RemoteShareViewHandle);
            });
        }

        private void RefreshAudioVideoCtrlView()
        {
            OnPropertyChanged("IfSendAudio");
            OnPropertyChanged("IfSendVedio");
            OnPropertyChanged("IfReceiveVedio");
            OnPropertyChanged("IfReceiveAudio");
            OnPropertyChanged("IsRemoteSendingVideo");
            OnPropertyChanged("IsRemoteSendingAudio");
        }
        private void RefreshCallStatusView()
        {
            OnPropertyChanged("CallStatus");
        }

        private void Output(String format, params object[] args)
        {
            ApplicationController.Instance.AppLogOutput(format, args);
        }

        private void UpdateRecentContactsStore()
        {
            if (currentCall == null
                || currentCall.To == null
                || currentCall.From == null)
            {
                return;
            }

            if (currentCall.Direction == Call.CallDirection.Outgoing)
            {
                ApplicationController.Instance.CurWebexManager?.RecentContacts?.AddRecentContactsStore(currentCall.To.PersonId);
            }
            else
            {
                ApplicationController.Instance.CurWebexManager.RecentContacts.AddRecentContactsStore(currentCall.From.PersonId);
            }
        }

        private void BackToMain(object o)
        {
            ApplicationController.Instance.CurCallView = null;
            ApplicationController.Instance.ChangeState(State.Main);
        }

        #endregion

        #region CallEvents
        private void RegisterCallEvent()
        {
            if (currentCall == null)
            {
                return;
            }
            currentCall.OnRinging += CurrentCall_onRinging;

            currentCall.OnConnected += CurrentCall_onConnected;

            currentCall.OnDisconnected += CurrentCall_onDisconnected;

            currentCall.OnMediaChanged += CurrentCall_onMediaChanged;

            currentCall.OnCapabilitiesChanged += CurrentCall_onCapabilitiesChanged;

            currentCall.OnCallMembershipChanged += CurrentCall_onCallMembershipChanged;

            RegisterMultiStream();
        }

        private void UnRegisterCallEvent()
        {
            if (currentCall == null)
            {
                return;
            }
            currentCall.OnRinging -= CurrentCall_onRinging;

            currentCall.OnConnected -= CurrentCall_onConnected;

            currentCall.OnDisconnected -= CurrentCall_onDisconnected;

            currentCall.OnMediaChanged -= CurrentCall_onMediaChanged;

            currentCall.OnCapabilitiesChanged -= CurrentCall_onCapabilitiesChanged;

            currentCall.OnCallMembershipChanged -= CurrentCall_onCallMembershipChanged;
        }

        private void CurrentCall_onRinging(WebexSDK.Call call)
        {
            RefreshCallStatusView();
        }
        private void CurrentCall_onConnected(WebexSDK.Call call)
        {
            RefreshCallStatusView();
        }

        private void CurrentCall_onDisconnected(CallDisconnectedEvent reason)
        {
            RefreshCallStatusView();
            UpdateRecentContactsStore();
            UnRegisterCallEvent();
            Output("call is disconnected for " + reason?.GetType().Name);
            this.curCallView.RefreshViews();
#pragma warning disable S125 // Sections of code should not be "commented out"
            //this.IfShowRatingView = true;
#pragma warning restore S125 // Sections of code should not be "commented out"
            currentCall = null;

            ApplicationController.Instance.CurWebexManager.CurCalleeAddress = null;
        }

        private void CurrentCall_onCallMembershipChanged(CallMembershipChangedEvent obj)
        {
            CallMemberships = new ObservableCollection<CallMembership>(currentCall?.Memberships);

            if (obj is CallMembershipJoinedEvent)
            {
                Output($"{obj.CallMembership.Email} joined");
            }
            else if (obj is CallMembershipLeftEvent)
            {
                Output($"{obj.CallMembership.Email} left");
            }
            else if (obj is CallMembershipDeclinedEvent)
            {
                Output($"{obj.CallMembership.Email} decline");
            }
            else if (obj is CallMembershipSendingAudioEvent)
            {
                if (obj.CallMembership.IsSendingAudio)
                {
                    Output($"{obj.CallMembership.Email} unmute audio");
                }
                else
                {
                    Output($"{obj.CallMembership.Email} mute audio");
                }
                
            }
            else if (obj is CallMembershipSendingVideoEvent)
            {
                if (obj.CallMembership.IsSendingVideo)
                {
                    Output($"{obj.CallMembership.Email} unmute video");
                }
                else
                {
                    Output($"{obj.CallMembership.Email} mute video");
                }
            }
            else if (obj is CallMembershipSendingShareEvent)
            {
                if (obj.CallMembership.IsSendingShare)
                {
                    Output($"{obj.CallMembership.Email} sending share");
                }
                else
                {
                    Output($"{obj.CallMembership.Email} stop share");
                }
            }


        }

        private void CurrentCall_onMediaChanged(MediaChangedEvent mediaChgEvent)
        {
            RefreshAudioVideoCtrlView();

            if (mediaChgEvent is LocalVideoViewSizeChangedEvent)
            {
                Output($"remote video size: width[{mediaChgEvent.Call.LocalVideoViewSize.Width}] height[{mediaChgEvent.Call.LocalVideoViewSize.Height}]");
                this.AspectRatioLocalVedio = mediaChgEvent.Call.LocalVideoViewSize.Width / (double)mediaChgEvent.Call.LocalVideoViewSize.Height;
            }
            else if (mediaChgEvent is RemoteVideoViewSizeChangedEvent)
            {
                Output($"remote video size: width[{mediaChgEvent.Call.RemoteVideoViewSize.Width}] height[{mediaChgEvent.Call.RemoteVideoViewSize.Height}]");
                this.AspectRatioRemoteVedio = mediaChgEvent.Call.RemoteVideoViewSize.Width / (double)mediaChgEvent.Call.RemoteVideoViewSize.Height;
            }
            else if (mediaChgEvent is RemoteShareViewSizeChangedEvent)
            {
                Output($"remote share size: width[{mediaChgEvent.Call.RemoteShareViewSize.Width}] height[{mediaChgEvent.Call.RemoteShareViewSize.Height}]");
                this.AspectShareScreenVideo = mediaChgEvent.Call.RemoteShareViewSize.Width / (double)mediaChgEvent.Call.RemoteShareViewSize.Height;
            }
            else if (mediaChgEvent is RemoteSendingVideoEvent remoteSendingVideoEvent)
            {
                this.curCallView?.RefreshRemoteViews();
                Output($"RemoteSendingVideoEvent: IsSending[{remoteSendingVideoEvent.IsSending}]");
                if (remoteSendingVideoEvent.IsSending)
                {
                    UpdateRemoteVideoView();   
                }
                else
                {
                    //show avatar or spinning circle
                    if(currentCall.ActiveSpeaker != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ShowAvartar(curCallView.RemoteViewHandle, currentCall?.ActiveSpeaker?.PersonId);
                        });
                    }
                }
            }
            else if (mediaChgEvent is RemoteSendingAudioEvent remoteSendingAudioEvent)
            {
                Output($"RemoteSendingAudioEvent: IsSending[{remoteSendingAudioEvent.IsSending}]");
            }
            else if (mediaChgEvent is RemoteSendingShareEvent remoteSendingShareEvent)
            {
                this.curCallView?.RefreshShareViews();
                Output($"RemoteSendingShareEvent: IsSending[{remoteSendingShareEvent.IsSending}]");
                curCallView.SwitchShareViewWithRemoteView(remoteSendingShareEvent.IsSending);
            }
            else if (mediaChgEvent is SendingVideoEvent sendingVideoEvent)
            {
                this.curCallView?.RefreshLocalViews();
                Output($"SendingVideoEvent: IsSending[{sendingVideoEvent.IsSending}]");
                if (sendingVideoEvent.IsSending)
                {
                    UpdateLocalVideoView();
                }
            }
            else if (mediaChgEvent is SendingAudioEvent sendingAudioEvent)
            {
                Output($"SendingAudioEvent: IsSending[{sendingAudioEvent.IsSending}]");
            }
            else if (mediaChgEvent is SendingShareEvent sendingShareEvent)
            {
                Output($"SendingShareEvent: IsSending[{sendingShareEvent.IsSending}]");
            }
            else if (mediaChgEvent is ReceivingVideoEvent receivingVideoEvent)
            {
                this.curCallView?.RefreshRemoteViews();
                Output($"ReceivingVideoEvent: IsReceiving[{receivingVideoEvent.IsReceiving}]");
            }
            else if (mediaChgEvent is ReceivingAudioEvent receivingAudioEvent)
            {
                Output($"ReceivingAudioEvent: IsReceiving[{receivingAudioEvent.IsReceiving}]");
            }
            else if (mediaChgEvent is ReceivingShareEvent receivingShareEvent)
            {
                this.curCallView?.RefreshShareViews();
                Output($"ReceivingShareEvent: IsReceiving[{receivingShareEvent.IsReceiving}]");
            }
            else if (mediaChgEvent is CameraSwitchedEvent cameraSwitchedEvent)
            {
                Output($"CameraSwitchedEvent: switch camera to {cameraSwitchedEvent.Camera.Name}");
            }
            else if (mediaChgEvent is SpeakerSwitchedEvent speakerSwitchedEvent)
            {
                Output($"SpeakerSwitchedEvent: switch speaker to {speakerSwitchedEvent.Speaker.Name}");
            }
            else if (mediaChgEvent is ActiveSpeakerChangedEvent activeSpeakerChanged)
            {
                if(activeSpeakerChanged.ToPerson != null)
                {
                    IfShowRemoteView = true;
                    ApplicationController.Instance.CurWebexManager.CurWebex.People.Get(activeSpeakerChanged.ToPerson.PersonId, r =>
                    {
                        if (r.IsSuccess)
                        {
                            ActiveSpeaker = r.Data.DisplayName;
                        }
                    });
                    Output($"ActiveSpeakerChangedEvent: active speaker is changed to {activeSpeakerChanged?.ToPerson?.Email}");
                }
                else
                {
                    IfShowRemoteView = false;
                    ActiveSpeaker = null;
                    Output($"ActiveSpeakerChangedEvent: active speaker is changed to null");
                }
            }
        }

        private void ShowAvartar(IntPtr handle, string personId)
        {
            if (handle == IntPtr.Zero || personId == null)
            {
                return;
            }

            ApplicationController.Instance.CurWebexManager.CurWebex.People.Get(personId, r =>
            {
                if (r.IsSuccess)
                {
                   curCallView.UpdateAvarta(handle, r.Data.Avatar);
                }
            });
        }

        private void CurrentCall_onCapabilitiesChanged(Capabilities capability)
        {   
            if (capability is CapabilitiesDTMF)
            {
                CapabilitiesDTMF dtmf = capability as CapabilitiesDTMF;
                if (dtmf.IsEnabled)
                {
                    Output($"DTMF Capability Enable");
                }
                else
                {
                    Output($"DTMF Capability Disable");
                }
            }
        }
        #endregion

        #region Rating
        private void HideRatingView(object o)
        {
            this.IfShowRatingView = false;
        }

        private bool CanSendFeedBack(object o)
        {
            return !string.IsNullOrEmpty(this.Comment) || this.ratingValue != int.MinValue;
        }
        private void SendFeedBack(object o)
        {
            this.IfShowRatingView = false;
            currentCall?.SendFeedbackWith(this.RatingValue, this.Comment, this.IfIncludeLog);
        }
        #endregion

        #region keyboard
        private void ShowHideKeyboard(object o)
        {
            this.IfShowkeyboard = !IfShowkeyboard;
        }


        private void SendDTMF(object o)
        {
            string key = o as string;
            if (key == null
                || key.Length != 1)
            {
                return;
            }
            InputKey += key;
            if (currentCall == null)
            {
                return;
            }
            if (!currentCall.IsSendingDTMFEnabled)
            {
                Output("Current call not support sending DTMF.");
                return;
            }
            currentCall.SendDtmf(key, r =>
            {
                if (r.IsSuccess)
                {
                    Output($"Send DTMF[{key}] Success!");
                }
                else
                {
                    Output($"Send DTMF[{key}] Fail!");
                }
            });
        }
        #endregion
    }
    

    public class AuxStreamView: ViewModelBase
    {
        public string Name { get; set; }
        public IntPtr Handle { get; set; }
        public bool IsInUse { get; set; }
        public AuxStream AuxVideo { get; set; }

        private bool isShow = false;
        public bool IsShow
        {
            get
            {
                return this.isShow;
            }
            set
            {
                if (value != isShow)
                {
                    this.isShow = value;
                    OnPropertyChanged("IsShow");
                }
            }
        }

        private string avartar;
        public string Avartar
        {
            get
            {
                return this.avartar;
            }
            set
            {
                this.avartar = value;
                
                OnPropertyChanged("Avartar");
            }
        }

        private string personName;
        public string PersonName {
            get
            {
                return this.personName;
            }
            set
            {
                this.personName = value;
                OnPropertyChanged("PersonName");
            }
        }

        private bool isSendingVideo;
        public bool IsSendingVideo
        {
            get
            {
                return this.isSendingVideo;
            }
            set
            {
                isSendingVideo = value;
                OnPropertyChanged("IsSendingVideo");
            }
        }
    }

}
