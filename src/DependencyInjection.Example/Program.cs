using Microsoft.Extensions.DependencyInjection;
using System;

namespace DependencyInjection.Example
{
	public interface IPrinter
	{
		void Print(string caller, string scope, string message);
	}

	public interface ISingletonOperation
	{
		void DoThing(string scope, string message);
	}

	public interface IScopedOperation
	{
		void DoThing(string scope, string message);
	}

	public interface ITransientOperation
	{
		void DoThing(string scope, string message);
	}

	public interface IOperationService
	{
		void DoSomeRealWork(string scope);
	}

	public class Printer : IPrinter
	{
		public void Print(string caller, string scope, string message)
		{
			Console.WriteLine($"{scope}: [ {caller}...{message} ]");
		}
	}

	public class OperationService : IOperationService
	{
		private readonly ITransientOperation _transientOperation;
		private readonly IScopedOperation _scopedOperation;
		private readonly ISingletonOperation _singletonOperation;

		public OperationService(
			ITransientOperation transientOperation,
			IScopedOperation scopedOperation,
			ISingletonOperation singletonOperation)
		{
			_transientOperation = transientOperation;
			_scopedOperation = scopedOperation;
			_singletonOperation = singletonOperation;
		}

		public void DoSomeRealWork(string scope)
		{
			_transientOperation.DoThing(scope, "Always different");
			_scopedOperation.DoThing(scope, "Changes only with scope");
			_singletonOperation.DoThing(scope, "Always the same");
		}
	}

	public class TransientOperation : ITransientOperation
	{
		private IPrinter _printer;

		public TransientOperation(IPrinter printer)
		{
			_printer = printer;
		}

		public void DoThing(string scope, string message)
		{
			_printer.Print("TransientOperation", scope, message);
		}
	}

	public class ScopedOperation : IScopedOperation
	{
		private IPrinter _printer;

		public ScopedOperation(IPrinter printer)
		{
			_printer = printer;
		}

		public void DoThing(string scope, string message)
		{
			_printer.Print("ScopedOperation", scope, message);
		}
	}

	public class SingletonOperation : ISingletonOperation
	{
		private IPrinter _printer;

		public SingletonOperation(IPrinter printer)
		{
			_printer = printer;
		}

		public void DoThing(string scope, string message)
		{
			_printer.Print("SingletonOperation", scope, message);
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			//setup our DI
			var serviceProvider = new ServiceCollection()
				.AddSingleton<IPrinter, Printer>()
				.AddScoped<IScopedOperation, ScopedOperation>()
				.AddTransient<ITransientOperation, TransientOperation>()
				.AddSingleton<ISingletonOperation, SingletonOperation>()
				.AddSingleton<IOperationService, OperationService>()
				.BuildServiceProvider();

			Console.WriteLine("Starting application");
			ExemplifyScoping(serviceProvider, "Scope 1");
			ExemplifyScoping(serviceProvider, "Scope 2");
			Console.WriteLine("All done!");
		}

		private static void ExemplifyScoping(IServiceProvider services, string scope)
		{
			IServiceScope serviceScope = services.CreateScope();
			IServiceProvider provider = serviceScope.ServiceProvider;

			IOperationService service = provider.GetService<IOperationService>();
			service.DoSomeRealWork($"{scope}-Call 2 .GetRequiredService<OperationService>()");

			Console.WriteLine("...");

			service = provider.GetRequiredService<IOperationService>();
			service.DoSomeRealWork($"{scope}-Call 2 .GetRequiredService<OperationService>()");

			Console.WriteLine();
		}
	}
}