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
	public class UnmanagedTemplate : Strategy
	{
		private Order  shortEntry          = null;
        private Order  longEntry           = null;
        private Order  targetLong          = null;
        private Order  targetShort         = null;
        private Order  stopLossShort       = null;
        private Order  stopLossLong        = null;
		private string oco;

        private int sumFilledLong = 0; // This variable tracks the quantities of each execution making up the entry order
        private int sumFilledShort = 0; // This variable tracks the quantities of each execution making up the entry order

        protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"This is a template for Unmanaged strategies that can be used as reference for developing your own Unmanaged strategy.";
				Name										= "UnmanagedTemplate NT8";
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
				TraceOrders									= true;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				IsUnmanaged 								= true;
				IsAdoptAccountPositionAware 				= true;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				
				EntryDistance								= 4;
				ProfitDistance								= 8;
				StopDistance								= 4;
			}
			else if (State == State.Configure)
			{
				 AddDataSeries(BarsPeriodType.Tick, 1);
			}
			else if (State == State.Realtime)
			{
			    var aStrategyPosition = this.Position;
			    var aRealAccount = PositionAccount;
                // convert any old historical order object references
                // to the new live order submitted to the real-time account
			    if (shortEntry != null)
			        shortEntry = GetRealtimeOrder(shortEntry);
				if (longEntry != null)
			        longEntry = GetRealtimeOrder(longEntry);
				if (targetLong != null)
			        targetLong = GetRealtimeOrder(targetLong);
				if (targetShort != null)
			        targetShort = GetRealtimeOrder(targetShort);
				if (stopLossShort != null)
			        stopLossShort = GetRealtimeOrder(stopLossShort);
				if (stopLossLong != null)
			        stopLossLong = GetRealtimeOrder(stopLossLong);
			}
		}

		protected override void OnBarUpdate()
		{
			if (State == State.Historical)
				return;
			
			if (BarsInProgress != 0)
				return;
			// Submit OCO entry limit orders if we currently don't have an entry order open
            if (longEntry == null && shortEntry == null
                && Position.MarketPosition == MarketPosition.Flat)
            {
                /* The entry orders objects will take on a unique ID from our SubmitOrderUnmanaged() that we can use
                later for order identification purposes in the OnOrderUpdate() and OnExecution() methods. */
				if (State == State.Historical)
					oco = DateTime.Now.ToString() + CurrentBar + "entry";
				else
					oco = GetAtmStrategyUniqueId() + "entry";
				
				if (shortEntry == null)
                	SubmitOrderUnmanaged(1, OrderAction.SellShort, OrderType.Limit, 1, High[0]+EntryDistance*TickSize, 0, oco, "Short limit entry");
 				if (longEntry == null)
                	SubmitOrderUnmanaged(1, OrderAction.Buy, OrderType.Limit, 1, Low[0]-EntryDistance*TickSize, 0,  oco, "Long limit entry");
            }
		}
		
		protected override void OnExecutionUpdate(Execution execution, string executionId, double price, int quantity, MarketPosition marketPosition, string orderId, DateTime time)
        {
            /* We advise monitoring OnExecution to trigger submission of stop/target orders instead of OnOrderUpdate()
            since OnExecution() is called after OnOrderUpdate() which ensures your strategy has received the execution
            which is used for internal signal tracking. */
            if (longEntry != null && longEntry == execution.Order)
            {
                if (execution.Order.OrderState == OrderState.Filled
                    || execution.Order.OrderState == OrderState.PartFilled
                    || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                {
                    // We sum the quantities of each execution making up the entry order
                    sumFilledLong += execution.Quantity;

                    if (State == State.Historical)
						oco = DateTime.Now.ToString() + CurrentBar + "LongExits";
					else
						oco = GetAtmStrategyUniqueId() + "LongExits";

                    if (stopLossLong == null && targetLong == null)
                    {
                        SubmitOrderUnmanaged(1, OrderAction.Sell, OrderType.StopMarket, execution.Order.Filled, 0, execution.Order.AverageFillPrice - StopDistance * TickSize, oco, "StopLossLong");
                        SubmitOrderUnmanaged(1, OrderAction.Sell, OrderType.Limit, execution.Order.Filled, execution.Order.AverageFillPrice + ProfitDistance * TickSize, 0, oco, "TargetLong");
                    }
                    else
                    {
                        // Submit exit orders for partial fills
                        if (execution.Order.OrderState == OrderState.PartFilled)
                        {
                            ChangeOrder(stopLossLong, execution.Order.Filled, 0, execution.Order.AverageFillPrice - StopDistance * TickSize);
                            ChangeOrder(targetLong, execution.Order.Filled, execution.Order.AverageFillPrice + ProfitDistance * TickSize, 0);
                        }
                        // Update our exit order quantities once orderstate turns to filled and we have seen execution quantities match order quantities
                        else if (execution.Order.OrderState == OrderState.Filled && sumFilledLong == execution.Order.Filled)
                        {
                            // Stop-Loss order for OrderState.Filled
                            ChangeOrder(stopLossLong, execution.Order.Filled, 0, execution.Order.AverageFillPrice - StopDistance * TickSize);
                            ChangeOrder(targetLong, execution.Order.Filled, execution.Order.AverageFillPrice + ProfitDistance * TickSize, 0);
                        }

                    }

                    // Resets the entryOrder object and the sumFilled counter to null / 0 after the order has been filled
                    if (execution.Order.OrderState != OrderState.PartFilled && sumFilledLong == execution.Order.Filled)
                    {
                        longEntry = null;
                        sumFilledLong = 0;
                    }
                }
            }          
            if (shortEntry != null && shortEntry == execution.Order)
            {              
                if (execution.Order.OrderState == OrderState.Filled
                    || execution.Order.OrderState == OrderState.PartFilled
                    || (execution.Order.OrderState == OrderState.Cancelled && execution.Order.Filled > 0))
                {
                    // We sum the quantities of each execution making up the entry order
                    sumFilledShort += execution.Quantity;

                    if (State == State.Historical)
						oco = DateTime.Now.ToString() + CurrentBar + "ShortExits";
					else
						oco = GetAtmStrategyUniqueId() + "ShortExits";

                    if (stopLossShort == null && targetShort == null)
                    {
                        SubmitOrderUnmanaged(1, OrderAction.BuyToCover, OrderType.StopMarket, execution.Order.Filled, 0, execution.Order.AverageFillPrice + StopDistance * TickSize, oco, "StopLossShort");
                        SubmitOrderUnmanaged(1, OrderAction.BuyToCover, OrderType.Limit, execution.Order.Filled, execution.Order.AverageFillPrice - ProfitDistance * TickSize, 0, oco, "TargetShort");
                    }
                    else
                    {
                        // Submit exit orders for partial fills
                        if (execution.Order.OrderState == OrderState.PartFilled)
                        {
                            ChangeOrder(stopLossShort, execution.Order.Filled, 0, execution.Order.AverageFillPrice + StopDistance * TickSize);
                            ChangeOrder(targetShort, execution.Order.Filled, execution.Order.AverageFillPrice - ProfitDistance * TickSize, 0);
                        }
                        // Update our exit order quantities once orderstate turns to filled and we have seen execution quantities match order quantities
                        else if (execution.Order.OrderState == OrderState.Filled && sumFilledShort == execution.Order.Filled)
                        {
                            // Stop-Loss order for OrderState.Filled
                            ChangeOrder(stopLossShort, execution.Order.Filled, 0, execution.Order.AverageFillPrice + StopDistance * TickSize);
                            ChangeOrder(targetShort, execution.Order.Filled, execution.Order.AverageFillPrice - ProfitDistance * TickSize, 0);
                        }
                    }

                    // Resets the entryOrder object and the sumFilled counter to null / 0 after the order has been filled
                    if (execution.Order.OrderState != OrderState.PartFilled && sumFilledShort == execution.Order.Filled)
                    {
                        shortEntry  = null;
                        sumFilledShort = 0;
                    }
                }
            }
             
            // Reset our stop order and target orders' Order objects after our position is closed.
            if ((stopLossLong != null && stopLossLong == execution.Order) || (targetLong != null && targetLong == execution.Order))
            {
                if (execution.Order.OrderState == OrderState.Filled
                    || execution.Order.OrderState == OrderState.PartFilled)
                {
                    stopLossLong = null;
                    targetLong = null;
                }
            }
            if ((stopLossShort != null && stopLossShort == execution.Order) || (targetShort != null && targetShort == execution.Order))
            {
                if (execution.Order.OrderState == OrderState.Filled
                    || execution.Order.OrderState == OrderState.PartFilled)
                {
                    stopLossShort = null;
                    targetShort = null;
                }
            }
        }
		
		protected override void OnOrderUpdate(Order order, double limitPrice, double stopPrice, int quantity, int filled, double averageFillPrice, OrderState orderState, DateTime time, ErrorCode error, string nativeError)
        {
  			// Assign Order objects here
  			// This is more reliable than assigning Order objects in OnBarUpdate, as the assignment is not guaranteed to be complete if it is referenced immediately after submitting
  			if (order.Name == "Short limit entry")
      			shortEntry = order;
			else if (order.Name == "Long limit entry")
      			longEntry = order;
			else if (order.Name == "StopLossLong")
      			stopLossLong = order;
			else if (order.Name == "TargetLong")
      			targetLong = order;
			else if (order.Name == "StopLossShort")
      			stopLossShort = order;
			else if (order.Name == "TargetShort")
      			targetShort = order;
			
            if (longEntry != null && longEntry == order)
            {  
                // Reset the longTop object to null if order was cancelled without any fill
                if (order.OrderState == OrderState.Cancelled && order.Filled == 0)
                {
                    longEntry = null;
                    sumFilledLong = 0;
                }
            }
             
            if (shortEntry != null && shortEntry == order)
            {  
                // Reset the shortTop object to null if order was cancelled without any fill
                if (order.OrderState == OrderState.Cancelled && order.Filled == 0)
                {
                    shortEntry = null;
                    sumFilledShort = 0;
                }
            }
            //sets all targets and stops to null if one of them is canceled
            //PLEASE NOTE: setting IOrders to null ***DOES NOT*** cancel them
            if ((targetLong != null && targetLong == order)
                ||(stopLossLong != null && stopLossLong == order)
                ||(targetShort != null && targetShort == order)
                ||(stopLossShort != null && stopLossShort == order)
                )
            {  
                if (order.OrderState == OrderState.Cancelled && order.Filled == 0)
                {
                    targetLong = stopLossLong = targetShort = stopLossShort = null;
                }
            }
             
             
        }
		
		protected void CancelAll()
        {  
            if(shortEntry != null)
            {              
                CancelOrder(shortEntry);
            }
            if(longEntry != null)
            {              
                CancelOrder(longEntry);
            }
            if(targetLong != null)
            {              
                CancelOrder(targetLong);
            }
            if(targetShort != null)
            {              
                CancelOrder(targetShort);
            }
            if(stopLossShort != null)
            {              
                CancelOrder(stopLossShort);
            }
            if(stopLossLong != null)
            {              
                CancelOrder(stopLossLong);
            }              
        }
		
		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="EntryDistance", Description="Distance of Entry Orders from High/Low", Order=1, GroupName="Parameters")]
		public int EntryDistance
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ProfitDistance", Description="Distance of Profit Target from AvgEntryPrice", Order=2, GroupName="Parameters")]
		public int ProfitDistance
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="StopDistance", Description="Distance of Stop Loss from AvgEntryPrice", Order=3, GroupName="Parameters")]
		public int StopDistance
		{ get; set; }
		#endregion
	}
}
