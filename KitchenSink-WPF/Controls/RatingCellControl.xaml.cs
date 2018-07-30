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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for StarRatingControl.xaml
    /// </summary>
    public partial class RatingCell : StackPanel
    {
        public RatingCell()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty RatingValueProperty = DependencyProperty.Register(
            "RatingValue",
            typeof(Int32),
            typeof(RatingCell),
            new PropertyMetadata(0, new PropertyChangedCallback(RatingValueChanged)));

        public Int32 RatingValue
        {
            get
            {

                return (Int32)GetValue(RatingValueProperty);
            }
            set
            {
                if (value < 0)
                {
                    SetValue(RatingValueProperty, 0);
                }
                else if (value > 5)
                {
                    SetValue(RatingValueProperty, 5);
                }
                else
                {
                    SetValue(RatingValueProperty, value);
                }
            }
        }

        private static void RatingValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RatingCell parent = sender as RatingCell;
            Int32 ratingValue = (Int32)e.NewValue;
            UIElementCollection children = parent.Children;

            ToggleButton button = null;
            for (Int32 i = 0; i < ratingValue; i++)
            {
                button = children[i] as ToggleButton;
                button.IsChecked = true;
            }

            if (ratingValue < 0) return;
            for (Int32 i = ratingValue; i < children.Count; i++)
            {
                button = children[i] as ToggleButton;
                button.IsChecked = false;
            }
        }

        private void RatingButtonClickEventHandler(Object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;
            RatingValue = Int32.Parse((String)button.Tag);
        }
    }
}
