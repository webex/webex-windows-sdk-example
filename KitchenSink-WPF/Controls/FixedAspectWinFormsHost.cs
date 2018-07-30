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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.Integration;

namespace KitchenSink
{
    public class FixedAspectWinFormsHost : WindowsFormsHost
    {
        static FixedAspectWinFormsHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FixedAspectWinFormsHost), new FrameworkPropertyMetadata(typeof(FixedAspectWinFormsHost)));
            
        }

        public static DependencyProperty AspectRatioDependecyProperty = DependencyProperty.Register("AspectRatio",
                typeof(double),
                typeof(FixedAspectWinFormsHost),
                new FrameworkPropertyMetadata(1.777778, FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(OnAspectRationChanged)));
        public double AspectRatio
        {
            get
            {
                return (double)GetValue(AspectRatioDependecyProperty);
            }
            set
            {
                SetValue(AspectRatioDependecyProperty, value);
            }
        }

       static void OnAspectRationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var faWfHost = sender as FixedAspectWinFormsHost;
            double desiredRatio = (double)e.NewValue;
            faWfHost.UpdateLayout();
        }
          

        private Size GetNewSize(Size constraint)
        {
            Size newSize = constraint;
            double curRatio = constraint.Width / constraint.Height;
            if (curRatio > this.AspectRatio)
            {
                newSize.Width = constraint.Height * this.AspectRatio;
            }
            else if (curRatio < this.AspectRatio)
            {
                newSize.Height = constraint.Width / this.AspectRatio;
            }
            return newSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Size newSize = this.GetNewSize(finalSize);
            return base.ArrangeOverride(newSize);
        }

    }
}
