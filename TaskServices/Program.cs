using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskServices
{
	class Program
	{
		static void Main (string[] args)
		{
			ActionItemNotifier.sendEmailNotification().Wait();
			ActiveToCompleted.MoveActiveProposalsToCompleted().Wait();
			//Console.Read();
		}
	}
}
