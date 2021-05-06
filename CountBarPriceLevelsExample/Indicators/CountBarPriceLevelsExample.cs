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
	public class CountBarPriceLevelsExample : Indicator
	{
		private double count;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "CountBarPriceLevelsExample";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnMarketData(MarketDataEventArgs marketDataUpdate)
		{
			if(marketDataUpdate.MarketDataType == MarketDataType.Last)
			{
				
				int count = 0;
				for (double i = High[0]; i >= Low[0]; i-=TickSize)  
				{
				  count++;				
				}
				Print(CurrentBar + " High[0] " +  High[0] + " Low[0] " + Low[0]  + " count: " + count);
			}
		}

		protected override void OnBarUpdate()
		{
			
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private CountBarPriceLevelsExample[] cacheCountBarPriceLevelsExample;
		public CountBarPriceLevelsExample CountBarPriceLevelsExample()
		{
			return CountBarPriceLevelsExample(Input);
		}

		public CountBarPriceLevelsExample CountBarPriceLevelsExample(ISeries<double> input)
		{
			if (cacheCountBarPriceLevelsExample != null)
				for (int idx = 0; idx < cacheCountBarPriceLevelsExample.Length; idx++)
					if (cacheCountBarPriceLevelsExample[idx] != null &&  cacheCountBarPriceLevelsExample[idx].EqualsInput(input))
						return cacheCountBarPriceLevelsExample[idx];
			return CacheIndicator<CountBarPriceLevelsExample>(new CountBarPriceLevelsExample(), input, ref cacheCountBarPriceLevelsExample);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.CountBarPriceLevelsExample CountBarPriceLevelsExample()
		{
			return indicator.CountBarPriceLevelsExample(Input);
		}

		public Indicators.CountBarPriceLevelsExample CountBarPriceLevelsExample(ISeries<double> input )
		{
			return indicator.CountBarPriceLevelsExample(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.CountBarPriceLevelsExample CountBarPriceLevelsExample()
		{
			return indicator.CountBarPriceLevelsExample(Input);
		}

		public Indicators.CountBarPriceLevelsExample CountBarPriceLevelsExample(ISeries<double> input )
		{
			return indicator.CountBarPriceLevelsExample(input);
		}
	}
}

#endregion
