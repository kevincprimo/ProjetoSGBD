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

    private List<string[]> tuplasResultado = new();
    private int numIOs = 0;
    private int numPagsGeradas = 0;

    public Operador(Tabela t1, Tabela t2, string c1, string c2)
    {
        tabela1 = t1;
        tabela2 = t2;
        col1 = c1;
        col2 = c2;
    }

    public void Executar()
    {
        // Ordena as duas tabelas pelas colunas de junção
        tabela1.OrdenarPor(col1);
        tabela2.OrdenarPor(col2);

        var tuplas1 = tabela1.GetTodasTuplasOrdenadas();
        var tuplas2 = tabela2.GetTodasTuplasOrdenadas();

        int idx1 = tabela1.IndiceColuna(col1);
        int idx2 = tabela2.IndiceColuna(col2);

        int i = 0, j = 0;

        // Junção Sort-Merge
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
                    tuplasResultado.Add(t1.Concat(tuplas2[tempJ]).ToArray());
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

        // Cálculo de métricas
        numIOs = tabela1.NumPaginasLidas() + tabela2.NumPaginasLidas();
        numPagsGeradas = (int)Math.Ceiling(tuplasResultado.Count / 10.0);
    }

    public int NumIOExecutados() => numIOs;
    public int NumPagsGeradas() => numPagsGeradas;
    public int NumTuplasGeradas() => tuplasResultado.Count;

    public void SalvarTuplasGeradas(string caminhoArquivo)
    {
        using StreamWriter sw = new(caminhoArquivo);
        foreach (var tupla in tuplasResultado)
        {
            sw.WriteLine(string.Join(",", tupla));
        }
    }
}