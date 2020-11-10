using Microsoft.Win32;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Schema;

namespace GameInfoWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<GameInfo> Game = new ObservableCollection<GameInfo>();

        public MainWindow()
        {
            InitializeComponent();
            LoadGames();
        }

        public class GameInfo
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public int grade { get; set; }
            public string image { get; set; }
        }

        private async void btn_AddImage(object sender, RoutedEventArgs e)
        {
            if (GameIdImg_Text.Text == string.Empty)
            {
                MessageBox.Show("Enter game ID!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (GameImagePath_Text.Text == string.Empty)
            {
                MessageBox.Show("Chose a image!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                HttpClient client = new HttpClient();
                try
                {
                    using (var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        var image = await File.ReadAllBytesAsync(GameImagePath_Text.Text);
                        var shortImageName = System.IO.Path.GetFileName(GameImagePath_Text.Text);
                        content.Add(new StreamContent(new MemoryStream(image)), "postImage", shortImageName);
                        content.Add(new StringContent(GameIdImg_Text.Text), "Id");
                        using (var message = await client.PostAsync("http://localhost:5000/img/", content))
                        {
                            var input = await message.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("No connection to server!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.ImageHidden.Visibility = Visibility.Hidden;
                this.btn_SaveHidden.Visibility = Visibility.Hidden;
                LoadGames();
            }          
        }

        //Borde kunna göra samma sak som i delete för att uppdatera här
        private async void btn_GameUpdate(object sender, RoutedEventArgs e)
        {
            int Parse;
            bool Input = Int32.TryParse(GameId_Text.Text + GameGrade_Text.Text, out Parse);

            if (Input)
            {
                if (GameId_Text.Text == string.Empty || GameName_Text.Text == string.Empty || GameDesc_Text.Text == string.Empty || GameGrade_Text.Text == string.Empty)
                {
                    MessageBox.Show("Enter all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    GameInfo Game = new GameInfo()
                    {
                        id = int.Parse(GameId_Text.Text),
                        name = GameName_Text.Text,
                        description = GameDesc_Text.Text,
                        grade = int.Parse(GameGrade_Text.Text),
                        image = GameImage_Text.Text
                    };

                    string PostData = JsonSerializer.Serialize(Game);
                    HttpClient client = new HttpClient();
                    try
                    {
                        var SendData = new StringContent(PostData, Encoding.UTF8, "application/json");
                        var respons = await client.PutAsync("http://localhost:5000/GameInfo", SendData);
                    }
                    catch (HttpRequestException)
                    {
                        MessageBox.Show("No connection to server!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    this.UpdateHidden.Visibility = Visibility.Hidden;
                    this.btn_UpdateHidden.Visibility = Visibility.Hidden;
                    LoadGames();
                }
            }
            else
            {
                MessageBox.Show("Enter a valid Grade!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_ShowAddGame(object sender, RoutedEventArgs e)
        {
            this.AddHidden.Visibility = Visibility.Visible;
            this.btn_AddHidden.Visibility = Visibility.Visible;
            this.StackPanelHidden.Visibility = Visibility.Hidden;
            this.GameImage.Visibility = Visibility.Hidden;
        }

        private async void btn_GameAdd(object sender, RoutedEventArgs e)
        {
            int Parse;
            bool Input = Int32.TryParse(GameGrade.Text, out Parse);
            if (Input)
            {
                if (GameName.Text == string.Empty || GameDesc.Text == string.Empty || GameGrade.Text == string.Empty)
                {
                    MessageBox.Show("Enter all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string Grade = GameGrade.Text;
                    int value = int.Parse(Grade);

                    GameInfo Game = new GameInfo()
                    {
                        name = GameName.Text,
                        description = GameDesc.Text,
                        grade = value,
                    };

                    string PostData = JsonSerializer.Serialize(Game);

                    HttpClient client = new HttpClient();
                    try
                    {
                        var SendData = new StringContent(PostData, Encoding.UTF8, "application/json");
                        var respons = await client.PostAsync("http://localhost:5000/GameInfo", SendData);
                    }
                    catch (HttpRequestException)
                    {
                        MessageBox.Show("No connection to server!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    this.AddHidden.Visibility = Visibility.Hidden;
                    this.btn_AddHidden.Visibility = Visibility.Hidden;
                    LoadGames();
                }
            }
            else
            {
                MessageBox.Show("Enter a valid grade!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            HttpClient client = new HttpClient();
            try
            {
                var response = await client.DeleteAsync("http://localhost:5000/GameInfo/" + TextHolder_Id.Text);
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("No connection to server!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LoadGames();
            this.StackPanelHidden.Visibility = Visibility.Hidden;
            this.GameImage.Visibility = Visibility.Hidden;
        }

        private void btn_Game_ImageAdd(object sender, RoutedEventArgs e)
        {

            this.StackPanelHidden.Visibility = Visibility.Hidden;
            this.GameImage.Visibility = Visibility.Hidden;
            this.btn_SaveHidden.Visibility = Visibility.Visible;

            GameIdImg_Text.Text = TextHolder_Id.Text;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;)|*.png;*.jpg;";

            if (openFileDialog.ShowDialog() == true)
            {
                GameImagePath_Text.Text = openFileDialog.FileName;
            }
        }

        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateHidden.Visibility = Visibility.Visible;
            this.btn_UpdateHidden.Visibility = Visibility.Visible;
            this.StackPanelHidden.Visibility = Visibility.Hidden;
            this.GameImage.Visibility = Visibility.Hidden;

            GameDesc_Text.Text = TextHolder_Desc.Text;
            GameGrade_Text.Text = TextHolder_Grade.Text;
            GameName_Text.Text = TextHolder_Name.Text;
            GameId_Text.Text = TextHolder_Id.Text;
            GameImage_Text.Text = TextHolder_Img.Text;
        }

        private async void LoadGames()
        {
            Game.Clear();
            HttpClient client = new HttpClient();
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var respons = await client.GetAsync("http://localhost:5000/GameInfo");
                var JsonData = await respons.Content.ReadAsStringAsync();

                var GetData = JsonSerializer.Deserialize<ObservableCollection<GameInfo>>(JsonData);

                foreach (var x in GetData)
                {
                    Game.Add(new GameInfo { id = x.id, name = x.name, description = x.description, grade = x.grade, image = x.image });
                }

                ListBox.ItemsSource = Game;
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("No connection to server!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void ListBox_MouseClick(object sender, MouseButtonEventArgs e)
        {
            
            var item = (ListBox)sender;
            var Info = (GameInfo)item.SelectedItem;

            if (Info == null)
            {
                MessageBox.Show("No game targeted!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.StackPanelHidden.Visibility = Visibility.Hidden;
            }
            else
            {                          
                TextHolder_Name.Text = Info.name;
                TextHolder_Desc.Text = Info.description;
                TextHolder_Id.Text = Info.id.ToString();
                TextHolder_Grade.Text = Info.grade.ToString();
                TextHolder_Img.Text = Info.image;

                this.GameImage.Visibility = Visibility.Visible;

                if (Info.image == null || Info.image == string.Empty)
                {
                    TextHolder_Name.Text = Info.name;
                    TextHolder_Desc.Text = Info.description;
                    TextHolder_Id.Text = Info.id.ToString();
                    TextHolder_Grade.Text = Info.grade.ToString();
                

                    this.GameImage.Visibility = Visibility.Hidden;
                }
                else
                {
                    //https://stackoverflow.com/questions/5346727/convert-memory-stream-to-bitmapimage
                    HttpClient client = new HttpClient();
                    try
                    {
                        using (var respons = await client.GetAsync("http://localhost:5000/img/" + Info.id.ToString()))
                        {
                            var ImgData = await respons.Content.ReadAsByteArrayAsync();
                            using (var stream = new MemoryStream(ImgData))
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.StreamSource = stream;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                                GameImage.Source = bitmap;
                            }
                        }
                    }
                    catch (HttpRequestException)
                    {
                        MessageBox.Show("No connection to server!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            this.StackPanelHidden.Visibility = Visibility.Visible;
            this.UpdateHidden.Visibility = Visibility.Hidden;
            this.AddHidden.Visibility = Visibility.Hidden;
            this.UpdateHidden.Visibility = Visibility.Hidden;
            this.ImageHidden.Visibility = Visibility.Hidden;
            this.btn_AddHidden.Visibility = Visibility.Hidden;
            this.btn_UpdateHidden.Visibility = Visibility.Hidden;
            this.btn_SaveHidden.Visibility = Visibility.Hidden;
        }

        //https://www.wpf-tutorial.com/listview-control/listview-filtering/
        private void SearchGame_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListBox.UnselectAll();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListBox.ItemsSource);
            view.Filter = GameFilter;
        }

        private bool GameFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchGame_Text.Text))
            {
                return true;
            }
            else
            {
                return ((item as GameInfo).name.IndexOf(SearchGame_Text.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }
        }

        private void txtFilter_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListBox.ItemsSource).Refresh();
        }
    }
}

