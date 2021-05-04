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
	public class ClickDrawRectangleWithText : Indicator
	{
		private Point							clickPoint	= new Point();
		private bool							clickSet	= false;
		
		private Account account;
		private Position position;
		
		private double realizedPnL;
		private double unRealizedPnL;
		private double cashValue;
		
		private SharpDX.Direct2D1.Brush labelTextBrush;
		private SharpDX.Direct2D1.Brush areaBrush;
		private SharpDX.Direct2D1.Brush borderBrush;
		private SharpDX.Direct2D1.Brush positiveBrush;
		private SharpDX.Direct2D1.Brush negativeBrush;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Demonstrates how to capture a mouse click and adjust for DPI settings properly";
				Name										= "ClickDrawRectangleWithText";
				Calculate									= Calculate.OnPriceChange;
				IsOverlay									= true;
				DisplayInDataBox							= false;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				
				Font 										= new SimpleFont("Arial", 12);
				PerformanceUnit								= PerformanceUnit.Currency;
				VerticalOffset								= 20;
				HorizontalOffset							= 20;
				LabelTextBrush								= Brushes.Black;
				AreaBrush									= Brushes.White;
				BorderBrush									= Brushes.Black;
				PositiveBrush								= Brushes.Lime;
				NegativeBrush								= Brushes.Red;
			}
			else if (State == State.DataLoaded)
			{				
				// Find our account
				lock (Account.All)
					account = Account.All.FirstOrDefault(a => a.Name == AccountName);

				// Subscribe to account item updates
				if (account != null)
				{
					account.AccountItemUpdate += OnAccountItemUpdate;
					account.PositionUpdate += OnPositionUpdate;
					
					foreach (Position pos in account.Positions)
						if (pos.Instrument == Instrument)
							position = pos;
					
					realizedPnL = account.Get(AccountItem.RealizedProfitLoss, Currency.UsDollar);
					cashValue = account.Get(AccountItem.CashValue, Currency.UsDollar);
				}
			}
			else if (State == State.Historical)
			{
				if (ChartControl != null)
					ChartPanel.MouseLeftButtonDown += MouseClicked;
			}
			else if (State == State.Terminated)
			{
				// Make sure to unsubscribe to the account item subscription
        		if (account != null)
				{
            		account.AccountItemUpdate -= OnAccountItemUpdate;
					account.PositionUpdate -= OnPositionUpdate;
				}
				
				if (ChartControl != null)
					ChartPanel.MouseLeftButtonDown -= MouseClicked;
			}
		}
		
		private void OnAccountItemUpdate(object sender, AccountItemEventArgs e)
		{
			if (e.AccountItem == AccountItem.RealizedProfitLoss)
				realizedPnL = e.Value;
			if (e.AccountItem == AccountItem.CashValue)
				cashValue = e.Value;
		}
		
		private void OnPositionUpdate(object sender, PositionEventArgs e)
		{
			if (e.Position.Instrument == Instrument && e.MarketPosition == MarketPosition.Flat)
				position = null;
			else if (e.Position.Instrument == Instrument && e.MarketPosition != MarketPosition.Flat)
				position = e.Position;	
		}

		protected override void OnBarUpdate() {	}
				
		protected void MouseClicked(object sender, MouseButtonEventArgs e)
		{
			// convert e.GetPosition for different dpi settings
			clickPoint.X = ChartingExtensions.ConvertToHorizontalPixels(e.GetPosition(ChartControl as IInputElement).X, ChartControl.PresentationSource);
			clickPoint.Y = ChartingExtensions.ConvertToVerticalPixels(e.GetPosition(ChartControl as IInputElement).Y, ChartControl.PresentationSource);

			if (clickPoint.Y > 0)
				clickSet = !clickSet;
			
			// trigger the chart invalidate so that the render loop starts even if there is no data being received
			ChartControl.InvalidateVisual();
			e.Handled = true;
		}
		
		public override void OnRenderTargetChanged()
		{
			if(RenderTarget != null)
			{
				if (labelTextBrush != null)
					labelTextBrush.Dispose();
				if (areaBrush != null)
					areaBrush.Dispose();
				if (borderBrush != null)
					borderBrush.Dispose();
				if (positiveBrush != null)
					positiveBrush.Dispose();
				if (negativeBrush != null)
					negativeBrush.Dispose();

				labelTextBrush 	= LabelTextBrush.ToDxBrush(RenderTarget);
				areaBrush 		= AreaBrush.ToDxBrush(RenderTarget);
				borderBrush 	= BorderBrush.ToDxBrush(RenderTarget);
				positiveBrush 	= PositiveBrush.ToDxBrush(RenderTarget);
				negativeBrush 	= NegativeBrush.ToDxBrush(RenderTarget);
			}
		}

		protected override void OnRender(ChartControl chartControl, ChartScale chartScale)
		{
			// if the click is set we have a dpi converted x and y position.
			// create the rendering object in OnRender so that any brushes used are created for the renderTarget
			if (clickSet)
			{
				base.OnRender(chartControl, chartScale);
				if (position != null)
					unRealizedPnL = position.GetUnrealizedProfitLoss(PerformanceUnit, Close.GetValueAt(CurrentBar));
				else
					unRealizedPnL = 0;
				
				string label 				= "UnRealized PnL" +"\n\n"+ "Realized PnL" +"\n\n"+ "Account Balance" +"\n";
				string realizedPnLstring 	= "\n" + "\n" + "\n" + realizedPnL.ToString("N2");
				string cashValuestring 		= "\n" + "\n" + "\n" + "\n" + "\n" + cashValue.ToString("N2");
				string unRealizedPnLstring;
				
				if (PerformanceUnit == PerformanceUnit.Percent)
					unRealizedPnLstring		= "\n" + unRealizedPnL.ToString("P");
				else
					unRealizedPnLstring		= "\n" + unRealizedPnL.ToString("N2");
				
				DrawTextBox(RenderTarget, ChartPanel, label, Font, labelTextBrush, (float)clickPoint.X + HorizontalOffset, (float)clickPoint.Y + VerticalOffset, areaBrush, borderBrush);

				DrawTextLayout(RenderTarget, ChartPanel, (float)clickPoint.X + HorizontalOffset, (float)clickPoint.Y + VerticalOffset, unRealizedPnLstring, Font, unRealizedPnL < 0 ? negativeBrush : positiveBrush);
				DrawTextLayout(RenderTarget, ChartPanel, (float)clickPoint.X + HorizontalOffset, (float)clickPoint.Y + VerticalOffset,   realizedPnLstring, Font,   realizedPnL < 0 ? negativeBrush : positiveBrush);
				DrawTextLayout(RenderTarget, ChartPanel, (float)clickPoint.X + HorizontalOffset, (float)clickPoint.Y + VerticalOffset,     cashValuestring, Font,     cashValue < 0 ? negativeBrush : positiveBrush);
				
			}
		}

		public void DrawTextBox(SharpDX.Direct2D1.RenderTarget renderTarget, ChartPanel chartPanel, string text, SimpleFont font, SharpDX.Direct2D1.Brush brush, double pointX, double pointY, SharpDX.Direct2D1.Brush areaBrush, SharpDX.Direct2D1.Brush borderBrush)
		{
			SharpDX.DirectWrite.TextFormat textFormat = font.ToDirectWriteTextFormat();
			SharpDX.DirectWrite.TextLayout textLayout =
			new SharpDX.DirectWrite.TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory,
				text, textFormat, 1000,
				textFormat.FontSize);
			SharpDX.Vector2 TextPlotPoint = new System.Windows.Point(pointX, pointY).ToVector2();
			
			float newW = textLayout.Metrics.Width; 
            float newH = textLayout.Metrics.Height;
			
            SharpDX.RectangleF PLBoundRect = new SharpDX.RectangleF((float)pointX - 2, (float)pointY - 2, newW + 4, newH + 4);
            renderTarget.FillRectangle(PLBoundRect, areaBrush);
			renderTarget.DrawRectangle(PLBoundRect, borderBrush);
			
			renderTarget.DrawTextLayout(TextPlotPoint, textLayout, brush, SharpDX.Direct2D1.DrawTextOptions.NoSnap);
			textLayout.Dispose();
			textFormat.Dispose();
		}
		
		public void DrawTextLayout(SharpDX.Direct2D1.RenderTarget renderTarget, ChartPanel chartPanel, float x, float y, string text, SimpleFont font, SharpDX.Direct2D1.Brush defaultForegroundBrush)
		{
			SharpDX.Vector2 origin = new SharpDX.Vector2(x, y);
			SharpDX.DirectWrite.TextFormat textFormat = font.ToDirectWriteTextFormat();
			SharpDX.DirectWrite.TextLayout textLayout = new SharpDX.DirectWrite.TextLayout(NinjaTrader.Core.Globals.DirectWriteFactory,
														text, textFormat, 1000,
														textFormat.FontSize);
			
			DrawTextLayout(renderTarget, origin, textLayout, defaultForegroundBrush);
			textFormat.Dispose();
			textFormat = null;
			textLayout.Dispose();
			textLayout = null;
		}
		
		public void DrawTextLayout(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Vector2 origin, SharpDX.DirectWrite.TextLayout textLayout, SharpDX.Direct2D1.Brush defaultForegroundBrush)
		{
			renderTarget.DrawTextLayout(origin, textLayout, defaultForegroundBrush);
		}
		
		#region Properties
		[TypeConverter(typeof(NinjaTrader.NinjaScript.AccountNameConverter))]
		public string AccountName { get; set; }
		
		public SimpleFont Font { get; set; }
		
		public PerformanceUnit PerformanceUnit { get; set; }
		
		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="VerticalOffset", Order=3, GroupName="Parameters")]
		public int VerticalOffset
		{ get; set; }

		[NinjaScriptProperty]
		[Range(0, int.MaxValue)]
		[Display(Name="HorizontalOffset", Order=4, GroupName="Parameters")]
		public int HorizontalOffset
		{ get; set; }

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="LabelTextBrush", Order=5, GroupName="Parameters")]
		public Brush LabelTextBrush
		{ get; set; }

		[Browsable(false)]
		public string LabelTextBrushSerializable
		{
			get { return Serialize.BrushToString(LabelTextBrush); }
			set { LabelTextBrush = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="AreaBrush", Order=6, GroupName="Parameters")]
		public Brush AreaBrush
		{ get; set; }

		[Browsable(false)]
		public string AreaBrushSerializable
		{
			get { return Serialize.BrushToString(AreaBrush); }
			set { AreaBrush = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="BorderBrush", Order=7, GroupName="Parameters")]
		public Brush BorderBrush
		{ get; set; }

		[Browsable(false)]
		public string BorderBrushSerializable
		{
			get { return Serialize.BrushToString(BorderBrush); }
			set { BorderBrush = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="PositiveBrush", Order=8, GroupName="Parameters")]
		public Brush PositiveBrush
		{ get; set; }

		[Browsable(false)]
		public string PositiveBrushSerializable
		{
			get { return Serialize.BrushToString(PositiveBrush); }
			set { PositiveBrush = Serialize.StringToBrush(value); }
		}			

		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="NegativeBrush", Order=9, GroupName="Parameters")]
		public Brush NegativeBrush
		{ get; set; }

		[Browsable(false)]
		public string NegativeBrushSerializable
		{
			get { return Serialize.BrushToString(NegativeBrush); }
			set { NegativeBrush = Serialize.StringToBrush(value); }
		}
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private ClickDrawRectangleWithText[] cacheClickDrawRectangleWithText;
		public ClickDrawRectangleWithText ClickDrawRectangleWithText(int verticalOffset, int horizontalOffset, Brush labelTextBrush, Brush areaBrush, Brush borderBrush, Brush positiveBrush, Brush negativeBrush)
		{
			return ClickDrawRectangleWithText(Input, verticalOffset, horizontalOffset, labelTextBrush, areaBrush, borderBrush, positiveBrush, negativeBrush);
		}

		public ClickDrawRectangleWithText ClickDrawRectangleWithText(ISeries<double> input, int verticalOffset, int horizontalOffset, Brush labelTextBrush, Brush areaBrush, Brush borderBrush, Brush positiveBrush, Brush negativeBrush)
		{
			if (cacheClickDrawRectangleWithText != null)
				for (int idx = 0; idx < cacheClickDrawRectangleWithText.Length; idx++)
					if (cacheClickDrawRectangleWithText[idx] != null && cacheClickDrawRectangleWithText[idx].VerticalOffset == verticalOffset && cacheClickDrawRectangleWithText[idx].HorizontalOffset == horizontalOffset && cacheClickDrawRectangleWithText[idx].LabelTextBrush == labelTextBrush && cacheClickDrawRectangleWithText[idx].AreaBrush == areaBrush && cacheClickDrawRectangleWithText[idx].BorderBrush == borderBrush && cacheClickDrawRectangleWithText[idx].PositiveBrush == positiveBrush && cacheClickDrawRectangleWithText[idx].NegativeBrush == negativeBrush && cacheClickDrawRectangleWithText[idx].EqualsInput(input))
						return cacheClickDrawRectangleWithText[idx];
			return CacheIndicator<ClickDrawRectangleWithText>(new ClickDrawRectangleWithText(){ VerticalOffset = verticalOffset, HorizontalOffset = horizontalOffset, LabelTextBrush = labelTextBrush, AreaBrush = areaBrush, BorderBrush = borderBrush, PositiveBrush = positiveBrush, NegativeBrush = negativeBrush }, input, ref cacheClickDrawRectangleWithText);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.ClickDrawRectangleWithText ClickDrawRectangleWithText(int verticalOffset, int horizontalOffset, Brush labelTextBrush, Brush areaBrush, Brush borderBrush, Brush positiveBrush, Brush negativeBrush)
		{
			return indicator.ClickDrawRectangleWithText(Input, verticalOffset, horizontalOffset, labelTextBrush, areaBrush, borderBrush, positiveBrush, negativeBrush);
		}

		public Indicators.ClickDrawRectangleWithText ClickDrawRectangleWithText(ISeries<double> input , int verticalOffset, int horizontalOffset, Brush labelTextBrush, Brush areaBrush, Brush borderBrush, Brush positiveBrush, Brush negativeBrush)
		{
			return indicator.ClickDrawRectangleWithText(input, verticalOffset, horizontalOffset, labelTextBrush, areaBrush, borderBrush, positiveBrush, negativeBrush);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.ClickDrawRectangleWithText ClickDrawRectangleWithText(int verticalOffset, int horizontalOffset, Brush labelTextBrush, Brush areaBrush, Brush borderBrush, Brush positiveBrush, Brush negativeBrush)
		{
			return indicator.ClickDrawRectangleWithText(Input, verticalOffset, horizontalOffset, labelTextBrush, areaBrush, borderBrush, positiveBrush, negativeBrush);
		}

		public Indicators.ClickDrawRectangleWithText ClickDrawRectangleWithText(ISeries<double> input , int verticalOffset, int horizontalOffset, Brush labelTextBrush, Brush areaBrush, Brush borderBrush, Brush positiveBrush, Brush negativeBrush)
		{
			return indicator.ClickDrawRectangleWithText(input, verticalOffset, horizontalOffset, labelTextBrush, areaBrush, borderBrush, positiveBrush, negativeBrush);
		}
	}
}

#endregion
