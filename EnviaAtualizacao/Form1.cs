using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Ler INI para ver onde esta o arquivo a ser enviado
// Ler a versão do executável
// Escrever um arquivo versao.txt com a versão e o texto
// Ler no INI o caminho a ser enviado
// Enviar o executável e a versão para o FTP
// Atualizar o Progress
// Encerrar o programa

namespace EnviaAtualizacao
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            button1.Enabled = (textBox1.Text.Length > 0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            INI cINI = new INI();
            string Pasta = cINI.ReadString("EnviaAtualizacao", "Pasta", "");
            int x = 0;
        }
    }
}
