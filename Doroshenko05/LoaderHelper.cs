using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontAwesome.WPF;
using System.Windows;
using System.Windows.Controls;

namespace Doroshenko05
{
    internal static class LoaderHelper
    {
        internal delegate void LoaderAction(Grid grid, ref ImageAwesome loader, bool isShow);

        internal static void OnRequestLoader(Grid grid, ref ImageAwesome loader, bool isShow)
        {
            if (isShow && loader == null)
            {
                loader = new ImageAwesome();
                grid.Children.Add(loader);
                loader.Icon = FontAwesomeIcon.Refresh;
                loader.Spin = true;
                loader.Width = loader.Height = 20;
                Grid.SetRow(loader, 0);
                Grid.SetColumn(loader, 0);
                Grid.SetColumnSpan(loader, 10);
                Grid.SetRowSpan(loader, 10);
                loader.HorizontalAlignment = HorizontalAlignment.Center;
                loader.VerticalAlignment = VerticalAlignment.Center;
            }
            else if (loader != null)
            {
                grid.Children.Remove(loader);
                loader = null;
            }
        }
    }
}
