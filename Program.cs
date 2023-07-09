using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;

class Program
{
    static void Main(string[] args)
    {
        string solutionFilePath = args[0]; // Pfad zur .sln-Datei
        string outputFilePath = args[1]; // Pfad zur Ausgabedatei

        // Einlesen der .sln-Datei
        SolutionFile solution = SolutionFile.Parse(solutionFilePath);

        // Ignorierte Verzeichnisse
        string[] ignoredDirectories = new[] { "bin", "obj" };

        using (StreamWriter sw = new StreamWriter(outputFilePath))
        {
            // Gehe durch jedes Projekt in der Lösung
            foreach (ProjectInSolution project in solution.ProjectsInOrder)
            {
                // Nur C#-Projekte beachten
                if (project.ProjectType != SolutionProjectType.KnownToBeMSBuildFormat) continue;

                string projectDirectory = Path.GetDirectoryName(project.AbsolutePath);

                // Gehe durch jede .cs-Datei im Projektverzeichnis und Unterordnern
                foreach (string file in Directory.EnumerateFiles(projectDirectory, "*.cs", SearchOption.AllDirectories))
                {
                    // Ignoriere Dateien in "bin" und "obj" Verzeichnissen
                    if (ignoredDirectories.Any(ignored => file.Contains(Path.DirectorySeparatorChar + ignored + Path.DirectorySeparatorChar))) continue;

                    // Schreibe den Dateipfad
                    sw.WriteLine(file);

                    // Schreibe den Dateiinhalt
                    try
                    {
                        string fileContent = File.ReadAllText(file);
                        sw.WriteLine(fileContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Lesen der Datei: {file}. Fehler: {ex.Message}");
                    }

                    // Trennzeile für die Lesbarkeit
                    sw.WriteLine(new string('-', 80));
                }
            }
        }

        Console.WriteLine($"Die Pfade und Inhalte der Dateien wurden erfolgreich in {outputFilePath} geschrieben.");
    }
}
