using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ColourMatchingDevice
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow:Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// 修改RGB配色
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RGBTextbox_TextChanged(object sender,TextChangedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			string text = textbox.Text.Trim();
			if(text != null && text != "")
			{
				Regex r = new Regex("^[0-9]*$");
				if(r.IsMatch(textbox.Text.Trim()) == false)
				{
					textbox.Text = textbox.Text.Remove(textbox.Text.Length - 1,1);
					textbox.SelectionStart = textbox.Text.Length;
				}
				//else
				//{
				//	//MatchButton.Tag = 1;
				//	int num = Convert.ToInt32(textbox.Text);
				//	if(num > 255)
				//	{
				//		MessageBox.Show("该值在0-255的范围内!");
				//		textbox.Text = null;
				//	}
				//}
			}
			MatchButton_Click(sender,e);
		}

		/// <summary>
		/// 修改#配色
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FFTextbox_TextChanged(object sender,TextChangedEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			string text = textbox.Text.Trim();
			if(text != null && text != "")
			{
				Regex r = new Regex("^[0-9A-Fa-f]*$");
				if(r.IsMatch(textbox.Text.Trim()) == false)
				{
					textbox.Text = textbox.Text.Remove(textbox.Text.Length - 1,1);
					textbox.SelectionStart = textbox.Text.Length;
				}
				else
				{
					//MatchButton.Tag = 2;
				}
			}
		}

		/// <summary>
		/// 空格或enter特殊处理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ColorTextbox_KeyDownOrUp(object sender,KeyEventArgs e)
		{
			TextBox textbox = sender as TextBox;
			string text = textbox.Text.Trim();

			if(e.Key == Key.Space)
			{
				textbox.Text = textbox.Text.Trim();
				textbox.SelectionStart = textbox.Text.Length;
			}
			else if(e.Key == Key.Enter)   //  if (e.KeyValue == 13) 判断是回车键
			{
				if(textbox.Name == "FFTextbox" || textbox.Name == "BTextbox")
				{
					//MatchButton.Focus();
					//MatchButton_Click(sender,e);
				}
				else if(textbox.Name == "RTextbox")
				{
					GTextbox.Focus();
				}
				else if(textbox.Name == "GTextbox")
				{
					BTextbox.Focus();
				}
			}
		}

		/// <summary>
		/// 保存事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveButton_Click(object sender,RoutedEventArgs e)
		{
			//if(FFTextbox.Text != null && FFTextbox.Text != "")
			//{
			//	int num = 0;

			//	for(int i = 0;i < ColorFF.Count;i++)
			//	{
			//		if(FFTextbox.Text != ColorFF[i])
			//		{
			//			num++;
			//		}
			//	}
			//	if(num == ColorFF.Count)
			//	{
			//		ColorFF.Add(FFTextbox.Text);
			//		File.WriteAllLines(ColorPath,ColorFF.ToArray());
			//		MessageBox.Show("保存成功!");
			//		LoadColor();
			//	}
			//	else
			//	{
			//		MessageBox.Show("保存失败!已有相同的颜色!");
			//	}
			//}
		}

		/// <summary>
		/// 配色按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MatchButton_Click(object sender,RoutedEventArgs e)
		{
			{
				if(RTextbox.Text != null && RTextbox.Text != "" && GTextbox.Text != null && GTextbox.Text != "" && BTextbox.Text != null && BTextbox.Text != "")
				{
					byte R = Convert.ToByte(RTextbox.Text);
					byte G = Convert.ToByte(GTextbox.Text);
					byte B = Convert.ToByte(BTextbox.Text);
					SolidColorBrush scBrush = new SolidColorBrush(Color.FromRgb(R,G,B));
					ColorGrid.Background = scBrush;
					//wrappanel.Background = scBrush;
					FFTextbox.Text = scBrush.Color.ToString().Remove(0,1);
				}
			}
		}
	}
}
