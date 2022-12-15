using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;
using Ashtray.Wrapper;
using Ashtray.Model;

namespace Ashtray.StressTest
{
    class Program
    {
        private static void Main()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var ashtrayBuilder = new AshtrayBuilder();
            var ashtrayParameters = new AshtrayParameters();
            ashtrayParameters.SetParameters("7", "42", "50",
                "80", "6");
            var streamWriter = new StreamWriter($"StressTest.txt", true);
            var modelCounter = 0;
            var computerInfo = new ComputerInfo();
            ulong usedMemory = 0;
            while (usedMemory * 0.96 <= computerInfo.TotalPhysicalMemory)
            {
                ashtrayBuilder.BuildAshtray(ashtrayParameters.Parameters[ParameterType.BottomThickness].Value,
                    ashtrayParameters.Parameters[ParameterType.Height].Value,
                    ashtrayParameters.Parameters[ParameterType.LowerDiameter].Value,
                    ashtrayParameters.Parameters[ParameterType.UpperDiameter].Value,
                    ashtrayParameters.Parameters[ParameterType.WallThickness].Value, "Нет");
                usedMemory = (computerInfo.TotalPhysicalMemory - computerInfo.AvailablePhysicalMemory);
                streamWriter.WriteLine(
                    $"{++modelCounter}\t{stopWatch.Elapsed:hh\\:mm\\:ss}\t{usedMemory}");
                streamWriter.Flush();
            }
            stopWatch.Stop();
            streamWriter.WriteLine("END");
            streamWriter.Close();
            streamWriter.Dispose();
        }
    }
}
