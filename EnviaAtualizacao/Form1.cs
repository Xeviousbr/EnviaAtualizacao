using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

// Escrever um arquivo versao.txt com a versão e o texto
// Ler no INI o caminho a ser enviado
// Enviar o executável e a versão para o FTP
// Atualizar o Progress
// Encerrar o programa

namespace EnviaAtualizacao
{
    public partial class Form1 : Form
    {
        private string vVersao = "";
        private string Pasta = "";

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
            Pasta = cINI.ReadString("EnviaAtualizacao", "Pasta", "");
            string Arq = cINI.ReadString("EnviaAtualizacao", "Arq", "");
            string Programa = Path.Combine(Pasta, Arq);
            FileVersionInfo versaoInfo = FileVersionInfo.GetVersionInfo(Programa);
            vVersao = versaoInfo.ProductVersion.Substring(0, 5);
            this.Text = "Praparar versão " + vVersao;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string caminhoArquivo = Path.Combine(Pasta, "versao.txt");
            try
            {
                File.WriteAllText(caminhoArquivo, vVersao + Environment.NewLine);
                File.AppendAllText(caminhoArquivo, textBox1.Text.ToUpper());
                MessageBox.Show("Arquivo versao.txt criado com sucesso.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao criar o arquivo versao.txt: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
