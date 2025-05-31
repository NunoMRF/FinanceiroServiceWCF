using System;
using System.Collections.Generic;

namespace ConsumidorProducao.Models
{
    public class Produto
    {
        public string Codigo_Peca { get; set; }
        public DateTime Data_Producao { get; set; }
        public TimeSpan Hora_Producao { get; set; }
        public int Tempo_Producao { get; set; }
        public List<Teste> Testes { get; set; } = new List<Teste>();
    }
}