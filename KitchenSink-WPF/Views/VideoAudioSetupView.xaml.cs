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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KitchenSink
{
    /// <summary>
    /// Interaction logic for VideoAudioSetup.xaml
    /// </summary>
    public partial class VideoAudioSetupView : UserControl
    {
        public VideoAudioSetupView()
        {
            InitializeComponent();
            this.Loaded += VideoAudioSetupView_Loaded;
            this.Unloaded += VideoAudioSetupView_Unloaded;
            this.pbCameraPreview.SizeChanged += PbCameraPreview_SizeChanged;
           
        }

        private void PbCameraPreview_SizeChanged(object sender, EventArgs e)
        {
            VideoAudioSetupViewModel vm = this.DataContext as VideoAudioSetupViewModel;
            vm.ResetPreviewWindow();
        }

        private void VideoAudioSetupView_Unloaded(object sender, RoutedEventArgs e)
        {
            VideoAudioSetupViewModel vm = this.DataContext as VideoAudioSetupViewModel;
            vm.OnViewClosed();
        }

        private void VideoAudioSetupView_Loaded(object sender, RoutedEventArgs e)
        {
            VideoAudioSetupViewModel vm = this.DataContext as VideoAudioSetupViewModel;
            vm.OnViewReady(this.pbCameraPreview.Handle, this.RefreshCameraPreview);
         
        }

        private void RefreshCameraPreview()
        {
            this.pbCameraPreview.Refresh();
        }
    }
}
