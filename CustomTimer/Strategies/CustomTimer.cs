#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class CustomTimer : Strategy
	{
		// Declare the WPF dispatcher timer
		private System.Timers.Timer myTimer;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "CustomTimer";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.DataLoaded)
			{
				// Instantiates the timer and sets the interval to 30 seconds.
				myTimer = new System.Timers.Timer(30000);
		  		myTimer.Elapsed += TimerEventProcessor;
				myTimer.Enabled = true;
			}
			else if (State == State.Terminated)
			{
		  		// Stops the timer and removes the timer event handler
				if (myTimer != null)
				{
			  		myTimer.Enabled = false;
			  		myTimer.Elapsed -= TimerEventProcessor;
					myTimer = null;
				}
			}
			
		}

		protected override void OnBarUpdate()
		{
			
		}
		// Timer's elapsed event handler. Called at every tick of 30000ms.
		private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
		{
		/* Important to use the TriggerCustomEvent() to ensure that NinjaScript indexes and pointers are correctly set.
		Do not process your code here. Process your code in the MyCustomHandler method. */
		TriggerCustomEvent(MyCustomHandler, 0, myTimer.Interval);
		}
		private void MyCustomHandler(object state)
		{
			Print("\tTime: " + DateTime.Now);
			Print("\tTimer Interval: " + state.ToString() + "ms");
            Print("\tStrategy Position: " + Position.MarketPosition);
			Print("\tAccount Position: " + PositionAccount.MarketPosition);
		}
	}
}
