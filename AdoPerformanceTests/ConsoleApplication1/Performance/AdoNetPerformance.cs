using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ConsoleApplication1.Performance
{
	public class AdoNetPerformance
	{
		private readonly List<PerformanceCounter> _performanceCounters = new List<PerformanceCounter>();
		private string _instanceName;

		public string InstanceName
		{
			get
			{
				return _instanceName;
			}
		}

		public AdoNetPerformance()
		{
			GetInstanceName();
			var counterNames = Enum.GetNames(typeof(AdoNetPerformanceCounters));

			foreach(var counterName in counterNames)
			{
				AddPerformanceCounter(counterName);
			}
		}

		private void AddPerformanceCounter(string counterName)
		{
			var performanceCounter = new PerformanceCounter
			{
				CategoryName = ".NET Data Provider for SqlServer",
				CounterName = counterName,
				InstanceName = _instanceName
			};

			_performanceCounters.Add(performanceCounter);
		}

		public void DisplayPerformanceCounters()
		{
			Console.WriteLine("---------------------------");
			foreach(var performanceCounter in _performanceCounters)
			{
				Console.WriteLine("{0} = {1}", performanceCounter.CounterName, performanceCounter.NextValue());
			}
			Console.WriteLine("---------------------------");
		}

		private void GetInstanceName()
		{
			//This works for Winforms apps.
			string instanceName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

			// Must replace special characters like (, ), #, /, \\
			string instanceName2 = AppDomain.CurrentDomain.FriendlyName.ToString().Replace('(', '[').Replace(')', ']').Replace('#', '_').Replace('/', '_').Replace('\\', '_');

			// For ASP.NET applications your instanceName will be your CurrentDomain's 
			// FriendlyName. Replace the line above that sets the instanceName with this:
			// instanceName = AppDomain.CurrentDomain.FriendlyName.ToString().Replace('(','[')
			// .Replace(')',']').Replace('#','_').Replace('/','_').Replace('\\','_');

			string pid = GetCurrentProcessId().ToString();
			instanceName = instanceName + "[" + pid + "]";
			_instanceName = instanceName;
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int GetCurrentProcessId();
	}
}