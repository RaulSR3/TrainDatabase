﻿using Helper;
using Microsoft.EntityFrameworkCore;
using Model;
using ModelTrainController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPF_Application;
using WPF_Application.Helper;
using WPF_Application.Infrastructure;

namespace Wpf_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Database db = new();
        public event PropertyChangedEventHandler? PropertyChanged;

        private ModelTrainController.ModelTrainController? controler;
        private Theme theme;

        ModelTrainController.ModelTrainController? Controler
        {
            get => controler; set
            {
                controler = value;
                if (controler is not null)
                    controler.LogOn();
            }
        }

        public Theme Theme
        {
            get => theme;
            set
            {
                theme = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            try
            {

#if RELEASE
                if (MessageBoxResult.No == MessageBox.Show("Achtung! Es handelt sich bei der Software um eine Alpha version! Es können und werden Bugs auftreten, wenn Sie auf JA drücken, stimmen Sie zu, dass der Entwickler für keinerlei Schäden, die durch die Verwendung der Software entstehen könnten, haftbar ist!", "Haftungsausschluss", MessageBoxButton.YesNo, MessageBoxImage.Information))
                {
                    Close();
                }
#endif
                Theme = new();
                InitializeComponent();
                db.Database.EnsureCreated();

                if (db.Vehicles.Any())
                    DrawAllVehicles(db.Vehicles.OrderBy(e => e.Position).ToList());
                else
                if (MessageBoxResult.Yes == MessageBox.Show("Sie haben noch keine Daten in der Datenbank. Möchten Sie jetzt welche importieren?", "Datenbank importieren", MessageBoxButton.YesNo, MessageBoxImage.Question))
                {
                    new Importer.ImportSelecter(db).Show();
                    this.Close();
                }
                Controler = new Z21(new StartData() { LanAdresse = Settings.ControllerIP.ToString(), LanPort = 21105 });
                Settings.GetJoyStickFunctionDictionary();
            }
            catch (Exception e)
            {
                Logger.Log($"Fehler beim öffnen vom Main Window: ex-Message:{e?.Message ?? ""}\ninnerex: {e?.InnerException?.ToString() ?? ""}\ninner ex message:{e?.InnerException?.Message ?? ""}", LoggerType.Error);
                MessageBox.Show($"Es ist ein Fehler beim öffnen der Applikation aufgetreten.\nException Message: '{e.Message}' \nIm Ordner '{Directory.GetCurrentDirectory() + @"\Log" + @"\Error"}' finden Sie ein Logfile. Bitte an jakob.eichberger@gmx.net als Zipfile schicken. (Dann kann ichs fixen) ;) ");
            }
        }

        private void DB_Import_Z21_new(object sender, RoutedEventArgs e)
        {
            VehicleGrid.Children.Clear();
            var x = new Importer.Z21(db);
            this.Close();
            x.Show();
        }

        public void DrawAllVehicles(IEnumerable<Vehicle> list)
        {
            VehicleGrid.Children.Clear();
            foreach (var item in list)
            {
                if (item is null) return;
                Border border = new();
                border.Padding = new(2);
                border.Margin = new(10);
                border.BorderThickness = new(1);
                border.BorderBrush = Brushes.Black;

                StackPanel sp = new();
                try
                {
                    string path = $"{Directory.GetCurrentDirectory()}\\Data\\VehicleImage\\";
                    path += string.IsNullOrWhiteSpace(item?.Image_Name) ? "default.png" : item?.Image_Name;
                    Image image = new();
                    BitmapImage bitmap = new();
                    bitmap.BeginInit();
                    bitmap.UriSource = new(path);
                    bitmap.EndInit();
                    image.Source = bitmap;
                    image.Width = 250;
                    image.Height = 100;
                    sp.Children.Add(image);

                }
                catch (Exception ex)
                {
                    Logger.Log($"{DateTime.UtcNow}: Image for Lok with adress '{item?.Address}' not found. Message: {ex.Message}", LoggerType.Warning);
                }
                TextBlock tb = new();
                tb.Text = !string.IsNullOrWhiteSpace(item?.Full_Name) ? item?.Full_Name : (!string.IsNullOrWhiteSpace(item?.Name) ? item?.Name : $"Adresse: {item?.Address}");
                sp.Height = 120;
                sp.Width = 250;
                sp.Children.Add(tb);
                sp.HorizontalAlignment = HorizontalAlignment.Left;
                sp.VerticalAlignment = VerticalAlignment.Top;
                sp.Background = Brushes.White;

                sp.ContextMenu = new();
                MenuItem miControlLoko = new();
                miControlLoko.Header = item.Type == VehicleType.Lokomotive ? "Lok steuern" : (item.Type == VehicleType.Steuerwagen ? "Steuerwagen steuern" : "Wagen steuern");
                miControlLoko.Click += ControlLoko_Click;
                sp.ContextMenu.Items.Add(miControlLoko);
                miControlLoko.Tag = item;
                MenuItem miEditLoko = new();
                miEditLoko.Header = item.Type == VehicleType.Lokomotive ? "Lok bearbeiten" : (item.Type == VehicleType.Steuerwagen ? "Steuerwagen bearbeiten" : "Wagen bearbeiten");
                miEditLoko.Click += EditLoko_Click;

                sp.ContextMenu.Items.Add(miEditLoko);
                border.Child = sp;
                VehicleGrid.Children.Add(border);
            }
        }

        void ControlLoko_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                var menu = ((MenuItem)e.Source);
                Vehicle? vehicle = (menu.Tag as Vehicle);
                if (vehicle is not null)
                    new TrainControl(Controler, vehicle, db).Show();
                else
                    MessageBox.Show("Öffnen nicht möglich da Vehilce null ist!", "Error");
            }
            catch (Exception ex)
            {
                Logger.Log($"{DateTime.UtcNow}: Method {nameof(ControlLoko_Click)} throw an exception! \nMessage: {ex.Message}\nIE: {ex.InnerException}\nIE Message: {ex.InnerException.Message}", LoggerType.Error);
                MessageBox.Show($"Beim öffnen ist ein Fehler aufgetreten! Fehlermeldung: {ex.Message}", "Error beim öffnen");
            }

        }

        void EditLoko_Click(Object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Does not work yet! Sorry!");
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbSearch.Text))
                DrawAllVehicles(db.Vehicles.Include(e => e.Category).Where(i => (i.Address + i.Article_Number + i.Category.Name + i.Owner + i.Railway + i.Description + i.Full_Name + i.Name + i.Type).ToLower().Contains(tbSearch.Text.ToLower())).OrderBy(e => e.Position));
            else
                DrawAllVehicles(db.Vehicles.Include(e => e.Category).OrderBy(e => e.Position));
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void Mw_Closing(object sender, CancelEventArgs e)
        {
            if (Controler is not null)
            {
                Controler.LogOFF();
                Controler = null;
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e) => new SettingsWindow().Show();

    }
}
