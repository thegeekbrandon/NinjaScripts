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
	public class DrawLineAboveEntryStrategy_NT8 : Strategy
	{
		private Order entryOrder;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "DrawLineAboveEntryStrategy_NT8";
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
		}

		protected override void OnOrderUpdate(Cbi.Order order, double limitPrice, double stopPrice, 
			int quantity, int filled, double averageFillPrice, 
			Cbi.OrderState orderState, DateTime time, Cbi.ErrorCode error, string comment)
		{
			// check if the current order matches the orderName passed in "EnterLong"()
			// Assign entryOrder in OnOrderUpdate() to ensure the assignment occurs when expected.
			// This is more reliable than assigning Order objects in OnBarUpdate, as the assignment is not guaranteed to be complete if it is referenced immediately after submitting
			if (order.Name == "entryOrder")
			    entryOrder = order;
			 
		 	// if entry order exists
			if (entryOrder != null && entryOrder == order)
			{
			    // Draws a dotted lime green line from 10 bars back to the current bar
				// with a width of 2 pixels
				Draw.Line(this, "myLine", false, 10, order.AverageFillPrice + (20 * TickSize), 0, order.AverageFillPrice + (20 * TickSize), Brushes.LimeGreen, DashStyleHelper.Solid, 2);
			}
		}

		protected override void OnBarUpdate()
		{
			if (State == State.Historical)
				return;
			
			if (entryOrder == null && Close[0] > Open[0])
      			EnterLong("entryOrder");
		}
	}
}
