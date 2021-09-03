using Microsoft.Extensions.DependencyInjection;
using System;

namespace DependencyInjection.Example
{
	public interface IPrinter
	{
		void Print(string caller, string message);
	}

	public interface IOperationService
	{
		void DoSomeRealWork();
	}

	public class Printer : IPrinter
	{
		public void Print(string caller, string message)
		{
			Console.WriteLine($"[ {caller}...{message} ]");
		}
	}

	public class OperationService : IOperationService
	{
		private IPrinter _printer;

		public OperationService(IPrinter printer)
		{
			_printer = printer;
		}

		public void DoSomeRealWork()
		{
			_printer.Print("OperationService", "Print DoSomeRealWork");
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			//setup our DI
			var serviceProvider = new ServiceCollection()
				.AddSingleton<IPrinter, Printer>()
				.AddSingleton<IOperationService, OperationService>()
				.BuildServiceProvider();

			Console.WriteLine("Starting application");
			IOperationService service = serviceProvider.GetService<IOperationService>();
			service.DoSomeRealWork();
			Console.WriteLine("All done!");
		}
	}
}