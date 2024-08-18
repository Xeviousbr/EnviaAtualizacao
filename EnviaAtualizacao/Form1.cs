using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EnviaAtualizacao
{
    public partial class Form1 : Form
    {
        private INI cINI;
        private string vVersao = "";
        List<(string caminho, int versao)> arquivos = new List<(string, int)>();

        private void Form1_Load(object sender, EventArgs e)
        {
            cINI = new INI();
            int Qtd = cINI.ReadInt("ListaObservar", "Qtd", 0);
            for (int i = 0; i < Qtd; i++)
            {                
                string nmArq = "Arq" + (i+1).ToString();
                string caminhoArquivo = cINI.ReadString("ListaObservar", nmArq, "");
                string nomeDoArquivo = Path.GetFileNameWithoutExtension(caminhoArquivo);
                string Vers = "Ver" + nomeDoArquivo;
                int vIni = cINI.ReadInt("ListaObservar", Vers, 0);
                Int32 Versao = ObterVersaoArquivo(caminhoArquivo);
                if (Versao > vIni)
                {
                    label1.Text += nomeDoArquivo + Environment.NewLine;
                    arquivos.Add((caminhoArquivo, Versao));
                }
                if (i==0)
                {
                    vVersao = FormatarVersao(Versao);
                    this.Text = $"Versão {vVersao}";
                }
            }
            if (arquivos.Count==0)
            {
                textBox1.Enabled = false;
                MessageBox.Show("Arquivos atualizados", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string FormatarVersao(int versao)
        {
            int major = versao / 100;
            int minor = (versao / 10) % 10;
            int patch = versao % 10;
            return $"{major}.{minor}.{patch}";
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            button1.Enabled = (textBox1.Text.Length > 0);
        }

        private int ObterVersaoArquivo(string caminhoArquivo)
        {
            try
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(caminhoArquivo);
                Version version = new Version(versionInfo.FileVersion);

                // Calcula a versão como um número inteiro
                int versaoInt = (version.Major * 100) + (version.Minor * 10) + version.Build;

                return versaoInt;
            }
            catch (Exception ex)
            {
                // Trate a exceção conforme necessário (por exemplo, log do erro)
                Console.WriteLine($"Erro ao obter versão do arquivo: {ex.Message}");
                return 0; // Ou outro valor padrão
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Pasta = AppDomain.CurrentDomain.BaseDirectory;
            string caminhoArquivoV = Path.Combine(Pasta, "versao.txt");
            StringBuilder conteudo = new StringBuilder();
            conteudo.Append(vVersao);
            conteudo.Append(";");
            conteudo.Append(textBox1.Text.ToUpper());
            if (!string.IsNullOrEmpty(txSql.Text))
            {
                conteudo.Append(";");
                conteudo.Append(txSql.Text);
            }
            File.WriteAllText(caminhoArquivoV, conteudo.ToString());

            // Criar arquivo arquivos.txt
            string caminhoArquivosTxt = Path.Combine(Pasta, "arquivos.txt");
            StringBuilder conteudoArquivos = new StringBuilder();
            foreach (var (caminho, versao) in arquivos)
            {
                conteudoArquivos.Append(Path.GetFileName(caminho)).Append(";");
            }
            File.WriteAllText(caminhoArquivosTxt, conteudoArquivos.ToString().TrimEnd(';'));

            string caminhoCompletoArquivosTxt = Path.Combine(Pasta, caminhoArquivosTxt);
            string caminhoCompletoArquivoV = Path.Combine(Pasta, caminhoArquivoV);

            // Adicionar arquivos.txt e versao.txt à lista de arquivos
            arquivos.Add((caminhoCompletoArquivosTxt, 0));
            arquivos.Add((caminhoCompletoArquivoV, 0));

            string host = cINI.ReadString("Config", "host", "");
            string user = cINI.ReadString("Config", "user", "");
            string pass = cINI.ReadString("Config", "pass", "");
            FTP cFPT = new FTP(host, user, pass);
            cFPT.setBarra(ref progressBar1);
            string PastaBaseFTP = @"\\public_html\\public\\entregas\\";
            List<string> caminhosDosArquivos = arquivos.Select(a => a.caminho).ToList();
            if (cFPT.UploadMultiplo(caminhosDosArquivos, PastaBaseFTP))
            {
                for (int i = 0; i < arquivos.Count - 2; i++)
                {
                    var (caminho, versao) = arquivos[i];
                    string nomeArquivo = Path.GetFileNameWithoutExtension(caminho);
                    cINI.WriteInt("ListaObservar", "Ver" + nomeArquivo, versao);
                }
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

public class ArquivoInfo
{
    public string Caminho { get; set; }
    public int Versao { get; set; }

    public ArquivoInfo(string caminho, int versao)
    {
        Caminho = caminho;
        Versao = versao;
    }
}