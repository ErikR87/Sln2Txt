using System;
using System.IO;
using Microsoft.Build.Construction;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Fehler: Zu wenige Argumente.");
            Console.WriteLine("Verwendung: dotnet run \".\\pfad\\zur\\dein.sln\" \".\\pfad\\zur\\ausgabe.txt\"");
            return;
        }

        string solutionFilePath = Path.GetFullPath(args[0]);
        string outputFilePath = Path.GetFullPath(args[1]);

        SolutionFile solution = SolutionFile.Parse(solutionFilePath);

        string[] ignoredDirectories = new[] { "bin", "obj" };

        int fileNumber = 0;

        using (StreamWriter sw = new StreamWriter(outputFilePath))
        {
            // Erklärungstext
            sw.WriteLine("Dieses Dokument enthält eine Übersicht über eine Visual Studio Lösung (.sln), inklusive aller Projekte, Ordner und Dateien.");
            sw.WriteLine("Der erste Abschnitt ist das Inhaltsverzeichnis, gefolgt von den Inhalten jeder Datei.");
            sw.WriteLine("Ignorierte Ordner: " + string.Join(", ", ignoredDirectories));
            sw.WriteLine(new string('=', 80));

            // Inhaltsverzeichnis der Ordnerstruktur
            sw.WriteLine("Inhaltsverzeichnis:");
            foreach (ProjectInSolution project in solution.ProjectsInOrder)
            {
                if (project.ProjectType != SolutionProjectType.KnownToBeMSBuildFormat) continue;
                string projectDirectory = Path.GetDirectoryName(project.AbsolutePath);
                WriteDirectoryContents(projectDirectory, sw, ignoredDirectories, Path.GetDirectoryName(solutionFilePath), indent: 2);
            }
            sw.WriteLine(new string('=', 80));

            // Gehe durch jedes Projekt in der Lösung
            foreach (ProjectInSolution project in solution.ProjectsInOrder)
            {
                if (project.ProjectType != SolutionProjectType.KnownToBeMSBuildFormat) continue;

                string projectDirectory = Path.GetDirectoryName(project.AbsolutePath);

                foreach (string file in Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories))
                {
                    if (ignoredDirectories.Any(ignored => file.Contains(Path.DirectorySeparatorChar + ignored + Path.DirectorySeparatorChar))) continue;

                    fileNumber++;
                    string relativePath = Path.GetRelativePath(Path.GetDirectoryName(solutionFilePath), file);
                    sw.WriteLine($"Datei #{fileNumber}: {relativePath}");

                    try
                    {
                        string fileContent = File.ReadAllText(file);
                        sw.WriteLine(fileContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Fehler beim Lesen der Datei: {file}. Fehler: {ex.Message}");
                    }

                    // Saubere Trennung zwischen den Dateien
                    sw.WriteLine(new string('=', 80));
                }
            }
        }

        Console.WriteLine($"Die Pfade und Inhalte der Dateien wurden erfolgreich in {outputFilePath} geschrieben.");
    }

    private static void WriteDirectoryContents(string directoryPath, StreamWriter writer, string[] ignoredDirectories, string rootPath, int indent)
    {
        string indentation = new string(' ', indent);

        foreach (string dir in Directory.GetDirectories(directoryPath))
        {
            if (ignoredDirectories.Any(ignored => dir.EndsWith(ignored))) continue;

            string relativeDirPath = Path.GetRelativePath(rootPath, dir);
            writer.WriteLine($"{indentation}{relativeDirPath}");
            WriteDirectoryContents(dir, writer, ignoredDirectories, rootPath, indent + 2);
        }

        foreach (string file in Directory.GetFiles(directoryPath))
        {
            string relativeFilePath = Path.GetRelativePath(rootPath, file);
            writer.WriteLine($"{indentation}{relativeFilePath}");
        }
    }
}
