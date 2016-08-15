using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Load_TXT : MonoBehaviour {

	protected string text = " "; //ex : 將讀檔的 "單行" 資料存在這裡以便分析
	static public Vector3[] _pos =new Vector3[8000000];		//ex : 總共可以存放8000000個座標
	protected int i , temp_int , temp_sum1 , order , max;
	float X = 0 , Y  = 0, Z = 0 ;
	bool location ;
	static public int[] sum_v = new int[8000];	//存放每層的gcode位址....對照於上面的_pos 記憶體區
	static public int[] sum_t = new int[8000];	//存放每層的gcode位址....對照於上面的_pos 記憶體區...差別在於上面的是用於顯示線條...而這裡的則是用來輸出成新的gcode
	string[] _File;


	private Material lineMaterial;	//ex : line

	private int speed;	//ex : 每次執行時處理幾筆讀檔資料

	private bool end_Signal;	//ex : 讀檔的結束訊號

	static public int sw = -1;	//ex : 讀檔的switch值

	static public float[] Z_height = new float[4000];

	void Start ()
	{
		i = 0;

		end_Signal = false;

		sw = -1;

		speed = 600;

		if (!lineMaterial)	//ex : 宣告線的屬性
		{
			lineMaterial = new Material ("Shader \"Lines/Colored Blended\" {" +
								"SubShader { Pass { " +
								"    Blend SrcAlpha OneMinusSrcAlpha " +
								"    ZWrite Off Cull Off Fog { Mode Off } " +
								"    BindChannels {" +
								"      Bind \"vertex\", vertex Bind \"color\", color }" +
								"} } }");
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}
	bool show_hide = false;	//ex : 是否展開快捷鍵說明
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.H))	//ex : 開啟 / 關閉快捷鍵說明
		{
			show_hide = !show_hide;
		}

		if(sw == -1)	//ex : 若是sw = -1....則清除檔名路徑資訊  反之則表示需要讀檔
		{
			_GUI.importFilePath = "";
			sw = 0;
			end_Signal = false;
		}
		else
		{
			if(_GUI.importFilePath.Length != 0)
			{
				for(int k= 0;k<speed;k++)
				{
					logic (_GUI.importFilePath);
				}
			}
		}

		////

		if(_GUI.SaveFilePath.Length != 0)	//ex : 如果有存檔路徑檔名~則開始進行存檔
		{
			int start_index = -1 , end_index = -1;

			for(int k=1 ; k <= i-1 ; k++)
			{
				if(_GUI.int_start <= Z_height[k])	//ex : 找出使用者期望的gcode底層的高度
				{
					start_index = k;
					break;
				}
			}

			for(int k=1 ; k <= i-1 ; k++)
			{
				if(_GUI.int_end <= Z_height[k])		//ex : 找出使用者期望的gcode頂層的高度
				{
					end_index = k;
					break;
				}
			}
			if( (end_index == -1)&&(_GUI.int_end > Z_height[i-1]) )	//ex : 預防使用者輸入錯誤的頂層高度...舉例來說~模型最高300mm....使用者卻輸入400mm
			{
				end_index = i-1;
			}
			if( (start_index == -1)&&(_GUI.int_start < Z_height[1]) )	//ex : 預防使用者輸入錯誤的底層高度...例子同上
			{
				end_index = 1;
			}

			if( (start_index != -1)&&(end_index != -1 ))	//ex : 資訊正確則開始進入存檔動作
			{
				Save_txt(sum_t[start_index-1],sum_t[end_index],_GUI.SaveFilePath);
			}

			_GUI.SaveFilePath = "";
		}

	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 120, 20),"FPS = " + ((int)(1/Time.deltaTime)).ToString());	//FPS
		GUI.Label(new Rect(10, 30, 120, 20),"click ' H' on / off Hot Key List");
		if(show_hide)	//ex : 如果有要開啟快捷鍵說明
		{
			GUI.Label(new Rect(10, 70, 400, 20),"Mouse Right Button + Mouse Move = Rotate view");
			GUI.Label(new Rect(10, 90, 400, 20),"Mouse Center Button + Mouse Move = Move view on XY plane");
			GUI.Label(new Rect(10, 110, 400, 20),"Shift + Center Mouse Button = Move Vertical View");
			GUI.Label(new Rect(10, 130, 400, 20),"Z = Default View");
			GUI.Label(new Rect(10, 150, 400, 20),"up, down or +, -  = single layer shift");
		}

		if((i > 0) && (sw == 0))	//ex : 如果有gcode
		{
			lineMaterial.SetPass( 0 );	//ex : 設定顯示樣子....0 = line
			
			GL.Begin( GL.LINES );	//ex : 開始繪製...需跟下面的GL.End();成雙成對
			if(_GUI.show_all)
			{
				GL.Color (new Color (1, 1, 1));		//ex : 設定顏色
				int temp_index;
				for(int k=1;k <= i-1;k++)
				{
					temp_index = k % (_GUI.int_Interval+1);
					if( ( temp_index <= 2)&&(temp_index != 0) )
					{
						for(int j=sum_v[k-1] ; j<(sum_v[k]-1);j++)
						{
							GL.Vertex3( _pos[j].x, _pos[j].z, _pos[j].y );
							GL.Vertex3( _pos[j+1].x, _pos[j+1].z, _pos[j+1].y );
						}
					}
				}
			}
			
			//

			GL.Color (new Color (1, 0, 0));

			int now_layer_index = 0;
			for(int k=1 ; k <= i-1 ; k++)
			{
				if(_GUI.show_layer == Z_height[k])
				{
					now_layer_index = k;
					break;
				}
			}
			if(now_layer_index != 0)
			{
				for(int j=sum_v[now_layer_index-1] ; j<(sum_v[now_layer_index]-1);j++)
				{
					GL.Vertex3( _pos[j].x, _pos[j].z, _pos[j].y );
					GL.Vertex3( _pos[j+1].x, _pos[j+1].z, _pos[j+1].y );
				}
			}
			GL.End();	//ex : 結束繪製
		}
		else
		{
			if(sw > 0)	//ex : 如果正在讀gcode檔
			{
				GUIStyle _skin_Label = new GUIStyle(GUI.skin.label);
				//	_skin_Label.font = _font;
				_skin_Label.fontSize = 16;
				_skin_Label.normal.textColor = Color.white;
				_skin_Label.hover.textColor = Color.white;

				GUI.Label(new Rect((UnityEngine.Screen.width/2)-60, (UnityEngine.Screen.height/2)-10, 500, 20),"Loading : "+ order.ToString() +" Lines",_skin_Label);
			}
		}
	}

	void logic(string filename)
	{
		switch (sw)
		{
		case 0:
			_File = File.ReadAllLines(filename);	//ex : 將所有資訊一次全部儲存進來....以換行碼來做區分
			max = _File.Length;		//ex : 總共有幾行

			order = 0;	//ex : 讀檔的index...目標要與max一樣大

			sw ++;
			i = 0;
			break;
		case 1:
			text = _File[order];	//ex : 取出單行

			if (max != order)
			{
				if(text.IndexOf("G90") != -1)	//ex : 在出現g90之前的內容多數是說明或是機台設定指令...所以算是讀檔的分水嶺
				{
					sw++;

				}
			}
			else
			{
				sw = 999;
			}
			break;
		case 2:		//ex : g90後出現z則開始計算高度
			text = _File[order];

			if (max != order)
			{
				string[] _temp = text.Split(';');

				temp_int = _temp[0].IndexOf('Z');
				if(temp_int != -1)
				{
					int temp_int2 = _temp[0].IndexOf(' ',temp_int);
					if(temp_int2 == -1)
					{
						temp_int2 = text.Length;
					}

					text = _temp[0].Substring( temp_int + 1 , temp_int2 - temp_int - 1 );
					Z = float.Parse(text);


					sum_v[0] = 0;
					temp_sum1 = 0;

					sum_t[i] = order;

					Z_height[i] = 0;

					sw++;
					i++;
				}
			}
			else
			{
				sw = 999;
			}
			break;
		case 3:		//ex : 同case 2 只是case 2 是第一筆的z...而這裡開始將正式取z的值 和 xy的值
			if (max != order)
			{
				location = false;
				text = _File[order];

				string[] _temp = text.Split(';');

				temp_int = _temp[0].IndexOf('Z');	//ex : 取z值
				if(temp_int != -1)
				{
					Z_height[i] = Z;

					int temp_int2 = _temp[0].IndexOf(' ',temp_int);
					if(temp_int2 == -1)
					{
						temp_int2 = text.Length;
					}

					text = _temp[0].Substring( temp_int + 1 , temp_int2 - temp_int - 1 );
					Z = float.Parse(text);

					sum_t[i] = order;

					sum_v[i] = temp_sum1;

					i++;
				}
				if(!end_Signal)
				{
					temp_int = _temp[0].IndexOf('X');	//ex : 取x值
					if(temp_int != -1)
					{
						location = true;

						int temp_int2 = _temp[0].IndexOf(' ',temp_int);

						if(temp_int2 < 0)
						{
							temp_int2 = _temp[0].Length;
						}

						text = _temp[0].Substring( temp_int + 1 , temp_int2 - temp_int - 1 );
						X = float.Parse(text);
					}

					temp_int = _temp[0].IndexOf('Y');	//ex : 取y值
					if(temp_int != -1)
					{
						location = true;

						int temp_int2 = _temp[0].IndexOf(' ',temp_int);
						if(temp_int2 < 0)
						{
							temp_int2 = _temp[0].Length;
						}
						text = _temp[0].Substring( temp_int + 1 , temp_int2 - temp_int - 1 );
				//		Debug.Log("int = " + text);
						Y = float.Parse(text);
					}

					if(location)	//ex : 將現在的三點座標紀錄起來
					{
						_pos[temp_sum1] = new Vector3(X , Y ,Z);
						temp_sum1 ++;
					}
				}

				temp_int = _temp[0].IndexOf("M104 S0");		//ex : 到此表示gocde結束
				if(temp_int != -1)
				{
					Z_height[i] = Z;
					sum_t[i] = order;

					sum_v[i] = temp_sum1;

					i++;

					end_Signal = true;
				}
			}
			else
			{
				sw = 999;
			}
			break;
		case 999:	//ex : 讀檔結束的資訊處理
			sw = -1;

			sum_t[i] = order-1;

			_GUI.str_start = Z_height[1].ToString();
			_GUI.str_end = Z_height[i-1].ToString();
			_GUI.Max_Layer = Z_height[i-1];
			_GUI.str_show = Z_height[1].ToString();
			_GUI.total_layer = i-1;
			//
			Vector3 v3 = new Vector3(0,0,0);
			int sum_point = 0;
			for(int j=0 ; j<(sum_v[i-1]);j++)
			{
				v3 = v3 + _pos[j];
				sum_point++;
			}
			_camera.V3 = new Vector3(v3.x/sum_point,v3.z/sum_point,v3.y/sum_point);// v3/sum_point;

			_GUI.show_all = true;
			_GUI.int_Interval = 10;

			_camera.Default_View = new Vector3(v3.x/sum_point,v3.z/sum_point,v3.y/sum_point);


			break;
		default:
			break;
		}

		order ++;
	}

	void Save_txt(int start , int end , string filename)	//ex : 存檔邏輯
	{
		float difference_z = 0;

		StreamWriter save_text = new StreamWriter(filename);	//ex : 建立新檔

		for(int j = 0 ; j < sum_t[0] ; j++)
		{
			save_text.WriteLine(_File[j]);	//單行存入...這裡存的是g90之前的說明檔和機台設定檔
		}

		for(int j= start ; j<end; j++)	//ex : 由底層到頂層存入
		{
			bool check = false;
			for(int k = 0 ; k < i ; k++)
			{
				if(j == sum_t[k])
					check = true;
			}
			if(check)
			{
				string[] _temp = _File[j].Split(';');
				
				temp_int = _temp[0].IndexOf('Z');	//ex : 把原本的z改成新版的z...舉例來說~原始第一層的z是3mm...如今使用者要從10mm開始做~則要把10mm改成3mm....依序每行z值都要修改

				int temp_int2 = _temp[0].IndexOf(' ',temp_int);
				text = _temp[0].Substring( temp_int + 1 , temp_int2 - temp_int - 1 );					

				if(j == start)
				{
					difference_z = float.Parse(text) - Z_height[1];
				}

				string temp_str1 = _File[j].Substring(0,temp_int+1);
				string temp_str2 = (float.Parse(text) - difference_z).ToString();
				string temp_str3 = _File[j].Substring(temp_int2,_File[j].Length - temp_int2);

				save_text.WriteLine(temp_str1 + temp_str2 + temp_str3);
			}
			else
			{
				save_text.WriteLine(_File[j]);
			}

		}

		for(int j = sum_t[i-1] ; j < sum_t[i] ; j++)
		{
			save_text.WriteLine(_File[j]);
		}

		save_text.Close();	//ex : 存檔完成
	}
}
