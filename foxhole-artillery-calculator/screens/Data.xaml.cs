using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace foxhole_artillery_calculator.screens
{
    /// <summary>
    /// Логика взаимодействия для Data.xaml
    /// </summary>
    public partial class Data : Window
    {
        private const string content  = @"<html>
        <head>
        <meta http-equiv=Content-Type content='text/html;'>
        <style>
            body {
                background: black;
                color: darkorange;
            }
            table, tbody, th, td {
                border: 1px solid #333;
            }
        </style>
        </head>
        <body scroll='no'>
        <table cellpadding=0 cellspacing=0 width=480 style='border-collapse:collapse;table-layout:fixed;width:360pt'>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt;width:135pt;text-align: center;'>Name</td>
          <td style='width:56pt;text-align: center;'>Health</td>
          <td style='width:56pt;text-align: center;'>Mortar</td>
          <td style='width:56pt;text-align: center;'>Artillery</td>
          <td style='width:56pt;text-align: center;'>Howitzer</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Storage box</td>
          <td>1500</td>
          <td>2</td>
          <td>1</td>
          <td>1</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Barracks</td>
          <td>1500</td>
          <td>2</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Encampment</td>
          <td>?</td>
          <td>4</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Field Base</td>
          <td>?</td>
          <td>8</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Outpost</td>
          <td>11500</td>
          <td>11</td>
          <td>?</td>
          <td>50</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Fort</td>
          <td>?</td>
          <td>13</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Garrison Camp</td>
          <td>?</td>
          <td>7</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Garrison Base</td>
          <td>?</td>
          <td>?</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Town Hall</td>
          <td>20000</td>
          <td>?</td>
          <td>6</td>
          <td>100</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Chain Link Fence</td>
          <td>500</td>
          <td>1</td>
          <td>1</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Sandbags</td>
          <td>1500</td>
          <td>2</td>
          <td>1</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Reinforced Wall</td>
          <td>3000</td>
          <td>3</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Heavy Gate</td>
          <td>8500</td>
          <td>9</td>
          <td>2</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Fortress Wall</td>
          <td>16500</td>
          <td>17</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Foxhole</td>
          <td>1500</td>
          <td>2</td>
          <td>1</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Gun Nest</td>
          <td>3000</td>
          <td>3</td>
          <td>2</td>
          <td>8</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Gun Turret</td>
          <td>2500</td>
          <td>3</td>
          <td>2</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Light Pillbox</td>
          <td>?</td>
          <td>3</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Pillbox</td>
          <td>4500</td>
          <td>5</td>
          <td>2</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Sunken Pillbox(active)</td>
          <td>4500</td>
          <td>?</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Sunken Pillbox(inactive)</td>
          <td>?</td>
          <td>25</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Garrisoned House</td>
          <td>5500</td>
          <td>6</td>
          <td>2</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Bunker</td>
          <td>20500</td>
          <td>21</td>
          <td>5</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Watch Tower</td>
          <td>1000</td>
          <td>2</td>
          <td>1</td>
          <td>1</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Howitzer</td>
          <td>1000</td>
          <td>2</td>
          <td>1</td>
          <td>1</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Wooden Bridge</td>
          <td>5500</td>
          <td>6</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Stone Bridge</td>
          <td>&gt;20000</td>
          <td>&gt;20</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Reinforced Drawbridge</td>
          <td>?</td>
          <td>?</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=20 style='height:15.0pt'>
          <td height=20 style='height:15.0pt'>Wooden Drawbridge</td>
          <td>?</td>
          <td>?</td>
          <td>?</td>
          <td>?</td>
         </tr>
         <tr height=0 style='display:none'>
          <td width=180 style='width:135pt'></td>
          <td width=75 style='width:56pt'></td>
          <td width=75 style='width:56pt'></td>
          <td width=75 style='width:56pt'></td>
          <td width=75 style='width:56pt'></td>
         </tr>
        </table>
        </div>
        </body>
        </html>
";
        public Data()
        {
            InitializeComponent();
            webBrowser.NavigateToString(content);
        }
    }
}
