using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConsoleApplication1.Data;
using ConsoleApplication1.Data.Impl;
using ConsoleApplication1.Performance;

namespace ConsoleApplication1
{
	internal class Program
	{
		private const string ConnectionString = @"Data Source=SQL;Initial Catalog=APMM_HR_WebPA;User Id=webpa;Password=webpa;";

		private enum MeasurementType
		{
			SingleSessionSingleTransaction,
			SingleSessionMultipleTransactions,
			MultipleSessionsMultipleTransactions,
			MultipleSessionsNoTransactions,
		}

		private static void Main()
		{
			var performance = new AdoNetPerformance();
			var database = new DatabaseImpl(ConnectionString);

			Console.WriteLine("Process: " + performance.InstanceName);
			Console.WriteLine();
			Console.WriteLine("Press any key to start ADO.NET performance test...");
			Console.ReadLine();

			var sw = new Stopwatch();
			sw.Start();

			for (int i = 0; i < 500; i++)
			{
				var session = new SessionImpl(database);
				var formTexts = session.FindAll<FormText>();
				session.Dispose();
				Console.Write(".");
			}
			sw.Stop();

			Console.WriteLine();
			Console.WriteLine(sw.ElapsedMilliseconds + " ms");

			performance.DisplayPerformanceCounters();
		}

		public class FormText
		{
			public int Id { get; set; }
			public int FormTypeId { get; set; }
			public int SectionNumber { get; set; }
			public string Name { get; set; }
			public string Title { get; set; }
			public string Text { get; set; }
			public string HelpTitle { get; set; }
			public string HelpText { get; set; }
			public string Guidelines { get; set; }
			public string JobGradeBand { get; set; }
		}

		private static void MeasurePerformance()
		{
			var program = new Program();

			const int queries = 5000;
			const int runs = 10;

			for(int x = 0; x<runs; x++)
			{
				Console.WriteLine("Pass {0}", x + 1);

				program.Measure((int) MeasurementType.SingleSessionSingleTransaction, db =>
				{
					using(ISession session = new SessionImpl(db))
					{
						using(ITransaction transaction = session.BeginTransaction())
						{
							for(int i = 0; i<queries; i++)
							{
								session.Get<FormText>(i);
							}
							transaction.Commit();
						}
					}
				});

				program.Measure((int) MeasurementType.SingleSessionMultipleTransactions, db =>
				{
					using(ISession session = new SessionImpl(db))
					{
						for(int i = 0; i<queries; i++)
						{
							using(ITransaction transaction = session.BeginTransaction())
							{
								session.Get<FormText>(i);
								transaction.Commit();
							}
						}
					}
				});

				program.Measure((int) MeasurementType.MultipleSessionsMultipleTransactions, db =>
				{
					for(int i = 0; i<queries; i++)
					{
						using(ISession session = new SessionImpl(db))
						{
							using(ITransaction transaction = session.BeginTransaction())
							{
								session.Get<FormText>(i);
								transaction.Commit();
							}
						}
					}
				});

				program.Measure((int) MeasurementType.MultipleSessionsNoTransactions, db =>
				{
					for(int i = 0; i<queries; i++)
					{
						using(ISession session = new SessionImpl(db))
						{
							session.Get<FormText>(i);
						}
					}
				});
				Console.WriteLine();
			}

			Console.WriteLine();
			program.DisplayStats();
		}

		private readonly IDatabase _database = new DatabaseImpl(ConnectionString);
		private readonly AdoNetPerformance _performance = new AdoNetPerformance();
		private readonly IDictionary<int, IList<long>> _idMeasureMap = new Dictionary<int, IList<long>>();
		private readonly bool _showPerformance;

		public Program() : this(false)
		{
		}

		public Program(bool showPerformance)
		{
			_showPerformance = showPerformance;
		}

		public void Measure(int id, Action<IDatabase> actionToMeasure)
		{
			try
			{
				var sw = new Stopwatch();
				sw.Start();
				actionToMeasure(_database);
				sw.Stop();

				if( !_idMeasureMap.ContainsKey(id) )
				{
					_idMeasureMap[id] = new List<long>();
				}
				_idMeasureMap[id].Add(sw.ElapsedMilliseconds);

				Console.Write(".");
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.ToString());
				if( _showPerformance )
				{
					_performance.DisplayPerformanceCounters();
				}
				return;
			}

			if( _showPerformance )
			{
				_performance.DisplayPerformanceCounters();
			}
		}

		public void DisplayStats()
		{
			foreach(var map in _idMeasureMap)
			{
				Console.WriteLine();
				Console.WriteLine("#{0}", map.Key);
				Console.WriteLine("Best:  {0} ms", map.Value.Max());
				Console.WriteLine("Worst: {0} ms", map.Value.Min());
				Console.WriteLine("Avg:   {0} ms", map.Value.Average());
			}
		}
	}
}