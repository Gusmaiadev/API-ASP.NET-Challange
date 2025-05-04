using DentalClinicAPI.Data;
using DentalClinicAPI.Models;
using DentalClinicAPI.Repositories;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DentalClinicAPI.Services
{
    public class MLService : IMLService
    {
        private readonly IRepository<Appointment> _appointmentRepo;
        private readonly IRepository<Patient> _patientRepo;
        private readonly MLContext _mlContext;
        private readonly string _modelPath;
        private PredictionEngine<AppointmentData, AppointmentPrediction> _predictionEngine;

        // Dicionário para mapear especialidades para códigos numéricos
        private readonly Dictionary<string, float> _specialtyCodes = new Dictionary<string, float>
        {
            { "Clínico Geral", 1 },
            { "Ortodontia", 2 },
            { "Endodontia", 3 },
            { "Cirurgia", 4 },
            { "Periodontia", 5 },
            { "Odontopediatria", 6 }
        };

        public MLService(IRepository<Appointment> appointmentRepo, IRepository<Patient> patientRepo)
        {
            _appointmentRepo = appointmentRepo;
            _patientRepo = patientRepo;
            _mlContext = new MLContext(seed: 42);
            _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appointment_model.zip");

            // Verificar se o modelo existe e carregar
            if (File.Exists(_modelPath))
            {
                LoadModel();
            }
        }

        private void LoadModel()
        {
            try
            {
                // Carregar modelo do arquivo
                var model = _mlContext.Model.Load(_modelPath, out _);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<AppointmentData, AppointmentPrediction>(model);
            }
            catch (Exception ex)
            {
                // Se houver erro, apenas registre-o, o modelo será treinado em outra chamada
                Console.WriteLine($"Erro ao carregar modelo: {ex.Message}");
            }
        }

        public async Task TrainModel()
        {
            // Obter todos os agendamentos
            var appointments = await _appointmentRepo.GetAll();

            // Simular dados históricos de comparecimento (num sistema real, você teria esta informação)
            var random = new Random(42);
            var trainingData = appointments.Select(a => new AppointmentData
            {
                DayOfWeek = (float)a.Date.DayOfWeek + 1, // Ajustando para 1-7
                HourOfDay = a.Date.Hour,
                Month = a.Date.Month,
                // Simulando alguma especialidade aleatória para exemplo
                SpecialtyCode = random.Next(1, 7),
                // Simulando histórico de faltas
                HasMissedBefore = random.NextDouble() > 0.8 ? 1 : 0,
                // Simulando se compareceu ou não (80% de chance de comparecimento)
                PatientAttended = random.NextDouble() <= 0.8
            }).ToList();

            // Se não houver dados suficientes, criar dados sintéticos para demonstração
            if (trainingData.Count < 10)
            {
                trainingData = GenerateSyntheticData();
                Console.WriteLine("Usando dados sintéticos para treinamento devido à falta de dados reais.");
            }

            // Criar dataset de treinamento
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            // Pipeline de processamento e treinamento
            var pipeline = _mlContext.Transforms.Concatenate("Features",
                    nameof(AppointmentData.DayOfWeek),
                    nameof(AppointmentData.HourOfDay),
                    nameof(AppointmentData.Month),
                    nameof(AppointmentData.SpecialtyCode),
                    nameof(AppointmentData.HasMissedBefore))
                .Append(_mlContext.BinaryClassification.Trainers.FastTree(
                    labelColumnName: nameof(AppointmentData.PatientAttended),
                    numberOfLeaves: 20,
                    numberOfTrees: 100,
                    minimumExampleCountPerLeaf: 5)); // Reduzido para os dados sintéticos

            // Treinar modelo
            var model = pipeline.Fit(dataView);

            // Salvar modelo para uso futuro
            _mlContext.Model.Save(model, dataView.Schema, _modelPath);

            // Atualizar engine de previsão
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<AppointmentData, AppointmentPrediction>(model);
        }

        // Método para gerar dados sintéticos para demonstração
        private List<AppointmentData> GenerateSyntheticData()
        {
            var random = new Random(42);
            var syntheticData = new List<AppointmentData>();

            // Gerar 200 amostras sintéticas com algumas regras básicas
            for (int i = 0; i < 200; i++)
            {
                float dayOfWeek = (float)random.Next(1, 8);
                float hourOfDay = (float)random.Next(8, 19);
                float month = (float)random.Next(1, 13);
                float specialtyCode = (float)random.Next(1, 7);
                float hasMissedBefore = random.NextDouble() > 0.8 ? 1 : 0;

                // Regras "reais" para simular padrões:
                // - Consultas no meio da semana têm maior frequência
                // - Consultas muito cedo ou muito tarde têm menor frequência
                // - Quem já faltou tem maior chance de faltar de novo

                bool willAttend = true;

                // Consultas em fins de semana têm menor frequência
                if (dayOfWeek == 1 || dayOfWeek == 7)
                {
                    willAttend = random.NextDouble() > 0.4;
                }

                // Consultas muito cedo ou muito tarde
                if (hourOfDay < 9 || hourOfDay > 16)
                {
                    willAttend = random.NextDouble() > 0.6;
                }

                // Histórico de faltas
                if (hasMissedBefore > 0)
                {
                    willAttend = random.NextDouble() > 0.5;
                }

                // Se for um caso normal, 85% de chance de comparecer
                if (willAttend)
                {
                    willAttend = random.NextDouble() > 0.15;
                }

                syntheticData.Add(new AppointmentData
                {
                    DayOfWeek = dayOfWeek,
                    HourOfDay = hourOfDay,
                    Month = month,
                    SpecialtyCode = specialtyCode,
                    HasMissedBefore = hasMissedBefore,
                    PatientAttended = willAttend
                });
            }

            return syntheticData;
        }

        public async Task<List<TimeSlotRecommendation>> GetRecommendedTimeSlots(int patientId, string specialty)
        {
            // Verificar se o modelo está carregado
            if (_predictionEngine == null)
            {
                await TrainModel();
            }

            // Verificar se o paciente existe
            var patient = await _patientRepo.GetById(patientId);
            if (patient == null)
            {
                throw new KeyNotFoundException($"Paciente com ID {patientId} não encontrado");
            }

            // Verificar se a especialidade é válida
            if (!_specialtyCodes.TryGetValue(specialty, out float specialtyCode))
            {
                // Se não encontrar, usar código genérico
                specialtyCode = 1;
            }

            // Pegar histórico de agendamentos do paciente para verificar se já faltou
            var appointments = await _appointmentRepo.GetAll();
            bool hasMissedBefore = appointments
                .Any(a => a.PatientId == patientId && a.Date < DateTime.Now);

            // Gerar predictions para diferentes horários
            var recommendations = new List<TimeSlotRecommendation>();
            string[] dayNames = { "Domingo", "Segunda", "Terça", "Quarta", "Quinta", "Sexta", "Sábado" };

            // Analisar horários para próxima semana
            for (int day = 1; day <= 5; day++) // Dias úteis (segunda a sexta)
            {
                for (int hour = 8; hour <= 17; hour++) // Horário comercial
                {
                    var data = new AppointmentData
                    {
                        DayOfWeek = day + 1, // Ajuste para começar segunda = 2
                        HourOfDay = hour,
                        Month = DateTime.Now.Month,
                        SpecialtyCode = specialtyCode,
                        HasMissedBefore = hasMissedBefore ? 1 : 0
                    };

                    // Fazer previsão
                    var prediction = _predictionEngine.Predict(data);

                    // Adicionar à lista
                    recommendations.Add(new TimeSlotRecommendation
                    {
                        DayOfWeek = day,
                        Hour = hour,
                        DayName = dayNames[day],
                        Score = prediction.AttendanceProbability
                    });
                }
            }

            // Retornar top 5 recomendações
            return recommendations
                .OrderByDescending(r => r.Score)
                .Take(5)
                .ToList();
        }

        public Task<float> PredictAttendanceProbability(AppointmentData appointmentData)
        {
            if (_predictionEngine == null)
            {
                throw new InvalidOperationException("Modelo não treinado. Execute TrainModel primeiro.");
            }

            var prediction = _predictionEngine.Predict(appointmentData);
            return Task.FromResult(prediction.AttendanceProbability);
        }
    }
}