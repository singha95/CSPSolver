using System; 
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


public class testCspbase
{
	
   /*
	Main function for program. 
	Creates a list representation of a nonogram puzzle and solves the puzzle. 
   */
   public static void Main(string[] args)
   {	
		
	    List<List<List<int>>> BirdTest = new List<List<List<int>>>{
			new List<List<int>>{new List<int>{1}, new List<int>{5}, new List<int>{2}, new List<int>{5}, new List<int>{2,1}, new List<int>{2}},
			new List<List<int>>{new List<int>{2,1}, new List<int>{1,3}, new List<int>{1,2}, new List<int>{3}, new List<int>{4}, new List<int>{1}}}; 
		
		
		/*List<List<List<int>>> test = new List<List<List<int>>>{
			new List<List<int>>{new List<int>{1}, new List<int>{1}}, 
			new List<List<int>>{new List<int>{1}, new List<int>{1}}};*/
		Tuple<CSP<string>, Variable<string>[,]> csp = Nonogram.CSPModel(BirdTest);
		BT<string> solver = new BT<string>(csp.Item1);
		solver.BTSearch(Propagators<string>.PropGAC);
		PrintArray(csp.Item2);
   }
   
   /*
	 Prints varaibles from array in a simple text fromat. 
   */
   public static void PrintArray(Variable<string>[,] arr)
   {
	    int rowLength = arr.GetLength(0);
        int colLength = arr.GetLength(1);

        for (int i = 0; i < rowLength; i++)
        {
            for (int j = 0; j < colLength; j++)
            {
                if(arr[i, j].AssignedValue==null){
					Console.Write(string.Format("{0} ", "None"));
				}
				else{
					Console.Write(string.Format("{0} ", arr[i, j].AssignedValue));
				}
				
            }
            Console.Write(Environment.NewLine + Environment.NewLine);
        }
        Console.ReadLine();
   }
   
   public static void DisplayNonogramGrid(Variable<string>[,] arr)
   {
	   Bitmap bm = new Bitmap(arr.GetLength(1) * 100, arr.GetLength(0) * 100);
	   Graphics g = Graphics.FromImage(bm);
	   SolidBrush blackBrush = new SolidBrush(Color.Black);
       SolidBrush whiteBrush = new SolidBrush(Color.White);
	   
	   for(int row = 0; row < arr.GetLength(1); row++)
	   {
		   for(int col = 0; col < arr.GetLength(0); col++)
		   {
			   if(arr[row, col].AssignedValue == "1")
			   {
				   g.FillRectangle(blackBrush, row * 100, col * 100, 100, 100);
			   }
			   else 
			   {
				   g.FillRectangle(whiteBrush, row * 100, col * 100, 100, 100);
			   }
		   }
	   }
	   g.DrawImage(bm, 150, 200);
	   //BackgroundImage = bm;
   }
}
