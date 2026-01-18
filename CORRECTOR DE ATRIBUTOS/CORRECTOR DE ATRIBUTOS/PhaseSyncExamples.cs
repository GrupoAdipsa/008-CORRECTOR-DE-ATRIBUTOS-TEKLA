using System;
using CORRECTOR_DE_ATRIBUTOS;

/// <summary>
/// Ejemplos de uso del módulo de sincronización de Phase.
/// </summary>
public class PhaseSyncExamples
{
    /// <summary>
    /// Ejemplo 1: Ejecución interactiva básica (muestra ventana al usuario).
    /// </summary>
    public static void Example1_InteractiveMode()
    {
        // Forma más simple: muestra el formulario al usuario
        PhaseSyncLauncher.Launch();
    }

    /// <summary>
    /// Ejemplo 2: Ejecución programática sin interfaz de usuario.
    /// </summary>
    public static void Example2_ProgrammaticMode()
    {
        var synchronizer = new PhaseSynchronizer();
        
        // Ejecuta selección interactiva pero sin mostrar formulario
        bool success = synchronizer.ExecuteInteractive();
        
        if (success)
        {
            Console.WriteLine("Sincronización completada:");
            Console.WriteLine(synchronizer.Report.GenerateSummary());
        }
        else
        {
            Console.WriteLine("La sincronización falló o fue cancelada.");
        }
    }

    /// <summary>
    /// Ejemplo 3: Procesar una selección existente (desde otro contexto).
    /// </summary>
    public static void Example3_ProcessExistingSelection()
    {
        var model = new Tekla.Structures.Model.Model();
        
        if (!model.GetConnectionStatus())
        {
            Console.WriteLine("No hay conexión con Tekla.");
            return;
        }

        // Obtener objetos ya seleccionados en Tekla
        var picker = new Tekla.Structures.Model.UI.Picker();
        var selectedObjects = picker.PickObjects(
            Tekla.Structures.Model.UI.Picker.PickObjectsEnum.PICK_N_OBJECTS,
            "Selecciona objetos para sincronizar Phase"
        );
        
        if (selectedObjects == null || selectedObjects.GetSize() == 0)
        {
            Console.WriteLine("No hay objetos seleccionados en Tekla.");
            return;
        }

        // Procesar la selección
        var synchronizer = new PhaseSynchronizer();
        bool success = synchronizer.ExecuteOnSelection(selectedObjects);
        
        // Mostrar reporte completo
        Console.WriteLine(synchronizer.Report.GenerateReport());
    }

    /// <summary>
    /// Ejemplo 4: Análisis del reporte sin modificar nada (dry-run).
    /// NOTA: Requiere modificar PhaseSynchronizer para soportar modo dry-run.
    /// </summary>
    public static void Example4_ReportAnalysis()
    {
        var synchronizer = new PhaseSynchronizer();
        bool success = synchronizer.ExecuteInteractive();
        
        if (success)
        {
            SyncReport report = synchronizer.Report;
            
            Console.WriteLine($"Assemblies procesados: {report.AssembliesProcessed}");
            Console.WriteLine($"Total de cambios: {report.PartsChanged + report.BoltsChanged + report.WeldsChanged}");
            
            if (report.HasErrors)
            {
                Console.WriteLine("\nErrores encontrados:");
                foreach (var error in report.GetErrors())
                {
                    Console.WriteLine($"  - {error}");
                }
            }
            
            if (report.HasWarnings)
            {
                Console.WriteLine("\nAdvertencias:");
                foreach (var warning in report.GetWarnings())
                {
                    Console.WriteLine($"  - {warning}");
                }
            }
        }
    }

    /// <summary>
    /// Ejemplo 5: Integración en un plugin existente.
    /// </summary>
    public static void Example5_IntegrateInPlugin()
    {
        try
        {
            // Dentro de un método Run() de plugin:
            var synchronizer = new PhaseSynchronizer();
            
            // Ejecutar con feedback mínimo
            if (synchronizer.ExecuteInteractive())
            {
                // Éxito: mostrar solo resumen
                System.Windows.Forms.MessageBox.Show(
                    synchronizer.Report.GenerateSummary(),
                    "Sincronización de Phase",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information
                );
            }
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(
                $"Error: {ex.Message}",
                "Error en Sincronización",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error
            );
        }
    }

    /// <summary>
    /// Ejemplo 6: Uso desde una macro de Tekla.
    /// Guarda este código como PhaseSync.cs en la carpeta de macros de Tekla.
    /// </summary>
    public static void MacroExample()
    {
        // En una macro, simplemente:
        PhaseSyncLauncher.Launch();
        
        // O para modo más controlado:
        /*
        var sync = new PhaseSynchronizer();
        if (sync.ExecuteInteractive())
        {
            // Guardar reporte en archivo
            string report = sync.Report.GenerateReport();
            System.IO.File.WriteAllText(
                @"C:\Temp\PhaseSync_Report.txt", 
                report
            );
        }
        */
    }
}
