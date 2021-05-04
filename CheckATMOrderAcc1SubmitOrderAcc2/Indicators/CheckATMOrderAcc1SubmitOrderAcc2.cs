#region Using declarations
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public class CheckATMOrderAcc1SubmitOrderAcc2 : Indicator
	{
		private Account account1;
		private Account account2;
		private AtmStrategy startedAtm;
		
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "CheckATMOrderAcc1SubmitOrderAcc2";
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
				AccountName = "Sim101";
				AccountName2 = "Sim102";
				
			}
			else if (State == State.DataLoaded)
			{
				lock (Account.All)
				{
		            account1 = Account.All.FirstOrDefault(a => a.Name == AccountName);
					account2 = Account.All.FirstOrDefault(a => a.Name == AccountName2);
				}
		        // Subscribe to account item and order updates
		        if (account1 != null)
				{	            
					account1.OrderUpdate 		+= OnOrderUpdate;
				}
			}
			else if(State == State.Terminated)
			{
				// Make sure to unsubscribe to the account item subscription
        		if (account1 != null)
				{
					account1.OrderUpdate 		-= OnOrderUpdate;				
				}
			}
		}
		
	    private void OnOrderUpdate(object sender, OrderEventArgs e)
	    {
			if (e.Order.Account == account1)
			{
				Order sellOrder = null;
				Order buyOrder = null;
				
				if (e.Order.GetOwnerStrategy() != null)
				{
					AtmStrategy atm = e.Order.GetOwnerStrategy() as AtmStrategy;
					
					if (atm != null && e.Order.Name == "Entry")
					{
						if (e.OrderState == OrderState.Submitted && e.Order.IsLong == true)
						{
							buyOrder = account2.CreateOrder(Instrument, OrderAction.Buy, OrderType.Market, OrderEntry.Manual, TimeInForce.Day, 1, 0, 0, "", "Entry", DateTime.MaxValue, null);
							startedAtm = NinjaTrader.NinjaScript.AtmStrategy.StartAtmStrategy(atm.Template, buyOrder);
						}
						else if (e.OrderState == OrderState.Submitted && e.Order.IsShort == true)
						{
							sellOrder = account2.CreateOrder(Instrument, OrderAction.Sell, OrderType.Market, OrderEntry.Manual, TimeInForce.Day, 1, 0, 0, "", "Entry", DateTime.MaxValue, null);
							startedAtm = NinjaTrader.NinjaScript.AtmStrategy.StartAtmStrategy(atm.Template, sellOrder);
						}
					}
					
					if ((e.Order.Name == "Exit" || e.Order.Name == "Close") && e.OrderState == OrderState.Submitted)
					{
						if (startedAtm != null)
							startedAtm.CloseStrategy(e.Order.Name);
					}
				}
				else
				{
					if (e.OrderState == OrderState.Submitted && e.Order.IsLong == true)
					{
						buyOrder = account2.CreateOrder(Instrument, OrderAction.Buy, OrderType.Market, OrderEntry.Manual, TimeInForce.Day, 1, 0, 0, "", e.Order.Name, DateTime.MaxValue, null);
						account2.Submit(new[] { buyOrder });
					}
					else if (e.OrderState == OrderState.Submitted && e.Order.IsShort == true)
					{
						sellOrder = account2.CreateOrder(Instrument, OrderAction.Sell, OrderType.Market, OrderEntry.Manual, TimeInForce.Day, 1, 0, 0, "", e.Order.Name, DateTime.MaxValue, null);
						account2.Submit(new[] { sellOrder });
					}
				}
			}
	    }
		
		protected override void OnBarUpdate()
		{
			if (State == State.Historical)
				return;
			
			if (CurrentBar < 1)
				return;	

		}
		
		[TypeConverter(typeof(NinjaTrader.NinjaScript.AccountNameConverter))]
		public string AccountName { get; set; }
		
		[TypeConverter(typeof(NinjaTrader.NinjaScript.AccountNameConverter))]
		public string AccountName2 { get; set; }
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private CheckATMOrderAcc1SubmitOrderAcc2[] cacheCheckATMOrderAcc1SubmitOrderAcc2;
		public CheckATMOrderAcc1SubmitOrderAcc2 CheckATMOrderAcc1SubmitOrderAcc2()
		{
			return CheckATMOrderAcc1SubmitOrderAcc2(Input);
		}

		public CheckATMOrderAcc1SubmitOrderAcc2 CheckATMOrderAcc1SubmitOrderAcc2(ISeries<double> input)
		{
			if (cacheCheckATMOrderAcc1SubmitOrderAcc2 != null)
				for (int idx = 0; idx < cacheCheckATMOrderAcc1SubmitOrderAcc2.Length; idx++)
					if (cacheCheckATMOrderAcc1SubmitOrderAcc2[idx] != null &&  cacheCheckATMOrderAcc1SubmitOrderAcc2[idx].EqualsInput(input))
						return cacheCheckATMOrderAcc1SubmitOrderAcc2[idx];
			return CacheIndicator<CheckATMOrderAcc1SubmitOrderAcc2>(new CheckATMOrderAcc1SubmitOrderAcc2(), input, ref cacheCheckATMOrderAcc1SubmitOrderAcc2);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.CheckATMOrderAcc1SubmitOrderAcc2 CheckATMOrderAcc1SubmitOrderAcc2()
		{
			return indicator.CheckATMOrderAcc1SubmitOrderAcc2(Input);
		}

		public Indicators.CheckATMOrderAcc1SubmitOrderAcc2 CheckATMOrderAcc1SubmitOrderAcc2(ISeries<double> input )
		{
			return indicator.CheckATMOrderAcc1SubmitOrderAcc2(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.CheckATMOrderAcc1SubmitOrderAcc2 CheckATMOrderAcc1SubmitOrderAcc2()
		{
			return indicator.CheckATMOrderAcc1SubmitOrderAcc2(Input);
		}

		public Indicators.CheckATMOrderAcc1SubmitOrderAcc2 CheckATMOrderAcc1SubmitOrderAcc2(ISeries<double> input )
		{
			return indicator.CheckATMOrderAcc1SubmitOrderAcc2(input);
		}
	}
}

#endregion
