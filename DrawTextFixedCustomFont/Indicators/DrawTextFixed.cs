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
	public class DrawTextFixed : Indicator
	{
		private NinjaTrader.Gui.Tools.SimpleFont myFont;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "DrawTextFixed";
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
			}
			else if (State == State.DataLoaded)
			{
				myFont = new NinjaTrader.Gui.Tools.SimpleFont("Courier New", 20);
			}
		}

		protected override void OnBarUpdate()
		{
			
			Draw.TextFixed(this, "myTextFixed", "Hello world!", TextPosition.BottomRight, ChartControl.Properties.ChartText, myFont, Brushes.Blue, Brushes.Transparent, 0);
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private DrawTextFixed[] cacheDrawTextFixed;
		public DrawTextFixed DrawTextFixed()
		{
			return DrawTextFixed(Input);
		}

		public DrawTextFixed DrawTextFixed(ISeries<double> input)
		{
			if (cacheDrawTextFixed != null)
				for (int idx = 0; idx < cacheDrawTextFixed.Length; idx++)
					if (cacheDrawTextFixed[idx] != null &&  cacheDrawTextFixed[idx].EqualsInput(input))
						return cacheDrawTextFixed[idx];
			return CacheIndicator<DrawTextFixed>(new DrawTextFixed(), input, ref cacheDrawTextFixed);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.DrawTextFixed DrawTextFixed()
		{
			return indicator.DrawTextFixed(Input);
		}

		public Indicators.DrawTextFixed DrawTextFixed(ISeries<double> input )
		{
			return indicator.DrawTextFixed(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.DrawTextFixed DrawTextFixed()
		{
			return indicator.DrawTextFixed(Input);
		}

		public Indicators.DrawTextFixed DrawTextFixed(ISeries<double> input )
		{
			return indicator.DrawTextFixed(input);
		}
	}
}

#endregion
