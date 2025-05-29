using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace FinanceiroServiceWCF
{
    [ServiceContract]
    public interface IFinanceiroService
    {
        [OperationContract]
        string GetPecaMaiorPrejuizo();

        [OperationContract]
        decimal GetCustoTotalProducao(DateTime dataInicio, DateTime dataFim);

        [OperationContract]
        decimal GetLucroTotal(DateTime dataInicio, DateTime dataFim);

        [OperationContract]
        List<PrejuizoPorPeca> GetPrejuizoTotalPorPeca(DateTime dataInicio, DateTime dataFim);

        [OperationContract]
        DadosFinanceiros GetDadosFinanceirosPorPeca(string codigoPeca);
    }

    // Classe auxiliar para prejuízo total por peça
    [DataContract]
    public class PrejuizoPorPeca
    {
        [DataMember]
        public string CodigoPeca { get; set; }

        [DataMember]
        public decimal TotalPrejuizo { get; set; }
    }

    // Classe auxiliar para dados financeiros detalhados
    [DataContract]
    public class DadosFinanceiros
    {
        [DataMember]
        public string CodigoPeca { get; set; }

        [DataMember]
        public decimal Custo { get; set; }

        [DataMember]
        public decimal Lucro { get; set; }

        [DataMember]
        public decimal Prejuizo { get; set; }

        [DataMember]
        public int TempoProducao { get; set; }
    }
}
