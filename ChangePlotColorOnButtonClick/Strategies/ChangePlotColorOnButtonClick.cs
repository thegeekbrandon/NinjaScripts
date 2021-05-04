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
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class ChangePlotColorOnButtonClick : Strategy
	{
		private bool myButtonClicked;
		private System.Windows.Controls.Button myButton;
		private System.Windows.Controls.Grid myGrid;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description							= @"ChangePlotColorOnButtonClick";
				Name								= "ChangePlotColorOnButtonClick";
				Calculate							= Calculate.OnEachTick;
				EntriesPerDirection					= 1;
				EntryHandling						= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy		= true;
				ExitOnSessionCloseSeconds			= 30;
				IsFillLimitOnTouch					= false;
				MaximumBarsLookBack					= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution					= OrderFillResolution.Standard;
				Slippage							= 0;
				StartBehavior						= StartBehavior.WaitUntilFlat;
				TimeInForce							= TimeInForce.Gtc;
				TraceOrders							= false;
				RealtimeErrorHandling				= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling					= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade					= 20;
				
				AddPlot(Brushes.Red, "MyPlot");
			}
			else if (State == State.Configure)
			{
			}
			else if (State == State.Historical)
			{
				if (UserControlCollection.Contains(myGrid))
					return;
				
				Dispatcher.InvokeAsync((() =>
				{
					myGrid = new System.Windows.Controls.Grid
					{
						Name = "MyCustomGrid", HorizontalAlignment = HorizontalAlignment.Right, VerticalAlignment = VerticalAlignment.Top
					};
					
					System.Windows.Controls.ColumnDefinition column1 = new System.Windows.Controls.ColumnDefinition();
					
					myGrid.ColumnDefinitions.Add(column1);
					
					myButton = new System.Windows.Controls.Button
					{
						Name = "ChangePlotColor", Content = "Change Plot Color", Foreground = Brushes.White, Background = Brushes.Gray
					};
					
					
					myButton.Click += OnButtonClick;
					
					System.Windows.Controls.Grid.SetColumn(myButton, 0);
					
					myGrid.Children.Add(myButton);
					
					UserControlCollection.Add(myGrid);
				}));
			}
			else if (State == State.Terminated)
			{
				Dispatcher.InvokeAsync((() =>
				{
					if (myGrid != null)
					{
						if (myButton != null)
						{
							myGrid.Children.Remove(myButton);
							myButton.Click -= OnButtonClick;
							myButton = null;
						}
					}
				}));
			}
		}

		protected override void OnBarUpdate()
		{
			Values[0][0] = Close[0];
			
			if (myButtonClicked)
				for (int i = 0; i < CurrentBar; i++)
				{
					PlotBrushes[0][i] = Brushes.Yellow;
				}
			
			if (myButtonClicked == false)
				for (int i = 0; i < CurrentBar; i++)
				{
					PlotBrushes[0][i] = Brushes.Red;
				}
		}
		
		private void OnButtonClick(object sender, RoutedEventArgs rea)
		{
			System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
			if (button == myButton && button.Name == "ChangePlotColor" && button.Content == "Change Plot Color")
			{
				button.Content = "Plot Yellow";
				button.Name = "ChangeBackPlotColor";
				myButtonClicked = true;
				IsVisible = true;
				return;
			}
			
			if (button == myButton && button.Name == "ChangeBackPlotColor" && button.Content == "Plot Yellow")
			{
				button.Content = "Change Plot Color";
				button.Name = "ChangePlotColor";
				myButtonClicked = false;
				return;
			}
		}
	}
}
