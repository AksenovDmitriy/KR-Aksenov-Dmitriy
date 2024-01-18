using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace Игра_Составь_слова_из_слова
{
    public partial class Form1 : Form
    {
        private GameData gameData;
        private Random random = new Random();
        private List<int> usedIndices = new List<int>();
        private string buttonName;
        public Form1(string buttonName)
        {
            this.buttonName = buttonName;
            InitializeComponent();
            gameData = new GameData();
            ReadRussianWords();
            if (buttonName == "Button1") // Если была нажата кнопка button1
            {
                FilterCandidateWords(); // То создаётся список больших слов, подобранных по специальным критериям
                int newIndex = GetNewRandomIndex(); // Создается случайный неповторяющийся индекс
                label3.Text = gameData.candidateWords[newIndex];  // Выбирается слово с данным индексом
            }
            if (buttonName == "Button2") // Если была нажата кнопка button2, то
            {
                CreateForm1(); // Открывается новая форма
            }
            UpdateData();
        }
        public class GameData // Класс хранит в себе данные игры:
        {
            public string bigWord { get; set; } // Большое слово, из букв которого составляются другие слова
            public string buttonName { get; set; } // Название нажатой кнопки из другой формы
            public List<string> russianWords { get; set; } // Список всех слов из файла russian_nouns.txt
            public List<string> candidateWords { get; set; } // Список слов, которые могут быть использованы как большое слово (Если выбран режим "Случайное слово")
            public int WordstoWinCount { get; set; } // Количество слов, достаточных для победы
        }
        public void ReadRussianWords() // Метод считывания слов из файла
        {
            gameData.russianWords = new List<string>(); // Создаётся список слов
            try
            {
                using (StreamReader sr = new StreamReader("russian_nouns.txt")) 
                {
                    string line; 
                    while ((line = sr.ReadLine()) != null) // Пока строки непустые,
                    {
                        gameData.russianWords.Add(line); // В список добавляется строка
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка чтения файла: " + ex.Message);
            }
        }
        private void FilterCandidateWords()
        {
            gameData.candidateWords = gameData.russianWords.Where(w => w.Length > 8 && GetPossibleWordsCount(w) > 20).ToList(); // Список больших слов, которые состоят из более чем 8 букв и из которых можно составить более 20 слов
        }
        private List<string> GetPossibleWords(string word, List<string> words)
        {
            return words.Where(w => IsWordPossible(w, word)).ToList(); // Возвращает список слов, которые можно составить из данного большого слова
        }
        private bool IsWordPossible(string word, string bigWord) // Данный метод проверяет, можно ли из символов строки `bigWord` составить строку `word`, используя каждый символ `bigWord` только один раз, и возвращает `true`, если это возможно, и `false`, если нет
        {
            string tempBigWord = bigWord; // Создается временное большое слово
            foreach (char c in word) // Для всех букв в маленьком слове
            {
                int index = tempBigWord.IndexOf(c); // Находится эта буква в большом слове
                if (index == -1) // Если буквы нет, то
                {
                    return false; // Из большого слова tempBigWord невозможно составить word
                }
                tempBigWord = tempBigWord.Remove(index, 1); // Удаляется найденная буква, что гарантирует вхождение только одного символа
            }
            return true; // Успешно использованы все символы большого слова, чтобы составить маленькое слово
        }
        private int GetPossibleWordsCount(string word)
        {
            return GetPossibleWords(word, gameData.russianWords).Count - 1; // Возвращается количество слов, которые можно составить из данного большого слова, исключая само большое слово
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (label1.Text == "Вы можете добавить это слово" && label1.Visible == true) // Если слово можно составить
            {
                string inputWord = textBox1.Text; // В строку записываются данные из текстового поля
                AddWordToFlowLayoutPanel(inputWord); // Эта строка записывается в специальном поле
                textBox1.Text = ""; // Текстовое поле очищается
                label6.Text = "Всего слов составлено: " + flowLayoutPanel1.Controls.Count; // Обновляется счетчик слов в поле
            }
            if (flowLayoutPanel1.Controls.Count == gameData.WordstoWinCount) // Если составлено достаточно слов
            {
                MessageBox.Show("Вы победили!\nТеперь Вы можете начать новую игру", "Поздравляем");
                button2.Visible = true; 
                button2.Enabled = true;
                // Появится кнопка, позволяющая начать новую игру
            }
            if (flowLayoutPanel1.Controls.Count == GetPossibleWordsCount(gameData.bigWord)) // Если отгаданы все возможные слова
            {
                MessageBox.Show("Вы ЛЕГЕНДА\nВы смогли составить ВСЕ слова", "Мои поздравления!");
                button3.Enabled = false; // Кнопка подсказки деактивируется (а зачем ей быть активной, если игра полностью пройдена?)
            }
        }
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) // Если была нажата клавиша Enter, то происходит
            {
                button1.PerformClick(); // Вызов события кнопки button1, как если бы пользователь щелкнул по кнопке
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string inputWord = textBox1.Text; // В строку записываются данные с текстового поля
            {
                if (GetPossibleWords(gameData.bigWord, gameData.russianWords).Contains(inputWord) && inputWord != gameData.bigWord && !ContainsWord(inputWord)) // Если слово действительно составляемое из букв большого слова, если слово не является самим большим словом и если такого слова еще нет в списке, то
                {
                    label1.Visible = true; // Появляется надпись "Вы можете добавить это слово"
                    button1.Enabled = true; // Появляется кнопка добавления слова в специальное поле
                }
                else
                {
                    label1.Visible = false;
                    button1.Enabled = false;
                }
            }
        }
        private bool ContainsWord(string word)
        {
            foreach (Label label in flowLayoutPanel1.Controls) // Для всех элементов коллекции
            {
                if (label.Text == word) // Если метка является словом
                {
                    return true; // Возвращается "Правда"
                }
            }
            return false; // Иначе возвращается "Ложь"
        }
        private SortFunction selectedSortFunction; 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSortType = comboBox1.SelectedItem.ToString();
            switch (selectedSortType)
            {
                case "В алфавитном порядке":
                    selectedSortFunction = SortWordsAlphabetically;
                    break;
                case "По длине слова":
                    selectedSortFunction = SortWordsByLength;
                    break;
                default:
                    break;
            }
        }
        private void AddWordToFlowLayoutPanel(string word) // Добавление слова в специальное поле
        {
            Label label = new Label
            {
                Text = word,
                AutoSize = true
            };
            flowLayoutPanel1.Controls.Add(label);
            selectedSortFunction?.Invoke(flowLayoutPanel1); // Поле динамически сортируется с каждым новым введённым словом
        }
        public delegate void SortFunction(FlowLayoutPanel flowLayoutPanel); // Используется для передачи методов в качестве аргументов к другим методам.
        public void SortWordsAlphabetically(FlowLayoutPanel flowLayoutPanel) // Сортировка по алфавиту
        {
            List<Label> labels = flowLayoutPanel.Controls.OfType<Label>().ToList();
            var sortedLabels = labels.OrderBy(l => l.Text).ToList();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.AddRange(sortedLabels.ToArray());
        }
        public void SortWordsByLength(FlowLayoutPanel flowLayoutPanel) // Сортировка по длине слова
        {
            List<Label> labels = flowLayoutPanel.Controls.OfType<Label>().ToList();
            var sortedLabels = labels.OrderBy(l => l.Text.Length).ToList();
            flowLayoutPanel.Controls.Clear();
            flowLayoutPanel.Controls.AddRange(sortedLabels.ToArray());
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (buttonName == "Button1") // Если был выбран режим "Случайное слово"
            {
                ClearForm();
                if (gameData.candidateWords.Count > 0)
                {
                    int newIndex = GetNewRandomIndex();
                    label3.Text = gameData.candidateWords[newIndex]; // Выведется еще не отгаданное слово
                }
                UpdateData();
                button2.Visible = false;
                button2.Enabled = false;
                button3.Enabled = true;
            }
            if (buttonName == "Button2") // Если был выбран режим "Своё слово"
            {
                ClearForm();
                CreateForm1(); // Появится форма для ввода следующего
                UpdateData();
                button2.Visible = false;
                button2.Enabled = false;
                button3.Enabled = true;
            }
        }
        private int GetNewRandomIndex()
        {
            if (usedIndices.Count == gameData.candidateWords.Count)
            {
                usedIndices.Clear(); // Если все слова были использованы, очистить список использованных индексов
            }

            int newIndex = random.Next(0, gameData.candidateWords.Count);
            while (usedIndices.Contains(newIndex))
            {
                newIndex = random.Next(0, gameData.candidateWords.Count); // выбрать новый случайный индекс, пока не найдется неиспользованный
            }
            usedIndices.Add(newIndex); // Добавить новый индекс в список использованных
            return newIndex;
        }
        private void ClearForm()
        {
            label3.Text = string.Empty;  // Обнуление текста метки
            gameData.WordstoWinCount = 0; // Обнуление счетчика
            textBox1.Text = string.Empty; // Очистка поля для ввода
                                          // Очистка FlowLayoutPanel1
            while (flowLayoutPanel1.Controls.Count > 0) // Пока есть хотя бы один элемент коллекции
            {
                Control ctrl = flowLayoutPanel1.Controls[0]; // Выбирается первый элемент коллекции
                flowLayoutPanel1.Controls.Remove(ctrl); // И удаляется
                ctrl.Dispose(); // При этом освобождается память
            }
        }
        private void CreateForm1() // Создание формы написания собственного слова
        {
            Form form = new Form();
            form.Text = "Введите своё слово";
            form.Size = new Size(300, 200);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ControlBox = false;
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
            textBox.Location = new Point(80, 60);
            textBox.Size = new Size(140, 50);
            System.Windows.Forms.Button submitButton = new System.Windows.Forms.Button();
            submitButton.Location = new Point(100, 100);
            submitButton.Size = new Size(100, 30);
            submitButton.Text = "Начать игру";
            submitButton.Click += (s, evt) =>
            {
                label3.Text = textBox.Text.ToLower(); // Как бы игрок не написал слово, оно будет полностью состоять из букв в нижнем регистре
                if (label3.Text.Any(char.IsWhiteSpace))
                {
                    MessageBox.Show("Напишите другое слово", "Игра не начнётся");
                }
                else if (GetPossibleWordsCount(label3.Text) < 5) // Если из этого слова вообще ничего нельзя составить или имеется пробел
                {
                    MessageBox.Show("Напишите другое слово", "Игра не начнётся"); // То игра не начнётся
                }
                else
                form.Close();
            };
            form.Controls.Add(textBox);
            form.Controls.Add(submitButton);
            form.ShowDialog();
        }
        private void UpdateData() // Обновление данных
        {
            gameData.bigWord = label3.Text; // Большое слово становится такое, как показано в метке Label3
            label4.Text = "Всего слов можно составить: " + GetPossibleWordsCount(gameData.bigWord); // Для данного большого слова пишется количество всевозможных слов, которые можно составить
            gameData.WordstoWinCount = (int)Math.Round(GetPossibleWordsCount(gameData.bigWord) - (0.8 * GetPossibleWordsCount(gameData.bigWord))); // Для данного большого слова высчитывется количество необходимых для победы слов
            label5.Text = "Для победы достаточно составить: " + gameData.WordstoWinCount; // Вывод результатов подсчёта
            label6.Text = "Всего слов составлено: " + flowLayoutPanel1.Controls.Count; // Обновление количества слов в специальном поле
        }
        private void button3_Click_1(object sender, EventArgs e) // Кнопка подсказки
        {
            // Генерация двух случайных целых чисел A и B
            Random random = new Random();
            int A = random.Next(1, 100);
            int B = random.Next(1, 100);

            // Генерация случайной операции (+, -, * или /)
            string[] operations = { "+", "-" };
            string operation = operations[random.Next(0, 2)];

            // Формирование математической задачи
            string message = "Решите задачу: " + A + operation + B;

            // Создание формы для ввода ответа
            System.Windows.Forms.Form inputForm = new System.Windows.Forms.Form();
            inputForm.Text = "Помощь";
            inputForm.Size = new System.Drawing.Size(250, 150);
            inputForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Text = message;
            label.Location = new System.Drawing.Point(20, 20);
            label.AutoSize = true;
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox();
            textBox.Location = new System.Drawing.Point(20, 50);
            System.Windows.Forms.Button submitButton = new System.Windows.Forms.Button();
            submitButton.Text = "Ответить";
            submitButton.Location = new System.Drawing.Point(20, 80);
            submitButton.Click += (object sender2, EventArgs e2) =>
            {
                if (int.TryParse(textBox.Text, out int userAnswer)) // Если введено ничего или введено что-либо, кроме цифр,
                {
                    int correctAnswer = 0;
                    switch (operation)
                    {
                        case "+":
                            correctAnswer = A + B;
                            break;
                        case "-":
                            correctAnswer = A - B;
                            break;
                    }
                    if (userAnswer == correctAnswer) // Если задачка решена правильно, то
                    {
                        string addword;
                        do
                        {
                            addword = GetRandomWord(); // Выбирается случайное слово
                            if (GetPossibleWords(gameData.bigWord, gameData.russianWords).Contains(addword) && addword != gameData.bigWord && !ContainsWord(addword)) // Если его еще не отгадали
                            {
                                break;
                            }
                        }
                        while (true);
                        MessageBox.Show("Добавится слово: " + addword, "Правильно"); // То это слово появится сначала в отдельном окне
                        AddWordToFlowLayoutPanel(addword); // А после и в специальном поле
                        label6.Text = "Всего слов составлено: " + flowLayoutPanel1.Controls.Count; // Обновятся данные
                        if (flowLayoutPanel1.Controls.Count == gameData.WordstoWinCount) // Если случилось так, что подсказочное слово было последним, необходимым для победы
                        {
                            MessageBox.Show("Вы победили!\nТеперь Вы можете начать новую игру", "Поздравляем");
                            button2.Visible = true;
                            button2.Enabled = true;
                            // То даём возможность начать новую игру
                        }
                        if (flowLayoutPanel1.Controls.Count == GetPossibleWordsCount(gameData.bigWord)) // Если данное слово было в принципе последним из всевозможных
                        {
                            MessageBox.Show("Вы ЛЕГЕНДА\nВы смогли составить ВСЕ слова", "Мои поздравления!");
                            button3.Enabled = false; // То кнопка подсказки больше не станет активной
                        }
                    }
                    else
                    {
                        MessageBox.Show("Правильный ответ: " + correctAnswer, "Неправильно"); // Иначе в отдельном окне появится правильный ответ и помощи не будет
                    }
                    inputForm.Close();
                }
                else
                {
                    MessageBox.Show("Введите корректный ответ"); // То программа попросит заново ввести ответ
                }
            };
            inputForm.Controls.Add(label);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(submitButton);
            inputForm.ShowDialog();
        }
        private string GetRandomWord()
        {
            Random rand = new Random();
            int index = rand.Next(GetPossibleWords(gameData.bigWord, gameData.russianWords).Count);
            string word = GetPossibleWords(gameData.bigWord, gameData.russianWords)[index];
            return word;
        }
    }
}