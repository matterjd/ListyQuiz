using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListyQuiz
{
    class Question
    {
        public string Text { get; set; }
        public int ID { get; set; }
        public string Chars { get; set; } // comma separated list (ie, 'A, L, G, W,')
        public int Points { get; set; }
        public bool Enabled { get; set; }
        
    }

    class Character
    {
        public string Name { get; set; }
        public int TotalPoints { get; set; }        
    }

    class ViewModel {
        public string[] chars = new string[4] { "G", "L", "A", "W" };
        public List<Question> Questions { get; set; }
        public List<Character> Characters { get; set; }
        public string Header { get { return "Which Lord of the Rings Character are you?"; } }
        public bool IsVisible { get; set; }

        //TODO: Add point calculation and addition logic
        
    }
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private List<Question> questions;
        private List<Character> characters;
        private ViewModel viewModel;
        private string[] chars;
        private int numClicks = 0;
        public MainPage()
        {
            InitializeComponent();
            InitializeEnvironment();
           
        }

        void InitializeEnvironment()
        {
            questions = new List<Question> {
                new Question {Text = "Do you like to drink 'til you can't stop?", Chars = "G", Points = 1, ID = 1, Enabled = true},
                new Question {Text = "Do you wish you could live over 100?", Chars = "L", Points = 2, ID = 2, Enabled = true},
                new Question {Text = "Do you have an long ancestry tree in your possession?", Chars = "A", Points = 6, ID = 6, Enabled = true},
                new Question {Text = "Are you a wizard?", Chars = "W", Points = 4, ID = 8, Enabled = true},
                new Question {Text = "Are you and your best friend too close?", Chars = "GL", Points = 5, ID = 4, Enabled = true},
                new Question {Text = "Do your friends consider you a leader?", Chars = "AW", Points = 7, ID = 7, Enabled = true},
                new Question {Text = "Do you like taking the long way around?", Chars = "ALG", Points = 6, ID = 5, Enabled = true},
                new Question {Text = "Are you considered taller than average?", Chars = "WAL", Points = 6, ID = 3, Enabled = true},
                new Question {Text = "Do you like to hangout with friends?", Chars = "GLAW", Points = 3, ID = 9, Enabled = true},
            };

            characters = new List<Character>
            {
                new Character {Name = "gimli", TotalPoints = 0},
                new Character {Name = "aragorn", TotalPoints = 0},
                new Character {Name = "legolas", TotalPoints = 0},
                new Character {Name = "gandalf", TotalPoints = 0},
            };

            viewModel = new ViewModel
            {
                Questions = questions,
                Characters = characters,
                IsVisible = true,
            };
            BindingContext = viewModel;
            chars = viewModel.chars;
            this.gimli.IsVisible = false;
            this.legolas.IsVisible = false;
            this.aragorn.IsVisible = false;
            this.gandalf.IsVisible = false;
            this.images.IsVisible = false;
            this.results.IsVisible = false;
            this.btnCalc.IsEnabled = false;
        }

        void OnTrue (object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            
            //TODO: Assign Points
            int ID = int.Parse(item.CommandParameter.ToString());
            int points = 0;
            string codes = "";
            int index = 0;
            bool isEnabled = true;
            foreach (Question q in questions)
            {
                if (q.ID == ID)
                {
                    index = questions.IndexOf(q);
                    codes = questions[index].Chars;
                    points = questions[index].Points;
                    questions[index].Enabled = false;
                    isEnabled = false;
                    break;
                }
            }
            AddPoints(codes, points);

            CheckItems();
            if (!isEnabled)
            {
                questions.RemoveAt(index);
                qList.ItemsSource = null;
                qList.ItemsSource = questions;
            }

            //DisplayAlert("Item True", "You have " + totalPoints.ToString(), "Ok");
        }

        void OnFalse (object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            //TODO: Add one point to all other codes
            int ID = int.Parse(item.CommandParameter.ToString());
            string codes = "";
            int index = 0;
            bool isEnabled = true;
            foreach (Question q in questions)
            {
                if (q.ID == ID)
                {
                    index = questions.IndexOf(q);
                    codes = questions[index].Chars;
                    questions[index].Enabled = false;
                    isEnabled = false;
                    break;
                }                    
            }
            int points = 1;
            
            foreach(string code in chars)
            {
                if (!codes.Contains(code))
                {
                    AddPoints(code, points);
                }
            }

            CheckItems();
            if (!isEnabled)
            {
                questions.RemoveAt(index);
                qList.ItemsSource = null;
                qList.ItemsSource = questions;
            }

        }

        async void OnCalculate(object sender, EventArgs e)
        {
            numClicks++;
            if (numClicks == 1)
            {
                string code = Calculate();
                viewModel.IsVisible = false;
                images.IsVisible = true;
                switch (code)
                {
                    case "G":
                        this.gimli.IsVisible = !viewModel.IsVisible;
                        results.Text = "You are Gimli!";
                        break;
                    case "L":
                        this.legolas.IsVisible = !viewModel.IsVisible;
                        results.Text = "You are Legolas!";
                        break;
                    case "A":
                        this.aragorn.IsVisible = !viewModel.IsVisible;
                        results.Text = "You are Aragorn!";
                        break;
                    case "W":
                        this.gandalf.IsVisible = !viewModel.IsVisible;
                        results.Text = "You are Gandalf!";
                        break;
                    default:
                        results.Text = "You are no-one";
                        break;
                }
                results.IsVisible = true;
                btnCalc.Text = "Try again?";
            }
            else
            {
                numClicks = 0;
                InitializeEnvironment();
            }
        }

        public void CheckItems()
        {
            foreach (Question q in questions)
            {
                if (q.Enabled)
                    return;
            }
            btnCalc.IsEnabled = true;
        }

        public void AddPoints(string codes, int points)
        {
            foreach (Character c in characters)
            {
                if (codes.Contains("G") && c.Name.Equals("gimli"))
                    c.TotalPoints += points;
                if (codes.Contains("L") && c.Name.Equals("legolas"))
                    c.TotalPoints += points;
                if (codes.Contains("A") && c.Name.Equals("aragorn"))
                    c.TotalPoints += points;
                if (codes.Contains("W") && c.Name.Equals("gandalf"))
                    c.TotalPoints += points;
            }
            
        }
        public string Calculate()
        {
            string code = "";
            int points = 0;
            foreach(Character c in characters)
            {
                if (c.TotalPoints > points)
                {
                    points = c.TotalPoints;
                    switch (c.Name)
                    {
                        case "gimli":
                            code = "G";
                            break;
                        case "legolas":
                            code = "L";
                            break;
                        case "aragorn":
                            code = "A";
                            break;
                        case "gandalf":
                            code = "W";
                            break;
                    }
                }
            }

            return code;
        }
    }
}
