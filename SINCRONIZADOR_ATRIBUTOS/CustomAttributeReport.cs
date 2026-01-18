using System;
using System.Collections.Generic;
using System.Text;

namespace SincronizadorAtributos
{
    /// <summary>
    /// Sistema de reportes para la sincronización de atributos personalizados.
    /// Acumula estadísticas y genera reportes formateados.
    /// </summary>
    public class CustomAttributeReport
    {
        // Contadores de estadísticas
        public int PartsEvaluated { get; set; }
        public int PartsChanged { get; set; }
        public int PartsSkipped { get; set; }

        public int BoltsEvaluated { get; set; }
        public int BoltsChanged { get; set; }
        public int BoltsSkipped { get; set; }

        // Listas de eventos
        private List<int> _assembliesProcessed;
        private List<string> _errors;
        private List<string> _warnings;
        private List<string> _info;

        public CustomAttributeReport()
        {
            _assembliesProcessed = new List<int>();
            _errors = new List<string>();
            _warnings = new List<string>();
            _info = new List<string>();

            PartsEvaluated = 0;
            PartsChanged = 0;
            PartsSkipped = 0;

            BoltsEvaluated = 0;
            BoltsChanged = 0;
            BoltsSkipped = 0;
        }

        public void AddAssemblyProcessed(int assemblyId)
        {
            if (!_assembliesProcessed.Contains(assemblyId))
            {
                _assembliesProcessed.Add(assemblyId);
            }
        }

        public void AddError(string message)
        {
            _errors.Add($"[ERROR] {DateTime.Now:HH:mm:ss} - {message}");
        }

        public void AddWarning(string message)
        {
            _warnings.Add($"[WARN] {DateTime.Now:HH:mm:ss} - {message}");
        }

        public void AddInfo(string message)
        {
            _info.Add($"[INFO] {DateTime.Now:HH:mm:ss} - {message}");
        }

        /// <summary>
        /// Genera un reporte formateado de la sincronización.
        /// </summary>
        public string GenerateReport()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("===============================================");
            sb.AppendLine("  REPORTE DE SINCRONIZACION DE ATRIBUTOS");
            sb.AppendLine("  Atributos: ESTATUS_PIEZA y PRIORIDAD");
            sb.AppendLine("===============================================");
            sb.AppendLine();

            // Resumen de Assemblies
            sb.AppendLine($"Assemblies procesados: {_assembliesProcessed.Count}");
            sb.AppendLine();

            // Resumen de Parts
            sb.AppendLine("--- PARTS ---");
            sb.AppendLine($"Total evaluadas: {PartsEvaluated}");
            sb.AppendLine($"  Modificadas:   {PartsChanged}");
            sb.AppendLine($"  Sin cambios:   {PartsSkipped}");
            sb.AppendLine();

            // Resumen de Bolts
            sb.AppendLine("--- BOLTS ---");
            sb.AppendLine($"Total evaluados: {BoltsEvaluated}");
            sb.AppendLine($"  Modificados:   {BoltsChanged}");
            sb.AppendLine($"  Sin cambios:   {BoltsSkipped}");
            sb.AppendLine();

            // Resultado final
            bool hasChanges = (PartsChanged > 0 || BoltsChanged > 0);
            bool hasErrors = (_errors.Count > 0);

            if (hasErrors)
            {
                sb.AppendLine("[ERROR] Sincronización completada con errores");
            }
            else if (hasChanges)
            {
                sb.AppendLine("[OK] Sincronización completada exitosamente");
            }
            else
            {
                sb.AppendLine("[INFO] No se realizaron cambios (todo ya estaba sincronizado)");
            }
            sb.AppendLine();

            // Sección de errores
            if (_errors.Count > 0)
            {
                sb.AppendLine("===============================================");
                sb.AppendLine($"  ERRORES ({_errors.Count})");
                sb.AppendLine("===============================================");
                foreach (string error in _errors)
                {
                    sb.AppendLine(error);
                }
                sb.AppendLine();
            }

            // Sección de advertencias
            if (_warnings.Count > 0)
            {
                sb.AppendLine("===============================================");
                sb.AppendLine($"  ADVERTENCIAS ({_warnings.Count})");
                sb.AppendLine("===============================================");
                foreach (string warning in _warnings)
                {
                    sb.AppendLine(warning);
                }
                sb.AppendLine();
            }


            // Sección de detalle ANTES/DESPUÉS siempre al final
            if (_info.Count > 0)
            {
                sb.AppendLine("===============================================");
                sb.AppendLine("  DETALLE DE CAMBIOS (ANTES / DESPUÉS)");
                sb.AppendLine("===============================================");
                foreach (string info in _info)
                {
                    sb.AppendLine(info);
                }
                sb.AppendLine();
            }

            sb.AppendLine("===============================================");
            sb.AppendLine($"Generado: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("===============================================");

            return sb.ToString();
        }

        /// <summary>
        /// Genera un resumen corto para mostrar en la interfaz.
        /// </summary>
        public string GenerateShortSummary()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("=== RESUMEN ===");
            sb.AppendLine($"Assemblies: {_assembliesProcessed.Count}");
            sb.AppendLine($"Parts modificadas: {PartsChanged}/{PartsEvaluated}");
            sb.AppendLine($"Bolts modificados: {BoltsChanged}/{BoltsEvaluated}");

            if (_errors.Count > 0)
            {
                sb.AppendLine($"Errores: {_errors.Count}");
            }

            if (_warnings.Count > 0)
            {
                sb.AppendLine($"Advertencias: {_warnings.Count}");
            }

            return sb.ToString();
        }
    }
}
