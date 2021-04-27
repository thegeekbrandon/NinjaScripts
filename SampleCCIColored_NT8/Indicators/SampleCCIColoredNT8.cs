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
	public class SampleCCIColoredNT8 : Indicator
	{
		private int period;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "SampleCCIColored";
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
				Period					= 14;
				AddPlot(Brushes.Orange, "SampleCCIColored");
				AddLine(Brushes.DarkGray, 200, "Level 2");
				AddLine(Brushes.DarkGray, 100, "Level 1");
				AddLine(Brushes.DarkGray, 0, "Zero line");
				AddLine(Brushes.DarkGray, -100, "Level -1");
				AddLine(Brushes.DarkGray, -200, "Level -2");
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar == 0)
				SampleCCIColored[0] = 0.00;
			else
			{
				double mean = 0;
				for (int idx = Math.Min(CurrentBar, Period - 1); idx >= 0; idx--)
					mean += Math.Abs(Typical[idx] - SMA(Typical, Period)[0]);
				SampleCCIColored[0] = ((Typical[0] - SMA(Typical, Period)[0]) / (mean == 0 ? 1 : (0.015 * (mean / Math.Min(Period, CurrentBar + 1)))));
			}
			//condition statement to change color of CCI plot if over bought or over sold
			if(SampleCCIColored[0] > 0)
			{
				PlotBrushes[0][0] = Brushes.Red;
			}	
			if(SampleCCIColored[0] < 0)
			{
				PlotBrushes[0][0] = Brushes.Green;
			}
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Period", Order=1, GroupName="Parameters")]
		public int Period
		{ 
		  get { return period; }
		  set { period = Math.Max(1, value); }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> SampleCCIColored
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
		private SampleCCIColoredNT8[] cacheSampleCCIColoredNT8;
		public SampleCCIColoredNT8 SampleCCIColoredNT8(int period)
		{
			return SampleCCIColoredNT8(Input, period);
		}

		public SampleCCIColoredNT8 SampleCCIColoredNT8(ISeries<double> input, int period)
		{
			if (cacheSampleCCIColoredNT8 != null)
				for (int idx = 0; idx < cacheSampleCCIColoredNT8.Length; idx++)
					if (cacheSampleCCIColoredNT8[idx] != null && cacheSampleCCIColoredNT8[idx].Period == period && cacheSampleCCIColoredNT8[idx].EqualsInput(input))
						return cacheSampleCCIColoredNT8[idx];
			return CacheIndicator<SampleCCIColoredNT8>(new SampleCCIColoredNT8(){ Period = period }, input, ref cacheSampleCCIColoredNT8);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.SampleCCIColoredNT8 SampleCCIColoredNT8(int period)
		{
			return indicator.SampleCCIColoredNT8(Input, period);
		}

		public Indicators.SampleCCIColoredNT8 SampleCCIColoredNT8(ISeries<double> input , int period)
		{
			return indicator.SampleCCIColoredNT8(input, period);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.SampleCCIColoredNT8 SampleCCIColoredNT8(int period)
		{
			return indicator.SampleCCIColoredNT8(Input, period);
		}

		public Indicators.SampleCCIColoredNT8 SampleCCIColoredNT8(ISeries<double> input , int period)
		{
			return indicator.SampleCCIColoredNT8(input, period);
		}
	}
}

#endregion
