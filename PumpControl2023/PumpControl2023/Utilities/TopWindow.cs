using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Media;
using GHIElectronics.TinyCLR.UI.Media.Imaging;
using GHIElectronics.TinyCLR.UI.Shapes;
using GHIElectronics.TinyCLR.UI.Threading;

namespace PumpControl2023
{
    public sealed class MainWindow : Window
    {

        private UIElement topBar;
        private readonly StackPanel mainStackPanel;
        private StackPanel[] iconStackPanels;

        const int IconColumn = 4;
        const int IconRow = 2;
        const int MaxWindows = IconColumn * IconRow;

        private ArrayList applicationWindows;

        public MainWindow(int width, int height) : base()
        {
            this.Width = width;
            this.Height = height;

            this.mainStackPanel = new StackPanel(Orientation.Vertical);
            this.Child = this.mainStackPanel;

            this.Background = new LinearGradientBrush(Colors.Red, Colors.Gray, 0, 0, width, height);

            this.CreateTopBar();
            this.CreateIcons();

            this.applicationWindows = new ArrayList();
        }

        private void CreateTopBar()
        {
            var topbar = new TopBar(this.Width, "Pump Control 2023!!", true);
            this.topBar = topbar.Child;
        }

        private void CreateIcons()
        {
            this.iconStackPanels = new StackPanel[IconRow];

            for (var r = 0; r < IconRow; r++)
            {
                this.iconStackPanels[r] = new StackPanel(Orientation.Horizontal);
            }
        }

        private void UpdateScreen()
        {
            this.mainStackPanel.Children.Clear();
            this.mainStackPanel.Children.Add(this.topBar);

            for (var r = 0; r < IconRow; r++)
            {
                this.mainStackPanel.Children.Add(this.iconStackPanels[r]);
            }

            this.Invalidate();
        }

        public void RegisterWindow(ApplicationWindow aw)
        {
            if (this.applicationWindows.Count == MaxWindows)
                throw new ArgumentOutOfRangeException("No more than " + MaxWindows + " windows");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            aw.Parent = this;
            aw.Id = this.applicationWindows.Count;
            aw.Icon.Width = this.Width / IconColumn;
            aw.Icon.Height = aw.Icon.Width;

            var r = this.applicationWindows.Count / IconColumn;

            this.applicationWindows.Add(aw);

            this.iconStackPanels[r].Children.Clear();

            for (var i = 0; i < IconColumn; i++)
            {
                if (r * IconColumn + i >= this.applicationWindows.Count)
                    break;

                var a = (ApplicationWindow)this.applicationWindows[r * IconColumn + i];

                if (a != null)
                {
                    this.iconStackPanels[r].Children.Add(a.Icon);
                    a.Icon.Click += this.Icon_Click;
                }
            }

            this.UpdateScreen();
        }

        private void Icon_Click(object sender, RoutedEventArgs e)
        {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0)
            {
                var icon = (Icon)sender;

                var applicationWindow = (ApplicationWindow)this.applicationWindows[icon.Id];

                this.Child = applicationWindow.Open();
            }
        }

        public void Open() => this.Child = this.mainStackPanel;
    }
}
