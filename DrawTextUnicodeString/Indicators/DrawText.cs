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
	public class DrawText : Indicator
	{
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "DrawText";
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

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 1)
				return;
			
			Draw.Text(this, "tag1", "◀", 0, Close[0] + 5*TickSize, Brushes.DodgerBlue);
			Draw.Text(this, "tag2", "▶", 0, Close[0] - 5*TickSize, Brushes.HotPink);
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private DrawText[] cacheDrawText;
		public DrawText DrawText()
		{
			return DrawText(Input);
		}

		public DrawText DrawText(ISeries<double> input)
		{
			if (cacheDrawText != null)
				for (int idx = 0; idx < cacheDrawText.Length; idx++)
					if (cacheDrawText[idx] != null &&  cacheDrawText[idx].EqualsInput(input))
						return cacheDrawText[idx];
			return CacheIndicator<DrawText>(new DrawText(), input, ref cacheDrawText);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.DrawText DrawText()
		{
			return indicator.DrawText(Input);
		}

		public Indicators.DrawText DrawText(ISeries<double> input )
		{
			return indicator.DrawText(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.DrawText DrawText()
		{
			return indicator.DrawText(Input);
		}

		public Indicators.DrawText DrawText(ISeries<double> input )
		{
			return indicator.DrawText(input);
		}
	}
}

#endregion
