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
                paginaAtual = new Pagina();
            }

            paginaAtual.tuplas[paginaAtual.qtd_tuplas_ocup++] = tupla;
        }

        if (paginaAtual.qtd_tuplas_ocup > 0)
            pags.Add(paginaAtual);
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
                paginaAtual = new Pagina();
            }
            paginaAtual.tuplas[paginaAtual.qtd_tuplas_ocup++] = nova;
        }
        if (paginaAtual.qtd_tuplas_ocup > 0)
            pags.Add(paginaAtual);
    }

    public int IndiceColuna(string nomeColuna)
    {
        // Assume que a primeira linha do CSV é o cabeçalho
        string cabecalho = File.ReadLines(caminhoArquivo).First();
        var colunas = cabecalho.Split(',');
        for (int i = 0; i < colunas.Length; i++)
            if (colunas[i].Trim().Equals(nomeColuna.Trim(), StringComparison.OrdinalIgnoreCase))
                return i;
        throw new Exception($"Coluna '{nomeColuna}' não encontrada no arquivo {caminhoArquivo}.");
    }

    public int NumPaginasLidas() => pags.Count;
}