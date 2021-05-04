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
	public class AddPlotExample : Indicator
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "AddPlotExample";
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
				AddPlot(Brushes.Blue, "MyPlot");
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			MyPlot[0] = Close[7];
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> MyPlot
		{
			get { return Values[0]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private AddPlotExample[] cacheAddPlotExample;
		public AddPlotExample AddPlotExample()
		{
			return AddPlotExample(Input);
		}

		public AddPlotExample AddPlotExample(ISeries<double> input)
		{
			if (cacheAddPlotExample != null)
				for (int idx = 0; idx < cacheAddPlotExample.Length; idx++)
					if (cacheAddPlotExample[idx] != null &&  cacheAddPlotExample[idx].EqualsInput(input))
						return cacheAddPlotExample[idx];
			return CacheIndicator<AddPlotExample>(new AddPlotExample(), input, ref cacheAddPlotExample);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.AddPlotExample AddPlotExample()
		{
			return indicator.AddPlotExample(Input);
		}

		public Indicators.AddPlotExample AddPlotExample(ISeries<double> input )
		{
			return indicator.AddPlotExample(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.AddPlotExample AddPlotExample()
		{
			return indicator.AddPlotExample(Input);
		}

		public Indicators.AddPlotExample AddPlotExample(ISeries<double> input )
		{
			return indicator.AddPlotExample(input);
		}
	}
}

#endregion
