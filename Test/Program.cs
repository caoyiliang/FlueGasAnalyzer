// See https://aka.ms/new-console-template for more information
using FlueGasAnalyzer;

Console.WriteLine("Hello, World!");

IFlueGasAnalyzer flueGasAnalyzer = new FlueGasAnalyzer.FlueGasAnalyzer(new Communication.Bus.PhysicalPort.SerialPort("COM1", 38400));
await flueGasAnalyzer.OpenAsync();
var rs = await flueGasAnalyzer.Read("31", "4113", "0001");

Console.ReadLine();