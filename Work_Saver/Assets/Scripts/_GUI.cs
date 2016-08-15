using UnityEngine;
using System.Collections;

using System.Text.RegularExpressions; //為了讓TextField只有數字

//using UnityEditor;


using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

public class _GUI : MonoBehaviour {

	static public string importFilePath = "";	//ex : 讀檔路徑

	static public string SaveFilePath = "";		//ex : 存檔路徑

	static public float Max_Layer = -1;	//ex : 當gocde讀取成功後~會紀錄該gcode最頂層的高度
	
	static public float int_start = 0 , int_end = 0 , show_layer = 0;	//ex : int_start = 使用者欲將gcode最底層的高度			int_end = 使用者欲將gcode最頂層的高度		show_layer = 當前看到紅色層的高度
	static public string str_start, str_end, str_show ; //ex : str_start = 使用者欲將gcode最底層的高度			str_end = 使用者欲將gcode最頂層的高度			str_show = 當前看到紅色層的高度
	//ex : 第一行為數字型態...用來做邏輯判斷用
	//ex : 第二行為字串型台...用來顯示給使用者觀看修改之用

	private string str_number;	//ex : 用來暫存字串...以方便將字串予數字間做轉換

	static public int total_layer;	//ex : 總層數

	static public bool show_all = false;    //ex : 是否所有gcode顯示
	static public int int_Interval = 5;


	Rect windowSize;	//ex : 與我們聯絡的視窗大小與位置的設定
	bool about_show;	//ex : 與我們聯絡的視窗是否正在顯示
	
