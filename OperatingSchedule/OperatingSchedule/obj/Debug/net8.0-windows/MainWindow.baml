   M S B A M L   `   `   `  ����L  HOperatingSchedule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:OperatingScheduleOperatingSchedule  5   	   "    OperatingSchedule.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:OperatingSchedule     5   	     � Title$  
MainWindow��5   	   $	��700��6   $
��1200��6)    � WindowStyle$
 None=�5	   	    � 
ResizeMode$ NoResize=�5
   	   
������  � AllowsTransparency$
 True��5   	   .��5      �� ���#272537 �� CornerRadius$ 20��5      ._�5   
   �  � ColumnDefinitions 5      �� 5      $	��200 �6#   �� 5      $��* �6#    � RowDefinitions 5      �� 5      $��75 �6    �� 5      .+�5      �� $��2��6   $��OR Schedule��5      $��Center=�5      $
��Left=�5      
�������$��22(�5       $��20,0,0,0q�5!      �� 5#      $��1��6   $��0��6&   .+�5%      	� -   5        $��Surgeons��5%      $��50��5&      
�������$��15(�5(         MenuButtonTheme#��[  5)      5*      	� 5,      -   5        $��Anesthesiologists��5,      $��50��5-      
�������$��15(�5/      #��[  50      51      	� 53      -   5        $��Proposed Schedule��53      $��50��54      
�������$��15(�56      #��[  57      58      5:      �� 5<      -   5        $��0��5<      $��1��6#   $	��250��5=      $��50��5>      $��Center=�5?      $
��Left=�5@      $��
20,10,0,15q�5A        ModernTextbox#��[ 5B      5C       � 5E      -   5        $��	MainFrame��5E      $��1��6%   $��1��62     � NavigationUIVisibility$ Hidden=�6B   5G      5I      5J      