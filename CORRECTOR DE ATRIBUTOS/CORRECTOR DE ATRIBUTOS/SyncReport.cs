using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CORRECTOR_DE_ATRIBUTOS
{
    /// <summary>
    /// Clase de reporte de resultados de sincronización de Phase.
    /// </summary>
    public class SyncReport
    {
        public int AssembliesProcessed { get; private set; }
        public int PartsEvaluated { get; set; }
        public int PartsChanged { get; set; }
        public int PartsSkipped { get; set; }
        public int BoltsEvaluated { get; set; }
        public int BoltsChanged { get; set; }
        public int BoltsSkipped { get; set; }
        public int WeldsEvaluated { get; set; }
        public int WeldsChanged { get; set; }
        public int WeldsSkipped { get; set; }

        private List<string> _errors = new List<string>();
        private List<string> _warnings = new List<string>();

        public SyncReport()
        {
            Reset();
        }

        /// <summary>
        /// Reinicia todos los contadores y listas.
        /// </summary>
        public void Reset()
        {
            AssembliesProcessed = 0;
            PartsEvaluated = 0;
            PartsChanged = 0;
            PartsSkipped = 0;
            BoltsEvaluated = 0;
            BoltsChanged = 0;
            BoltsSkipped = 0;
            WeldsEvaluated = 0;
            WeldsChanged = 0;
            WeldsSkipped = 0;
            _errors.Clear();
            _warnings.Clear();
        }

        /// <summary>
        /// Registra un Assembly procesado.
        /// </summary>
        public void AddAssemblyProcessed(object id)
        {
            AssembliesProcessed++;
        }

        /// <summary>
        /// Agrega un error al reporte.
        /// </summary>
        public void AddError(string error)
        {
            _errors.Add(error);
        }

        /// <summary>
        /// Agrega una advertencia al reporte.
        /// </summary>
        public void AddWarning(string warning)
        {
            _warnings.Add(warning);
        }

        /// <summary>
        /// Obtiene la lista de errores.
        /// </summary>
        public List<string> GetErrors()
        {
            return new List<string>(_errors);
        }

        /// <summary>
        /// Obtiene la lista de advertencias.
        /// </summary>
        public List<string> GetWarnings()
        {
            return new List<string>(_warnings);
        }

        /// <summary>
        /// Indica si hubo errores durante la sincronización.
        /// </summary>
        public bool HasErrors
        {
            get { return _errors.Count > 0; }
        }

        /// <summary>
        /// Indica si hubo advertencias durante la sincronización.
        /// </summary>
        public bool HasWarnings
        {
            get { return _warnings.Count > 0; }
        }

        /// <summary>
        /// Genera un reporte completo en formato texto.
        /// </summary>
        public string GenerateReport()
        {
            var report = new StringBuilder();
            report.AppendLine("???????????????????????????????????????????????????");
            report.AppendLine("  REPORTE DE SINCRONIZACIÓN DE PHASE");
            report.AppendLine("???????????????????????????????????????????????????");
            report.AppendLine();
            report.AppendLine($"Assemblies procesados: {AssembliesProcessed}");
            report.AppendLine();
            
            report.AppendLine("???????????????????????????????????????????????????");
            report.AppendLine("SECONDARY PARTS:");
            report.AppendLine($"  • Evaluadas:  {PartsEvaluated}");
            report.AppendLine($"  • Cambiadas:  {PartsChanged}");
            report.AppendLine($"  • Omitidas:   {PartsSkipped}");
            report.AppendLine();

            report.AppendLine("BOLTS:");
            report.AppendLine($"  • Evaluados:  {BoltsEvaluated}");
            report.AppendLine($"  • Cambiados:  {BoltsChanged}");
            report.AppendLine($"  • Omitidos:   {BoltsSkipped}");
            report.AppendLine();

            report.AppendLine("WELDS:");
            report.AppendLine($"  • Evaluadas:  {WeldsEvaluated}");
            report.AppendLine($"  • Cambiadas:  {WeldsChanged}");
            report.AppendLine($"  • Omitidas:   {WeldsSkipped}");
            report.AppendLine();

            int totalEvaluated = PartsEvaluated + BoltsEvaluated + WeldsEvaluated;
            int totalChanged = PartsChanged + BoltsChanged + WeldsChanged;
            int totalSkipped = PartsSkipped + BoltsSkipped + WeldsSkipped;

            report.AppendLine("???????????????????????????????????????????????????");
            report.AppendLine("TOTALES:");
            report.AppendLine($"  • Total evaluados: {totalEvaluated}");
            report.AppendLine($"  • Total cambiados: {totalChanged}");
            report.AppendLine($"  • Total omitidos:  {totalSkipped}");
            report.AppendLine();

            if (_warnings.Count > 0)
            {
                report.AppendLine("???????????????????????????????????????????????????");
                report.AppendLine($"ADVERTENCIAS ({_warnings.Count}):");
                foreach (var warning in _warnings.Take(10))
                {
                    report.AppendLine($"  ? {warning}");
                }
                if (_warnings.Count > 10)
                    report.AppendLine($"  ... y {_warnings.Count - 10} advertencias más");
                report.AppendLine();
            }

            if (_errors.Count > 0)
            {
                report.AppendLine("???????????????????????????????????????????????????");
                report.AppendLine($"ERRORES ({_errors.Count}):");
                foreach (var error in _errors.Take(10))
                {
                    report.AppendLine($"  ? {error}");
                }
                if (_errors.Count > 10)
                    report.AppendLine($"  ... y {_errors.Count - 10} errores más");
                report.AppendLine();
            }

            report.AppendLine("???????????????????????????????????????????????????");

            return report.ToString();
        }

        /// <summary>
        /// Genera un resumen corto del reporte.
        /// </summary>
        public string GenerateSummary()
        {
            int totalChanged = PartsChanged + BoltsChanged + WeldsChanged;
            int totalEvaluated = PartsEvaluated + BoltsEvaluated + WeldsEvaluated;

            return $"Procesados {AssembliesProcessed} assemblies. " +
                   $"Cambiados {totalChanged} de {totalEvaluated} objetos. " +
                   $"Errores: {_errors.Count}, Advertencias: {_warnings.Count}";
        }

        /// <summary>
        /// Indica si la operación fue exitosa (sin errores críticos).
        /// </summary>
        public bool IsSuccessful
        {
            get 
            { 
                return !HasErrors && AssembliesProcessed > 0 && 
                       (PartsChanged + BoltsChanged + WeldsChanged) > 0; 
            }
        }
    }
}
