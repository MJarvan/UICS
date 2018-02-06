﻿using System;
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

namespace UICS
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
		/// 图片名字
		/// </summary>
		List<string> ImageName = new List<string>();

		/// <summary>
		/// 图片颜色
		/// </summary>
		List<string> ColorFF = new List<string>();

		/// <summary>
		/// 图片路径
		/// </summary>
		static string ColorPath;

		
		private void Window_Loaded(object sender,RoutedEventArgs e)
		{
			LoadColor();
			scrollviewer.DragEnter += scDragEnter;
			scrollviewer.Drop += scDrop;
		}

		private void scDrop(object sender,DragEventArgs e)
		{
			//foreach(string str in e.Data.GetFormats())
			//{
			//	MessageBox.Show(str);
			//}

			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Link;

				string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);
				foreach(string path in paths)
				{
					CreateImage(path);
				}
			}
		}

		private void scDragEnter(object sender,DragEventArgs e)
		{
			if(e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Link;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}

		/// <summary>
		/// 加载底色
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LoadColor()
		{
			colorCombobox.Items.Clear();
			ColorFF.Clear();

			string systempath = System.Windows.Forms.Application.StartupPath;
			ColorPath = systempath + "\\Color.txt";
			double height = colorCombobox.ActualHeight;
			double width = colorCombobox.ActualWidth;

			if(File.Exists(ColorPath))
			{
				ColorFF = File.ReadAllLines(ColorPath).ToList();
				int errorNum = 0;

				for(int i = 0;i < ColorFF.Count;i++)
				{
					if(ColorFF[i].Length == 3 || ColorFF[i].Length == 4 || ColorFF[i].Length == 6 || ColorFF[i].Length == 8)
					{
						SolidColorBrush scBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#" + ColorFF[i])));
						ComboBoxItem cbitem = new ComboBoxItem();
						TextBlock textblock = new TextBlock();
						textblock.Background = scBrush;
						textblock.Width = width;
						textblock.Height = height;
						cbitem.Content = textblock;
						colorCombobox.Items.Add(cbitem);
					}
					else if(MessageBox.Show("由第" + (i + 1) + "行的 '" + ColorFF[i] + "' 引发的数据异常,是否尝试修复?","提示",MessageBoxButton.YesNo,MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
					{
						string errorStr = ColorFF[i];
						if(errorStr.Length > 8)
						{
							string firstColor = errorStr.Remove(8,errorStr.Length - 8);
							string secondColor = errorStr.Remove(0,8);
							ColorFF.RemoveAt(i);
							ColorFF.Add(firstColor);
							if(secondColor.Length == 3 || secondColor.Length == 4 || secondColor.Length == 6 || secondColor.Length == 8)
							{
								SolidColorBrush sb = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#" + secondColor)));
								secondColor = sb.Color.ToString().Remove(0,1);
								ColorFF.Add(secondColor);
							}
							File.WriteAllLines(ColorPath,ColorFF.ToArray());
							MessageBox.Show("修复成功!");
							LoadColor();
						}
						else
						{
							ColorFF.RemoveAt(i);
							File.WriteAllLines(ColorPath,ColorFF.ToArray());
							MessageBox.Show("修复成功!");
							LoadColor();
						}
					}
					else
					{
						errorNum++;
					}
				}

				if(errorNum != 0)
				{
					MessageBox.Show("已成功载入" + (ColorFF.Count - errorNum) + "个底色,还有" + errorNum + "个底色加载失败!");
				}
			}
			else
			{
				return;
			}
		}

		/// <summary>
		/// 换底色
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void colorCombobox_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			ComboBox combo = sender as ComboBox;
			if(combo.SelectedItem != null)
			{
				ComboBoxItem item = combo.SelectedItem as ComboBoxItem;
				TextBlock textblock = item.Content as TextBlock;
				wrappanel.Background = textblock.Background;
				ColorGrid.Background = textblock.Background;
				SolidColorBrush scBrush = wrappanel.Background as SolidColorBrush;
				FFTextbox.Text = scBrush.Color.ToString().Remove(0,1);
				RTextbox.Text = scBrush.Color.R.ToString();
				GTextbox.Text = scBrush.Color.G.ToString();
				BTextbox.Text = scBrush.Color.B.ToString();
			}
		}


		private void test_Click(object sender,RoutedEventArgs e)
		{
			Clipboard.SetText(FFTextbox.Text);
		}

		/// <summary>
		/// 选择路径
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void choiceButton_Click(object sender,RoutedEventArgs e)
		{
			ImageName.Clear();
			wrappanel.Children.Clear();

			System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
			fbd.ShowDialog();
			if(fbd.SelectedPath != string.Empty)
			{
				string FolderPath = fbd.SelectedPath;

				ShowImage(FolderPath);
			}
		}

		/// <summary>
		/// 递归寻找图片
		/// </summary>
		/// <param name="FolderPath"></param>
		private void ShowImage(string FolderPath)
		{
			if(Directory.Exists(FolderPath))
			{
				foreach(string d in Directory.GetFileSystemEntries(FolderPath))
				{
					if(File.Exists(d))
					{
						CreateImage(d);
					}
					else
					{
						ShowImage(d);
					}
				}
			}
		}

		/// <summary>
		/// 生成图片
		/// </summary>
		/// <param name="imagePath"></param>
		private void CreateImage(string imagePath)
		{
			#region 生成图片
			using(FileStream fileStream = new FileStream(imagePath,FileMode.Open,FileAccess.Read,FileShare.Read))
			{

				//获取包含扩展名的文件名 
				//string fileFullName = fileStream.Name.Substring(fileStream.Name.LastIndexOf("\\") + 1);
				//获取扩展名
				string fileTypeName = fileStream.Name.Substring(fileStream.Name.LastIndexOf(".") + 1);
				//获取不包含扩展名的文件名
				string fileName = fileStream.Name.Substring(fileStream.Name.LastIndexOf("\\") + 1,fileStream.Name.LastIndexOf(".") - fileStream.Name.LastIndexOf("\\") - 1);

				fileName = CheckFileName(fileName);

				fileStream.Flush();
				fileStream.Close();
				fileStream.Dispose();

				if(fileTypeName == "png" || fileTypeName == "jpg")
				{
					ImageBrush imagebrush = new ImageBrush();
					BitmapImage bi = new BitmapImage(new Uri(imagePath));
					double dpix = Math.Round(bi.DpiX,MidpointRounding.AwayFromZero);
					double dpiy = Math.Round(bi.DpiY,MidpointRounding.AwayFromZero);

					imagebrush.ImageSource = bi;

					TextBlock textblock = new TextBlock();
					textblock.Text = fileName;
					textblock.TextWrapping = TextWrapping.Wrap;
					textblock.VerticalAlignment = VerticalAlignment.Center;
					//textblock.HorizontalAlignment = HorizontalAlignment.Center;

					fileName = TransformNumbers(fileName);

					Button button = new Button();
					try
					{
						button.Name = fileName;
						ImageName.Add(fileName);
					}
					catch(Exception ex)
					{
						MessageBox.Show(ex.Message + " 使用功能时会导致程序崩溃，请重新命名。");
					}

					WrapPanel wpanel = new WrapPanel();
					wpanel.Orientation = Orientation.Vertical;
					wpanel.Children.Add(new TextBlock() { Text = "X.DPI= " + dpix.ToString() });
					wpanel.Children.Add(new TextBlock() { Text = "Y.DPI= " + dpiy.ToString() });
					wpanel.Children.Add(new TextBlock() { Text = textblock.Text.ToString() });

					button.ToolTip = wpanel;
					button.Margin = new Thickness(20);
					button.Background = new SolidColorBrush(Colors.Transparent);
					button.BorderThickness = new Thickness(0);
					button.Style = Resources["WindowStateButtonStyle"] as Style;

					Grid grid = new Grid();
					RowDefinition cd1 = new RowDefinition();
					cd1.Height = new GridLength(1,GridUnitType.Auto);
					grid.RowDefinitions.Add(cd1);
					RowDefinition cd2 = new RowDefinition();
					cd2.Height = new GridLength(1,GridUnitType.Auto);
					grid.RowDefinitions.Add(cd2);

					Grid imageGrid = new Grid();
					imageGrid.Background = imagebrush;

					if(sizeCombobox.SelectedItem != null)
					{
						ComboBoxItem cbi = sizeCombobox.SelectedItem as ComboBoxItem;
						imageGrid.Width = Convert.ToInt32(cbi.Tag);
						imageGrid.Height = Convert.ToInt32(cbi.Tag);
						if(Convert.ToInt32(cbi.Tag) >= 128)
						{
							button.Width = Convert.ToInt32(cbi.Tag) * 1.5;
							button.Height = Convert.ToInt32(cbi.Tag) * 1.5;
						}
						else
						{
							button.Width = 128;
							button.Height = 128;
						}
					}
					else
					{
						imageGrid.Width = 32;
						imageGrid.Height = 32;
						button.Width = 128;
						button.Height = 128;
					}

					grid.Children.Add(imageGrid);
					grid.Children.Add(textblock);
					Grid.SetRow(imageGrid,0);
					Grid.SetRow(textblock,1);
					button.Content = grid;

					wrappanel.Children.Add(button);
				}
			}
			#endregion
		}

		/// <summary>
		/// 名字容错
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private string CheckFileName(string fileName)
		{
			//名字容错
			if(fileName.Contains("、") || fileName.Contains("(") || fileName.Contains(")") || fileName.Contains("（") ||
			   fileName.Contains("）") || fileName.Contains("-") || fileName.Contains("[") || fileName.Contains("]") ||
			   fileName.Contains("&") || fileName.Contains("{") || fileName.Contains("}") || fileName.Contains("$") ||
			   fileName.Contains("`") || fileName.Contains("~") || fileName.Contains("@") || fileName.Contains("%"))
			{
				fileName = fileName.Replace("(","_");
				fileName = fileName.Replace(")","_");
				fileName = fileName.Replace("（","_");
				fileName = fileName.Replace("）","_");
				fileName = fileName.Replace("、","_");
				fileName = fileName.Replace("-","_");
				fileName = fileName.Replace("[","_");
				fileName = fileName.Replace("]","_");
				fileName = fileName.Replace("&","_");
				fileName = fileName.Replace("{","_");
				fileName = fileName.Replace("}","_");
				fileName = fileName.Replace("$","_");
				fileName = fileName.Replace("`","_");
				fileName = fileName.Replace("~","_");
				fileName = fileName.Replace("@","_");
				fileName = fileName.Replace("%","_");
			}

			return fileName;
		}

		/// <summary>
		/// 更换尺寸
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sizeCombobox_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			ComboBox combo = sender as ComboBox;
			ComboBoxItem item = combo.SelectedItem as ComboBoxItem;
			int size = Convert.ToInt32(item.Tag);

			if(ImageName != null)
			{
				for(int i = 0;i < ImageName.Count;i++)
				{
					string buttonName = ImageName[i];
					Button button = GetChildObject<Button>(wrappanel,buttonName);
					Grid grid = button.Content as Grid;
					Grid imageGrid = grid.Children[0] as Grid;
					imageGrid.Width = size;
					imageGrid.Height = size;
					if(size >= 128)
					{
						button.Width = size * 1.5;
						button.Height = size * 1.5;
					}
					else
					{
						button.Width = 128;
						button.Height = 128;
					}
				}
			}
		}

		/// <summary>
		/// 搜索
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void searchButton_Click(object sender,RoutedEventArgs e)
		{
			string searchText = searchTextBox.Text;

			searchText = TransformNumbers(searchText);

			if(ImageName != null)
			{
				for(int i = 0;i < ImageName.Count;i++)
				{
					string buttonName = ImageName[i];
					Button button = GetChildObject<Button>(wrappanel,buttonName);

					if(buttonName.ToLower().Contains(searchText.ToLower()))
					{
						button.Visibility = Visibility.Visible;
					}
					else
					{
						button.Visibility = Visibility.Collapsed;
					}
				}
			}
		}

		/// <summary>
		/// 全部
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void showAll_Click(object sender,RoutedEventArgs e)
		{
			if(ImageName != null)
			{
				for(int i = 0;i < ImageName.Count;i++)
				{
					string buttonName = ImageName[i];
					Button button = GetChildObject<Button>(wrappanel,buttonName);
					button.Visibility = Visibility.Visible;
				}
			}
			Rect rc = SystemParameters.WorkArea;//获取工作区大小  
			this.Left = 0;//设置位置  
			this.Top = 0;
			this.Width = rc.Width;
			this.Height = rc.Height;
		}

		/// <summary>
		/// enter搜索
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void searchTextBox_KeyDown(object sender,KeyEventArgs e)
		{
			if(e.Key == Key.Enter)   //  if (e.KeyValue == 13) 判断是回车键
			{
				searchButton.Focus();
				searchButton_Click(sender,e);
			}
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
					if(sender.GetType() == typeof(TextBox))
					{
						ColorGrid.Background = scBrush;
					}
					else if(sender.GetType() == typeof(Button))
					{
						wrappanel.Background = scBrush;
					}
					FFTextbox.Text = scBrush.Color.ToString().Remove(0,1);
				}
			}
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
			}
			MatchButton_Click(sender,e);
		}

		///// <summary>
		///// 空格或enter特殊处理
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//private void ColorTextbox_KeyDownOrUp(object sender,KeyEventArgs e)
		//{
		//	TextBox textbox = sender as TextBox;
		//	string text = textbox.Text.Trim();

		//	if(e.Key == Key.Space)
		//	{
		//		textbox.Text = textbox.Text.Trim();
		//		textbox.SelectionStart = textbox.Text.Length;
		//	}
		//	else if(e.Key == Key.Enter)   //  if (e.KeyValue == 13) 判断是回车键
		//	{
		//		if(textbox.Name == "FFTextbox" || textbox.Name == "BTextbox")
		//		{
		//			//MatchButton.Focus();
		//			MatchButton_Click(sender,e);
		//		}
		//		else if(textbox.Name == "RTextbox")
		//		{
		//			GTextbox.Focus();
		//		}
		//		else if(textbox.Name == "GTextbox")
		//		{
		//			BTextbox.Focus();
		//		}
		//	}
		//}

		/// <summary>
		/// 保存事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveButton_Click(object sender,RoutedEventArgs e)
		{
			if(FFTextbox.Text != null && FFTextbox.Text != "")
			{
				int num = 0;

				for(int i = 0;i < ColorFF.Count;i++)
				{
					if(FFTextbox.Text != ColorFF[i])
					{
						num++;
					}
				}
				if(num == ColorFF.Count)
				{
					ColorFF.Add(FFTextbox.Text);
					File.WriteAllLines(ColorPath,ColorFF.ToArray());
					MessageBox.Show("保存成功!");
					LoadColor();
				}
				else
				{
					MessageBox.Show("保存失败!已有相同的颜色!");
				}
			}
			
		}

		/// <summary>
		/// 删除按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DeleteButton_Click(object sender,RoutedEventArgs e)
		{
			if(colorCombobox.SelectedItem != null)
			{
				if(MessageBox.Show("确定删除吗?","提示",MessageBoxButton.YesNo,MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
				{
					ComboBoxItem item = colorCombobox.SelectedItem as ComboBoxItem;
					TextBlock textblock = item.Content as TextBlock;
					SolidColorBrush scBrush = textblock.Background as SolidColorBrush;
					string BrushName = scBrush.Color.ToString().Remove(0,1);
					for(int i = 0;i < ColorFF.Count;i++)
					{
						if(BrushName == ColorFF[i])
						{
							ColorFF.RemoveAt(i);
							File.WriteAllLines(ColorPath,ColorFF.ToArray());
							MessageBox.Show("删除成功!");
							LoadColor();
							break;
						}
					}
				}
			}
			else
			{
				MessageBox.Show("请选择要删除的项!");
			}
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
			}
		}

		/// <summary>
		/// 弹出框鼠标移入
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CMD_MouseEnter(object sender,MouseEventArgs e)
		{
			popup.IsOpen = true;
			popup.StaysOpen = true;
		}

		/// <summary>
		/// 弹出框鼠标移出
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PopupMouseLeave(object sender,MouseEventArgs e)
		{
			if(!popup.IsMouseOver)
			{
				popup.IsOpen = false;
				popup.StaysOpen = false;
			}
		}

		#region 辅助函数
		/// <summary>
		/// 父控件+控件名找到子控件
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public T GetChildObject<T>(DependencyObject obj,string name) where T : FrameworkElement
		{
			DependencyObject child = null;
			T grandChild = null;
			for(int i = 0;i <= VisualTreeHelper.GetChildrenCount(obj) - 1;i++)
			{
				child = VisualTreeHelper.GetChild(obj,i);
				if(child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
				{
					return (T)child;
				}
				else
				{
					grandChild = GetChildObject<T>(child,name);
					if(grandChild != null)
						return grandChild;
				}
			}
			return null;
		}


		/// <summary>
		/// 数字转换
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public string TransformNumbers(string filename)
		{
			string returnname = string.Empty;

			Regex r = new Regex("^[0-9]*$");

			List<string> name = new List<string>();

			for(int i = 0;i < filename.Length;i++)
			{
				name.Add(filename.Substring(i,1));
			}

			for(int i = 0;i < name.Count;i++)
			{
				#region 转换
				if(r.IsMatch(name[i]))
				{
					switch(Convert.ToInt32(name[i]))
					{
						case 0:
							{
								name[i] = "零";
								break;
							}
						case 1:
							{
								name[i] = "一";
								break;
							}
						case 2:
							{
								name[i] = "二";
								break;
							}
						case 3:
							{
								name[i] = "三";
								break;
							}
						case 4:
							{
								name[i] = "四";
								break;
							}
						case 5:
							{
								name[i] = "五";
								break;
							}
						case 6:
							{
								name[i] = "六";
								break;
							}
						case 7:
							{
								name[i] = "七";
								break;
							}
						case 8:
							{
								name[i] = "八";
								break;
							}
						case 9:
							{
								name[i] = "九";
								break;
							}
					}
				}
				#endregion
			}

			for(int i = 0;i < name.Count;i++)
			{
				returnname = returnname + name[i];
			}

			return returnname;
		}
		#endregion

	}
}
