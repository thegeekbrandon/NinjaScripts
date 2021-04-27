#region Using declarations
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript.DrawingTools;

using NinjaTrader.Cbi;
using NinjaTrader.Core;
using NinjaTrader.Gui;
using NinjaTrader.Gui.NinjaScript.AtmStrategy;
using System.Linq;
using System.Windows.Data;
using System.Xml.Linq;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators
{
	public class ChartToolbarCustomMenus : Indicator
	{
		
		#region Common chart object variables
		private System.Windows.Controls.Grid				chartGrid;
		private NinjaTrader.Gui.Chart.ChartTab				chartTab;
		private System.Windows.Style						mainMenuItemStyle, systemMenuStyle;
		private NinjaTrader.Gui.Chart.Chart					chartWindow;
		private System.Windows.Media.SolidColorBrush		controlFontBrush;
		private System.Windows.Media.LinearGradientBrush	controlBackgroundBrush, dropMenuBackgroundBrush;
		private System.Windows.Controls.TabItem				tabItem;
		#endregion
		
		private bool										ntBarActive;
		private Menu										ntBarMenu, ntBarMenu2;
		private NinjaTrader.Gui.Tools.NTMenuItem			ntBartopMenuItem, ntBartopMenuItemSubItem1, ntBartopMenuItemSubItem2, ntBartopMenuItem2 ,ntBartopMenuItem2SubItem1, ntBartopMenuItem2SubItem2;
		private SolidColorBrush								chartBackgroundBrush, controlBorderBrush, subMenuBrush;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Indicator here.";
				Name										= "ChartToolbarCustomMenus";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= false;
				PaintPriceMarkers							= false;
				IsSuspendedWhileInactive					= false;
			}
			else if (State == State.DataLoaded)
			{
				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					{
						LoadBrushesFromSkin();
						CreateWPFControls();
					}));
				}
			}
			else if (State == State.Terminated)
			{
				if (ChartControl != null)
				{
					ChartControl.Dispatcher.InvokeAsync((Action)(() =>
					{
						DisposeWPFControls();
					}));
				}
			}
		}
		
		protected void CreateWPFControls()
		{
			// the main chart window
			chartWindow			= System.Windows.Window.GetWindow(ChartControl.Parent) as Chart;
			// if not added to a chart, do nothing
			if (chartWindow == null)
				return;
			// this is the grid in the chart window
			chartGrid			= chartWindow.MainTabControl.Parent as Grid;
			
			// this is the actual object that you add to the chart windows Main Menu
			// which will act as a container for all the menu items
			ntBarMenu = new Menu
			{
				// important to set the alignment, otherwise you will never see the menu populated
				VerticalAlignment			= VerticalAlignment.Top,
				VerticalContentAlignment	= VerticalAlignment.Top,
				// make sure to style as a System Menu	
				Style						= systemMenuStyle
			};
			
			ntBarMenu2 = new Menu
			{
				// important to set the alignment, otherwise you will never see the menu populated
				VerticalAlignment			= VerticalAlignment.Top,
				VerticalContentAlignment	= VerticalAlignment.Top,
				// make sure to style as a System Menu	
				Style						= systemMenuStyle
			};

			// thanks to Jesse for these figures to use for the icon
			System.Windows.Media.Geometry topMenuItem1Icon = Geometry.Parse("m 70.5 173.91921 c -4.306263 -1.68968 -4.466646 -2.46776 -4.466646 -21.66921 0 -23.88964 -1.364418 -22.5 22.091646 -22.5 23.43572 0 22.08568 -1.36412 22.10832 22.33888 0.0184 19.29356 -0.19638 20.3043 -4.64473 21.85501 -2.91036 1.01455 -32.493061 0.99375 -35.08859 -0.0247 z M 21 152.25 l 0 -7.5 20.25 0 20.25 0 0 7.5 0 7.5 -20.25 0 -20.25 0 0 -7.5 z m 93.75 0 0 -7.5 42.75 0 42.75 0 0 7.5 0 7.5 -42.75 0 -42.75 0 0 -7.5 z m 15.75 -38.33079 c -4.30626 -1.68968 -4.46665 -2.46775 -4.46665 -21.66921 0 -23.889638 -1.36441 -22.5 22.09165 -22.5 23.43572 0 22.08568 -1.364116 22.10832 22.338885 0.0185 19.293555 -0.19638 20.304295 -4.64473 21.855005 -2.91036 1.01455 -32.49306 0.99375 -35.08859 -0.0247 z M 21 92.25 l 0 -7.5 50.25 0 50.25 0 0 7.5 0 7.5 -50.25 0 -50.25 0 0 -7.5 z m 153.75 0 0 -7.5 12.75 0 12.75 0 0 7.5 0 7.5 -12.75 0 -12.75 0 0 -7.5 z M 55.5 53.919211 C 51.193737 52.229528 51.033354 51.451456 51.033354 32.25 51.033354 8.3603617 49.668936 9.75 73.125 9.75 96.560723 9.75 95.210685 8.3858835 95.23332 32.088887 95.25177 51.382441 95.03694 52.393181 90.588593 53.943883 87.678232 54.95844 58.095529 54.93764 55.5 53.919211 Z M 21 32.25 l 0 -7.5 12.75 0 12.75 0 0 7.5 0 7.5 -12.75 0 -12.75 0 0 -7.5 z m 78.75 0 0 -7.5 50.25 0 50.25 0 0 7.5 0 7.5 -50.25 0 -50.25 0 0 -7.5 z");

			// this is the menu item which will appear on the chart's Main Menu
			ntBartopMenuItem = new NTMenuItem()
			{
				// comment out or delete the Header assignment below to only show the icon
				Header				= "Menu 1",
				Icon				= topMenuItem1Icon,
				Margin				= new Thickness(0),
				Padding				= new Thickness(1),
				Style				= mainMenuItemStyle,
				VerticalAlignment	= VerticalAlignment.Center
			};

			ntBarMenu.Items.Add(ntBartopMenuItem);

			ntBartopMenuItemSubItem1 = new NTMenuItem()
			{
				BorderThickness		= new Thickness(0),
				Header				= "Enable"
			};

			ntBartopMenuItemSubItem1.Click += NTBarMenu_Click;
			ntBartopMenuItem.Items.Add(ntBartopMenuItemSubItem1);

			ntBartopMenuItemSubItem2 = new NTMenuItem()
			{
				Header				= "Disable"
			};

			ntBartopMenuItemSubItem2.Click += NTBarMenu_Click;
			ntBartopMenuItem.Items.Add(ntBartopMenuItemSubItem2);
			
			ntBartopMenuItem2 = new NTMenuItem()
			{
				// comment out or delete the Header assignment below to only show the icon
				Header				= "Menu 2",
				Icon				= topMenuItem1Icon,
				Margin				= new Thickness(0),
				Padding				= new Thickness(1),
				Style				= mainMenuItemStyle,
				VerticalAlignment	= VerticalAlignment.Center
			};
			
			ntBarMenu2.Items.Add(ntBartopMenuItem2);

			ntBartopMenuItem2SubItem1 = new NTMenuItem()
			{
				BorderThickness		= new Thickness(0),
				Header				= "Enable"
			};

			ntBartopMenuItem2SubItem1.Click += NTBarMenu_Click2;
			ntBartopMenuItem2.Items.Add(ntBartopMenuItem2SubItem1);

			ntBartopMenuItem2SubItem2 = new NTMenuItem()
			{
				Header				= "Disable"
			};

			ntBartopMenuItem2SubItem2.Click += NTBarMenu_Click2;
			ntBartopMenuItem2.Items.Add(ntBartopMenuItem2SubItem2);

			// add the menu which contains all menu items to the chart
			//chartWindow.MainMenu.Add(ntBarMenu);
			
			if (TabSelected())
				ShowWPFControls();

			chartWindow.MainTabControl.SelectionChanged += TabChangedHandler;
		}
		
		private void DisposeWPFControls()
		{
			if (chartWindow != null)
				chartWindow.MainTabControl.SelectionChanged -= TabChangedHandler;

			HideWPFControls();
			
			if (ntBartopMenuItemSubItem1 != null)
				ntBartopMenuItemSubItem1.Click -= NTBarMenu_Click;

			if (ntBartopMenuItemSubItem2 != null)
				ntBartopMenuItemSubItem2.Click -= NTBarMenu_Click;

			if (ntBarMenu != null)
			{
				chartWindow.MainMenu.Remove(ntBarMenu);
				ntBarActive = false;
			}
			
			if (ntBartopMenuItem2SubItem1 != null)
				ntBartopMenuItem2SubItem1.Click -= NTBarMenu_Click2;

			if (ntBartopMenuItem2SubItem2 != null)
				ntBartopMenuItem2SubItem2.Click -= NTBarMenu_Click2;

			if (ntBarMenu2 != null)
			{
				chartWindow.MainMenu.Remove(ntBarMenu2);
				ntBarActive = false;
			}
		}
		
		private void HideWPFControls()
		{
			if (ntBarActive)
			{
				chartWindow.MainMenu.Remove(ntBarMenu);
				chartWindow.MainMenu.Remove(ntBarMenu2);
				ntBarActive					= false;
			}
			
			
		}
		
		private void LoadBrushesFromSkin()
		{
			// while pulling brushes from a skin to use later in the chart,
			// sometimes we need to be in the thread of the chart when the brush is initialized

			chartBackgroundBrush		= System.Windows.Application.Current.TryFindResource("ChartControl.ChartBackground") as SolidColorBrush ?? new SolidColorBrush(Brushes.Purple.Color);
			mainMenuItemStyle			= Application.Current.TryFindResource("MainMenuItem") as Style;
			systemMenuStyle				= Application.Current.TryFindResource("SystemMenuStyle") as Style;

			controlFontBrush			= Application.Current.TryFindResource("FontButtonBrush") as SolidColorBrush ?? new SolidColorBrush(Brushes.Purple.Color);
			controlBackgroundBrush		= Application.Current.TryFindResource("ButtonBackgroundBrush") as LinearGradientBrush ?? new LinearGradientBrush(Colors.Purple, Colors.Pink, 1);
			controlBorderBrush			= Application.Current.TryFindResource("ButtonBorderBrush") as SolidColorBrush ?? Brushes.Purple;
			dropMenuBackgroundBrush		= Application.Current.TryFindResource("ComboBoxBackgroundBrush") as LinearGradientBrush ?? new LinearGradientBrush(Colors.Purple, Colors.Pink, 1);
			subMenuBrush				= Application.Current.TryFindResource("SubMenuBackground") as SolidColorBrush ?? new SolidColorBrush(Brushes.Purple.Color);
			
		}

		protected override void OnBarUpdate() { }
		
		private void ShowWPFControls()
		{
			if (!ntBarActive)
			{
				chartWindow.MainMenu.Add(ntBarMenu);
				chartWindow.MainMenu.Add(ntBarMenu2);
				ntBarActive					= true;
			}
		}
		
		protected void NTBarMenu_Click(object sender, RoutedEventArgs eventArgs)
		{
			MenuItem menuItem = sender as MenuItem;

			if (menuItem == ntBartopMenuItemSubItem1)
				Draw.TextFixed(this, "tag1", "Titlebar > NTBar Menu 1> Sub-MenuItem 1", TextPosition.TopLeft, Brushes.Green, new SimpleFont("Arial", 14), Brushes.Transparent, Brushes.Transparent, 100);

			else if (menuItem == ntBartopMenuItemSubItem2)
				RemoveDrawObject("tag1");
			
			ForceRefresh();
		}
		
		protected void NTBarMenu_Click2(object sender, RoutedEventArgs eventArgs)
		{
			MenuItem menuItem2 = sender as MenuItem;

			if (menuItem2 == ntBartopMenuItem2SubItem1)
				Draw.TextFixed(this, "tag2", "Titlebar > NTBar Menu 2> Sub-MenuItem 2", TextPosition.TopRight, Brushes.Green, new SimpleFont("Arial", 14), Brushes.Transparent, Brushes.Transparent, 100);

			else if (menuItem2 == ntBartopMenuItem2SubItem2)
				RemoveDrawObject("tag2");
			
			ForceRefresh();
		}
		
		private bool TabSelected()
		{
			bool tabSelected = false;

			if (ChartControl.ChartTab == ((chartWindow.MainTabControl.Items.GetItemAt(chartWindow.MainTabControl.SelectedIndex) as TabItem).Content as ChartTab))
				tabSelected = true;

			return tabSelected;
		}
		
		// Runs ShowWPFControls if this is the selected chart tab, other wise runs HideWPFControls()
		private void TabChangedHandler(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count <= 0)
				return;

			tabItem = e.AddedItems[0] as TabItem;
			if (tabItem == null)
				return;

			chartTab = tabItem.Content as ChartTab;
			if (chartTab == null)
				return;

			if (TabSelected())
				ShowWPFControls();
			else
				HideWPFControls();
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ChartToolbarCustomMenus[] cacheChartToolbarCustomMenus;
		public ChartToolbarCustomMenus ChartToolbarCustomMenus()
		{
			return ChartToolbarCustomMenus(Input);
		}

		public ChartToolbarCustomMenus ChartToolbarCustomMenus(ISeries<double> input)
		{
			if (cacheChartToolbarCustomMenus != null)
				for (int idx = 0; idx < cacheChartToolbarCustomMenus.Length; idx++)
					if (cacheChartToolbarCustomMenus[idx] != null &&  cacheChartToolbarCustomMenus[idx].EqualsInput(input))
						return cacheChartToolbarCustomMenus[idx];
			return CacheIndicator<ChartToolbarCustomMenus>(new ChartToolbarCustomMenus(), input, ref cacheChartToolbarCustomMenus);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ChartToolbarCustomMenus ChartToolbarCustomMenus()
		{
			return indicator.ChartToolbarCustomMenus(Input);
		}

		public Indicators.ChartToolbarCustomMenus ChartToolbarCustomMenus(ISeries<double> input )
		{
			return indicator.ChartToolbarCustomMenus(input);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ChartToolbarCustomMenus ChartToolbarCustomMenus()
		{
			return indicator.ChartToolbarCustomMenus(Input);
		}

		public Indicators.ChartToolbarCustomMenus ChartToolbarCustomMenus(ISeries<double> input )
		{
			return indicator.ChartToolbarCustomMenus(input);
		}
	}
}

#endregion
