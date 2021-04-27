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
	public class PlotAndPrintFibRetracementValuesExample : Strategy
	{
		// Define a FibonacciRetracements object outside of OnBarUpdate(), so the same object can be re-used
		FibonacciRetracements myRetracements;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "PlotAndPrintFibRetracementValuesExample";
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
				
				AddPlot(new Stroke(Brushes.Magenta, DashStyleHelper.Dash, 2), PlotStyle.Line, "p1");
				AddPlot(new Stroke(Brushes.Magenta, DashStyleHelper.Dash, 2), PlotStyle.Line, "p2");
				AddPlot(new Stroke(Brushes.Magenta, DashStyleHelper.Dash, 2), PlotStyle.Line, "p3");
				AddPlot(new Stroke(Brushes.Magenta, DashStyleHelper.Dash, 2), PlotStyle.Line, "p4");
				AddPlot(new Stroke(Brushes.Magenta, DashStyleHelper.Dash, 2), PlotStyle.Line, "p5");
				AddPlot(new Stroke(Brushes.Magenta, DashStyleHelper.Dash, 2), PlotStyle.Line, "p6");
				AddPlot(new Stroke(Brushes.Magenta, DashStyleHelper.Dash, 2), PlotStyle.Line, "p7");
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 25)
				return;
			
			myRetracements = Draw.FibonacciRetracements(this, "fib", true, 20, High[20], 2, Low[2]);
			ClearOutputWindow();
			// Print each price level and the corresponding value in the PriceLevels collection contain in myRetracements
		  	// setting isInverted correctly is important for the Fibonacci Retracements since it will define which starting point is used, as it changes based     // on the anchors, i.e. if the Fibonacci is drawn from 100% to 0% (default) or the other inverted way (0% to 100%).
				foreach (PriceLevel p in myRetracements.PriceLevels)
  				{
					double totalPriceRange = myRetracements.EndAnchor.Price - myRetracements.StartAnchor.Price;
				 	double price = p.GetPrice(myRetracements.StartAnchor.Price, totalPriceRange, true);
					Print(price);
					
					if (p.Value == 0.00)
						p1[0] = p.GetPrice(myRetracements.StartAnchor.Price, totalPriceRange, true);
					if (p.Value == 23.60)
					 	p2[0] = p.GetPrice(myRetracements.StartAnchor.Price, totalPriceRange, true);
					if (p.Value == 38.20)
					  	p3[0] = p.GetPrice(myRetracements.StartAnchor.Price, totalPriceRange, true);
					if (p.Value == 50.00)
					  	p4[0] = p.GetPrice(myRetracements.StartAnchor.Price, totalPriceRange, true);
					if (p.Value == 61.80)
					  	p5[0] = p.GetPrice(myRetracements.StartAnchor.Price, totalPriceRange, true);
					if (p.Value == 76.40)
					  	p6[0] = p.GetPrice(myRetracements.StartAnchor.Price, totalPriceRange, true);
					if (p.Value == 100.00)
					  	p7[0] = p.GetPrice(myRetracements.StartAnchor.Price, totalPriceRange, true);
					
  				}
		}
		
		#region Properties
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> p1
		{
			get { return Values[0]; }
		}
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> p2
		{
			get { return Values[1]; }
		}
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> p3
		{
			get { return Values[2]; }
		}
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> p4
		{
			get { return Values[3]; }
		}
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> p5
		{
			get { return Values[4]; }
		}
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> p6
		{
			get { return Values[5]; }
		}
		[Browsable(false)]
		[XmlIgnore]
		public Series<double> p7
		{
			get { return Values[6]; }
		}
		#endregion
	}
}
