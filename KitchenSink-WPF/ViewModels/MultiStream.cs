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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WebexSDK;

namespace KitchenSink
{
    partial class CallViewModel : IMultiStreamObserver
    {
        private void RegisterMultiStream()
        {
            if (currentCall == null)
            {
                return;
            }
            currentCall.MultiStreamObserver = this;
        }

        public IntPtr OnAuxStreamAvailable()
        {
            Output($"OnAuxStreamAvailable, Total Count:{currentCall.AvailableAuxStreamCount}");
            var idleView = AuxStreamViews.FirstOrDefault(x => !x.IsInUse);
            if (idleView == null)
            {
                return IntPtr.Zero;
            }
            idleView.IsInUse = true;
            return idleView.Handle;
        }

        public IntPtr OnAuxStreamUnAvailable()
        {
            Output($"OnAuxStreamUnAvailable, Total Count:{currentCall.AvailableAuxStreamCount}");
            int InUseAuxViewCount = 0;
            foreach (var item in AuxStreamViews)
            {
                if (item.IsInUse)
                {
                    InUseAuxViewCount++;
                }
                if (InUseAuxViewCount > currentCall.AvailableAuxStreamCount)
                {
                    item.IsInUse = false;
                    return item.Handle;
                }
            }
            return IntPtr.Zero;
        }
        public void OnAuxStreamEvent(AuxStreamEvent e)
        {

            if (e is AuxStreamOpenedEvent auxStreamOpenedEvent)
            {
                var handle = auxStreamOpenedEvent.Result.Data;
                var view = AuxStreamViews.FirstOrDefault(x => x.Handle == handle);
                if (view != null)
                {
                    if (auxStreamOpenedEvent.Result.IsSuccess)
                    {
                        view.IsInUse = true;
                        view.IsShow = true;
                        Output($"AuxStreamOpenedEvent: {view.Name}");
                    }
                    else
                    {
                        view.IsInUse = false;
                        Output($"{auxStreamOpenedEvent.Result.Error.ErrorCode}, Reason:{auxStreamOpenedEvent.Result.Error.Reason}");
                    }
                }
            }
            else if (e is AuxStreamClosedEvent auxStreamClosedEvent)
            {
                var handle = auxStreamClosedEvent.Result.Data;
                var view = AuxStreamViews.FirstOrDefault(x => x.Handle == handle);
                if (view != null)
                {
                    if (auxStreamClosedEvent.Result.IsSuccess)
                    {
                        view.IsInUse = false;
                        view.IsShow = false;
                        Output($"AuxStreamClosedEvent: {view.Name}");
                    }
                    else
                    {
                        view.IsInUse = true;
                        Output($"{auxStreamClosedEvent.Result.Error.ErrorCode}, Reason:{auxStreamClosedEvent.Result.Error.Reason}");
                    }
                }
            }
            else if (e is AuxStreamPersonChangedEvent personChangedEvent)
            {
                //curCallView.RefreshViews();
                var auxStream = personChangedEvent.AuxStream;

                var find = AuxStreamViews.FirstOrDefault(x => x.Handle == auxStream.Handle);
                if (personChangedEvent.ToPerson != null)
                {
                    ApplicationController.Instance.CurWebexManager.CurWebex.People.Get(personChangedEvent.ToPerson.PersonId, r =>
                    {
                        if (r.IsSuccess)
                        {
                            find.PersonName = r.Data.DisplayName;
                        }
                    });
                    Output($"AuxStreamPersonChangedEvent: {find.Name} is changed to {personChangedEvent?.ToPerson?.Email}");
                }
                else
                {
                    Output($"AuxStreamPersonChangedEvent: {find.Name} is changed to null.");
                }
            }
            else if (e is AuxStreamSizeChangedEvent auxViewSizeChanged)
            {
                var viewSize = auxViewSizeChanged.AuxStream.AuxStreamSize;
                var index = currentCall.AuxStreams.IndexOf(auxViewSizeChanged.AuxStream);
                Output($"AuxStreamSizeChangedEvent: aux[{index}] view size changes to width[{viewSize.Width}] height[{viewSize.Height}]");
            }
            else if (e is AuxStreamSendingEvent auxStreamSending)
            {
                var index = currentCall.AuxStreams.IndexOf(auxStreamSending.AuxStream);
                Output($"AuxStreamSendingEvent: aux[{index}] IsSendingVideo[{auxStreamSending.AuxStream.IsSendingVideo}]");
                var auxStream = auxStreamSending.AuxStream;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var find = AuxStreamViews.First(x => x.Handle == auxStream.Handle);
                    find.IsSendingVideo = auxStream.IsSendingVideo;
                });

                if (auxStream.IsSendingVideo)
                {
                    UpdateAuxStreamView(auxStream);
                }
                else
                {
                    //show avatar or spinning circle
                    if (auxStream.Person != null)
                    {
                        ShowAvartar(auxStream.Handle, auxStream.Person.PersonId);
                    }
                }
            }

        }
    }
}
