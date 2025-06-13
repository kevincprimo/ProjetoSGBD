using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Operador
{
    private Tabela tabela1;
    private Tabela tabela2;
    private string col1;
    private string col2;

    private List<Pagina> paginasResultado = new(); // Agora usamos páginas como na estrutura de tabela
    private int numIOs = 0;
    private int numPagsGeradas = 0;
    private int numTuplas = 0;

    public Operador(Tabela t1, Tabela t2, string c1, string c2)
    {
        tabela1 = t1;
        tabela2 = t2;
        col1 = c1;
        col2 = c2;
    }

    public void Executar()
    {
        // 🔍 ETAPA 1: Ordenação externa simulada
        tabela1.OrdenarPor(col1);
        tabela2.OrdenarPor(col2);

        // 🔎 ETAPA 2: Coleta das tuplas ordenadas (simula buffer de entrada)
        var tuplas1 = tabela1.GetTodasTuplasOrdenadas();
        var tuplas2 = tabela2.GetTodasTuplasOrdenadas();

        int idx1 = tabela1.IndiceColuna(col1);
        int idx2 = tabela2.IndiceColuna(col2);

        int i = 0, j = 0;
        Pagina paginaAtual = new();

        // 🔄 ETAPA 3: Algoritmo de Sort-Merge Join
        while (i < tuplas1.Count && j < tuplas2.Count)
        {
            var t1 = tuplas1[i];
            var t2 = tuplas2[j];

            int cmp = string.Compare(t1[idx1], t2[idx2]);

            if (cmp == 0)
            {
                int tempJ = j;
                while (tempJ < tuplas2.Count && t2[idx2] == tuplas2[tempJ][idx2])
                {
                    // 🧬 Criação da tupla de junção
                    var nova = new Tupla(t1.Concat(tuplas2[tempJ]).ToArray());
                    paginaAtual.tuplas[paginaAtual.qtd_tuplas_ocup++] = nova;
                    numTuplas++;

                    // 📤 Quando enche uma página, salva e inicia nova
                    if (paginaAtual.qtd_tuplas_ocup == 10)
                    {
                        paginasResultado.Add(paginaAtual);
                        numIOs++; // conta escrita
                        paginaAtual = new Pagina();
                    }

                    tempJ++;
                }
                i++;
            }
            else if (cmp < 0)
            {
                i++;
            }
            else
            {
                j++;
            }
        }

        // 💾 Salva a última página se tiver tuplas
        if (paginaAtual.qtd_tuplas_ocup > 0)
        {
            paginasResultado.Add(paginaAtual);
            numIOs++; // conta escrita da última página
        }

        // 📊 Atualiza métricas de desempenho
        numIOs += tabela1.NumPaginasLidas(); // leituras da tabela1
        numIOs += tabela2.NumPaginasLidas(); // leituras da tabela2
        numPagsGeradas = paginasResultado.Count;
    }

    public int NumIOExecutados() => numIOs;
    public int NumPagsGeradas() => numPagsGeradas;
    public int NumTuplasGeradas() => numTuplas;

    public void SalvarTuplasGeradas(string caminhoArquivo)
    {
        using StreamWriter sw = new(caminhoArquivo);
        foreach (var pagina in paginasResultado)
        {
            for (int i = 0; i < pagina.qtd_tuplas_ocup; i++)
            {
                sw.WriteLine(string.Join(",", pagina.tuplas[i].cols));
            }
        }
    }
}