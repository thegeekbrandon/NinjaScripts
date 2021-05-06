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
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class DrawLineAboveEntryIndicatorNT8 : Indicator
	{
		private Account account;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "DrawLineAboveEntryIndicatorNT8";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				
				// Find our Sim101 account
		        lock (Account.All)
		        	account = Account.All.FirstOrDefault(a => a.Name == "Sim101");

        		// Subscribe to order updates
        		if (account != null)
             		account.OrderUpdate += OnOrderUpdate;
				
			}
			else if (State == State.Terminated)
			{
				account.OrderUpdate -= OnOrderUpdate;
			}
		}
		
		private void OnOrderUpdate(object sender, OrderEventArgs e)
	    {
			if (e.Order != null && e.Order.Name == "Entry" && e.Order.OrderState == OrderState.Filled && e.Order.AverageFillPrice > 0)
			{
				TriggerCustomEvent(o =>
    			{
					Draw.Line(this, "myLine", false, 5, e.Order.AverageFillPrice + (20*TickSize), 0, e.Order.AverageFillPrice + (20*TickSize), Brushes.LimeGreen, DashStyleHelper.Solid, 2);
				}, null); 	
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 10)
				return;
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private DrawLineAboveEntryIndicatorNT8[] cacheDrawLineAboveEntryIndicatorNT8;
		public DrawLineAboveEntryIndicatorNT8 DrawLineAboveEntryIndicatorNT8()
		{
			return DrawLineAboveEntryIndicatorNT8(Input);
		}

		public DrawLineAboveEntryIndicatorNT8 DrawLineAboveEntryIndicatorNT8(ISeries<double> input)
		{
			if (cacheDrawLineAboveEntryIndicatorNT8 != null)
				for (int idx = 0; idx < cacheDrawLineAboveEntryIndicatorNT8.Length; idx++)
					if (cacheDrawLineAboveEntryIndicatorNT8[idx] != null &&  cacheDrawLineAboveEntryIndicatorNT8[idx].EqualsInput(input))
						return cacheDrawLineAboveEntryIndicatorNT8[idx];
			return CacheIndicator<DrawLineAboveEntryIndicatorNT8>(new DrawLineAboveEntryIndicatorNT8(), input, ref cacheDrawLineAboveEntryIndicatorNT8);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.DrawLineAboveEntryIndicatorNT8 DrawLineAboveEntryIndicatorNT8()
		{
			return indicator.DrawLineAboveEntryIndicatorNT8(Input);
		}

		public Indicators.DrawLineAboveEntryIndicatorNT8 DrawLineAboveEntryIndicatorNT8(ISeries<double> input )
		{
			return indicator.DrawLineAboveEntryIndicatorNT8(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.DrawLineAboveEntryIndicatorNT8 DrawLineAboveEntryIndicatorNT8()
		{
			return indicator.DrawLineAboveEntryIndicatorNT8(Input);
		}

		public Indicators.DrawLineAboveEntryIndicatorNT8 DrawLineAboveEntryIndicatorNT8(ISeries<double> input )
		{
			return indicator.DrawLineAboveEntryIndicatorNT8(input);
		}
	}
}

#endregion
