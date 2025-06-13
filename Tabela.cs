using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Tabela
{
    public List<Pagina> pags = new();
    public int qtd_pags => pags.Count;
    public int qtd_cols;
    public string caminhoArquivo;

    // Variáveis para simular I/O
    private int leituras = 0;
    private int escritas = 0;

    public Tabela(string caminhoArquivo)
    {
        this.caminhoArquivo = caminhoArquivo;
    }

    public void CarregarDados()
    {
        string[] linhas = File.ReadAllLines(caminhoArquivo);
        int colunas = linhas[0].Split(',').Length;
        qtd_cols = colunas;

        Pagina paginaAtual = new();
        foreach (string linha in linhas)
        {
            string[] dados = linha.Split(',');

            Tupla tupla = new(dados);

            if (paginaAtual.qtd_tuplas_ocup == 10)
            {
                pags.Add(paginaAtual);
                leituras++; // Simula leitura de página
                paginaAtual = new Pagina();
            }

            paginaAtual.tuplas[paginaAtual.qtd_tuplas_ocup++] = tupla;
        }

        if (paginaAtual.qtd_tuplas_ocup > 0)
        {
            pags.Add(paginaAtual);
            leituras++; // Última página
        }
    }

    public List<string[]> GetTodasTuplasOrdenadas()
    {
        List<string[]> todas = new();
        foreach (var pagina in pags)
        {
            for (int i = 0; i < pagina.qtd_tuplas_ocup; i++)
                todas.Add(pagina.tuplas[i].cols);
        }
        return todas;
    }

    public void OrdenarPor(string nomeColuna)
    {
        int idx = IndiceColuna(nomeColuna);
        var todasTuplas = GetTodasTuplasOrdenadas();
        todasTuplas.Sort((a, b) => string.Compare(a[idx], b[idx]));

        // Recria as páginas ordenadas
        pags.Clear();
        Pagina paginaAtual = new();
        foreach (var cols in todasTuplas)
        {
            Tupla nova = new(cols);
            if (paginaAtual.qtd_tuplas_ocup == 10)
            {
                pags.Add(paginaAtual);
                escritas++; // Simula escrita de página ordenada
                paginaAtual = new Pagina();
            }
            paginaAtual.tuplas[paginaAtual.qtd_tuplas_ocup++] = nova;
        }

        if (paginaAtual.qtd_tuplas_ocup > 0)
        {
            pags.Add(paginaAtual);
            escritas++; // Última página ordenada
        }
    }

    public int IndiceColuna(string nomeColuna)
    {
        string cabecalho = File.ReadLines(caminhoArquivo).First();
        var colunas = cabecalho.Split(',');
        for (int i = 0; i < colunas.Length; i++)
            if (colunas[i].Trim().Equals(nomeColuna.Trim(), StringComparison.OrdinalIgnoreCase))
                return i;

        throw new Exception($"Coluna '{nomeColuna}' não encontrada no arquivo {caminhoArquivo}.");
    }

    // Métricas
    public int NumPaginasLidas() => pags.Count;
    public int NumLeituras() => leituras;
    public int NumEscritas() => escritas;
    public int TotalIOs() => leituras + escritas;
}
