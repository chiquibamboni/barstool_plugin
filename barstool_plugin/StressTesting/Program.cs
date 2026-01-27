using BarstoolPlugin.Services;
using BarstoolPluginCore.Model;
using Microsoft.VisualBasic.Devices;
﻿using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;

namespace BarstoolStressTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("== Barstool Plugin Infinite Stress Test ==");
            Console.WriteLine();

            Console.WriteLine("Select barstool parameters:");
            Console.WriteLine("1 - Minimal parameters (small stool)");
            Console.WriteLine("2 - Average parameters (medium stool)");
            Console.WriteLine("3 - Maximum parameters (large stool)");
            Console.Write("Your choice: ");

            var choice = Console.ReadLine();
            Parameters parameters;

            switch (choice)
            {
                case "1":
                    parameters = GetMinimalParameters();
                    Console.WriteLine("Using minimal parameters");
                    break;
                case "2":
                    parameters = GetAverageParameters();
                    Console.WriteLine("Using average parameters");
                    break;
                case "3":
                    parameters = GetMaximalParameters();
                    Console.WriteLine("Using maximum parameters");
                    break;
                default:
                    Console.WriteLine("Invalid choice! " +
                        "Using average parameters.");
                    parameters = GetAverageParameters();
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Starting infinite stress test...");
            Console.WriteLine("Press Ctrl+C to stop");
            Console.WriteLine();

            RunSimpleInfiniteTest(parameters);
        }

        private static void RunSimpleInfiniteTest(Parameters parameters)
        {
            var builder = new Builder();
            var stopWatch = new Stopwatch();
            System.Diagnostics.Process.GetCurrentProcess();

            const double gigabyteInByte = 0.000000000931322574615478515625;
            var count = 0;
            var startTime = DateTime.Now;

            var fileName = $"barstool_stress_" +
                $"{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            using var streamWriter = new StreamWriter(fileName);

            streamWriter.WriteLine($"Stress Test Started: {DateTime.Now}");
            streamWriter.WriteLine($"Parameters: " +
                $"D={parameters.GetValue(ParameterType.SeatDiameterD)}, " +
                $"D1={parameters.GetValue(ParameterType.LegDiameterD1)}, " +
                $"D2={parameters.GetValue(
                    ParameterType.FootrestDiameterD2)}, " +
                $"H={parameters.GetValue(ParameterType.StoolHeightH)}, " +
                $"H1={parameters.GetValue(
                    ParameterType.FootrestHeightH1)}, " +
                $"S={parameters.GetValue(ParameterType.SeatDepthS)}, " +
                $"C={parameters.GetValue(ParameterType.LegCountC)}");
            streamWriter.WriteLine("Count\tTime\tRAM (GB)");
            streamWriter.Flush();

            Console.WriteLine($"Log file: {fileName}");
            Console.WriteLine("Count\tTime\t\tRAM (GB)\tTotal RAM (GB)");
            Console.WriteLine("-----------------------------------------" +
                "---------------");

            try
            {
                while (true)
                {
                    count++;

                    try
                    {
                        stopWatch.Start();
                        builder.Build(parameters);
                        stopWatch.Stop();
                        builder.CloseDocument();
                    }
                    catch (Exception ex)
                    {
                        stopWatch.Stop();
                        Console.WriteLine($"\nBuild {count} failed:" +
                            $" {ex.GetType().Name}");

                        if (ex is OutOfMemoryException)
                        {
                            Console.WriteLine("*** OUT OF MEMORY ***");
                            streamWriter.WriteLine($"\nOUT OF MEMORY at " +
                                $"build {count}: {ex.Message}");
                            break;
                        }
                    }

                    var computerInfo = new ComputerInfo();
                    var usedMemory = (computerInfo.TotalPhysicalMemory
                    - computerInfo.AvailablePhysicalMemory) * gigabyteInByte;
                    var totalPhysicalMemory = 
                        computerInfo.TotalPhysicalMemory * gigabyteInByte;
                    var elapsedTime = stopWatch.Elapsed;

                    // Запись в лог файл
                    streamWriter.WriteLine(
                        $"{count}\t{elapsedTime:hh\\:mm\\:ss\\.fff}" +
                        $"\t{usedMemory:F3}");
                    streamWriter.Flush();

                    var totalElapsed = DateTime.Now - startTime;
                    Console.WriteLine($"{count}" +
                        $"\t{elapsedTime:hh\\:mm\\:ss\\.fff}" +
                        $"\t{usedMemory:F3}\t\t{totalPhysicalMemory:F3} " +
                        $"| Total: {totalElapsed:hh\\:mm\\:ss}");

                    stopWatch.Reset();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\n*** CRITICAL ERROR: " +
                    $"{ex.GetType().Name} ***");
                streamWriter.WriteLine($"\nCRITICAL ERROR: " +
                    $"{ex.GetType().Name} - {ex.Message}");
            }
            finally
            {
                var totalElapsed = DateTime.Now - startTime;

                streamWriter.WriteLine($"\nTest Finished: {DateTime.Now}");
                streamWriter.WriteLine($"Total builds: {count}");
                streamWriter.WriteLine($"Total time: " +
                    $"{totalElapsed:hh\\:mm\\:ss}");
                streamWriter.WriteLine($"Average time per build: " +
                    $"{totalElapsed.TotalMilliseconds / count:F0} ms");
                

                Console.WriteLine($"\n\n=== Test Results ===");
                Console.WriteLine($"Total builds: {count}");
                Console.WriteLine($"Total time: " +
                    $"{totalElapsed:hh\\:mm\\:ss}");
                Console.WriteLine($"Average time per build: " +
                    $"{totalElapsed.TotalMilliseconds / count:F0} ms");
                Console.WriteLine($"Results saved to: {fileName}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Получает минимальные параметры барного стула
        /// </summary>
        private static Parameters GetMinimalParameters()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.LegDiameterD1, 25);
            parameters.SetValue(ParameterType.FootrestDiameterD2, 10);
            parameters.SetValue(ParameterType.SeatDiameterD, 300);
            parameters.SetValue(ParameterType.FootrestHeightH1, 200);
            parameters.SetValue(ParameterType.StoolHeightH, 700);
            parameters.SetValue(ParameterType.SeatDepthS, 20);
            parameters.SetValue(ParameterType.LegCountC, 3);
            return parameters;
        }

        /// <summary>
        /// Получает средние параметры барного стула
        /// </summary>
        private static Parameters GetAverageParameters()
        {
            var parameters = new Parameters();
            return parameters;
        }

        /// <summary>
        /// Получает максимальные параметры барного стула
        /// </summary>
        private static Parameters GetMaximalParameters()
        {
            var parameters = new Parameters();
            parameters.SetValue(ParameterType.LegDiameterD1, 70);
            parameters.SetValue(ParameterType.FootrestDiameterD2, 50);
            parameters.SetValue(ParameterType.SeatDiameterD, 500);
            parameters.SetValue(ParameterType.FootrestHeightH1, 400);
            parameters.SetValue(ParameterType.StoolHeightH, 900);
            parameters.SetValue(ParameterType.SeatDepthS, 100);
            parameters.SetValue(ParameterType.LegCountC, 6);
            return parameters;
        }
    }
}