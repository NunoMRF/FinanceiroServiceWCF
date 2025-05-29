using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace FinanceiroServiceWCF
{
    public class FinanceiroService : IFinanceiroService
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ContabilidadeDB"].ConnectionString;

        public string GetPecaMaiorPrejuizo()
        {
            string codigoPeca = "Nenhuma";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 1 Codigo_Peca
                    FROM Custos_Peca
                    GROUP BY Codigo_Peca
                    ORDER BY SUM(Prejuizo) DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                var result = cmd.ExecuteScalar();

                if (result != null)
                    codigoPeca = result.ToString();
            }

            return codigoPeca;
        }

        public decimal GetCustoTotalProducao(DateTime dataInicio, DateTime dataFim)
        {
            decimal total = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT SUM(cp.Custo_Producao)
            FROM Contabilidade.dbo.Custos_Peca cp
            JOIN Producao.dbo.Produto p ON cp.ID_Produto = p.ID_Produto
            WHERE p.Data_Producao BETWEEN @DataInicio AND @DataFim";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                cmd.Parameters.AddWithValue("@DataFim", dataFim);

                conn.Open();
                var result = cmd.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                    total = Convert.ToDecimal(result);
            }

            return total;
        }

        public decimal GetLucroTotal(DateTime dataInicio, DateTime dataFim)
        {
            decimal total = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT SUM(cp.Lucro)
            FROM Contabilidade.dbo.Custos_Peca cp
            JOIN Producao.dbo.Produto p ON cp.ID_Produto = p.ID_Produto
            WHERE p.Data_Producao BETWEEN @DataInicio AND @DataFim";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                cmd.Parameters.AddWithValue("@DataFim", dataFim);

                conn.Open();
                var result = cmd.ExecuteScalar();

                if (result != DBNull.Value && result != null)
                    total = Convert.ToDecimal(result);
            }

            return total;
        }

        public List<PrejuizoPorPeca> GetPrejuizoTotalPorPeca(DateTime dataInicio, DateTime dataFim)
        {
            List<PrejuizoPorPeca> lista = new List<PrejuizoPorPeca>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT cp.Codigo_Peca, SUM(cp.Prejuizo) AS TotalPrejuizo
            FROM Contabilidade.dbo.Custos_Peca cp
            JOIN Producao.dbo.Produto p ON cp.ID_Produto = p.ID_Produto
            WHERE p.Data_Producao BETWEEN @DataInicio AND @DataFim
            GROUP BY cp.Codigo_Peca";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                cmd.Parameters.AddWithValue("@DataFim", dataFim);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new PrejuizoPorPeca
                    {
                        CodigoPeca = reader["Codigo_Peca"].ToString(),
                        TotalPrejuizo = Convert.ToDecimal(reader["TotalPrejuizo"])
                    });
                }
            }

            return lista;
        }

        public DadosFinanceiros GetDadosFinanceirosPorPeca(string codigoPeca)
        {
            DadosFinanceiros dados = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                Codigo_Peca,
                SUM(Custo_Producao) AS Custo,
                SUM(Lucro) AS Lucro,
                SUM(Prejuizo) AS Prejuizo,
                SUM(Tempo_Producao) AS TempoTotal
            FROM Contabilidade.dbo.Custos_Peca
            WHERE Codigo_Peca = @Codigo
            GROUP BY Codigo_Peca";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Codigo", codigoPeca);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    dados = new DadosFinanceiros
                    {
                        CodigoPeca = reader["Codigo_Peca"].ToString(),
                        Custo = Convert.ToDecimal(reader["Custo"]),
                        Lucro = Convert.ToDecimal(reader["Lucro"]),
                        Prejuizo = Convert.ToDecimal(reader["Prejuizo"]),
                        TempoProducao = Convert.ToInt32(reader["TempoTotal"])
                    };
                }
            }

            return dados;
        }

    }
}
