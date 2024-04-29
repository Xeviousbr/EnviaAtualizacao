using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

// Enviar o executável e a versão para o FTP
// Atualizar o Progress
// Encerrar o programa

namespace EnviaAtualizacao
{
    public partial class Form1 : Form
    {
        private string vVersao = "";
        private string Pasta = "";
        private string Arq = "";

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
            Arq = cINI.ReadString("EnviaAtualizacao", "Arq", "");
            string Programa = Path.Combine(Pasta, Arq);
            FileVersionInfo versaoInfo = FileVersionInfo.GetVersionInfo(Programa);
            vVersao = versaoInfo.ProductVersion.Substring(0, 5);
            this.Text = "Praparar versão " + vVersao;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string caminhoArquivo = Path.Combine(Pasta, "versao.txt");
            File.WriteAllText(caminhoArquivo, vVersao + Environment.NewLine);
            File.AppendAllText(caminhoArquivo, textBox1.Text.ToUpper());
            INI MeuIni = new INI();
            string host = MeuIni.ReadString("Config", "host", "");
            string user = MeuIni.ReadString("Config", "user", "");
            string pass = MeuIni.ReadString("Config", "pass", "");
            FTP cFPT = new FTP(host, user, pass);
            cFPT.setBarra(ref progressBar1);
            if (cFPT.Upload(Arq, Pasta))
            {
                cFPT.Upload("versao.txt", Pasta);
                MessageBox.Show("Atualização " + vVersao + " Enviada ao FTP", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            }
            else
            {
                MessageBox.Show("Erro no envio ao ftp.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
