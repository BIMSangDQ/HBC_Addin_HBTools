   M S B A M L   `   `   `  ����G  CHoaBinhTools, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:HoaBinhTools.AutocadToRevit.Wall.Views&HoaBinhTools.AutocadToRevit.Wall.Views  5   	   5    /HoaBinhTools.AutocadToRevit.Wall.Views.WallView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Flocal4clr-namespace:HoaBinhTools.AutocadToRevit.Wall.Views     5   	     � Title$  AutoCad2Revit - Wall��5      $	��295��6   $	��400��6+    � Topmost . 
�������.��5
      �� $��2,2,2,2q�6   .+�5      �  � RowDefinitions 5      �� 5      $��25 �6   �� 5      $	��150 �6   �� 5      $��25 �6   �� 5      $��25 �6    � ColumnDefinitions 5      �� 5      $	��100 �6   �� 5      $��* �6   �� 5      $	��100 �6   �� 5      $��* �6   .+�5      �� $��Layer of Hatch��6   $��2,0,0,0q�6&   $��Center=�5      �� 5      $��0��6   $��1��6   $��3��6+   $��20��5   >   $��Center=�6J   ��5      �� *	AllLayers+ �� SelectedItem 5      �� *SelectedLayer+6J   �� 5       $��1��6   $��0��6   $��4��6+   ��6?   �� *ListWallTypes+ 5!      �� *SelectWallTypeItem+ �� View 5"      �� 5#      .P�5$      ��  �� Width$	 365��6   .��5%   	   �� $��Add WallType��6   
������ $��20��6@   $	��360��6L   
������ ��5&   +   �� *btnAdd+ �� CellTemplate 5(   	   �� 5)   
   .R�5*      ��  �� Orientation$ 
Horizontal=�6   .+�5+      �� $	��250��6   	 �� DisplayMemberPath$
	 Name��5-      ��5,      �� *AllWallTypes+ 5.      �� *WallTypeSelect+�� 5/      $��50��6   ��6   �� *With+
 �� UpdateSourceTrigger$
 PropertyChanged=�50      51      53   	   54      56      �� 58      $��2��6   $��2,0,0,0q�6   $��
Base Level��59      $��Center=�5:      �� 5;      $��2��6   $��1��6   $
	 Name��5=      $��20��5>   :   $��Center=�6F   ��5<      �� *
AllLevel+ 5>      �� *	BaseLevel+6F   �� 5?      $��2��6   $��2��6   $��2,0,0,0q�6-   $��Ofset from level��5@      $��Center=�5A      �� 5B      $��2��6   $��3��6   ��5C      �� *
BaseOffset+�� 5E      $��4��6   $��2,0,0,0q�6   $��	Top Level��5F      $��Center=�5G      �� 5H      $��4��6   $��1��6   $
	 Name��5K      $��20��5L      $��Center=�6#   ��5I      �� *
AllLevel+ 5J      �� *
TopLevel+5L   #   �� 5M      $��4��6   $��2��6   $��2,0,0,0q�6-   $��Ofset from level��5N   	   $��Center=�5O      �� 5P      $��4��6   $��3��6   ��5Q      �� *	TopOffset+5R      �� 5S      $ 
Horizontal=�6   $��Right=�6(   .+�5T      �� $��50��6   $��20��6   $��OK��6#   
������ 
��� � �$��5,4,5,0q�5V   &   ��6   �� *btnOk+6&   5W      5Z      5[      