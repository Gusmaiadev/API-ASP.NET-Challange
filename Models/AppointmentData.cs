namespace DentalClinicAPI.Models
{
    // Classe para dados de entrada do modelo
    public class AppointmentData
    {
        // Dia da semana (1-7, onde 1 = Domingo)
        public float DayOfWeek { get; set; }

        // Hora do dia (0-23)
        public float HourOfDay { get; set; }

        // Mês (1-12)
        public float Month { get; set; }

        // Especialidade do dentista (codificada como número)
        public float SpecialtyCode { get; set; }

        // Histórico de faltas (0 = nunca faltou, 1 = já faltou)
        public float HasMissedBefore { get; set; }

        // Resultado: paciente compareceu? (0 = não, 1 = sim)
        public bool PatientAttended { get; set; }
    }

    // Classe para predição do modelo
    public class AppointmentPrediction
    {
        // Probabilidade de comparecimento
        [Microsoft.ML.Data.ColumnName("Score")]
        public float AttendanceProbability { get; set; }
    }

    // Classe para recomendação de horário
    public class TimeSlotRecommendation
    {
        public int DayOfWeek { get; set; }
        public int Hour { get; set; }
        public string DayName { get; set; }
        public float Score { get; set; }
    }
}