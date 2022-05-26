using ProjektWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjektWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string GameUrl = "http://localhost:5000/Game/";
        private string gameSelected;
        private List<Game> games;
        private GameProcessor gp;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gp = new GameProcessor(GameUrl);
                games = await gp.GetAllGames();
                AllGamesBinding.ItemsSource = games;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void TextBlockMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TextBlock gameClicked = sender as TextBlock;
                string gameName = gameClicked.Text;

                Game game = games.FirstOrDefault(i => i.name == gameName);
                gameSelected = game.id;

                gp = new GameProcessor(GameUrl + game.id);
                Game g = await gp.GetGame();
                List<Game> list = new() { g };
                InformationBinding.ItemsSource = list;
                SetEditorTextBoxToEmpty();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gp = new GameProcessor(GameUrl);
                Game game = new()
                {
                    name = nameText.Text,
                    description = descriptionText.Text,
                    grade = int.Parse(gradeNumber.Text),
                    image = imageText.Text
                };
                await gp.AddGame(game);
                LoadButton_Click(sender, e);
                SetEditorTextBoxToEmpty();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gp = new GameProcessor(GameUrl + gameSelected);

                Game game = games.FirstOrDefault(i => i.id == gameSelected);

                //game.id = game.id;
                game.name = nameText.Text;
                game.description = descriptionText.Text;
                game.grade = int.Parse(gradeNumber.Text);
                game.image = imageText.Text;
                await gp.UpdateGame(game);
                LoadButton_Click(sender, e);
                SetEditorTextBoxToEmpty();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Game game = games.Where(i => i.id == gameSelected).FirstOrDefault();
                nameText.Text = game.name;
                descriptionText.Text = game.description;
                gradeNumber.Text = game.grade.ToString();
                imageText.Text = game.image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gp = new GameProcessor(GameUrl + gameSelected);
                await gp.DeleteGame();
                LoadButton_Click(sender , e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Makes sure that textbox is number
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SetEditorTextBoxToEmpty()
        {
            nameText.Text = "";
            descriptionText.Text = "";
            gradeNumber.Text = "";
            imageText.Text = "";
        }
    }
}
