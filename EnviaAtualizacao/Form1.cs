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
            string caminhoArquivoV = Path.Combine(Pasta, "versao.txt");
            File.WriteAllText(caminhoArquivoV, vVersao + Environment.NewLine);
            File.AppendAllText(caminhoArquivoV, textBox1.Text.ToUpper());
            if (txSql.Text.Length>0)
            {
                File.AppendAllText(caminhoArquivoV, txSql.Text);
            }
            INI MeuIni = new INI();
            string host = MeuIni.ReadString("Config", "host", "");
            string user = MeuIni.ReadString("Config", "user", "");
            string pass = MeuIni.ReadString("Config", "pass", "");
            FTP cFPT = new FTP(host, user, pass);
            cFPT.setBarra(ref progressBar1);
            string PastaBaseFTP = @"\\public_html\\public\\entregas\\";
            string caminhoArquivo = @"C:\Prog\T-Bonifacio\T-Bonifacio\bin\Release\TeleBonifacio.exe";
            if (cFPT.Upload(caminhoArquivo, PastaBaseFTP))
            {
                cFPT.Upload(caminhoArquivoV, PastaBaseFTP);
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