	// Use this for initialization
	void Start ()
	{
		windowSize = new Rect ((UnityEngine.Screen.width/2)-175, (UnityEngine.Screen.height/2)-100, 350, 200);	//ex : 程式一開始時設定與我們聯絡的視窗大小位置的設定
		about_show = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private int check1 , check2;
	
	private bool one_time = true;
	void OnGUI()
	{

		if( (Load_TXT.sw == 0)&&(Max_Layer != -1) )	//ex : 若是有讀取gcode且已經讀取完畢時執行以下內容
		{			
			str_start = GUI.TextField(new Rect (UnityEngine.Screen.width-130, 120, 100, 20),str_start);		//ex : 存取使用者欲將gcode最底層的高度
			str_start = Regex.Replace(str_start, "[^0123456789.]", "");	//只留數字

			check1 = str_start.IndexOf('.');
			if(check1 != -1)	//ex : 會執行到此表示字串str_start內容有小數點
			{
				check2 = str_start.IndexOf('.',check1 + 1);
				if(check2 != -1)	//ex : 會執行到此表示字串str_start內容有兩個小數點 屬於錯誤需修正的字串
				{
					str_start = str_start.Substring(0,check2);
				}
			}

			if(check1 == 0 )//ex : 會執行到此表示小數點在第一位~比如.123~如此我們便要將他修改為0.123
			{
				str_number =  "0" + str_start;
			}
			else
			{
				str_number = str_start;
			}

			if(str_number=="")
			{
				int_start = 0;
			}
			else
			{
				int_start = float.Parse (str_number);	//ex : 將字串轉為數字

			}
			
			//
			
			str_end = GUI.TextField(new Rect (UnityEngine.Screen.width-130, 200, 100, 20),str_end);	//ex : 存取使用者欲將gcode最頂層的高度
			str_end = Regex.Replace(str_end, "[^0123456789.]", "");	//只留數字
			
			check1 = str_end.IndexOf('.');
			if(check1 != -1)	//ex : 會執行到此表示字串str_end內容有小數點
			{
				check2 = str_end.IndexOf('.',check1 + 1);
				if(check2 != -1)	//ex : 會執行到此表示字串str_end內容有兩個小數點 屬於錯誤需修正的字串
				{
					str_end = str_end.Substring(0,check2);
				}
			}
			
			if(check1 == 0 )	//ex : 會執行到此表示小數點在第一位~比如.123~如此我們便要將他修改為0.123
			{
				str_number =  "0" + str_end;
			}
			else
			{
				str_number = str_end;
			}
			
			if(str_number=="")
			{
				int_end = 0;
			}
			else
			{
				int_end = float.Parse (str_number);	//ex : 將字串轉為數字
				
			}
			
			//
			
			str_show = GUI.TextField(new Rect (UnityEngine.Screen.width-130, 280, 100, 20),str_show);	//ex : 存取使用者欲將gcode顯示層的高度
			str_show = Regex.Replace(str_show, "[^0123456789.]", "");	//只留數字
			
			check1 = str_show.IndexOf('.');
			if(check1 != -1)	//ex : 會執行到此表示字串str_show內容有小數點
			{
				check2 = str_show.IndexOf('.',check1 + 1);
				if(check2 != -1)	//ex : 會執行到此表示字串str_show內容有兩個小數點 屬於錯誤需修正的字串
				{
					str_show = str_show.Substring(0,check2);
				}
			}
			
			if(check1 == 0 )	//ex : 會執行到此表示小數點在第一位~比如.123~如此我們便要將他修改為0.123
			{
				str_number =  "0" + str_show;
			}
			else
			{
				str_number = str_show;
			}
			
			if(str_number=="")
			{
				show_layer = 0;
			}
			else
			{
				show_layer = float.Parse (str_number);
				
			}
			
			////
			
			GUI.Label(new Rect(UnityEngine.Screen.width-95, 100, 120, 20),"Start");
			
			GUI.Label(new Rect(UnityEngine.Screen.width-95, 180, 120, 20),"End");
			
			GUI.Label(new Rect(UnityEngine.Screen.width-125, 220, 120, 20),"Max mm = " + Max_Layer.ToString());
			
			GUI.Label(new Rect(UnityEngine.Screen.width-120, 260, 120, 20),"Show_Layer");
			
			////

			if(GUI.Button(new Rect(UnityEngine.Screen.width-130, 400, 100, 20),"Save"))	//ex : 當按下save按鍵...則開始進行存檔...此button只負責取路徑...正式的存檔動作在Load_TXT.cs
			{
				System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
				sfd.InitialDirectory ="file://"+UnityEngine.Application.dataPath;//開啟時的預設目錄
				sfd.Filter = "All files (*.*)|*.*|gco files (*.gco)|*.gco" ;
				sfd.FilterIndex = 2 ;//預設副檔名的index
				sfd.RestoreDirectory = true ;
				
				if(sfd.ShowDialog()== System.Windows.Forms.DialogResult.OK)	//ex : 若是有選檔成功
				{
					SaveFilePath = sfd.FileName;	//ex : 得到路徑 + 檔名		如 : c:\123\456.txt
				}
			}
			
			////
			bool move = false;	//ex : 檢查 觀看層(紅色那層)gcode值是否有被修改
			float temp_show_layer = show_layer;
			show_layer = GUI.HorizontalSlider(new Rect(UnityEngine.Screen.width-130, 320, 100, 20), show_layer, Load_TXT.Z_height[1]-0.1f, Max_Layer);	//ex : gui...功能為水平滑動條
			if(show_layer != temp_show_layer)	//ex : 藉由比較拉動水平滑動條前後的值來得知是否有拉動水平滑動條
			{
				move = true;
			}

			//ex : 因為使用者很難自己去計算每層正確的高度~所以假設第n層高度為299mm  第n+1層為302mm
			//ex : 那麼當使用者輸入300時~系統自動將其對比到299mm
			int i_show_Z_height = 0;
			for(i_show_Z_height=1 ; i_show_Z_height<=total_layer; i_show_Z_height++)
			{
				if(show_layer <= Load_TXT.Z_height[i_show_Z_height])
				{
					show_layer = Load_TXT.Z_height[i_show_Z_height];
					break;
				}
			}


			if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))	//ex : 當按下鍵盤上...則上升觀看層gcode高度
			{
				if(one_time)
				{
					if(i_show_Z_height < total_layer)
					{
						i_show_Z_height++;
						show_layer = Load_TXT.Z_height[i_show_Z_height];
						
						move = true;
					}

					one_time = false;
				}
			}
			else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))	//ex : 當按鍵盤下...則下降觀看層gcode高度
			{
				if(one_time)
				{
					if(i_show_Z_height > 0)
					{
						i_show_Z_height--;
						show_layer = Load_TXT.Z_height[i_show_Z_height];
						
						move = true;
					}

					one_time = false;
				}
			}
			else
			{
				one_time = true;
			}

			if(move)
			{
				str_show = show_layer.ToString();
			}

			///

			show_all = GUI.Toggle (new Rect(30, 180, 100, 20),show_all," Show ALL");	//ex : gui功能 開關功能 是否要顯示全部的gcode路線 不要的話只會顯示單層路線
			if(show_all)	//ex : 要顯示全部gcode的話則會跑此段...可以選擇每隔幾層做一次顯示
			{
				GUI.Label(new Rect(35, 220, 120, 20),"Interval = " + int_Interval.ToString());
				float temp_Interval = (float)int_Interval;
				temp_Interval = GUI.HorizontalSlider(new Rect(30, 250, 100, 20), temp_Interval, 1.0f, 10.0f);
				int_Interval = (int)temp_Interval;
			}
		}

		if(GUI.Button(new Rect(UnityEngine.Screen.width-130, 50, 100, 20),"Open"))	//ex : 當按下OPEN按鍵...則開啟讀檔...此button只負責取路徑...正式的讀檔動作在Load_TXT.cs
		{
			System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog ();//開啟舊檔
			ofd.InitialDirectory ="file://"+UnityEngine.Application.dataPath;//開啟時的預設目錄
			ofd.Filter = "All files (*.*)|*.*|gco files (*.gco)|*.gco" ;
			ofd.FilterIndex = 2 ;//預設副檔名的index
			ofd.RestoreDirectory = true ;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK) //ex : 若是有選檔成功
			{
				Max_Layer = -1;
				importFilePath = ofd.FileName;	//ex : 得到路徑 + 檔名		如 : c:\123\456.txt
			}
		}

		////

		if (GUI.Button (new Rect (UnityEngine.Screen.width - 130, UnityEngine.Screen.height - 50, 100, 20), "about"))	//ex : 當按下about按鈕~則顯示聯絡我們的視窗
		{
			about_show = true;
		}

		if(about_show)
		{
			GUI.Window(0,windowSize,about,"about");    //創造一個2D視窗裡面執行showList()            
		}
	}

	void about(int windowID)
	{
		if (GUI.Button (new Rect ( 150-50, 200-40 , 100, 20), "OK"))	//ex : 當按下ok的按鈕 則關閉聯絡我們的視窗
		{
			about_show = false;
		}
        GUI.Label(new Rect(10, 30, 330, 25), "Developed by MegJia Inc. (Kaohsiung) all right reserved");
        GUI.Label(new Rect(10, 60, 330, 25), "Tel:+886-963-258-888");
        GUI.Label(new Rect(10, 90, 330, 25), "email: hugoelec@gmail.com");
	}
}
