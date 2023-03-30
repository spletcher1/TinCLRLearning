using GHIElectronics.TinyCLR.UI;
using GHIElectronics.TinyCLR.UI.Controls;
using GHIElectronics.TinyCLR.UI.Media;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using CarWashExample.Properties;

namespace CarWashExample {
    public sealed class PaymentWindow {
        private Canvas canvas;
        private Font font;
        private Font fontB;        

        public UIElement Elements { get; }

        public PaymentWindow() {
            this.canvas = new Canvas();
            this.font = Resources.GetFont(Resources.FontResources.droid_reg09);
            this.fontB = Resources.GetFont(Resources.FontResources.droid_reg12);
            OnScreenKeyboard.Font = this.fontB;


            this.Elements = this.CreatePage();

        }    

        private UIElement CreatePage() {
            this.canvas.Children.Clear();

         

            var creditCardTextBox = new TextBox() {
                Text = "#########",
                Font = fontB,
                Width = 120,
                Height = 25,

            };

            Canvas.SetLeft(creditCardTextBox, 250);
            Canvas.SetTop(creditCardTextBox, 15);

            this.canvas.Children.Add(creditCardTextBox);

            var backButton = new Button() {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "Back") {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            var goButton = new Button() {
                Child = new GHIElectronics.TinyCLR.UI.Controls.Text(this.fontB, "Next") {
                    ForeColor = Colors.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
                Width = 100,
                Height = 40,
            };

            Canvas.SetLeft(backButton, 10);
            Canvas.SetTop(backButton, 220);

            this.canvas.Children.Add(backButton);

            Canvas.SetLeft(goButton, 370);
            Canvas.SetTop(goButton, 220);

            this.canvas.Children.Add(goButton);

            backButton.Click += this.BackButton_Click;
            goButton.Click += this.GoButton_Click;

            return this.canvas;
        }



        private void GoButton_Click(object sender, RoutedEventArgs e) {
            if (e.RoutedEvent.Name.CompareTo("TouchUpEvent") == 0) {               

                var msgBox = new MessageBox(this.fontB);

                msgBox.Show("Are you sure?", "Confirm", MessageBox.MessageBoxButtons.YesNo);

                msgBox.ButtonClick += (a, b) => {

                    if (b.DialogResult == MessageBox.DialogResult.Yes) {
                        Program.WpfWindow.Child = Program.LoadingPage.Elements;
                        Program.LoadingPage.Active();
                    }

                };

                Program.WpfWindow.Invalidate();
            }

        }

        private void BackButton_Click(object sender, RoutedEventArgs e) {    
            Program.WpfWindow.Child = Program.SelectServicePage.Elements;
            Program.WpfWindow.Invalidate();
        }
    }
}
