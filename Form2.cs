using System;
using System.Windows.Forms;
namespace Игра_Составь_слова_из_слова
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1("Button1");
            form1.FormClosed += (s,args)=> this.Show();
            this.Hide();
            form1.Show();
        }
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Игра Составь слова из слова - это игра на эрудицию.\nИгрок должен из букв задананного большого слова составлять слова поменьше.\nНапример, дано слово САМОСВАЛ. Из его букв можно составить такие слова:\nВАЛ, СОМ, ЛАВА, ОСА и т.д.\nУзнавайте новые слова и прокачивайте свои мозги.\nПриятной Вам игры!", "Об игре");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Form form1 = new Form1("Button2");
            form1.FormClosed += (s,args)=>this.Show();
            this.Hide();
            form1.Show();
        }
    }

}
